using System;
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

namespace SuperbrainManagement.Controllers
{
    public class ClassesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        private ScheduleHelper scheduleHelper = new ScheduleHelper();

        // GET: Classes
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string idBranch)
        {
            var branches = db.Branches.ToList();
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                branches = db.Branches.Where(x => x.Id == idbranch).ToList();
            }
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = branches.First().Id.ToString();
            }
            ViewBag.IdBranch = new SelectList(branches, "Id", "Name", idBranch);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            ViewBag.Schedule = db.Schedules.Include(x => x.Employee).Include(x => x.Class).Include(x => x.User).ToList();

            ViewBag.EmployeeDDData = db.Employees.Where(x => x.IdBranch == idbranch).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            ViewBag.RoomDDData = db.Rooms.Where(x => x.IdBranch == idbranch).Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();

            ViewBag.UserId = int.Parse(CheckUsers.iduser());

            var classes = db.Classes.ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                classes = classes.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    classes = classes.OrderByDescending(s => s.Name).ToList();
                    break;
                case "date":
                    classes = classes.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    classes = classes.OrderBy(s => s.Name).ToList();
                    break;
                default:
                    classes = classes.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = classes.ToPagedList(pageNumber, pageSize);

            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };

            ViewBag.PagedListRenderOptions = pagedListRenderOptions;

            return View(pagedData);
        }

        [HttpPost]
        public ActionResult UpdateScheduleBulk(
            string selectedClassId,
            string scheduleData)
        {
            if (ModelState.IsValid)
            {
                var updatedData = JsonConvert.DeserializeObject<List<ScheduleViewDTO>>(scheduleData);

                var scheduleDataUpdated = AutoMapperConfig.Mapper.Map<List<Schedule>>(updatedData);
                scheduleDataUpdated.ForEach(x =>
                {
                    db.Entry(x).State = EntityState.Modified;
                });

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // If ModelState is not valid, return the view with validation errors
            return View();
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
        /*
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
            ViewBag.IdClass = new SelectList(db.Classes.Where(x=>x.Enable==true && x.IdBranch == idbranch), "Id", "Name");
            ViewBag.IdCourse = new SelectList(course, "Id", "Name");
            ViewBag.StudentJoinedClass = studentJoinedClass;
            ViewBag.TimeTableData = timeTableData;
            ViewBag.SessionNumber = sessionNumber;

            return View();
        }
        */
        public ActionResult Filter(int? id, int? idCourse, string studentName)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());

            // Lọc sinh viên theo lớp học và khóa học
            var studentJoinedClass = db.StudentJoinClasses.AsQueryable();

            if (id != null)
            {
                studentJoinedClass = studentJoinedClass.Where(x => x.IdClass == id);
            }

            if (idCourse != null)
            {
                studentJoinedClass = studentJoinedClass.Where(x => x.IdCourse == idCourse);
            }

            if (!string.IsNullOrEmpty(studentName))
            {
                studentJoinedClass = studentJoinedClass.Where(x => x.Student.Name.ToLower().Contains(studentName.ToLower()));
            }

            var studentJoinedClassList = studentJoinedClass.ToList();

            var course = db.Courses
                           .Where(x => x.CourseBranches.Any(m => m.IdBranch == idbranch))
                           .OrderBy(x => x.Program.DisplayOrder)
                           .ThenBy(x => x.DisplayOrder)
                           .ToList();

            var schedulesbyClass = db.Schedules
                                     .Where(x => x.IdClass == id && (bool)x.Active)
                                     .Include(x => x.Employee)
                                     .ToList()
                                     .Select(x => scheduleHelper.GetDayName(x.IdWeek));

            List<ClassFilterDTO> timeTableData = new List<ClassFilterDTO>();
            int sessionNumber = 0;

            foreach (var item in studentJoinedClassList)
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
                                                             x.IdClass == id)
                                                 .ToList();

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

                sessionNumber = Math.Max(sessionNumber, sessions.Count);  // Cập nhật số lượng sessions tối đa
                timeTableData.Add(classFilterDTO);
            }

            ViewBag.ClassSelectedId = id;
            ViewBag.IdClass = new SelectList(db.Classes.Where(x => x.Enable == true && x.IdBranch == idbranch), "Id", "Name");
            ViewBag.IdCourse = new SelectList(course, "Id", "Name");
            ViewBag.StudentJoinedClass = studentJoinedClassList;
            ViewBag.TimeTableData = timeTableData;
            ViewBag.SessionNumber = sessionNumber;

            return View();
        }



        public ActionResult GetCheckedSessionDetaiByClass(int idClass, int studentId, string date)
        {
            var dateConverted = Convert.ToDateTime(date);
            var StudentDetailCheckInByClassAndDate = db.StudentCheckins.FirstOrDefault(x => x.IdClass == idClass
            && x.IdStudent == studentId
            && (DateTime)x.DateCreate == dateConverted);

            var item = StudentDetailCheckInByClassAndDate == null ?
                null
                :
                new
                {
                    StatusCheckin = StudentDetailCheckInByClassAndDate.StatusCheckin,
                    Lesson = StudentDetailCheckInByClassAndDate.Lesson,
                    Complete = StudentDetailCheckInByClassAndDate.Complete,
                    Exactly = StudentDetailCheckInByClassAndDate.Exactly,
                    OnClassNumber = StudentDetailCheckInByClassAndDate.OnClassNumber,
                    OnClassRow = StudentDetailCheckInByClassAndDate.OnClassRow,
                    OnClassPaper = StudentDetailCheckInByClassAndDate.OnClassPaper,
                    HomeComplete = StudentDetailCheckInByClassAndDate.HomeComplete,
                    HomeExactly = StudentDetailCheckInByClassAndDate.HomeExactly,
                    Focus = StudentDetailCheckInByClassAndDate.Focus,
                    Confident = StudentDetailCheckInByClassAndDate.Confident,
                    Remember = StudentDetailCheckInByClassAndDate.Remember,
                    Reflex = StudentDetailCheckInByClassAndDate.Reflex,
                    Other = StudentDetailCheckInByClassAndDate.Other,
                    Power = StudentDetailCheckInByClassAndDate.Power,
                    SendSMS = StudentDetailCheckInByClassAndDate.SendSMS
                };

            return Json(item, JsonRequestBehavior.AllowGet);
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
            bool isSMS
            )
        {
            var dateConverted = Convert.ToDateTime(dateCheckedIn);

            var checkInExisted = db.StudentCheckins.FirstOrDefault(x => x.IdClass == classId
            && x.IdStudent == studentId
            && (DateTime)x.DateCreate == dateConverted);

            if (checkInExisted != null)
            {
                db.StudentCheckins.Remove(checkInExisted);
                db.SaveChanges();
            }

            var checkInDetail = new StudentCheckin
            {
                IdClass = classId,
                DateCreate = DateTime.ParseExact(dateCheckedIn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                IdStudent = studentId,
                IdUser = int.Parse(CheckUsers.iduser()),
                Enable = true,
                Status = true,
                StatusCheckin = checkIn,
                Lesson = lesson,
                Complete = completely,
                Exactly = exactRate,
                OnClassNumber = textNumber,
                OnClassRow = row,
                OnClassPaper = second,
                HomeComplete = home_completely,
                HomeExactly = home_exactRate,
                Focus = focusRate,
                Confident = confidentRate,
                Remember = rememberRate,
                Reflex = reflexRate,
                Other = other,
                Power = power,
                SendSMS = Convert.ToString(isSMS)
            };
            db.StudentCheckins.Add(checkInDetail);
            db.SaveChanges();

            return RedirectToAction("Filter", new { id = classId });
        }

        public async Task<ActionResult> Delete_Classes(int id)
        {
            var status = "ok";
            var message = "Phòng đã được xóa thành công.";

            var room = await db.Classes.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra khóa ngoại
            var hasJoinClass = db.StudentJoinClasses.Any(s => s.IdClass == id);

            if (hasJoinClass)
            {
                status = "error";
                message = "Không thể xóa lớp này vì đang có học viên tham gia lớp học này.";
                return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
            }

            db.Classes.Remove(room);
            await db.SaveChangesAsync();

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
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
