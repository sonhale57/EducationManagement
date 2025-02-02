﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using SuperbrainManagement.DTOs;
using Newtonsoft.Json;
using static SuperbrainManagement.MvcApplication;
using SuperbrainManagement.Helpers;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using System.Data.Entity.Validation;
using System.Web.Security;

namespace SuperbrainManagement.Controllers
{
    public class ClassesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        private ScheduleHelper scheduleHelper = new ScheduleHelper();

        // GET: Classes
        public ActionResult Index(string idBranch)
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            var branches = db.Branches.Where(x=>x.Enable==true).ToList();
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                branches = db.Branches.Where(x => x.Id == idbranch).ToList();
            }
            ViewBag.IdBranch = new SelectList(branches, "Id", "Name", idBranch);

            return View();
        }
        public ActionResult Loadlist(int? IdBranch,string searchString)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            if (IdBranch == null)
            {
                IdBranch = idbranch;
            }
            
            string str = "";
            int stt = 0;
            var classes = db.Classes.Where(x => x.Enable == true && x.IdBranch==IdBranch);
            if (!string.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(x=>x.Name.Contains(searchString));
            }
            foreach ( var c in classes)
            {
                stt++;
                str += "<tr>"
                    +"<td class='text-center align-content-center'>"+stt+"</td>"
                    + "<td class='align-content-center text-center'>" + c.Code+"</td>"
                    + "<td class='align-content-center'>" + c.Name+ "</td>"
                    + "<td class='align-content-center'>" + c.Description+ "</td>"
                    + "<td class='text-start align-content-center'>" + TKB(c.Id) +"</td>"
                    + "<td class='text-center align-content-center'>" + GetTeacher(c.Id)+"</td>"
                    + "<td class='text-center align-content-center'><span class='btn btn-sm btn-outline-success'>" + db.StudentJoinClasses.Count(x=>x.IdClass==c.Id && x.Todate>DateTime.Now)+ "</td>"
                    + "<td class='text-end align-content-center'>"
                    + "<a href=\"javascript:Edit_class("+c.Id+")\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>" +
                    "<a href=\"javascript:Delete_Classes("+c.Id+")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>" +
                    "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                    "<i class=\"ti ti-dots-vertical\"></i>" +
                    "</a>" +
                    "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">" +
                    "<li><a class=\"dropdown-item\" href=\"javascript:LoadSchedulebyClass("+c.Id+")\"><i class=\"ti ti-calendar-event\"></i> Thời khóa biểu</a></li>" +
                    "</ul>"
                    + "</td>"
                    + "</tr>";
            }

            return Json(new {str}, JsonRequestBehavior.AllowGet);
        }
        public string Getsiso(int id)
        {
            var count = db.StudentJoinClasses.Where(x=>x.IdClass==id&&x.Todate>DateTime.Now).Count();
            return count.ToString();
        }
        public string TKB(int classId)
        {
            Language lg = new Language();
            // Giả định có danh sách lịch học với thông tin các buổi trong tuần và giờ học tương ứng
            var schedules = db.Schedules.Where(s => s.IdClass == classId && s.Active==true).ToList();
            if (schedules.Count() == 0)
            {
                return "<span class='fst-italic text-danger'> Chưa cài TKB </span>";
            }
            // Dictionary để nhóm các giờ giống nhau
            Dictionary<string, List<string>> groupedSchedule = new Dictionary<string, List<string>>();

            foreach (var schedule in schedules)
            {
                // Đổi tên ngày từ dạng dài sang dạng ngắn
                string shortDay = lg.Week_viet(schedule.IdWeek);

                // Ghép thời gian học lại (ví dụ: "08:00 - 09:00")
                string time = schedule.FromHour.Value.ToString("HH:mm") + " - " + schedule.ToHour.Value.ToString("HH:mm");

                // Kiểm tra nếu thời gian này đã tồn tại trong dictionary
                if (!groupedSchedule.ContainsKey(time))
                {
                    groupedSchedule[time] = new List<string>();
                }

                // Thêm ngày đã chuyển đổi vào nhóm tương ứng với thời gian đó
                groupedSchedule[time].Add(shortDay);
            }

            // Tạo chuỗi kết quả cuối cùng
            List<string> result = new List<string>();
            foreach (var group in groupedSchedule)
            {
                // Gộp các ngày vào chuỗi và ghép với thời gian tương ứng
                string days = string.Join(" và ", group.Value);
                result.Add($"{days}, lúc {group.Key}");
            }

            // Trả về kết quả đã ghép lại
            return string.Join(", ", result);
        }
        public string GetTeacher(int IdClass) 
        {
            var s = db.Schedules
                     .Where(x => x.IdClass == IdClass && x.Active == true)
                     .GroupBy(x => x.Employee.Id) // Nhóm theo IdEmployee để loại bỏ trùng lặp
                     .Select(g => g.FirstOrDefault().Employee) // Chỉ lấy một giáo viên mỗi nhóm
                     .ToList();
            if (s.Count() == 0)
            {
                return "-";
            }
            string str = "";
            foreach (var teacher in s)
            {
                str += "<a href='javascript:void(0);' class='avatar avatar-lg avatar-xs rounded-circle me-1' "
                       + "data-toggle='tooltip' data-placement='bottom' data-original-title='" + teacher.Name + "' title='" + teacher.Name + "'>"
                       + "<img height='25' width='25' class='rounded-circle' alt='Image placeholder' src='"
                       + (teacher.Image == null ? "/assets/images/profile/user-1.jpg" : teacher.Image) + "'>"
                       + "</a>";
            }
            string strs = "<div class=\"avatar-group ms-auto\">"+ str+ "</div>";
            return strs;
        }
        public ActionResult Schedules()
        {
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x => x.Enable == true), "Name", "Id");
            return View();
        }
        public ActionResult ShowWeeklySchedule()
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            // Giả sử tuần bắt đầu từ thứ 2 và kết thúc vào Chủ nhật
            DateTime today = DateTime.Today;
            int deltaToMonday = DayOfWeek.Monday - today.DayOfWeek;
            DateTime monday = today.AddDays(deltaToMonday);
            DateTime sunday = monday.AddDays(6);

            // Truy vấn dữ liệu thời khóa biểu trong khoảng thời gian từ thứ 2 đến Chủ nhật
            var scheduleData = (from s in db.Schedules
                                join c in db.Classes on s.IdClass equals c.Id
                                where s.Active == true && c.IdBranch == idbranch
                                select new
                                {
                                    s.IdWeek,
                                    s.IdClass,
                                    c.Name,
                                    s.FromHour,
                                    s.ToHour
                                }).ToList();

            // Tạo chuỗi HTML cho bảng 7 cột (từ thứ 2 đến Chủ nhật)
            string str = "<tr>";

            // Biến lưu dữ liệu theo từng cột (ngày trong tuần)
            for (int i = 0; i < 7; i++)
            {
                // Kiểm tra IdWeek tương ứng từ Chủ Nhật (0) đến Thứ 7 (6)
                var dayData = scheduleData.Where(s => (s.IdWeek == (i == 6 ? 0 : i + 1))).ToList(); // Chủ Nhật là 0, Thứ 2 là 1, Thứ 3 là 2, ...

                string cellContent = "";

                foreach (var schedule in dayData)
                {
                    cellContent += "<div class=\"card border-start border-4 border-danger mb-3 pt-0 pb-0\">" +
                                        "<div class=\"card-body\">" +
                                           "<p class=\"mb-0 fs-2 fw-semibold text-muted\"><i class='ti ti-clock'></i> " + schedule.FromHour.Value.ToString("hh:mm tt") + " - " + schedule.ToHour.Value.ToString("hh:mm tt") + "</span></p>" +
                                           "<p class=\"text-success fw-bolder fs-4 mb-0\"><i class=\"ti ti-calendar\"></i> " + schedule.Name + "</p>" +
                                           "<p class=\"fw-bold mb-0\"><i class=\"ti ti-user\"></i>Sỉ số: " + Getsiso(schedule.IdClass) + "</p>" +
                                        "</div>" +
                                    "</div>";
                }

                if (string.IsNullOrEmpty(cellContent))
                {
                    cellContent = "<div>Không có lớp</div>"; // Trường hợp không có lớp
                }

                str += $"<td style='min-width:300px;'>{cellContent}</td>";
            }
            str += "</tr>";

            // Trả về chuỗi HTML
            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResultCourse()
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var linq = from c in db.Courses
                       join cb in db.CourseBranches on c.Id equals cb.IdCourse
                       where cb.IdBranch == idbranch
                       orderby c.Program.DisplayOrder, c.DisplayOrder
                       select c;
            ViewBag.IdClass = new SelectList(db.Classes.Where(x => x.Enable == true && x.IdBranch == idbranch), "Id", "Name");
            ViewBag.IdCourse = new SelectList(linq, "Id", "Name");
            return View();
        }
        public ActionResult GetStudentsOnClass(int? IdClass, int? IdCourse, string searchString)
        {
            int idBranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "";

            // Lấy danh sách học viên còn đang học (Todate > DateTime.Now)
            var students = db.StudentJoinClasses
                     .Where(s => s.Todate > DateTime.Now &&
                                 (IdClass == null || s.IdClass == IdClass) &&
                                 (IdCourse == null || s.IdCourse == IdCourse) &&
                                 (string.IsNullOrEmpty(searchString) || s.Student.Name.Contains(searchString)))
                             .Select(s => new
                             {
                                 s.Id,
                                 s.Student.Name,
                                 ClassName = s.Class.Name,
                                 CourseName = s.Course.Name,
                                 s.Fromdate,
                                 s.Todate,
                                 s.IdStudent,
                                 s.IdClass, 
                                 s.IdCourse,
                                 s.IdRegistration
                             })
                             .ToList();
            int stt = 0;
            foreach (var student in students)
            {
                string btnCheckMiddle = "";
                string btnCheckEnd = "";
                var resultCourseMiddle = db.ResultCourses.FirstOrDefault(x => x.IdStudent == student.IdStudent
                                      && x.IdClass == IdClass && x.IdCourse == student.IdCourse && x.IdRegistration==student.IdRegistration && x.Type=="middle"); 

                var resultCourseEnd = db.ResultCourses.FirstOrDefault(x => x.IdStudent == student.IdStudent
                                      && x.IdClass == IdClass && x.IdCourse==student.IdCourse && x.IdRegistration == student.IdRegistration && x.Type == "end");

                if (resultCourseMiddle != null)
                {
                    btnCheckMiddle =  "<input type=\"checkbox\" onclick=\"loadResultData(" + student.IdRegistration + ","+student.IdCourse+" ,"+student.IdClass+" ," + student.IdStudent + ",'middle')\" class='form-check-input bg-success' checked />";
                }
                else
                {
                    btnCheckMiddle = "<input type=\"checkbox\" onclick=\"loadResultData(" + student.IdRegistration + ","+student.IdCourse+" ,"+student.IdClass+" , " + student.IdStudent + ",'middle')\" class='form-check-input' />";
                }
                if(resultCourseEnd != null)
                {
                    btnCheckEnd = "<input type=\"checkbox\" onclick=\"loadResultData(" + student.IdRegistration + ","+student.IdCourse+" ,"+student.IdClass+" , " + student.IdStudent + ",'end')\" class='form-check-input bg-success' checked />";
                }
                else
                {
                    btnCheckEnd = "<input type=\"checkbox\" onclick=\"loadResultData(" + student.IdRegistration + ","+student.IdCourse+" ,"+student.IdClass+" , " + student.IdStudent + ",'end')\" class='form-check-input' />";
                }

                stt++;
                str += "<tr>"
                    + "<td class='text-center align-content-center'>" + stt + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center'>" + student.Name + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center'>" + student.CourseName + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center'>" + student.ClassName + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center text-center'>" + student.Fromdate.Value.ToString("dd/MM/yyyy") + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center text-center'>" + student.Todate.Value.ToString("dd/MM/yyyy") + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center text-center'>" + btnCheckMiddle + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center text-center'>" + btnCheckEnd + "</td>"
                    + "</tr>";
            }
            // Trả về chuỗi JSON
            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetResultData(int IdRegistration, int IdStudent,int IdCourse, string Type)
        {
            // Lấy dữ liệu từ cơ sở dữ liệu dựa trên IdRegistration, IdStudent và Type
            var result = db.ResultCourses
                .FirstOrDefault(r => r.IdRegistration == IdRegistration && r.IdStudent == IdStudent && r.IdCourse ==IdCourse && r.Type == Type);

            if (result != null)
            {
                // Trả về kết quả nếu có
                return Json(new
                {
                    status = "success",
                    data = new
                    {
                        listen = result.Listen,
                        view = result.View,
                        speed = result.Speed,
                        online = result.Online,
                        totalScore = result.TotalScore,
                        focusGet = result.FocusGet,
                        focusNeed = result.FocusNeed,
                        confidentGet = result.ConfidentGet,
                        confidentNeed = result.ConfidentNeed,
                        rememberGet = result.RememberGet,
                        rememberNeed = result.RememberNeed,
                        reflexGet = result.ReflexGet,
                        reflexNeed = result.ReflexNeed,
                        description = result.Description,
                        orentation = result.Orentation
                    }
                },JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Không có dữ liệu
                return Json(new { status = "not_found" },JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadSchedulebyClass(int? IdClass)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "";
            var schedule = db.Schedules.Where(x=>x.IdClass == IdClass);
            if (schedule == null)
            {
                return Json(new { status = "error",message="Không tìm thấy thời khóa biểu của lớp này." },JsonRequestBehavior.AllowGet);
            }
            else
            {
                var employees = db.Employees.Where(e => e.IdBranch == idbranch&&e.Enable==true&&e.IsOfficial==true).ToList();
                var rooms = db.Rooms.Where(r => r.IdBranch == idbranch).ToList();
                foreach (var sc in schedule)
                {
                    // Tạo danh sách các options cho select giáo viên
                    string employeeOptions = "";
                    foreach (var employee in employees)
                    {
                        employeeOptions += $"<option value='{employee.Id}' {(sc.IdEmployee == employee.Id ? "selected" : "")}>{employee.Name}</option>";
                    }

                    // Tạo danh sách các options cho select phòng học
                    string roomOptions = "";
                    foreach (var room in rooms)
                    {
                        roomOptions += $"<option value='{room.Id}' {(sc.IdRoom == room.Id ? "selected" : "")}>{room.Name}</option>";
                    }
                    str += "<tr>"
                        + "<td class='text-center align-content-center'><input type='checkbox' id='myCheckbox' name='myCheckbox' " + (sc.Active == true ? "checked" : "") + " /><input type='hidden' name='IdWeek' value='"+sc.IdWeek+"'/></td>"
                        + "<td class='text-center align-content-center'>" + scheduleHelper.GetDayName(sc.IdWeek) + "</td>"
                        + "<td class='text-center align-content-center'><input type=\"text\" name=\"fromHourtxt\" value=\""+sc.FromHour.Value.ToString("hh:mm tt")+"\" class=\"form-control\"></td>"
                        + "<td class='text-center align-content-center'><input type=\"text\" name=\"toHourtxt\" value=\""+sc.ToHour.Value.ToString("hh:mm tt")+"\" class=\"form-control\"></td>"
                        + "<td class='text-center align-content-center'><select class='form-control' id='employeeId" + sc.IdWeek + IdClass + "' name='employeeId" + sc.IdWeek + IdClass + "'>"
                        + employeeOptions
                        + "</select></td>"
                        // Tạo select cho phòng học
                        + "<td class='text-center align-content-center'><select class='form-control' id='roomId" + sc.IdWeek + IdClass + "' name='roomId" + sc.IdWeek + IdClass + "'>"
                        + roomOptions
                        + "</select></td>"
                        + "</tr>"; 
                }
            }
            return Json(new { status = "ok", str },JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Submit_savechangeResult(string type, int studentId, int classId, int courseId, int registrationId,
                                 string listen, string view, string speed, string online, string totalScore,
                                 string focusGet, string focusNeed, string confidentGet, string confidentNeed,
                                 string rememberGet, string rememberNeed, string reflexGet, string reflexNeed,
                                 string orentation, string description)
        {
            try
            {
                // Kiểm tra sự tồn tại của học viên, lớp học và khóa học
                if (!await db.Students.AnyAsync(s => s.Id == studentId))
                {
                    return Json(new { status = "error", message = "Học viên không tồn tại." }, JsonRequestBehavior.AllowGet);
                }
                if (!await db.Classes.AnyAsync(c => c.Id == classId))
                {
                    return Json(new { status = "error", message = "Lớp học không tồn tại." }, JsonRequestBehavior.AllowGet);
                }
                if (!await db.Courses.AnyAsync(c => c.Id == courseId))
                {
                    return Json(new { status = "error", message = "Khóa học không tồn tại." }, JsonRequestBehavior.AllowGet);
                }

                // Tìm kết quả học tập dựa trên studentId, registrationId và type
                var resultCourse = await db.ResultCourses.SingleOrDefaultAsync(x => x.IdStudent == studentId && x.IdRegistration == registrationId && x.Type == type);

                if (resultCourse == null)
                {
                    // Nếu chưa có kết quả, tạo mới
                    var newResult = new ResultCourse
                    {
                        Type = type,
                        IdClass = classId,
                        IdCourse = courseId,
                        IdStudent = studentId,
                        IdRegistration = registrationId,
                        Listen = listen,
                        View = view,
                        Speed = speed,
                        Online = online,
                        TotalScore = totalScore,
                        ScoreBoard = totalScore,

                        Focus = focusGet,
                        FocusGet = focusGet,
                        FocusNeed = focusNeed,

                        Confident = confidentGet,
                        ConfidentGet = confidentGet,
                        ConfidentNeed = confidentNeed,

                        Remember = rememberGet,
                        RememberGet = rememberGet,
                        RememberNeed = rememberNeed,

                        Reflex = reflexGet,
                        ReflexGet = reflexGet,
                        ReflexNeed = reflexNeed,

                        Orentation = orentation,
                        Description = description,

                        DateCreate = DateTime.Now,
                        IdUser = Convert.ToInt32(CheckUsers.iduser()), // Giả sử CheckUsers trả về giá trị id người dùng hiện tại
                        EmailSendCount = 0
                    };

                    db.ResultCourses.Add(newResult);
                    await db.SaveChangesAsync();

                    return Json(new { status = "success", message = "Kết quả đã được lưu thành công." });
                }
                else
                {
                    // Nếu đã có kết quả, cập nhật thông tin
                    resultCourse.Listen = listen;
                    resultCourse.View = view;
                    resultCourse.Speed = speed;
                    resultCourse.Online = online;
                    resultCourse.TotalScore = totalScore;

                    resultCourse.FocusGet = focusGet;
                    resultCourse.FocusNeed = focusNeed;
                    resultCourse.ConfidentGet = confidentGet;
                    resultCourse.ConfidentNeed = confidentNeed;

                    resultCourse.RememberGet = rememberGet;
                    resultCourse.RememberNeed = rememberNeed;
                    resultCourse.ReflexGet = reflexGet;
                    resultCourse.ReflexNeed = reflexNeed;
                    resultCourse.Description = description;
                    resultCourse.Orentation = orentation;

                    db.Entry(resultCourse).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return Json(new { status = "success", message = "Kết quả đã được cập nhật thành công." });
                }
            }
            catch (DbEntityValidationException ex)
            {
                // Xử lý lỗi Validation của Entity Framework, trả về thông tin chi tiết
                var validationErrors = ex.EntityValidationErrors
                    .SelectMany(e => e.ValidationErrors)
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");

                return Json(new { status = "error", message = "Lỗi Validation xảy ra.", details = validationErrors });
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi chung chung khác
                return Json(new { status = "error", message = "Đã xảy ra lỗi trong quá trình lưu kết quả.", details = ex.Message });
            }
        }


        public ActionResult CheckinStudent()
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var linq = from c in db.Courses
                       join cb in db.CourseBranches on c.Id equals cb.IdCourse
                       where cb.IdBranch == idbranch
                       orderby c.Program.DisplayOrder, c.DisplayOrder
                       select c;
            ViewBag.IdClass = new SelectList(db.Classes.Where(x => x.Enable == true && x.IdBranch == idbranch), "Id", "Name");
            ViewBag.IdCourse = new SelectList(linq, "Id", "Name");
            return View();
        }
        public ActionResult GetStudentSchedule(int? IdClass, int? IdCourse,string searchString)
        {
            int idBranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "";

            // Lấy danh sách học viên còn đang học (Todate > DateTime.Now)
            var students = db.StudentJoinClasses
                     .Where(s => s.Todate > DateTime.Now &&
                                 (IdClass == null || s.IdClass == IdClass) &&
                                 (IdCourse == null || s.IdCourse == IdCourse) &&
                                 (string.IsNullOrEmpty(searchString) || s.Student.Name.Contains(searchString)))
                             .Select(s => new
                             {
                                 s.Id,
                                 s.Student.Name,
                                 ClassName = s.Class.Name,
                                 CourseName = s.Course.Name,
                                 s.Fromdate,
                                 s.Todate,
                                 s.IdStudent
                             })
                             .ToList();

            // Lấy số buổi của khóa học từ bảng CourseBranch
            var courseBranch = db.CourseBranches
                                 .Where(cb => cb.IdBranch == idBranch && cb.IdCourse == IdCourse)
                                 .FirstOrDefault();

            int numberOfSessions = courseBranch?.Sessons ?? 0;

            // Lấy thời khóa biểu từ bảng Schedule
            var schedule = db.Schedules
                             .Where(sc => (IdClass == null || sc.IdClass == IdClass) && sc.Active==true)
                             .OrderBy(sc => sc.IdWeek)
                             .ThenBy(sc => sc.FromHour)
                             .ToList();
            // Lấy lịch nghỉ của cơ sở hiện tại
            var vacationSchedules = db.VacationSchedules
                                      .Where(vs => vs.IdBranch == idBranch)
                                      .ToList();

            str += "<thead class=\"border-2 border-bottom border-muted\"><tr>"
                + "<th class='text-center'>STT</th>"
                + "<th style='min-width:200px;'>TÊN</th>"
                + "<th style='min-width:200px;'>KHÓA HỌC</th>"
                + "<th style='min-width:200px;'>LỚP HỌC</th>";

            for (int i = 1; i <= numberOfSessions; i++)
            {
                str += "<th class='text-center' style='min-width:120px;'>Buổi " + i + "</th>";
            }

            str += "</tr></thead>";
            int stt = 1;

            foreach (var student in students)
            {
                str += "<tr>"
                    + "<td class='text-center align-content-center'>" + stt + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center'>" + student.Name + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center'>" + student.CourseName + "</td>"
                    + "<td style='min-width:200px;' class='align-content-center'>" + student.ClassName + "</td>";
                int? IdStudent = student.IdStudent;
                // Tính toán ngày của từng buổi học
                DateTime startDate = student.Fromdate ?? DateTime.Now;
                List<DateTime> sessionDates = new List<DateTime>();

                // Lấy danh sách các ngày trong tuần có lịch học
                var daysOfWeek = schedule.Select(sc => sc.IdWeek).Distinct().ToList();

                // Tạo danh sách các buổi học dựa trên lịch học và số buổi yêu cầu
                for (int sessionCount = 1; sessionCount <= numberOfSessions;)
                {
                    // Kiểm tra từng ngày trong tuần có lịch học
                    foreach (var dayOfWeek in daysOfWeek)
                    {
                        if (sessionCount > numberOfSessions) break;

                        // Tính toán ngày học tiếp theo dựa trên ngày bắt đầu và ngày trong tuần
                        DateTime nextSessionDate = GetNextDateForDay(startDate, dayOfWeek);

                        if (nextSessionDate > student.Todate) break;

                        bool isVacation = vacationSchedules.Any(vs => vs.Fromdate <= nextSessionDate && vs.Todate >= nextSessionDate);

                        if (!isVacation)
                        {
                        sessionDates.Add(nextSessionDate);
                        sessionCount++;
                        }
                        // Cập nhật ngày bắt đầu là ngày kế tiếp
                        startDate = nextSessionDate.AddDays(1);
                    }
                }
                for (int i = 0; i < numberOfSessions; i++)
                {
                    if (i < sessionDates.Count)
                    {
                        // Lấy ngày học của buổi hiện tại
                        var currentSessionDate = sessionDates[i].Date;

                        // Truy vấn thông tin check-in của học viên cho ngày hiện tại
                        var studentCheckedIn = db.StudentCheckins.FirstOrDefault(x => x.IdStudent == student.IdStudent
                                              && x.IdClass == IdClass
                                              && DbFunctions.TruncateTime(x.DateCreate) == currentSessionDate);
                        //string studentStatus = studentCheckedIn.StatusCheckin.ToString();
                        // Kiểm tra nếu học viên đã check-in
                        if (studentCheckedIn != null)
                        {
                            // Học viên đã check-in
                            if (studentCheckedIn.StatusCheckin == true)
                            {
                                str += "<td class='text-center' style='min-width:120px;'>"
                                    + "<input type=\"checkbox\" data-bs-toggle=\"modal\" data-bs-target=\"#scheduleModal\" "
                                    + "id=\"selectToggle\" onclick=\"storeSelected(" + IdClass + ", " + student.IdStudent + ", '"
                                    + sessionDates[i].ToString("dd/MM") + "')\" class='form-check-input bg-success' checked /><br/>"
                                    + sessionDates[i].ToString("dd/MM/yyyy")
                                    + "</td>";
                            }
                            else
                            {
                                // Học viên vắng
                                str += "<td class='text-center' style='min-width:120px;'>"
                                    + "<input type=\"checkbox\" data-bs-toggle=\"modal\" data-bs-target=\"#scheduleModal\" "
                                    + "id=\"selectToggle\" onclick=\"storeSelected(" + IdClass + ", " + student.IdStudent + ", '"
                                    + sessionDates[i].ToString("dd/MM") + "')\" class='form-check-input bg-danger' checked/><br/>"
                                    + sessionDates[i].ToString("dd/MM/yyyy")
                                    + "</td>";
                            }
                        }
                        else
                        {
                            // Học viên chưa check-in
                            str += "<td class='text-center' style='min-width:120px;'>"
                                + "<input type=\"checkbox\" data-bs-toggle=\"modal\" data-bs-target=\"#scheduleModal\" "
                                + "id=\"selectToggle\" onclick=\"storeSelected(" + IdClass + ", " + student.IdStudent + ", '"
                                + sessionDates[i].ToString("dd/MM") + "')\" class='form-check-input' /><br/>"
                                + sessionDates[i].ToString("dd/MM/yyyy")
                                + "</td>";
                        }
                    }
                    else
                    {
                        // Không có buổi học nào nữa
                        str += "<td class='text-center'>-</td>";
                    }
                }


                str += "</tr>";
                stt++;
            }

            // Trả về chuỗi JSON
            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }

        // Hàm tính toán ngày học tiếp theo dựa trên ngày bắt đầu và ngày trong tuần
        private DateTime GetNextDateForDay(DateTime startDate, int dayOfWeek)
        {
            int daysToAdd = ((int)dayOfWeek - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysToAdd == 0 ? 7 : daysToAdd);
        }

        public JsonResult GetCheckedSessionDetaiByClass(int idClass, int studentId, string date)
        {
            DateTime sessionDate = DateTime.ParseExact(date, "dd/MM/yyyy", null);

            // Truy vấn dữ liệu check-in từ cơ sở dữ liệu
            var checkinData = db.StudentCheckins
                                .Where(x => x.IdClass == idClass && x.IdStudent == studentId && DbFunctions.TruncateTime(x.DateCreate) == sessionDate)
                                .Select(x => new {
                                    x.Lesson,
                                    x.OnClassNumber,
                                    x.OnClassRow,
                                    x.OnClassPaper,
                                    x.Other,
                                    x.Power,
                                    x.StatusCheckin,
                                    x.SendSMS,
                                    x.Complete,
                                    x.Exactly,
                                    x.HomeComplete,
                                    x.HomeExactly,
                                    x.Focus,
                                    x.Confident,
                                    x.Remember,
                                    x.Reflex,
                                    x.Description
                                })
                                .FirstOrDefault();

            // Trả về JSON cho client
            return Json(checkinData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateScheduleBulk(List<ScheduleUpdateModel> schedules)
        {
            try
            {
                // Duyệt qua danh sách các schedule và cập nhật từng mục
                foreach (var schedule in schedules)
                {
                    var duration = schedule.ToHour - schedule.FromHour;

                    // Kiểm tra nếu thời gian không hợp lệ (dưới 1 tiếng hoặc hơn 4 tiếng)
                    if ( duration.TotalHours < 1 || duration.TotalHours > 4)
                    {
                        return Json(new { status = "error", message = $"Khoảng thời gian xét thời khóa biểu không hợp lệ. Thời gian hiện tại: {duration.TotalHours} giờ." });
                    }
                    // Kiểm tra xem có lịch trùng không (trùng IdRoom, IdWeek và khoảng giờ)
                    var isConflict = db.Schedules.Any(s => s.IdRoom == schedule.RoomId
                                                           && s.IdWeek == schedule.ScheduleId
                                                           && s.IdClass != schedule.ClassId // Bỏ qua chính nó
                                                           && ((s.FromHour >= schedule.FromHour && s.FromHour < schedule.ToHour)  // Thời gian bắt đầu nằm trong khoảng
                                                                || (s.ToHour > schedule.FromHour && s.ToHour <= schedule.ToHour)   // Thời gian kết thúc nằm trong khoảng
                                                                || (s.FromHour <= schedule.FromHour && s.ToHour >= schedule.ToHour))); // Thời gian của lịch nằm bao trùm

                    if (isConflict)
                    {
                        // Nếu có xung đột lịch, trả về lỗi
                        return Json(new { status = "error", message = $"Lịch cho phòng {schedule.RoomId} vào khoảng thời gian {schedule.FromHour} - {schedule.ToHour} đã tồn tại." });
                    }

                    var dbSchedule = db.Schedules.FirstOrDefault(s => s.IdWeek == schedule.ScheduleId && s.IdClass==schedule.ClassId);  // Tìm schedule trong DB
                    if (dbSchedule != null)
                    {
                        dbSchedule.Active = schedule.IsActive;
                        dbSchedule.FromHour =schedule.FromHour;
                        dbSchedule.ToHour = schedule.ToHour;
                        dbSchedule.IdEmployee = schedule.EmployeeId;
                        dbSchedule.IdRoom = schedule.RoomId;

                        // Lưu thay đổi vào DB
                        db.SaveChanges();
                    }
                }

                // Trả về kết quả thành công
                return Json(new { status = "ok", message = "Lưu thay đổi thành công." });
            }
            catch (Exception ex)
            {
                // Trả về kết quả lỗi
                return Json(new { status = "error", message = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult UpdateStatus(int id, int status)
        {
            Class classes = db.Classes.Find(id);
            if (classes != null)
            {
                classes.Active = status == 1;
                db.SaveChanges();
                return Json(classes.Active);
            }
            else
            {
                return Json(false);
            }
        }
        // GET: Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser,Enable,Active")] Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.DateCreate = DateTime.Now;
                @class.IdBranch = int.Parse(CheckUsers.idBranch());
                @class.IdUser = int.Parse(CheckUsers.iduser());

                db.Classes.Add(@class);

                db.SaveChanges();

                var scheduleDefault = scheduleHelper.GetScheduleDefault(@class.Id);

                db.Schedules.AddRange(scheduleDefault);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", @class.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", @class.IdUser);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", @class.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", @class.IdUser);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser,Enable,Active")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", @class.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", @class.IdUser);
            return View(@class);
        }

        // GET: Classes/Filter/5

        public ActionResult Filter(int? id, int? idCourse, string studentName)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var studentJoinedClass = db.StudentJoinClasses.Where(x => x.IdClass == id).ToList();
            var course = db.Courses.Where(x => x.CourseBranches.Any(m => m.IdBranch == idbranch)).OrderBy(x => x.Program.DisplayOrder).OrderBy(x => x.DisplayOrder).ToList();

            if (idCourse != null)
            {
                studentJoinedClass = db.StudentJoinClasses.Where(x => x.IdCourse == idCourse).ToList();
            }

            if (!string.IsNullOrEmpty(studentName))
            {
                studentJoinedClass = db.StudentJoinClasses.Where(x => x.Student.Name.ToLower().Contains(studentName.ToLower())).ToList();
            }

            var schedulesbyClass = db.Schedules
                                    .Where(x => x.IdClass == id && (bool)x.Active).Include(x => x.Employee).ToList()
                                    .Select(x => scheduleHelper.GetDayName(x.IdWeek));

            List<ClassFilterDTO> timeTableData = new List<ClassFilterDTO>();

            var sessionNumber = 0;

            foreach (var item in studentJoinedClass)
            {
                ClassFilterDTO classFilterDTO = new ClassFilterDTO
                {
                    StudentID = (int)item.IdStudent,
                    CourseName = item.Course.Name,
                    StudentName = item.Student.Name
                };

                var sessions = new List<Session>();

                for (var date = (DateTime)item.Fromdate; date <= item.Todate; date = date.AddDays(1))
                {
                    var DayOfWeekVNCompared = scheduleHelper.ConvertEnglishDayToVietnamese(date.DayOfWeek.ToString());
                    var scheduleMatched = schedulesbyClass.FirstOrDefault(x => x == DayOfWeekVNCompared);

                    if (scheduleMatched != null)
                    {
                        var studentCheckedIn = db.StudentCheckins
                        .Where(x => x.IdStudent == item.IdStudent &&
                        x.IdClass == id).ToList();

                        var DateCreate = studentCheckedIn
                                     .Select(x => new { datetime = (DateTime)x.DateCreate, checkInStatus = x.StatusCheckin })
                                     .Select(x => new { datetime = x.datetime.ToString("dd:MM:yyyy"), checkInStatus = x.checkInStatus })
                                     .ToList();

                        var dateChecked = DateCreate.FirstOrDefault(x => x.datetime == date.ToString("dd:MM:yyyy"));

                        bool? isCheckedIn = null;

                        if (studentCheckedIn != null && studentCheckedIn.Any() && dateChecked != null)
                        {
                            isCheckedIn = dateChecked.checkInStatus;
                        }

                        sessions.Add(new Session
                        {
                            DayOfWeek = DayOfWeekVNCompared,
                            Date = date.ToString("dd/MM"),
                            IsCheckedIn = isCheckedIn
                        });
                    }
                }

                classFilterDTO.Sessions = sessions;

                sessionNumber = sessions.Count;

                timeTableData.Add(classFilterDTO);
            }
            ViewBag.ClassSelectedId = id;
            ViewBag.IdClass = new SelectList(db.Classes.Where(x => x.Enable == true && x.IdBranch == idbranch), "Id", "Name");
            ViewBag.IdCourse = new SelectList(course, "Id", "Name");
            ViewBag.StudentJoinedClass = studentJoinedClass;
            ViewBag.TimeTableData = timeTableData;
            ViewBag.SessionNumber = sessionNumber;

            return View();
        }
       
        // POST: Classes/CheckIn
        [HttpPost]
        public ActionResult CheckIn(int? classId,
            int studentId,
            bool checkIn,
            string dateCheckedIn,
            string lesson,
            string completely,
            string exactRate,
            string textNumber,
            string row,
            string second,
            string home_completely,
            string home_exactRate,
            string focusRate,
            string confidentRate,
            string rememberRate,
            string reflexRate,
            string other,
            int power,
            string description,
            bool isSMS)
        {
            string status = "error";
            string message = "Đã xảy ra lỗi khi check-in.";

            try
            {
                if (!db.Students.Any(s => s.Id == studentId))
                {
                    message = "Học viên không tồn tại.";
                    return Json(new { status = "error", message }, JsonRequestBehavior.AllowGet);
                }
                if (!db.Classes.Any(c => c.Id == classId))
                {
                    message = "Lớp học không tồn tại.";
                    return Json(new { status = "error", message }, JsonRequestBehavior.AllowGet);
                }
                if (lesson.Length > 250)
                {
                    message = "Tên bài học quá dài.";
                    return Json(new { status = "error", message }, JsonRequestBehavior.AllowGet);
                }
                // Chuyển đổi dateCheckedIn thành DateTime với định dạng dd/MM/yyyy
                var dateConverted = DateTime.ParseExact(dateCheckedIn, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                // Kiểm tra sự tồn tại của bản ghi CheckIn
                var checkInExisted = db.StudentCheckins.FirstOrDefault(x => x.IdClass == classId
                    && x.IdStudent == studentId
                    && DbFunctions.TruncateTime(x.DateCreate) == dateConverted.Date);

                if (checkInExisted != null)
                {
                    db.StudentCheckins.Remove(checkInExisted);
                    db.SaveChanges();
                }

                // Tạo mới thông tin CheckIn
                var checkInDetail = new StudentCheckin
                {
                    IdStudent = studentId,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    DateCreate = dateConverted,
                    Enable = true,
                    Status = true,
                    StatusCheckin = checkIn,
                    IdClass = classId,
                    Lesson = lesson,
                    Complete = completely,
                    Exactly = exactRate,
                    Power = power,
                    Other = other,
                    Remember = rememberRate,
                    Reflex = reflexRate,
                    Confident = confidentRate,
                    Focus = focusRate,
                    Description = description,
                    Homework = "",
                    OnClassPaper = second,
                    OnClassNumber = textNumber,
                    OnClassRow = row,
                    HomeComplete = home_completely,
                    HomeExactly = home_exactRate,
                    PracticeOnline = "",
                    SendSMS = Convert.ToString(isSMS)
                };

                if (ModelState.IsValid)
                {
                    db.StudentCheckins.Add(checkInDetail);
                    db.SaveChanges();
                    status = "success";
                    message = "Check-in thành công.";
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    message = "Dữ liệu không hợp lệ.";
                }

                // Gửi tin nhắn SMS nếu isSMS = true
                if (isSMS)
                {
                    SendCheckInSMS(studentId, dateConverted, classId);
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                var errorMessages = dbEx.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                message = "Validation error: " + fullErrorMessage;
            }
            catch (FormatException ex)
            {
                // Xử lý lỗi định dạng ngày không hợp lệ
                message = "Định dạng ngày không hợp lệ.";
            }
            catch (Exception ex)
            {
                // Ghi lại InnerException (nếu có)
                if (ex.InnerException != null)
                {
                    message = "Đã xảy ra lỗi khi check-in: " + ex.InnerException.Message;
                }
                else
                {
                    message = "Đã xảy ra lỗi khi check-in: " + ex.Message;
                }
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        // Hàm gửi tin nhắn SMS khi check-in thành công
        private void SendCheckInSMS(int studentId, DateTime dateCheckedIn, int? classId)
        {
            // Gửi SMS cho học viên
            var student = db.Students.Find(studentId);
            if (student != null && !string.IsNullOrEmpty(student.Phone))
            {
                string message = $"Học viên {student.Name} đã check-in vào lớp học {classId} ngày {dateCheckedIn:dd/MM/yyyy}.";
                // Gọi hàm gửi SMS của bạn ở đây, ví dụ: SMSService.Send(student.Phone, message);
            }
        }

        public ActionResult Loadedit_class(int id) { 
            var c = db.Classes.Find(id);
            return Json(new {id=c.Id,code=c.Code,name=c.Name,description=c.Description},JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_savechanges(string action, int? Id, string Code,string Name, string Description)
        {
            int iduser= Convert.ToInt32(CheckUsers.iduser());
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            if (action == "create")
            {
                if(!db.Employees.Any(x=>x.Enable==true && x.IsOfficial == true && x.IdBranch == idbranch))
                {
                    return Json(new { status = "error", message = "Lỗi cập nhật: Cơ sở chưa có nhân sự!" }, JsonRequestBehavior.AllowGet);
                }
                if (!db.Rooms.Any(x => x.IdBranch == idbranch))
                {
                    return Json(new { status = "error", message = "Lỗi cập nhật: Cơ sở chưa có phòng học!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Class cla = new Class()
                    {
                        Name = Name,
                        Description = Description,
                        DateCreate = DateTime.Now,
                        IdBranch = idbranch,
                        IdUser = iduser,
                        Enable = true,
                        Active = true,
                        Code = Code
                    };
                    db.Classes.Add(cla);
                    var scheduleDefault = scheduleHelper.GetScheduleDefault(cla.Id);
                    db.Schedules.AddRange(scheduleDefault);
                    db.SaveChanges();
                    return Json(new { status = "ok", message = "Thành công: Đã thêm mới lớp học!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var c = db.Classes.Find(Id);
                c.Name = Name;
                c.Code = Code;
                c.Description = Description;
                db.Entry(c).State=EntityState.Modified;
                db.SaveChanges();
                return Json(new { status="ok", message = "Thành công: Đã cập nhật lớp học!"}, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> Delete_Classes(int id)
        {
            var classes = await db.Classes.FindAsync(id);
            if (classes == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra khóa ngoại
            if (db.StudentJoinClasses.Any(s => s.IdClass == id))
            {
                if (db.StudentJoinClasses.Any(x => x.IdClass == id && x.Todate > DateTime.Now))
                {
                    return Json(new { status = "error", message = "Lỗi cập nhật: Đang có học viên đang học lớp này!" }, JsonRequestBehavior.AllowGet);
                }
                classes.Enable = false;
                db.Entry(classes).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "ok", message = "Thành công: Lớp học đã được cập nhật!" }, JsonRequestBehavior.AllowGet);
            }

            db.Classes.Remove(classes);
            await db.SaveChangesAsync();

            return Json(new { status = "ok", message = "Thành công: Lớp học đã được xóa!" }, JsonRequestBehavior.AllowGet);
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
