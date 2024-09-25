using SuperbrainManagement.Helpers;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers
{
    public class HomeController : Controller
    {
        ModelDbContext db= new ModelDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Loadlist_thongbao()
        {
            string str = "";
            var list = db.Feeds.Where(x => x.Todate > DateTime.Now && x.Active == true && x.Enable==true).OrderByDescending(x => x.Id).Take(7).ToList(); 
            foreach(var item in list)
            {
                str += "<li class=\"list-group-item border-0 d-flex justify-content-between ps-0 mb-3 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"me-3 text-center\">" +
                                    "<img src=\""+(item.User.Employee.Image==null? "/Assets/images/profile/user-1.jpg" : item.User.Employee.Image)+"\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">" +
                                "</div>" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0'><a href=\"javascript:View_thongbao("+item.Id+")\" class='text-muted fw-bolder'>"+item.Name+"</a></h6>" +
                                    "<small class=\"fst-italic\"><i class=\"ti ti-user-circle\" aria-hidden=\"true\"></i> " + item.User.Name + " , <i class=\"ti ti-clock\" aria-hidden=\"true\"></i> " + item.DateCreate.Value.ToString("dd/MM/yyyy") + "</small>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:View_thongbao("+item.Id+")\" class=\"btn btn-sm text-dark icon-move-right my-auto\"><i class=\"ti ti-chevron-right\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>";
            }
            var json = new
            {
                str
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_thongbao(int id)
        {
            var list = db.Feeds.Find(id);
            var json = new
            {
                tieude =list.Name,
                ngay =list.DateCreate.Value.ToString("dd/MM/yyyy"),
                str = list.Description
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Loadlist_vattu()
        {
            string str = "";
            string idBranch = CheckUsers.idBranch();
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Name,p.Image,p.Unit,p.Price,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + idBranch + "), 0) -"
                                    + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + idBranch + "), 0) AS Tonkho"
                                    + " FROM product p"
                                    + " where p.enable=1 ";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int dinhmuc = Convert.ToInt32(reader["Quota"]);
                    int tonkho = Convert.ToInt32(reader["tonkho"]);
                    if (tonkho <= dinhmuc)
                    {
                        str += "<li class=\"list-group-item border-0 d-flex justify-content-between ps-0 mb-3 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"me-3 text-center\">" +
                                    "<img src=\"" + (reader["Image"].ToString() == null ? "/assets/images/logos/icon web.png" : reader["Image"]) + "\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">" +
                                "</div>" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0'><a href=\"javascript:void(0)\" class='text-muted fw-bold'>" + reader["Name"] + "</a></h6>" +
                                    "<small class=\"fst-italic\"><i class=\"ti ti-basket\" aria-hidden=\"true\"></i> Tồn kho: " + tonkho + ".</small>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:void(0)\" class=\"btn btn-sm text-dark icon-move-right my-auto\"><i class=\"ti ti-chevron-right\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>";
                    }
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadNumber_statistics()
        {
            int NumberStudent = 0, NumberOnClass = 0, NumberWaiting = 0;
            int NumberStudentPrevMonth = 0, NumberOnClassPrevMonth = 0, NumberWaitingPrevMonth = 0;
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());

            DateTime prevMonthDate = DateTime.Now.AddMonths(-1);
            // Lấy tất cả sinh viên thuộc chi nhánh
            var students = db.Students.Where(x => x.IdBranch == idbranch).Select(x => x.Id).ToList();
            if (students.Any())
            {
                // Đếm số sinh viên đã đăng ký học (CheckStatusStudent)
                NumberStudent = db.Registrations
                    .Where(r => students.Contains((int)r.IdStudent))
                    .Select(r => r.IdStudent)
                    .Distinct()
                    .Count();

                // Đếm số sinh viên đang trong lớp (CheckOnClass)
                NumberOnClass = db.StudentJoinClasses
                    .Where(sj => students.Contains((int)sj.IdStudent) && sj.Todate > DateTime.Now)
                    .Select(sj => sj.IdStudent)
                    .Distinct()
                    .Count();

                // Đếm số sinh viên đang chờ (CheckWaiting)
                NumberWaiting = db.StudentJoinClasses
                    .Where(sj => students.Contains((int)sj.IdStudent) && sj.Todate < DateTime.Now)
                    .Join(db.RegistrationCourses,
                          sj => sj.IdRegistration,
                          rc => rc.IdRegistration,
                          (sj, rc) => new { sj.IdStudent, rc.StatusJoinClass })
                    .Where(x => x.StatusJoinClass==false)
                    .Select(x => x.IdStudent)
                    .Distinct()
                    .Count();

                // Thống kê cho cùng ngày tháng trước
                NumberStudentPrevMonth = db.Registrations
                    .Where(r => students.Contains((int)r.IdStudent) && r.DateCreate <= prevMonthDate)
                    .Select(r => r.IdStudent)
                    .Distinct()
                    .Count();

                NumberOnClassPrevMonth = db.StudentJoinClasses
                    .Where(sj => students.Contains((int)sj.IdStudent) && sj.Todate > prevMonthDate)
                    .Select(sj => sj.IdStudent)
                    .Distinct()
                    .Count();

                NumberWaitingPrevMonth = db.StudentJoinClasses
                    .Where(sj => students.Contains((int)sj.IdStudent) && sj.Todate < prevMonthDate)
                    .Join(db.RegistrationCourses,
                          sj => sj.IdRegistration,
                          rc => rc.IdRegistration,
                          (sj, rc) => new { sj.IdStudent, rc.StatusJoinClass })
                    .Where(x =>x.StatusJoinClass==false)
                    .Select(x => x.IdStudent)
                    .Distinct()
                    .Count();
            }
            double percentChangeStudent = CalculatePercentageChange(NumberStudent, NumberStudentPrevMonth);
            double percentChangeOnClass = CalculatePercentageChange(NumberOnClass, NumberOnClassPrevMonth);
            double percentChangeWaiting = CalculatePercentageChange(NumberWaiting, NumberWaitingPrevMonth);

            return Json(new
            {
                NumberStudent,
                NumberOnClass,
                NumberWaiting,
                percentChangeStudent,
                percentChangeOnClass,
                percentChangeWaiting
            }, JsonRequestBehavior.AllowGet);
        }
        // Hàm tính phần trăm thay đổi
        private double CalculatePercentageChange(int currentValue, int previousValue)
        {
            if (previousValue == 0)
                return currentValue > 0 ? 100 : 0; // Tránh chia cho 0
            return Math.Round(((double)(currentValue - previousValue) / previousValue) * 100, 1);
        }

        public ActionResult GetInfoBranch()
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var branch = db.Branches.Find(idbranch);
            string str = "";
            if (branch == null) {
                str = "<li class=\"list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"me-3 text-center\">" +
                                    "<img src=\"" + "/Assets/images/profile/user-1.jpg" + "\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">" +
                                "</div>" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0'>Không tìm thấy thông tin cơ sở</h6>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:void(0)\" class=\"btn btn-sm text-dark icon-move-right my-auto\"><i class=\"ti ti-chevron-right\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>";
            }
            else
            {
                str = "<li class=\"list-group-item border-0 d-flex justify-content-between mb-0 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0 fw-bold'>" + branch.Name + "</h6>" +
                                    "<small class=\"fst-italic mt-1\"><i class=\"ti ti-phone\" aria-hidden=\"true\"></i> " + branch.Phone + " , <i class=\"ti ti-mail\" aria-hidden=\"true\"></i> " + branch.Email + "</small>" +
                                    "<small class=\"fst-italic mt-1\"><i class=\"ti ti-address-book\" aria-hidden=\"true\"></i> " + branch.Address + "</small>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:void(0)\" class=\"btn btn-sm text-dark icon-move-right my-auto\"><i class=\"ti ti-chevron-right\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>"+
                        "<li class=\"list-group-item border-0 d-flex justify-content-between mb-0 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0'><i class='ti ti-list'></i> Danh sách học viên đang học</h6>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:void(0)\" class=\"btn btn-sm text-dark icon-move-right my-auto\"><i class=\"ti ti-chevron-right\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>"+
                        "<li class=\"list-group-item border-0 d-flex justify-content-between mb-0 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0'><i class='ti ti-list'></i> Danh sách tính phí T</h6>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:void(0)\" class=\"btn btn-sm text-dark icon-move-right my-auto\"><i class=\"ti ti-chevron-right\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>";
            }
            return Json(new {str}, JsonRequestBehavior.AllowGet); 
        }
    }
}