using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using Google.Protobuf.WellKnownTypes;
using Mysqlx.Crud;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using SuperbrainManagement.DTOs;
using SuperbrainManagement.Helpers;
using System.IO;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.BuilderProperties;
using Org.BouncyCastle.Utilities.Encoders;
using System.Web.Helpers;
using System.Xml.Linq;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Transactions;
using Transaction = SuperbrainManagement.Models.Transaction;
using Microsoft.Ajax.Utilities;

namespace SuperbrainManagement.Controllers
{
    public class StudentsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();
        private ScheduleHelper scheduleHelper = new ScheduleHelper();
        private StudentHelper studentHelper = new StudentHelper();

        public enum FilterEnum
        {
            Potential = 1,
            Official = 0
        }

        // GET: Students
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string IdBranch, FilterEnum filterEnum = FilterEnum.Official)
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            var branches = db.Branches.Where(x=>x.Enable==true).ToList();
            int idbranch = int.Parse(CheckUsers.idBranch());
            ViewBag.IdBranch = new SelectList(branches, "Id", "Name", IdBranch);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var students = filterEnum == FilterEnum.Potential ? studentHelper.GetPotentialStudent() : studentHelper.GetOfficialStudent();
            if (!string.IsNullOrEmpty(IdBranch))
            {
                students = students.Where(x => x.IdBranch == int.Parse(IdBranch)).ToList();
                ViewBag.CurrentBranch=IdBranch;
            }
            else
            {
                students = students.Where(x => x.IdBranch == idbranch).ToList();
                ViewBag.CurrentBranch= idbranch;
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name).ToList();
                    break;
                case "date":
                    students = students.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    students = students.OrderBy(s => s.Name).ToList();
                    break;
                default:
                    students = students.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 50;
            int pageNumber = (page ?? 1);

            var studentsWithStatus = filterEnum == FilterEnum.Potential ?
                                                    studentHelper.GetOfficialStudentWithStatus(students, true)
                                                    : studentHelper.GetOfficialStudentWithStatus(students, false);

            var pagedData = studentsWithStatus.ToPagedList(pageNumber, pageSize);

            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };

            ViewBag.PagedListRenderOptions = pagedListRenderOptions;
            ViewBag.SelectedFilter = filterEnum;
            return View(pagedData);
        }

        public ActionResult Loadlist(string sortOrder, string searchString, int? idBranch, bool? status = true)
        {
            string str = "",trangthai="";
            if (idBranch == null)
            {
                idBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            // Lấy danh sách học viên
            var students = db.Students
                .Where(s => s.Enable == true && s.IdBranch == idBranch);

            // Tìm kiếm học viên theo tên nếu có searchString
            if (!string.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString));
            }

            // Phân loại học viên chính thức và tiềm năng
            if (status == true || status == null)
            {
                // Lọc học viên chính thức (có đăng ký trong Registration)
                students = students.Where(s => db.Registrations.Any(r => r.IdStudent == s.Id));
            }
            else
            {
                // Lọc học viên tiềm năng (không có đăng ký trong Registration)
                students = students.Where(s => !db.Registrations.Any(r => r.IdStudent == s.Id));
            }

            // Sắp xếp danh sách học viên
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;
                case "name":
                    students = students.OrderBy(s => s.Name);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.Id);
                    break;
                case "date":
                    students = students.OrderBy(s => s.Id);
                    break;
                default:
                    students = students.OrderByDescending(s => s.Id);
                    break;
            }
            if (!students.Any())
            {
                str = "<tr><td colspan=8 class='text-center'>Không tìm thấy dữ liệu học viên!</td></tr>";
                return Json(new { str }, JsonRequestBehavior.AllowGet);
            }
            int count = 0;
            foreach (var item in students)
            {
                if (!db.Registrations.Any(x => x.IdStudent == item.Id))
                {
                    trangthai = "<span class=\"badge text-success bg-light\">Tiềm năng</span>";
                }
                else
                {
                    // Xác định trạng thái học viên
                    if (db.StudentJoinClasses.Any(jc => jc.IdStudent == item.Id && jc.Todate > DateTime.Today))
                    {
                        trangthai = "<span class=\"badge text-white bg-success\">Đang học</span>";
                    }
                    else if (db.RegistrationCourses.Any(rc => rc.Registration.IdStudent == item.Id &&
                                                              (rc.StatusJoinClass == null || rc.StatusJoinClass == false)))
                    {
                        trangthai = "<span class=\"badge text-white bg-info\">Đang đợi xét lớp</span>";
                    }
                    else
                    {
                        trangthai = "<span class=\"badge text-white bg-danger\">Đã kết khóa</span>";
                    }
                }
                

                count++;
                str += "<tr>"
                    + "<td class='text-center'><a href=\"/students/details/"+item.Id+"\" class='text-muted'>" + count + "</a></td>"
                    + "<td class='text-center'><a href=\"/students/details/"+item.Id+ "\" class='text-muted'>" + item.Code + "</a></td>"
                    + "<td class='text-start'><a href=\"/students/details/"+item.Id+ "\" class='text-muted'>" + item.Name + "</a></td>"
                    + "<td class='text-center'><a href=\"/students/details/"+item.Id+ "\" class='text-muted'>" + item.DateOfBirth.Value.ToString("dd/MM/yyyy") + "</a></td>"
                    + "<td class='text-center'><a href=\"/students/details/"+item.Id+ "\" class='text-muted'>" + item.Phone + "</a></td>"
                    + "<td class='text-start'><a href=\"/students/details/"+item.Id+ "\" class='text-muted'>" + item.User.Name + "</a></td>"
                    + "<td class='text-center'><a href=\"/students/details/"+item.Id+ "\" class='text-muted'>" + trangthai + "</a></td>"
                    + "<td class='text-end'>"
                    + "<a href=\"/students/details/"+item.Id+"\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                    + "<a href=\"javascript:Submit_Delete("+item.Id+")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                    + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\"><i class=\"ti ti-dots-vertical\"></i></a>"
                    + "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                    + "<li><a class=\"dropdown-item\" href=\"/Students/AddCourseProgramOfStudents?IdStudent="+item.Id+"\"><i class=\"ti ti-script-plus\"></i> Đăng ký khóa học</a></li>"
                    + "<li><a class=\"dropdown-item\" href=\"javascript:exchangeBranch("+item.Id+")\"><i class=\"ti ti-exchange\"></i> Chuyển cơ sở</a></li>"
                    + "<li><a class=\"dropdown-item\" href=\"/Students/Sendemail/"+item.Id+"\"><i class=\"ti ti-mail-forward\"></i> Gửi email</a></li>"
                    + "</ul>"
                    + "</td>"
                    + "</tr>";
            }
            return Json(new {str},JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddCourseProgramOfStudents(int IdStudent)
        {
            Student student = Connect.SelectSingle<Student>("select * from Student where Id='" + IdStudent + "'");
            Session["infoUser"] = student;
            return View(student);
        }
       
        public ActionResult RegistrationPrints(int? IdRegistration)
        {
            if (IdRegistration == null)
            {
                return Redirect("/Error/E404");
            }
            var linq = (from reg in db.Registrations
                       join s in db.Students on reg.IdStudent equals s.Id
                       join b in db.Branches on s.IdBranch equals b.Id
                       where reg.Id == IdRegistration
                       select new { 
                            Name = s.Name,
                            dienthoai = s.Phone,
                            email = s.Email,
                            NameBranch = b.Name,
                            PhoneB = b.Phone,
                            EmailB = b.Email,
                            AddressB = b.Address,
                            CodeInvoice = reg.Code,
                            DateInvoice = reg.DateCreate
                       }).FirstOrDefault();
            TempData["ma_hoadon"] = linq.CodeInvoice;
            TempData["date_hoadon"] = linq.DateInvoice.Value.ToString("dd/MM/yyyy");
            TempData["ten_hocvien"] = linq.Name;
            TempData["info_hocvien "] = "Số điện thoại: " + linq.dienthoai + "<br/> Email:" + linq.email;
            TempData["ten_coso"] = linq.NameBranch;
            TempData["dienthoai"] = linq.PhoneB;
            TempData["diachi"] = linq.AddressB;
            TempData["email"] = linq.EmailB;
            var rec = (from reg in db.Registrations
                       join rc in db.RegistrationCourses on reg.Id equals rc.IdRegistration
                       where reg.Id == IdRegistration
                       select new{

                       }).ToList();
            return View();
        }
        [HttpPost] // Use POST for actions that modify data
        public ActionResult Deletes(int IdCourse, int IdRegistration)
        {
            try
            {
                var deleteItem = db.RegistrationCourses.FirstOrDefault(x => x.IdCourse == IdCourse && x.IdRegistration == IdRegistration);
                if (deleteItem != null)
                {
                    db.RegistrationCourses.Remove(deleteItem);
                    db.SaveChanges();
                    return Json(deleteItem.IdRegistration);

                }
                else
                {
                    return Json(new { success = false, message = "Item not found" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Item not found" });
            }
        }
        public string GenerateCode(int IdBranch)
        {
            string code=db.Branches.Find(IdBranch).Code;
            string prefix = "DK_"+code;

            // Generate a random number
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999); // Adjust range as needed
            var registation = db.Registrations.Where(x=>x.IdBranch==IdBranch).Count();
            // Generate the new code
            string newCode = $"{prefix}_{registation+1}";

            return newCode;
        }
        string Getcode_Student(int IdBranch)
        {
            int nextCode = db.Students.Where(x=>x.IdBranch== IdBranch).Count() + 1;
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(IdBranch);
            string str = cn.Code +"-"+ code;
            return str;
        }

        public ActionResult Registrations(int idStudent,int? idRegistration)
        {
            ViewBag.IdStudent = idStudent;
            ViewBag.IdRegistration = idRegistration;
            return View();
        }
        public decimal Check_paymentRegistration(int IdRegistration) {
            decimal vouc = 0;
            var voucher = db.StudentVouchers.Where(x=>x.IdTransaction== IdRegistration);
            if (voucher != null) {
                foreach(var i in voucher)
                {
                    vouc +=(decimal)i.Voucher;
                }
            }
            decimal tongthanhtoan = 0;
            var trans = db.Transactions.Where(x => x.IdRegistration ==IdRegistration);
            if (trans.Count() > 0) {
                foreach(var item in trans)
                {
                    tongthanhtoan +=(decimal)item.TotalAmount;
                }
            }
            else
            {
                tongthanhtoan = 0;
            }
            return tongthanhtoan+vouc;
        }
        public ActionResult getData(string IdRegistration)
        {
            DataTable dataTableCourse = Connect.SelectAll("SELECT cour.Name AS NameCourse, rescourse.IdCourse, res.Id, rescourse.Price, pro.Name AS NameProgram, res.Amount, rescourse.TotalAmount, res.Code, res.DateCreate, rescourse.Discount FROM Registration res INNER JOIN RegistrationCourse rescourse ON rescourse.IdRegistration = res.Id INNER JOIN Course cour ON cour.Id = rescourse.IdCourse INNER JOIN Program pro ON pro.Id = cour.IdProgram WHERE res.Id = '" + IdRegistration + "'");
            DataTable dataTableProduct = Connect.SelectAll("SELECT resproduct.Discount,pro.Name, resproduct.Price, resproduct.TotalAmount, res.Id, resproduct.IdProduct FROM Registration res INNER JOIN RegistrationProduct resproduct ON res.Id = resproduct.IdRegistration INNER JOIN Product pro ON pro.Id = resproduct.IdProduct WHERE res.Id = '" + IdRegistration + "'");
            DataTable dataTableOther = Connect.SelectAll("select revenue.Name,revenue.Price,other.Discount,other.Amount,other.TotalAmount,res.Id,other.IdReference from Registration res inner join RegistrationOther other on other.IdRegistration = res.Id inner join RevenueReference revenue on revenue.Id = other.IdReference where other.IdRegistration = '"+IdRegistration+"'");
            Registration registration = Connect.SelectSingle<Registration>("SELECT * FROM Registration WHERE Id = '" + IdRegistration + "'");

            var data = new StringBuilder();
            var i = 0;
            var tongtien = 0;
            var chietkhau = 0;
            var tongthanhtoan = 0;
            decimal dathanhtoan = 0;
            decimal conlai = 0;
            List<DataRow> allRows = new List<DataRow>();
            allRows.AddRange(dataTableCourse.AsEnumerable());
            allRows.AddRange(dataTableProduct.AsEnumerable());
            allRows.AddRange(dataTableOther.AsEnumerable());

            foreach (DataRow row in allRows)
            {
                i++;
                string dongia = "";
                string giam = "";
                string thanhtien = "";
                string name = "";
                var idobject = 0;
                int type = 0;
                if (row.Table == dataTableCourse)
                {
                    type = 1;
                    dongia = string.Format("{0:N0}", row["Price"]);
                    giam = string.Format("{0:N0}", row["Discount"]);
                    thanhtien = string.Format("{0:N0}", row["TotalAmount"]);

                    tongtien += Convert.ToInt32(row["Price"]);
                    chietkhau += Convert.ToInt32(row["Discount"]);

                    idobject = Convert.ToInt32(row["IdCourse"]);
                    name = row["NameProgram"].ToString() + "<br/><i class='ti ti-corner-down-right'></i> Khóa học: <b>" + row["NameCourse"].ToString()+"</b>";
                }
                else if (row.Table == dataTableProduct)
                {
                    type = 2;
                    dongia = string.Format("{0:N0}", row["Price"]);
                    giam = string.Format("{0:N0}", row["Discount"]);
                    thanhtien = string.Format("{0:N0}", row["TotalAmount"]);

                    tongtien += Convert.ToInt32(row["Price"]);
                    chietkhau += Convert.ToInt32(row["Discount"]);

                    idobject = Convert.ToInt32(row["IdProduct"]);
                    name = row["Name"].ToString();
                }else if(row.Table == dataTableOther)
                {
                    type = 3;
                    dongia = string.Format("{0:N0}", row["Price"]);
                    giam = string.Format("{0:N0}", row["Discount"]);
                    thanhtien = string.Format("{0:N0}", row["TotalAmount"]);

                    tongtien += Convert.ToInt32(row["Price"]);
                    chietkhau += Convert.ToInt32(row["Discount"]);

                    idobject = Convert.ToInt32(row["IdReference"]);
                    name = row["Name"].ToString();
                }
                string btn = "<a href='javascript:Delete_item(" + IdRegistration + "," + idobject + "," + type + ")' class='text-danger'>" +
                        "<i class='ti ti-trash font-size-18'></i>" +
                    "</a>";
                if(registration.Status == true)
                {
                    btn = "";
                }
                var newRow = "<tr>" +
                    "<td class='text-center align-content-center'>" + i + "</td>" +
                    "<td class='text-start align-content-center'>" + name + "</td>" +
                    "<td class='text-end align-content-center'>" + dongia + "</td>" +
                    "<td class='text-center align-content-center'>1</td>" +
                    "<td class='text-end align-content-center'>" +  giam + "</td>" +
                    "<td class='text-end align-content-center'>" + thanhtien + "</td>" +
                    "<td class='text-center align-content-center'>" +
                    btn+
                    "</td>" +
                    "</tr>";

                data.Append(newRow);
            }
            tongthanhtoan = tongtien - chietkhau;
            dathanhtoan = Check_paymentRegistration(Convert.ToInt32(IdRegistration));
            conlai = tongthanhtoan - dathanhtoan;
            var checkStatusProduct = db.RegistrationProducts.Any(x=>x.Status==false);
            var result = new
            {
                datalist = data.ToString(),
                Tongtien = string.Format("{0:N0}", tongtien),
                Chietkhau = string.Format("{0:N0}", chietkhau),
                Tongthanhtoan = string.Format("{0:N0}", tongthanhtoan),
                Dathanhtoan = string.Format("{0:N0}", dathanhtoan),
                Conlai = string.Format("{0:N0}", conlai),
                DateCreate = Convert.ToDateTime(registration.DateCreate).ToString("dd/MM/yyyy"),
                idRegistrations = registration.Id,
                Code = registration.Code,
                Status = registration.Status,
                checkStatusProduct = checkStatusProduct
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class listProduct
        {
           public int isChecked {  get; set; }
           public int idpro {  get; set; }
        }

        [HttpPost]
        public ActionResult SaveRegistration(int? IdRegistration, int type, int? IdObject, decimal? price, decimal? totalamount, int? amount, string Description, int? Discount,int? promotionId, int? promotionProductId)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    string iduser = CheckUsers.iduser();
                    Student student = Session["infoUser"] as Student;
                    int IdBranch = Convert.ToInt32(student.IdBranch);
                    Registration registration = Connect.SelectSingle<Registration>("select * from Registration where Id='" + IdRegistration + "'");
                    // Create new Registration
                    if (registration == null)
                    {
                        Registration NewRegistration = new Registration
                        {
                            IdStudent = student.Id,
                            IdUser = Convert.ToInt32(iduser),
                            Amount = amount,
                            Code = GenerateCode(IdBranch),
                            TotalAmount = totalamount,
                            DateCreate = DateTime.Now,
                            Status = false,
                            Enable = true,
                            Description = Description,
                            IdBranch = student.IdBranch
                        };
                        db.Registrations.Add(NewRegistration);
                        db.SaveChanges();
                        registration = NewRegistration;
                        Session["IdRegistration"] = registration.Id;
                    }

                    // Check type luồng Product, Course, Order
                    switch (type)
                    {
                        case 1:
                            RegistrationProduct registrationProduct = Connect.SelectSingle<RegistrationProduct>("select * from RegistrationProduct where IdProduct = '" + IdObject + "' and IdRegistration = '" + registration.Id + "'");
                            if (registrationProduct != null)
                            {
                                return Json(null);
                            }
                            else
                            {
                                RegistrationProduct NewregistrationProduct = new RegistrationProduct
                                {
                                    IdRegistration = registration.Id,
                                    IdProduct = Convert.ToInt32(IdObject),
                                    Status = false,
                                    Price = price,
                                    Amount = amount,
                                    TotalAmount = totalamount,
                                    Discount = Discount,
                                    IdPromotion = (promotionProductId !=0 ? promotionProductId:null)
                                };
                                db.RegistrationProducts.Add(NewregistrationProduct);
                                db.SaveChanges();
                                scope.Complete(); // Commit transaction
                                return Json(NewregistrationProduct.IdRegistration);
                            }
                        case 2:
                            RegistrationCourse registrationCourse = Connect.SelectSingle<RegistrationCourse>("select * from RegistrationCourse where IdRegistration = '" + registration.Id + "' AND IdCourse = '" + IdObject + "'");
                            if (registrationCourse != null)
                            {
                                return Json(null);
                            }
                            else
                            {
                                RegistrationCourse NewregistrationCourse = new RegistrationCourse
                                {
                                    Status = false,
                                    Price = price,
                                    Amount = amount,
                                    TotalAmount = totalamount,
                                    IdRegistration = registration.Id,
                                    IdCourse = Convert.ToInt32(IdObject),
                                    Discount = Discount,
                                    StatusExchangeCourse = false,
                                    StatusExtend = false,
                                    StatusJoinClass = false,    
                                    StatusReserve = false,
                                    IdPromotion = (promotionId!=0 ? promotionId:null)
                                };
                                db.RegistrationCourses.Add(NewregistrationCourse);
                                db.SaveChanges();

                                var listproduct = db.ProductCourses.Where(x => x.IdCourse == IdObject).ToList(); // Convert to list for multiple enumerations
                                if (listproduct.Any())
                                {
                                    foreach (var p in listproduct)
                                    {
                                        int IdProduct = Convert.ToInt32(p.IdProduct);
                                        // Check if tonkho is greater than 0 before creating RegistrationProduct
                                        if (Check_tonkho(IdProduct)) // Ensure to pass IdBranch parameter if needed in Check_tonkho
                                        {
                                            if (!db.RegistrationProducts.Any(x => x.IdProduct == IdProduct && x.IdRegistration==registration.Id))
                                            {
                                                RegistrationProduct NewregistrationProduct = new RegistrationProduct
                                                {
                                                    IdRegistration = registration.Id,
                                                    IdProduct = IdProduct,
                                                    Status = false,
                                                    Price = p.Product.Price,
                                                    Amount = p.Amount,
                                                    TotalAmount = p.Product.Price * p.Amount, // Assuming TotalAmount should be price multiplied by amount
                                                    Discount = 0,
                                                };
                                                db.RegistrationProducts.Add(NewregistrationProduct);
                                            }
                                        }
                                    }
                                    // Save all changes in one go to improve performance
                                    db.SaveChanges();
                                }
                                scope.Complete(); // Commit transaction
                                return Json(NewregistrationCourse.IdRegistration);
                            }
                        case 3:
                            RegistrationOther registrationOther = Connect.SelectSingle<RegistrationOther>("select * from RegistrationOther where IdRegistration = '" + registration.Id + "' and IdReference = '" + IdObject + "'");
                            if (registrationOther != null)
                            {
                                return Json(null);
                            }
                            else
                            {
                                RegistrationOther NewRegistrationOther = new RegistrationOther
                                {
                                    IdRegistration = registration.Id,
                                    IdReference = Convert.ToInt32(IdObject),
                                    Discount = Discount,
                                    Status = "0",
                                    Price = price,
                                    Amount = amount,
                                    TotalAmount = totalamount
                                };
                                db.RegistrationOthers.Add(NewRegistrationOther);
                                db.SaveChanges();
                                scope.Complete(); // Commit transaction
                                return Json(NewRegistrationOther.IdRegistration);
                            }
                        default:
                            return Json(null);
                    }
                }
                catch (Exception)
                {
                    // Handle exception if needed
                    return Json(null);
                }
            }
        }
        public bool Check_tonkho(int? IdObject)
        {
            int IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            // Calculate the total stock (tonkho) for the given product and branch using LINQ
            var incomingStock = db.ProductReceiptionDetails
                .Where(d => d.IdProduct == IdObject && d.Type == true && d.WarehouseReceiption.IdBranch == IdBranch)
                .Sum(d => (int?)d.Amount) ?? 0;

            var outgoingStock = db.ProductReceiptionDetails
                .Where(d => d.IdProduct == IdObject && d.Type == false && d.WarehouseReceiption.IdBranch == IdBranch)
                .Sum(d => (int?)d.Amount) ?? 0;

            // Calculate the current stock
            int tonkho = incomingStock - outgoingStock;

            // Return true if stock is greater than zero
            return tonkho > 0;
        }

        [HttpPost]
        public ActionResult Submit_deleteItem(int IdRegistration, int Id, int Type)
        {
            string status, message;
            if (Type == 1)
            {
                //Course
                var pc = db.RegistrationCourses.SingleOrDefault(x => x.IdCourse == Id && x.IdRegistration == IdRegistration);
                if (pc == null)
                {
                    status = "error";
                    message = "Khóa học này đã được xóa khỏi phiếu!";
                }
                db.RegistrationCourses.Remove(pc);
                db.SaveChanges();
                status = "ok";
                message = "Đã xóa khóa thành công khỏi phiếu!";
            }
            else if (Type == 2)
            {
                //Product
                var pc = db.RegistrationProducts.SingleOrDefault(x => x.IdProduct == Id && x.IdRegistration == IdRegistration);
                if (pc == null)
                {
                    status = "error";
                    message = "Vật tư này đã được xóa khỏi phiếu!";
                }
                db.RegistrationProducts.Remove(pc);
                db.SaveChanges();
                status = "ok";
                message = "Đã xóa vật tư thành công khỏi phiếu!";
            }
            else
            {
                //Order
                var pc = db.RegistrationOthers.SingleOrDefault(x => x.IdReference == Id && x.IdRegistration == IdRegistration);
                if (pc == null)
                {
                    status = "error";
                    message = "Khoản thu đã được xóa khỏi phiếu!";
                }
                db.RegistrationOthers.Remove(pc);
                db.SaveChanges();
                status = "ok";
                message = "Đã xóa khoản thu thành công khỏi phiếu!";
            }
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_lophoc(int? idStudent)
        {
            string str = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select course.Id as IdCourse,re.id as IdRegistration,re.Code,course.Name as NameCourse,c.Name as NameClass,joinclass.Sessions,joinclass.DateCreate,joinclass.Fromdate,joinclass.Todate,us.Name as NameUser " +
                                "from StudentJoinClass joinclass inner join Registration re on re.id=joinclass.IdRegistration,Course course,Class c,[User] us " +
                                "where course.Id = joinclass.IdCourse and c.id=joinclass.IdClass and us.id=joinclass.IdUser and re.IdStudent=" + idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var todate = DateTime.Parse(reader["Todate"].ToString());
                    string StatusStudent = "";
                    if (todate > DateTime.Now)
                    {
                        StatusStudent = "<span class='badge bg-success'>Đang học</span>";
                    }
                    else
                    {
                        StatusStudent = "<span class='badge bg-danger'>Đã kết thúc</span>";
                    }
                    count++;
                    str +="<tr>"
                        + "<td class='text-center'>" + count + "</td>"
                        + "<td>" + reader["Code"].ToString() + "</td>"
                        + "<td>" + reader["NameCourse"].ToString() + "</td>"
                        + "<td>" + reader["NameClass"].ToString() + "</td>"
                        + "<td class='text-center'>" + DateTime.Parse(reader["Fromdate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                        + "<td class='text-center'>" + DateTime.Parse(reader["Todate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                        + "<td class='text-center'>0/" + reader["Sessions"].ToString() + " <a target='_blank' href='/students/printschedule?IdRegistration=" + reader["IdRegistration"] +"&IdCourse=" + reader["IdCourse"] +"'><i class='ti ti-printer text-success'></i></a></td>"
                        + "<td class='text-center'>" + DateTime.Parse(reader["DateCreate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                        + "<td class='text-center'>"+StatusStudent+"</td>"
                        + "</tr>";
                }
                reader.Close();
            }
            var data = new { str };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_khoadangky(int? idStudent)
        {
            string str = "";
            int count = 0;

            // Kiểm tra đầu vào
            if (idStudent == null)
            {
                return Json(new { str = "ID sinh viên không hợp lệ" }, JsonRequestBehavior.AllowGet);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Truy vấn sử dụng tham số hóa để ngăn chặn SQL Injection
                string query = @"
                                SELECT s.Id, re.Id AS IdRegistration, re.Code AS Code, rec.IdCourse, course.Name AS NameCourse, 
                                re.DateCreate, rec.Status, rec.TotalAmount, rec.StatusExchangeCourse, 
                                rec.StatusJoinClass, rec.StatusExtend, rec.StatusReserve 
                                FROM Student s 
                                INNER JOIN Registration re ON re.IdStudent = s.Id 
                                INNER JOIN RegistrationCourse rec ON rec.IdRegistration = re.Id 
                                INNER JOIN Course course ON course.Id = rec.IdCourse 
                                WHERE s.Id = @IdStudent";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IdStudent", idStudent);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int IdRegistration = Convert.ToInt32(reader["IdRegistration"]);
                        int IdCourse = Convert.ToInt32(reader["IdCourse"]);
                        var studentJoinClass = db.StudentJoinClasses.FirstOrDefault(x => x.IdStudent == idStudent && x.IdRegistration == IdRegistration && x.IdCourse == IdCourse);

                        var fromdateJoinClass = studentJoinClass?.Fromdate;
                        var todateJoinClass = studentJoinClass?.Todate;
                        string btn = "";
                        double amount = Convert.ToDouble(reader["TotalAmount"]);
                        bool status = Convert.ToBoolean(reader["Status"]);
                        bool statusJoinClass = Convert.ToBoolean(reader["StatusJoinClass"]);
                        bool statusExtend = Convert.ToBoolean(reader["StatusExtend"]);
                        bool statusReserve = Convert.ToBoolean(reader["StatusReserve"]);
                        bool statusExchangeCourse = Convert.ToBoolean(reader["StatusExchangeCourse"]);
                        string strStatusJoinClass = "";
                        if (status)
                        {
                            if (statusJoinClass)
                            {
                                if (studentJoinClass.Todate >= DateTime.Now)
                                {
                                    strStatusJoinClass = "<span class='badge bg-success'>Đang học</span>";
                                    btn += "<li><a class=\"dropdown-item\" href='/students/printschedule?IdRegistration=" + reader["IdRegistration"] + "&IdCourse=" + reader["IdCourse"] + "'><i class=\"ti ti-printer\"></i> In thời khóa biểu</a></li>";
                                    if (!statusReserve && todateJoinClass.Value.AddDays(30) > DateTime.Now)
                                    {
                                        btn += "<li><a class=\"dropdown-item\" href='javascript:Baoluu_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] + ")'><i class=\"ti ti-album-off\"></i> Bảo lưu khóa học</a></li>";
                                    }
                                    if (!statusExtend)
                                    {
                                        btn += "<li><a class=\"dropdown-item\" href='javascript:Giahan_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] + ")'><i class=\"ti ti-plus\"></i> Gia hạn khóa học</a></li>";
                                    }
                                    if (!statusExchangeCourse)
                                    {
                                        btn += "<li><a class=\"dropdown-item\" href='javascript:Capnhat_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] + ")'><i class=\"ti ti-exchange\"></i> Cập nhật khóa học</a></li>";
                                    }
                                }
                                else
                                {
                                    // Kiểm tra khoảng thời gian từ ngày bắt đầu đến ngày kết thúc
                                    //bool isLessThanThreeMonths = false;
                                    var duration = todateJoinClass.Value - fromdateJoinClass.Value;
                                    if (duration.TotalDays < 90) // Kiểm tra nếu khoảng thời gian ít hơn 3 tháng (90 ngày)
                                    {
                                        //isLessThanThreeMonths = true;
                                        if (statusReserve)
                                        {
                                            strStatusJoinClass = "<span class='badge bg-info'>Đang bảo lưu</span>";
                                            btn += "<li><a class=\"dropdown-item\" href='javascript:Deactive_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] + ")'><i class=\"ti ti-album\"></i> Kích hoạt lại</a></li>";
                                        }
                                    }
                                    else
                                    {
                                        if (!statusExtend && todateJoinClass.Value.AddDays(14) > DateTime.Now)
                                        {
                                            strStatusJoinClass = "<span class='badge bg-danger'>Đã kết thúc</span>";
                                            btn += "<li><a class=\"dropdown-item\" href='javascript:Giahan_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] + ")'><i class=\"ti ti-plus\"></i> Gia hạn khóa học</a></li>";
                                        }
                                        else
                                        {
                                            strStatusJoinClass = "<span class='badge bg-danger'>Đã kết thúc</span>";
                                            btn += "<li><a class=\"dropdown-item\" href='javascript:void(0)'><i class=\"ti ti-cancel\"></i> Khóa học kết thúc</a></li>";
                                        }
                                    }

                                }
                            }
                            else
                            {
                                strStatusJoinClass = "<span class='badge bg-secondary'>Chờ xét lớp</span>";
                                btn += "<li><a class=\"dropdown-item\" href='javascript:Xetlop_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] + ")'><i class=\"ti ti-eye-check\"></i> Xét vào lớp</a></li>";
                            }
                        }
                        else
                        {
                            strStatusJoinClass = "<span class='badge bg-secondary'>Chờ xét lớp</span>";
                            btn += "<li><a class=\"dropdown-item\" href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "&modalPayment=True' target='_blank'><i class=\"ti ti-credit-card\"></i> Thu tiền</a></li>";
                        }
                        count++;
                        str += "<tr>"
                             + "<td class='text-center'><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + count + "</a></td>"
                             + "<td><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + reader["Code"].ToString() + "</a></td>"
                             + "<td><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + reader["NameCourse"].ToString() + "</a></td>"
                             + "<td class='text-end'><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + string.Format("{0:N0} đ", amount) + "</a></td>"
                             + "<td class='text-center'><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + Convert.ToDateTime(reader["DateCreate"]).ToString("dd/MM/yyyy") + "</a></td>"
                             + "<td class='text-center'><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + (status ? "<i class='ti ti-circle-check text-success'></i>" : "Chưa thanh toán") + "</a></td>"
                             + "<td class='text-center'><a class='text-muted' target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "'>" + strStatusJoinClass + "</a></td>"
                             + "<td class='text-end'>"
                             + "<a target='_blank' href='/students/AddCourseProgramOfStudents?IdStudent=" + idStudent + "&IdRegistration=" + reader["IdRegistration"] + "' class=\"text-warning\"><i class='ti ti-edit text-primary'></i></a>"
                             + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                             + "<i class=\"ti ti-dots-vertical\"></i>"
                             + "</a>"
                             + "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                             + btn
                             + "</ul>"
                             + "</td>"
                             + "</tr>";
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi và trả về thông báo lỗi
                    return Json(new { str = "Lỗi khi tải dữ liệu: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_exchange(int IdRegistration, int IdCourse, int IdClass, DateTime Todate,DateTime Fromdate)
        {
            try
            {
                int idBranch = int.Parse(CheckUsers.idBranch());

                // Kiểm tra xem bản ghi joinclass có tồn tại không
                var joinclass = db.StudentJoinClasses.SingleOrDefault(x => x.IdRegistration == IdRegistration && x.IdCourse == IdCourse);
                if (joinclass == null)
                {
                    return Json(new { status = "error", message = "Không tìm thấy lớp học của sinh viên." }, JsonRequestBehavior.AllowGet);
                }
                // Cập nhật thông tin
                joinclass.Fromdate = Fromdate;
                joinclass.Todate = Todate;
                joinclass.IdClass = IdClass;
                db.Entry(joinclass).State = EntityState.Modified;

                // Lưu thay đổi
                db.SaveChanges();

                var item = new { status = "ok", message = "Đã gia hạn thành công!" };
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { status = "error", message = "Lỗi khi gia hạn: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Load_exchange(int idRegistration, int idCourse)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "", strCourse = "";
            bool StatusStudent = true;
            var registrion = db.RegistrationCourses.Where(x => x.IdRegistration == idRegistration && x.IdCourse == idCourse);
            var joinClass = db.StudentJoinClasses.SingleOrDefault(x => x.IdRegistration == idRegistration && x.IdCourse == idCourse);

            if (joinClass == null)
            {
                str += "<option value='0' selected>Chưa có lớp học</option>";
                StatusStudent = false;
            }

            var classes = db.Classes.Where(x => x.IdBranch == idbranch && x.Enable == true);

            if (classes == null || !classes.Any())
            {
                str += "<option value='0' selected>Không tìm thấy lớp khả dụng</option>";
            }
            else
            {
                foreach (var c in classes)
                {
                    string selected = c.Id == joinClass.IdClass ? " selected" : "";
                    str += "<option value='" + c.Id + "' " + selected + ">" + c.Name + "</option>";
                }
            }

            if (joinClass != null)
            {
                strCourse += "<option value='" + joinClass.IdCourse + "' selected>" + joinClass.Course.Name + "</option>";

                DateTime fromdate = joinClass.Fromdate.Value;
                DateTime todate = joinClass.Todate.Value;
                
                var item = new
                {
                    str,
                    strCourse,
                    fromdate = fromdate.ToString("dd/MM/yyyy"),
                    todate = todate.ToString("dd/MM/yyyy"),
                    registrionCode = registrion.First().Registration.Code,
                    StatusStudent
                };

                return Json(item, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Không tìm thấy lớp học." }, JsonRequestBehavior.AllowGet);
            }
        }

        public bool Check_statusStudent(int? id)
        {
            var student = db.Students.FirstOrDefault(x => x.Id == id);

            if (student == null)
            {
                return false;
            }

            else
            {
                if (student.StudentJoinClasses.Any(x => x.Todate > DateTime.Now))
                {
                    return false;
                }
                return true;
            }
        }
        public ActionResult Onchange_schedule(int idClass)
        {
            var schedulesbyClass = db.Schedules
                .Where(x => x.IdClass == idClass && (bool)x.Active).Include(x => x.Employee).ToList()
                .Select(x => new ClassAssignmentDTO
                {
                    DayOfWeek = scheduleHelper.GetDayName(x.IdWeek),
                    TeacherName = x.Employee.Name,
                    TimeSlot = scheduleHelper.GetTimeSlot((DateTime)x.FromHour, (DateTime)x.ToHour),
                    HourQuantity = scheduleHelper.GetHourQuantity((DateTime)x.FromHour, (DateTime)x.ToHour).ToString(),
                });
            return Json(new {status="ok",schedulesbyClass},JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit_deactive(int IdRegistration, int IdCourse, int IdClass, DateTime Todate)
        {
            try
            {
                int idBranch = int.Parse(CheckUsers.idBranch());

                // Kiểm tra xem bản ghi joinclass có tồn tại không
                var joinclass = db.StudentJoinClasses.SingleOrDefault(x => x.IdRegistration == IdRegistration && x.IdCourse == IdCourse);
                if (joinclass == null)
                {
                    return Json(new { status = "error", message = "Không tìm thấy lớp học của sinh viên." }, JsonRequestBehavior.AllowGet);
                }
                // Cập nhật thông tin
                joinclass.Todate = Todate;
                joinclass.IdClass = IdClass;
                db.Entry(joinclass).State = EntityState.Modified;

                // Lưu thay đổi
                db.SaveChanges();

                var item = new { status = "ok", message = "Đã kích hoạt lại khóa học thành công!" };
                return Json(item, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return Json(new { status = "error", message = "Lỗi khi kích hoạt lại: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Load_deactive(int idRegistration, int idCourse)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string str = "", strCourse = "";
            bool StatusStudent = true;
            var registrion = db.RegistrationCourses.Where(x => x.IdRegistration == idRegistration && x.IdCourse == idCourse);
            var joinClass = db.StudentJoinClasses.FirstOrDefault(x => x.IdRegistration == idRegistration && x.IdCourse == idCourse);

           

            if (joinClass == null)
            {
                str += "<option value='0' selected>Chưa có lớp học</option>";
                StatusStudent = false;
            }

            var classes = db.Classes.Where(x => x.IdBranch == idbranch && x.Enable == true);

            if (classes == null || !classes.Any())
            {
                str += "<option value='0' selected>Không tìm thấy lớp khả dụng</option>";
            }
            else
            {
                foreach (var c in classes)
                {
                    string selected = c.Id == joinClass.IdClass ? " selected" : "";
                    str += "<option value='" + c.Id + "' " + selected + ">" + c.Name + "</option>";
                }
            }

            if (joinClass != null)
            {
                strCourse += "<option value='" + joinClass.IdCourse + "' selected>" + joinClass.Course.Name + "</option>";

                DateTime fromdate = joinClass.Fromdate.Value;
                DateTime todate = joinClass.Todate.Value;
                TimeSpan timeStudied = todate - fromdate;

                // Tính thời gian đã học
                int daysStudied = timeStudied.Days;

                // Tính todate mới
                DateTime today = DateTime.Now;
                int remainingDays = 90 - daysStudied;
                DateTime newTodate = today.AddDays(remainingDays);

                // Lấy danh sách lịch nghỉ của chi nhánh
                var vacationSchedules = db.VacationSchedules
                                          .Where(vs => vs.IdBranch == idbranch && vs.Fromdate <= newTodate && vs.Todate >= fromdate)
                                          .ToList();

                int extraDays = 0;
                foreach (var vacation in vacationSchedules)
                {
                    DateTime vacationStart = (DateTime)vacation.Fromdate;
                    DateTime vacationEnd = (DateTime)vacation.Todate;

                    // Tính số ngày nghỉ trong khoảng từ fromdate đến todate
                    if (vacationStart < fromdate)
                        vacationStart = fromdate; // Bắt đầu tính từ fromdate nếu ngày nghỉ bắt đầu trước fromdate

                    if (vacationEnd > todate)
                        vacationEnd = todate; // Dừng tính tại todate nếu ngày nghỉ kéo dài sau todate

                    // Thêm số ngày nghỉ vào extraDays
                    extraDays += (vacationEnd - vacationStart).Days + 1; // +1 để tính cả ngày cuối cùng
                }

                // Cộng số ngày nghỉ vào todate
                newTodate = newTodate.AddDays(extraDays);


                // Chuẩn bị dữ liệu trả về
                var item = new
                {
                    str,
                    strCourse,
                    fromdate = fromdate.ToString("dd/MM/yyyy"),
                    todate = todate.ToString("dd/MM/yyyy"),
                    newTodate = newTodate.ToString("dd/MM/yyyy"),
                    daysStudied = daysStudied,
                    registrionCode = registrion.First().Registration.Code,
                    StatusStudent
                };

                return Json(item, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Không tìm thấy lớp học." }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Load_xetlop(int idRegistration,string idCourse,int idStudent,DateTime fromdate) {
            string str = "";
            string registrionCode =db.Registrations.Find(idRegistration).Code;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            // Bắt đầu với todate là 90 ngày từ fromdate
            DateTime todate = fromdate.AddDays(90);

            // Lấy danh sách lịch nghỉ của chi nhánh
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var vacationSchedules = db.VacationSchedules
                                      .Where(vs => vs.IdBranch == idbranch && vs.Fromdate <= todate && vs.Todate >= fromdate)
                                      .ToList();
            int extraDays = 0;
            foreach (var vacation in vacationSchedules)
            {
                DateTime vacationStart = (DateTime)vacation.Fromdate;
                DateTime vacationEnd = (DateTime)vacation.Todate;

                // Tính số ngày nghỉ trong khoảng từ fromdate đến todate
                if (vacationStart < fromdate)
                    vacationStart = fromdate; // Bắt đầu tính từ fromdate nếu ngày nghỉ bắt đầu trước fromdate

                if (vacationEnd > todate)
                    vacationEnd = todate; // Dừng tính tại todate nếu ngày nghỉ kéo dài sau todate

                // Thêm số ngày nghỉ vào extraDays
                extraDays += (vacationEnd - vacationStart).Days + 1; // +1 để tính cả ngày cuối cùng
            }

            // Cộng số ngày nghỉ vào todate
            todate = todate.AddDays(extraDays);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select rec.IdCourse,c.Name,rec.Status from RegistrationCourse rec inner join Registration re on re.id= rec.IdRegistration, Course c where c.id=rec.IdCourse and re.IdStudent = "+idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                        str += "<option value='" + reader["IdCourse"] +"' data-status='" + reader["Status"] +"' selected>" + reader["Name"] +"</option>";
                }
                reader.Close();
            }
            var statusStudent = Check_statusStudent(idStudent);
            var item = new { 
                str,registrionCode,statusStudent,todate =todate.ToString("dd/MM/yyyy")
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_giahan(int idRegistration, int idCourse)
        {
            string str = "",strCourse="";
            bool StatusStudent = true;
            var registrion = db.RegistrationCourses.Where(x=>x.IdRegistration==idRegistration&&x.IdCourse==idCourse);
            var joinClass=db.StudentJoinClasses.FirstOrDefault(x=>x.IdRegistration== idRegistration&&x.IdCourse==idCourse);
            if (joinClass == null)
            {
                str += "<option value='0' selected>Chưa có lớp học</option>";
                StatusStudent = false;
            }
            str += "<option value='"+joinClass.IdClass+"' selected>"+joinClass.Class.Name+"</option>";
            strCourse += "<option value='"+joinClass.IdCourse+"' selected>"+joinClass.Course.Name+"</option>";
            var item = new
            {
                str,
                strCourse,
                fromdate = joinClass.Fromdate.Value.ToString("dd/MM/yyyy"),
                todate = joinClass.Todate.Value.ToString("dd/MM/yyyy"),

                registrionCode =registrion.First().Registration.Code,
                StatusStudent
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_giahan(int IdRegistration, int IdCourse, DateTime Todate)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            int month = DateTime.Now.Month + 1;
            int year = DateTime.Now.Year;
            var joinclass = db.StudentJoinClasses.SingleOrDefault(x => x.IdRegistration == IdRegistration && x.IdCourse == IdCourse);
            var rec = db.RegistrationCourses.SingleOrDefault(x => x.IdRegistration == IdRegistration && x.IdCourse == IdCourse);
            rec.DateExtend = DateTime.Today;
            rec.StatusExtend = true;
            joinclass.Todate = Todate;
            db.Entry(rec).State = EntityState.Modified;
            db.Entry(joinclass).State = EntityState.Modified;
            db.SaveChanges();
            var item = new { status = "ok",message="Đã gia hạn thành công!" };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_xetlop(int idRegistration,int idCourse,int idStudent,int idClass,DateTime fromdate,DateTime todate)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            var sessions = db.CourseBranches.Where(x=>x.IdCourse==idCourse&&x.IdBranch==idBranch).First().Sessons;
            int month =DateTime.Now.Month+1;
            int year = DateTime.Now.Year;
            if (DateTime.Now.Day > 25) {
                month += 1;
            }
            if (DateTime.Now.Year > 12)
            {
                year += 1;
            }
            var joinclass = new StudentJoinClass()
            {
                DateCreate = DateTime.Now,
                IdClass = idClass,
                IdRegistration = idRegistration,
                IdStudent = idStudent,
                IdCourse = idCourse,
                Fromdate = fromdate, // DateTime.ParseExact(fromdate, "MM/dd/yyyy", CultureInfo.InvariantCulture),
                Todate = todate, // DateTime.ParseExact(todate,"MM/dd/yyyy",CultureInfo.InvariantCulture),
                IdUser =int.Parse(CheckUsers.iduser()),
                MonthFee = month,
                YearFee = year,
                Enable = true,
                Sessions = sessions
            };
            db.StudentJoinClasses.Add(joinclass);
            var registrionCourse = db.RegistrationCourses.SingleOrDefault(x => x.IdCourse == idCourse && x.IdRegistration == idRegistration);
            registrionCourse.StatusJoinClass = true;
            db.Entry(registrionCourse);
            db.SaveChanges();
            var item = new { status="ok"};
            return Json(item,JsonRequestBehavior.AllowGet);
        }


        public ActionResult Load_tuvan(int idStudent)
        {
            string str = "";int count = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select us.Name,s.DateCreate,s.AppointmentDate,s.Description,s.Result,s.Status from StudentAdvise s,[User] us where us.id=s.idUser and s.IdStudent = " + idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    count++;
                    str += "<li class='timeline-item d-flex position-relative overflow-hidden'>"
                                + "<div class='timeline-time text-dark flex-shrink-0 text-end'>" + DateTime.Parse(reader["DateCreate"].ToString()).ToString("dd/MM/yyyy hh:mm tt")
                                + "<span class='text-primary d-block fw-bold'>" + reader["Name"] +"</span>"
                                +"</div>"
                                + "<div class='timeline-badge-wrap d-flex flex-column align-items-center'>"
                                    + "<span class='timeline-badge border-2 border border-primary flex-shrink-0 my-8'></span>"
                                    + "<span class='timeline-badge-border d-block flex-shrink-0'></span>"
                                + "</div>"
                                + "<div class=timeline-desc fs-3 text-dark mt-n1>" + reader["Description"] 
                                +"<span class='text-primary d-block fw-bold'>"+ DateTime.Parse(reader["AppointmentDate"].ToString()).ToString("dd/MM/yyyy") + "</span>"
                                +"</div>"
                            + "</li>";
                }
                    reader.Close();
            }
                var item = new
                {
                    str,count
                };
                return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_taikhoan(int idStudent)
        {
            string str = ""; int count = 0;
            decimal nhap = 0,xuat=0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select us.Name,s.DateCreate,s.Voucher,s.Description,s.Type,s.Enable,s.Active from StudentVoucher s,[User] us where us.id=s.idUser and s.IdStudent = " + idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count++;
                    decimal voucher = Convert.ToDecimal(reader["Voucher"]);
                    if (Convert.ToBoolean(reader["Type"]))
                    {
                        nhap += Convert.ToDecimal(reader["Voucher"]);
                    }
                    else
                    {
                        xuat += Convert.ToDecimal(reader["Voucher"]);
                    }
                    str += "<tr>"
                        +"<td>"+count+"</td>"
                        +"<td>"+ string.Format("{0:N0}",voucher) + "</td>"
                        +"<td>"+ reader["Description"] + "</td>"
                        +"<td>" + DateTime.Parse(reader["DateCreate"].ToString()).ToString("dd/MM/yyyy hh:mm tt") + "</td>"
                        +"<td>"+ reader["Name"] + "</td>"
                        + "<td class='text-center'>" + (Convert.ToBoolean(reader["Type"])?"Nạp":"Xuất")+"</td>"
                        + "</tr>";
                }
                decimal tong = nhap - xuat;
                str += "<tr class='bg-light'><td colspan=5 class='text-start'>Số dư:</td><td class='text-center'>"+string.Format("{0:N0}",tong)+"</td></tr>";
                reader.Close();
            }
            var item = new
            {
                str,
                count
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_taikhoan(int idStudent, Decimal Sotien, string Noidung)
        {
            var st = new StudentVoucher()
            {
                Voucher = Sotien,
                IdStudent = idStudent,
                DateCreate = DateTime.Now,
                Description = Noidung,
                IdUser = int.Parse(CheckUsers.iduser()),
                Active = true,
                Type =true,
                Enable = true,
            };
            db.StudentVouchers.Add(st);
            db.SaveChanges();
            var item = new
            {
                status = "ok"
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_tuvan(int idStudent,DateTime Ngayhen,string Noidung)
        {
            var st = new StudentAdvise() { 
                AppointmentDate = Ngayhen,
                IdStudent = idStudent,
                DateCreate = DateTime.Now,
                Description = Noidung,
                IdUser = int.Parse(CheckUsers.iduser()),
                Status = true
            };
            db.StudentAdvises.Add(st);
            db.SaveChanges();
            var item = new
            {
                status="ok"
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getDataComboxOther()
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            List<RevenueReference> revenueReferences = Connect.Select<RevenueReference>("select * from RevenueReference Where IdBranch='"+idBranch+"'");
            var str = "";
            foreach (var items in revenueReferences)
            {
                var dongia = items.Price;
                var dongiagiam = items.Discount;
                var statusdiscount = items.StatusDiscount;
                str += "<option value='" + items.Id + "' data-name='" + items.Name + "' data-dongia='"+(statusdiscount==true?string.Format("{0:N0}",dongiagiam):string.Format("{0:N0}",dongia))+"'>" + items.Name + "</option>";
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getDataComboxProduct()
        {
            var str = "";
            int idBranch = int.Parse(CheckUsers.idBranch());
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Name,p.Image,p.Unit,p.Price,p.Code,p.Quota,p.IsFixed,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + idBranch + "), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + idBranch + "), 0) AS Tonkho"
                                        + " FROM product p"
                                        + " where p.enable=1 and p.isCore=1 and p.Active=1";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                     int tonkho = Convert.ToInt32(reader["tonkho"]);
                     double dongia = Convert.ToDouble(reader["Price"]);
                    str += "<option value='" + reader["Id"] + "' data-name='" + reader["Name"] + "' data-tonkho='"+tonkho+"' data-dongia='"+string.Format("{0:N0}",dongia)+"' data-isfixed='"+ reader["IsFixed"] +"'>" + reader["Name"] + "</option>";
                }
                reader.Close();
            }
            string strpromotion = "";
            List<Promotion> promotions = Connect.Select<Promotion>("Select * from Promotion where idBranch='" + idBranch + "' and type='2' and todate>getdate() and active='1'");
            if (promotions.Count > 0)
            {
                foreach (var promotion in promotions)
                {
                    strpromotion += "<option value='" + promotion.Id + "' data-value='" + string.Format("{0:N0}", promotion.Value) + "' data-name='" + promotion.Name + "'>" + promotion.Name + "</option>";

                }
            }
            else
            {
                strpromotion += "<option value='0' data-name='--' data-value='0'>--</option>";
            }
            return Json(new {str,strpromotion}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDataCombobox(int? IdProgram)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            var strcour = "";
            var strpro = "";
            var strpromotion = "";
            string sqlquery = "";
            List<Program> programs= Connect.Select<Program>("SELECT * FROM Program where enable='1'");
            string seleted = "";
            foreach (var pro in programs)
            {
                if (pro.Id == IdProgram)
                {
                    seleted = " selected";
                }
                else
                {
                    seleted = "";
                }
                strpro += "<option value='" + pro.Id + "' data-name='" + pro.Name + "'"+seleted+">" + pro.Name + "</option>";
            }
            List<Promotion> promotions = Connect.Select<Promotion>("Select * from Promotion where idBranch='"+idBranch+"' and type='1' and todate>getdate() and active='1'");
            if (promotions.Count > 0)
            {
                foreach (var promotion in promotions)
                {
                    
                    strpromotion += "<option value='" +promotion.Id  + "' data-value='"+string.Format("{0:N0}", promotion.Value)+"' data-name='" + promotion.Name + "'>" + promotion.Name + "</option>";
                }
            }
            else
            {
                strpromotion += "<option value='0' data-name='--' data-value='0'>--</option>";
            }
            if (IdProgram == 0)
            {
                sqlquery = " and c.IdProgram='" + programs[0].Id + "'";
            }
            else
            {
                sqlquery = " and c.IdProgram='" + IdProgram + "'";
            }
            
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select c.Id,c.Code,c.Name,cb.PriceCourse,cb.DiscountPrice,cb.StatusDiscount" +
                                " from Course c join CourseBranch cb on c.id = cb.IdCourse" +
                                " where cb.IdBranch = '" + idBranch + "'" + sqlquery + " order by c.DisplayOrder";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.HasRows)
                    {
                        strcour += "<option value='0' data-name='0'>Không có khóa học</option>";
                    }
                    else
                    {
                        strcour += "<option value='" + reader["Id"] + "' data-name='" + reader["Name"] + "'>" + reader["Name"] + "</option>";
                    }
                }
                reader.Close();
            }
            var item = new { 
                strcour,
                strpro,
                strpromotion
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetInfoCourse(int IdCourse)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            string priceCourse = "";
            string strtable = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select c.Id,c.Code,c.Name,cb.PriceCourse,cb.DiscountPrice,cb.StatusDiscount" +
                                " from Course c join CourseBranch cb on c.id = cb.IdCourse" +
                                " where cb.IdBranch = '" + idBranch + "' and c.Id=" + IdCourse + " order by c.DisplayOrder";

                string querykho = "select p.Id,p.Name,p.Unit,p.Price,pc.Amount,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + idBranch + "), 0) -" +
                                " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + idBranch + "), 0) AS Tonkho                                      " +
                                " from ProductCourse pc " +
                                " join Product p on p.id=pc.IdProduct" +
                                " where pc.IdCourse=" + IdCourse;

                SqlCommand command = new SqlCommand(query, connection);
                SqlCommand commandkho = new SqlCommand(querykho, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                SqlDataReader readerkho = commandkho.ExecuteReader();
                while (reader.Read())
                {
                    double amount = Double.Parse(reader["PriceCourse"].ToString(), 0);
                    priceCourse = string.Format("{0:N0}", amount);
                }

                while (readerkho.Read())
                {
                    double dongia = Double.Parse(readerkho["Price"].ToString(), 0);
                    int tonkho = Convert.ToInt32(readerkho["tonkho"]);
                    string css = "",check="";
                    if (tonkho <= 0)
                    {
                        check = "";
                        css = "text-decoration-line-through text-danger";
                    }
                    else
                    {
                        check = "checked";
                        css = "";
                    }
                    int IdProduct = Convert.ToInt32(readerkho["Id"].ToString());  
                    strtable += "<tr class='"+css+"'>"
                        + "<td class='text-center'><input class='form-check-input' type='checkbox' id='IdProduct_" + readerkho["Id"] +"' "+check+ " disabled/></td>"
                        + "<td>" + readerkho["Name"] + "</td>"
                        + "<td class='text-center'>" + readerkho["Unit"].ToString() + "</td>"
                        + "<td class='text-end'>" + string.Format("{0:N0}", dongia) + "</td>"
                        + "</tr>";
                }
                readerkho.Close();
                reader.Close();
            }
            var item = new
            {
                priceCourse,
                strtable
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            int idBranch = Convert.ToInt32(CheckUsers.idBranch());

            // Sử dụng giá trị số nguyên đã chuyển đổi trong truy vấn LINQ
            var countkhoadangky = (from rec in db.RegistrationCourses
                                  join re in db.Registrations on rec.IdRegistration equals re.Id
                                  where re.IdStudent==id 
                                  select rec).Count();
            var countlophoc = (from rec in db.StudentJoinClasses
                               where rec.IdStudent ==id
                               select rec).Count();
            ViewBag.Count = countkhoadangky;
            ViewBag.Countlop = countlophoc;
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", student.IdBranch);
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns.Where(x=>x.Enable==true), "Id", "Name",student.IdMKT);
            ViewBag.IdClass = new SelectList(db.Classes.Where(x => x.IdBranch == idBranch).ToList(), "Id", "Name");
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name");
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns.Where(x=>x.Enable==true && x.IdBranch==idbranch || x.IsPublic==true), "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student, HttpPostedFileBase file)
        {
            int idBranch = Convert.ToInt32(CheckUsers.idBranch());
            int IdUser = Convert.ToInt32(CheckUsers.iduser());

            if (ModelState.IsValid)
            {
                string checkResult = Check_availidStudent(student.Name, student.Phone, student.Email);
                if (checkResult == "ok")
                {
                    try
                    {
                        // Xử lý tải tệp lên
                        if (file != null && file.ContentLength > 0)
                        {
                            // Generate a unique file name
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                            // Specify the path to save the file
                            string _path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);
                            file.SaveAs(_path);
                            student.Image = "/Uploads/Images/" + fileName;
                        }

                        // Xử lý thông tin chi nhánh
                        if (student.IdBranch == null)
                        {
                            student.Code = Getcode_Student(idBranch);
                            student.IdBranch = idBranch;
                        }
                        else
                        {
                            int IdBranch = Convert.ToInt32(student.IdBranch);
                            student.Code = Getcode_Student(IdBranch);
                            student.IdBranch = IdBranch;
                        }

                        // Gán các giá trị còn lại cho đối tượng student
                        student.Username = Get_usernameStudent(student.Name, student.Phone);
                        student.Password = "taptrung";
                        student.DateCreate = DateTime.Now;
                        student.IdUser = IdUser;
                        student.Enable = true;

                        // Thêm student vào cơ sở dữ liệu và lưu lại
                        db.Students.Add(student);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        // Thêm lỗi vào ModelState nếu có ngoại lệ
                        ModelState.AddModelError("", "Có lỗi xảy ra khi tạo sinh viên: " + ex.Message);
                    }
                }
                else
                {
                    // Thêm lỗi kiểm tra vào ModelState
                    ModelState.AddModelError("", checkResult);
                }
            }
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name",student.IdBranch);
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns.Where(x => x.Enable == true&&x.IdBranch==idBranch), "Id", "Name");
            // Nếu ModelState không hợp lệ hoặc có lỗi, trả về View với các thông báo lỗi
            return View(student);
        }

        public string Get_usernameStudent(string Name, string Phone)
        {
            ConvertText convi = new ConvertText();

            // Tách lấy tên cuối cùng từ chuỗi tên đầy đủ
            string[] nameParts = Name.Trim().Split(' ');
            string lastName = nameParts.Length > 0 ? nameParts[nameParts.Length - 1] : "";

            // Chuyển đổi tên cuối cùng sang định dạng thích hợp
            string fixedLastName = convi.fixtex(lastName);

            // Kết hợp tên cuối cùng với số điện thoại
            string username = fixedLastName + Phone;

            // Kiểm tra trong cơ sở dữ liệu xem tên người dùng đã tồn tại hay chưa
            var student = db.Students.SingleOrDefault(x => x.Username == username);
            var st = db.Students.Count(x => x.Username.Contains(username));

            // Nếu chưa tồn tại, trả về tên người dùng mới tạo
            if (student == null)
            {
                return username;
            }

            // Nếu đã tồn tại, trả về tên người dùng với một số bổ sung để đảm bảo tính duy nhất
            return username + "_" + st;
        }

        public string Check_availidStudent(string Name,string Phone,string Email)
        {
            var student = db.Students.SingleOrDefault(x=>x.Name==Name && x.Phone==Phone &&x.Email==Email);
            var checkPhone = db.Students.Where(x=>x.Phone==Phone).Count();
            var checkEmail = db.Students.Where(x=>x.Email==Email).Count();
            if (checkEmail > 2)
            {
                return "Email đã đủ số lượng đăng ký!";
            }
            if (checkPhone > 2)
            {
                return "Số điện thoại đã đủ số lượng đăng ký!";
            }
            if (student != null) return "Đã tồn tại học viên này, vui lòng kiểm tra trong danh sách tiềm năng hoặc danh sách đã xóa!";
            return "ok";
        }

        public ActionResult Load_paymentInfo(int IdRegistration)
        {
            decimal tongtien = 0;
            decimal dathanhtoan = 0;
            decimal conlai = 0;
            var rc = db.RegistrationCourses.Where(x=>x.IdRegistration==IdRegistration);
            var rp = db.RegistrationProducts.Where (x=>x.IdRegistration == IdRegistration);
            var ro = db.RegistrationOthers.Where(x => x.IdRegistration == IdRegistration);
            var registration = db.Registrations.Find(IdRegistration);
            int? IdStudent = registration.IdStudent;
            var voucher = db.StudentVouchers.Where(x=>x.IdStudent==IdStudent);
            decimal? valueVoucher = 0;
                decimal? thu = 0, chi = 0;
            foreach(var i in rc)
            {
                tongtien += (decimal)i.TotalAmount;
            }
            foreach (var i in rp)
            {
                tongtien += (decimal)i.TotalAmount;
            }
            foreach (var i in ro)
            {
                tongtien += (decimal)i.TotalAmount;
            }
            foreach(var i in voucher)
            {
                if (i.Type == true)
                {
                    thu += i.Voucher;
                }
                else
                {
                    chi += i.Voucher;
                }
            }
            valueVoucher = thu - chi;
            dathanhtoan = Check_paymentRegistration(IdRegistration);
            conlai = tongtien - dathanhtoan;

            var item = new { 
                Conlai=string.Format("{0:N0}",conlai) ,
                Code= registration.Code,
                Name= registration.Student.Name,
                valueVoucher = string.Format("{0:N0}", valueVoucher)
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        string Getcode_transaction(bool type)
        {
            string loai = "";
            var idbranch = int.Parse(CheckUsers.idBranch());
            int nextCode = 0;
            if (type == false)
            {
                loai = "PC";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "PT";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(idbranch);
            string str = cn.Code + loai + DateTime.Now.Year + code;

            return str;
        }
        public ActionResult Submit_paymentRegistration(int IdRegistration, decimal Tienthu, decimal Voucher, string Method)
        {
            string status = "", message = "";

            // Kiểm tra giá trị đầu vào
            if (Tienthu <= 0 && Voucher <= 0)
            {
                return Json(new { status = "error", message = "Số tiền thu không thể nhỏ hơn 0!" }, JsonRequestBehavior.AllowGet);
            }

            var registration = db.Registrations.Find(IdRegistration);
            if (registration == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy đăng ký!" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    var rc = db.RegistrationCourses.Where(x => x.IdRegistration == IdRegistration).ToList();
                    var rp = db.RegistrationProducts.Where(x => x.IdRegistration == IdRegistration).ToList();
                    var ro = db.RegistrationOthers.Where(x => x.IdRegistration == IdRegistration).ToList();

                    var newTransaction = new Transaction()
                    {
                        IdBranch = registration.IdBranch,
                        IdStudent = registration.IdStudent,
                        DateCreate = DateTime.Now,
                        Description = "Thu tiền phiếu đăng ký khóa học " + registration.Code,
                        Discount = 0,
                        Status = true,
                        Address = registration.Student.Address,
                        Phone = registration.Student.Phone,
                        Name = registration.Student.Name,
                        Type = true,
                        Code = Getcode_transaction(true),
                        TotalAmount = Tienthu,
                        Amount = Tienthu,
                        PaymentMethod = Method,
                        IdUser = Convert.ToInt32(CheckUsers.iduser()),
                        IdRegistration = IdRegistration
                    };

                    db.Transactions.Add(newTransaction);

                    // Cập nhật trạng thái đăng ký và các bản ghi liên quan
                    registration.Status = true;
                    db.Entry(registration).State = EntityState.Modified;

                    if (rc.Any())
                    {
                        foreach (var i in rc)
                        {
                            i.Status = true;
                            db.Entry(i).State = EntityState.Modified;
                        }
                    }

                    if (ro.Any())
                    {
                        foreach (var i in ro)
                        {
                            i.Status = "1";
                            db.Entry(i).State = EntityState.Modified;
                        }
                    }

                    if (Voucher > 0)
                    {
                        int? iddk = IdRegistration;
                        var studentVoucher = new StudentVoucher()
                        {
                            Voucher = Voucher,
                            IdStudent = registration.IdStudent,
                            DateCreate = DateTime.Now,
                            Description = "Sử dụng cho phiếu đăng ký " + registration.Code,
                            IdUser = int.Parse(CheckUsers.iduser()),
                            Active = true,
                            Type = false,
                            Enable = true,
                            IdTransaction = IdRegistration
                        };
                        db.StudentVouchers.Add(studentVoucher);
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    status = "ok";
                    message = "Đã tạo phiếu thu thành công!";
                }
            }
            catch (Exception ex)
            {
                status = "error";
                message = $"Lỗi hệ thống: {ex.Message}";
                // Ghi log lỗi tại đây nếu cần thiết
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        string Getcode_receiption(bool type)
        {
            string loai = "";
            int idBranch = int.Parse(CheckUsers.idBranch());
            int nextCode = 0;
            if (type == false)
            {
                loai = "XK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == idBranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "NK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == idBranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(7, '0');
            var cn = db.Branches.Find(int.Parse(CheckUsers.idBranch()));
            string str = cn.Code + loai + code;
            return str;
        }
        public ActionResult Submit_xuatkho(int IdRegistration)
        {
            string status = "error";
            string message = "Có lỗi xảy ra!";
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var rec = db.RegistrationProducts.Where(x => x.IdRegistration == IdRegistration && x.Status==false).ToList(); // Fetch list to avoid multiple queries
                    var reg = db.Registrations.Find(IdRegistration);

                    if (reg == null)
                    {
                        message = "Đăng ký không tồn tại!";
                        return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    if (rec.Count == 0)
                    {
                        message = "Không có sản phẩm nào để xuất kho!";
                        return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
                    }

                    // Create warehouse receipt
                    var phieunhap = new WarehouseReceiption()
                    {
                        DateCreate = DateTime.Now,
                        IdUser = Convert.ToInt32(CheckUsers.iduser()),
                        IdBranch = idbranch,
                        TotalAmount = 0,
                        Status = true,
                        Name = reg.Student.Name,
                        Phone = reg.Student.Phone,
                        Address = reg.Student.Address,
                        Description = "Phiếu đăng ký khóa học " + reg.Code,
                        Code = Getcode_receiption(false),
                        Credit = 0,
                        Debit = 0,
                        Active = true,
                        Type = false,
                        Enable = true,
                        Cat="hocvien"
                    };
                    db.WarehouseReceiptions.Add(phieunhap);
                    db.SaveChanges();

                    int warehouseReceiptionId = phieunhap.Id;

                    // Process each product registration and add details
                    foreach (var p in rec)
                    {
                        p.Status = true;
                        var details = new ProductReceiptionDetail()
                        {
                            IdReceiption = warehouseReceiptionId,
                            IdProduct = p.IdProduct,
                            Amount = (int)p.Amount,
                            Price = (decimal)p.Price,
                            TotalAmount = (decimal)p.TotalAmount,
                            Status = true,
                            Type = false,
                            Discount = 0
                        };
                        db.Entry(p).State= EntityState.Modified;
                        db.ProductReceiptionDetails.Add(details);
                    }

                    db.SaveChanges(); // Save all changes at once
                    transaction.Commit(); // Commit transaction

                    status = "ok";
                    message = "Đã xuất kho thành công!";
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback transaction if there's an error
                    message = "Có lỗi xảy ra: " + ex.Message;
                }
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }


        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x => x.Enable == true), "Id", "Name", student.IdBranch);
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns, "Id", "Name", student.IdMKT);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    // Generate a unique file name
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                    // Specify the path to save the file
                    string _path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);
                    file.SaveAs(_path);
                    student.Image = "/Uploads/Images/" + fileName;
                }
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name", student.IdBranch);
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns.Where(x => x.Enable == true), "Id", "Name", student.IdMKT);
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit_createStudent(bool? Enable, int? IdBranch, string Name, DateTime DateOfBirth, string Sex, string Username, string Password, string School, string Class, string Description, string ParentName, string Phone, string Email, DateTime ParentDateOfBirth, string City, string District, string Address, string Relationship, string Job, string Facebook, string Hopeful, string Known, int? IdMKT, HttpPostedFileBase Image)
        {
            string status = "", message = "";
            try
            {
                // Check if the student already exists
                string checkResult = Check_availidStudent(Name, Phone, Email);

                if (checkResult != "ok")
                {
                    return Json(new { status = "error", message = checkResult }, JsonRequestBehavior.AllowGet);
                }
                
                // Initialize the new student object
                var student = new Student
                {
                   Code = Getcode_Student(IdBranch ?? Convert.ToInt32(CheckUsers.idBranch())),
                    Username = string.IsNullOrEmpty(Username) ? Get_usernameStudent(Name, Phone) : Username,
                    Password = string.IsNullOrEmpty(Password) ? "taptrung" : Password,
                    DateOfBirth = DateOfBirth,
                    Name = Name,
                    Sex = Sex,
                    School = School,
                    Class = Class,
                    Description = Description,
                    ParentName = ParentName,
                    Phone = Phone,
                    Email = Email,
                    ParentDateOfBirth = ParentDateOfBirth,
                    City = City,
                    District = District,
                    Address = Address,
                    Relationship = Relationship,
                    Job = Job,
                    Facebook = Facebook,
                    Hopeful = Hopeful,
                    Known = Known,
                    IdMKT = IdMKT,
                    IdBranch = IdBranch ?? Convert.ToInt32(CheckUsers.idBranch()),
                    DateCreate = DateTime.Now,
                    IdUser = Convert.ToInt32(CheckUsers.iduser()),
                    Enable = Enable ?? true  // Defaulting Enable to true if null
                };

                // Process the image upload if any
                if (Image != null && Image.ContentLength > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                    string extension = Path.GetExtension(Image.FileName);
                    fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                    string path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);

                    if (!Directory.Exists(Server.MapPath("~/Uploads/Images")))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Uploads/Images"));
                    }

                    Image.SaveAs(path);
                    student.Image = "/Uploads/Images/" + fileName;
                }

                db.Students.Add(student);
                db.SaveChanges();
                status = "ok";
                message = "Đã thêm mới thành công!";
            }
            catch (DbEntityValidationException ex)
            {
                status = "error";
                message = "Đã xảy ra lỗi khi lưu học viên: ";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        message += $"{validationError.PropertyName}: {validationError.ErrorMessage} ";
                    }
                }
            }
            catch (Exception ex)
            {
                status = "error";
                message = "Đã xảy ra lỗi: " + ex.Message;
            }

            var item = new { status, message };
            return Json(item, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit_editStudent(int Id, string Code, string Name, DateTime? Ngaysinh, string Sex, string Username, string Password, string School, string Class, string Description, string ParentName, string Phone, string Email, DateTime? Ngaysinhphuhuynh, string City, string District, string Address, string Relationship, string Job, string Facebook, string Hopeful, string Known, int? IdMKT, HttpPostedFileBase Image)
        {
            string status = "", message = "";

            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của dữ liệu
            {
                var student = db.Students.Find(Id);
                if (student != null)
                {
                    try
                    {
                        // Cập nhật thông tin học viên
                        student.DateOfBirth = Ngaysinh;
                        student.Code = Code;
                        student.Name = Name;
                        student.Sex = Sex;
                        student.Username = Username;
                        student.Password = Password;
                        student.School = School;
                        student.Class = Class;
                        student.Description = Description;
                        student.ParentName = ParentName;
                        student.Phone = Phone;
                        student.Email = Email;
                        student.ParentDateOfBirth = Ngaysinhphuhuynh;
                        student.City = City;
                        student.District = District;
                        student.Address = Address;
                        student.Relationship = Relationship;
                        student.Job = Job;
                        student.Facebook = Facebook;
                        student.Hopeful = Hopeful;
                        student.Known = Known;
                        student.IdMKT = IdMKT;

                        // Xử lý upload file ảnh
                        if (Image != null && Image.ContentLength > 0)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                            string extension = Path.GetExtension(Image.FileName);
                            fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                            string path = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);

                            try
                            {
                                Image.SaveAs(path);
                                student.Image = "/Uploads/Images/" + fileName;
                            }
                            catch (Exception ex)
                            {
                                status = "error";
                                message = "Không thể lưu ảnh: " + ex.Message;
                                return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        db.Entry(student).State = EntityState.Modified;
                        db.SaveChanges();

                        status = "ok";
                        message = "Cập nhật thành công!";
                    }
                    catch (DbEntityValidationException dbEx) // Bắt lỗi từ Entity Framework
                    {
                        var errors = dbEx.EntityValidationErrors
                                        .SelectMany(e => e.ValidationErrors)
                                        .Select(e => e.PropertyName + ": " + e.ErrorMessage)
                                        .ToList();
                        status = "error";
                        message = "Lỗi khi cập nhật: " + string.Join("; ", errors);
                    }
                    catch (Exception ex)
                    {
                        status = "error";
                        message = "Đã xảy ra lỗi: " + ex.Message;
                    }
                }
                else
                {
                    status = "error";
                    message = "Không tìm thấy học viên này!";
                }
            }
            else
            {
                status = "error";
                message = "Dữ liệu không hợp lệ!";
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete_Students(int id)
        {
            string status, message;
            var st = db.Students.Find(id);

            if (st == null)
            {
                status = "error";
                message = "Không tồn tại học viên này!";
                return Json(new { status, message }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                st.Enable = false;
                db.Entry(st).State = EntityState.Modified;
                db.SaveChanges();

                status = "ok";
                message = "Đã xóa thành công!";
            }
            catch (DbEntityValidationException ex)
            {
                status = "error";
                message = "Có lỗi xảy ra khi xóa dữ liệu!";
                // Log hoặc hiển thị các lỗi xác thực chi tiết nếu cần
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                status = "error";
                message = "Có lỗi xảy ra: " + ex.Message;
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy học viên cần xóa!" }, JsonRequestBehavior.AllowGet);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy học viên cần xóa!" }, JsonRequestBehavior.AllowGet);
            }
            bool hasRelatedRecords = db.StudentJoinClasses.Any(cr => cr.IdStudent == id && cr.Todate>DateTime.Now);
            if (hasRelatedRecords)
            {
                return Json(new { status = "error", message = "Học viên này đang học và không thể xóa!" }, JsonRequestBehavior.AllowGet);
            }

            // Vô hiệu hóa học viên
            student.Enable = false;
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "ok", message = "Đã xóa thành công!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_Reserve(int IdRegistration, int IdCourse)
        {
            var rec = db.RegistrationCourses.SingleOrDefault(x=>x.IdRegistration==IdRegistration&&x.IdCourse==IdCourse);
            if (rec == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy khóa học cần bảo lưu!" }, JsonRequestBehavior.AllowGet);
            }
            bool hasRelatedRecords = db.StudentJoinClasses.Any(cr => cr.IdRegistration == IdRegistration && cr.IdCourse==IdCourse && cr.Todate > DateTime.Now);
            if (!hasRelatedRecords)
            {
                return Json(new { status = "error", message = "Khóa học này đã kết thúc!" }, JsonRequestBehavior.AllowGet);
            }
            var joinClass = db.StudentJoinClasses.FirstOrDefault(x => x.IdRegistration == IdRegistration && x.IdCourse == IdCourse);
            if (joinClass == null)
            {
                return Json(new { status = "error", message = "Khóa học chưa được xét lớp" }, JsonRequestBehavior.AllowGet);
            }
            joinClass.Todate = DateTime.Now;
            rec.StatusReserve = true;
            rec.DateReserve = DateTime.Now;
            db.Entry(joinClass).State= EntityState.Modified;
            db.Entry(rec).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "ok", message = "Đã xóa thành công!" }, JsonRequestBehavior.AllowGet);
        }
        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }
        public ActionResult GetSchedule(int idClass, string fromDate, string toDate,int? idCourse)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            // Lấy lịch nghỉ của cơ sở hiện tại
            var vacationSchedules = db.VacationSchedules
                                      .Where(vs => vs.IdBranch == idbranch)
                                      .ToList();

            var courseBranch = db.CourseBranches.Where(x=>x.IdCourse==idCourse&&x.IdBranch==idbranch).FirstOrDefault();
            var schedulesbyClass = db.Schedules
                .Where(x => x.IdClass == idClass && (bool)x.Active).Include(x => x.Employee).ToList()
                .Select(x => new ClassAssignmentDTO
                {
                    DayOfWeek = scheduleHelper.GetDayName(x.IdWeek),
                    TeacherName = x.Employee.Name,
                    TimeSlot = scheduleHelper.GetTimeSlot((DateTime)x.FromHour, (DateTime)x.ToHour),
                    HourQuantity = scheduleHelper.GetHourQuantity((DateTime)x.FromHour, (DateTime)x.ToHour).ToString(),
                });

            List<TimeTableDTO> timeTableData = new List<TimeTableDTO>();

            fromDate = string.IsNullOrEmpty(fromDate) ? DateTime.Now.ToString("yyyy-MM-dd") : fromDate;
            toDate = string.IsNullOrEmpty(toDate) ? DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd") : toDate;

            DateTime fromDateTime = DateTime.ParseExact(fromDate, "yyyy-MM-dd", null);

            DateTime toDateTime = DateTime.ParseExact(toDate, "yyyy-MM-dd", null);

            for (DateTime date = fromDateTime; date <= toDateTime; date = date.AddDays(1))
            {
                // Kiểm tra ngày hiện tại có trùng lịch nghỉ không
                bool isVacation = vacationSchedules.Any(vs => vs.Fromdate <= date && vs.Todate >= date);

                if (isVacation)
                {
                    // Nếu ngày này trùng lịch nghỉ, bỏ qua và tiếp tục
                    continue;
                }

                var DayOfWeekVNCompared = scheduleHelper.ConvertEnglishDayToVietnamese(date.DayOfWeek.ToString());
                var scheduleMatched = schedulesbyClass.FirstOrDefault(x => x.DayOfWeek == DayOfWeekVNCompared);

                if (scheduleMatched != null)
                {
                    if (timeTableData.Count() < courseBranch.Sessons)
                    {
                        timeTableData.Add(new TimeTableDTO
                        {
                            DayOfWeek = DayOfWeekVNCompared,
                            Date = date.ToString("dd/MM/yyyy"),
                            TimeSlot = scheduleMatched.TimeSlot,
                            HourQuantity = scheduleMatched.HourQuantity
                        });
                    }
                }
            }

            var item = new
            {
                schedulesbyClass,
                timeTableData
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowTimeTableData(int? idClassShowTimeTbl, string fromDate, string toDate, int studentId, int idCourse)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            var courseBranch = db.CourseBranches.Where(x => x.IdCourse == idCourse && x.IdBranch == idbranch).FirstOrDefault();
            var schedulesbyClass = db.Schedules
                .Where(x => x.IdClass == idClassShowTimeTbl && (bool)x.Active).Include(x => x.Employee).ToList()
                .Select(x => new ClassAssignmentDTO
                {
                    DayOfWeek = scheduleHelper.GetDayName(x.IdWeek),
                    TeacherName = x.Employee.Name,
                    TimeSlot = scheduleHelper.GetTimeSlot((DateTime)x.FromHour, (DateTime)x.ToHour),
                    HourQuantity = scheduleHelper.GetHourQuantity((DateTime)x.FromHour, (DateTime)x.ToHour).ToString(),
                });

            List<TimeTableDTO> timeTableData = new List<TimeTableDTO>();

            fromDate = string.IsNullOrEmpty(fromDate) ? DateTime.Now.ToString("yyyy-MM-dd") : fromDate;
            toDate = string.IsNullOrEmpty(toDate) ? DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd") : toDate;

            DateTime fromDateTime = DateTime.ParseExact(fromDate, "yyyy-MM-dd", null);

            DateTime toDateTime = DateTime.ParseExact(toDate, "yyyy-MM-dd", null);

            for (DateTime date = fromDateTime; date <= toDateTime; date = date.AddDays(1))
            {
                var DayOfWeekVNCompared = scheduleHelper.ConvertEnglishDayToVietnamese(date.DayOfWeek.ToString());
                var scheduleMatched = schedulesbyClass.FirstOrDefault(x => x.DayOfWeek == DayOfWeekVNCompared);

                if (scheduleMatched != null)
                {
                    if (timeTableData.Count() < courseBranch.Sessons)
                    {
                        timeTableData.Add(new TimeTableDTO
                        {
                            DayOfWeek = DayOfWeekVNCompared,
                            Date = date.ToString("dd/MM/yyyy"),
                            TimeSlot = scheduleMatched.TimeSlot,
                            HourQuantity = scheduleMatched.HourQuantity
                        });
                    }
                }
            }

            var student = db.Students.FirstOrDefault(x => x.Id == studentId);

            ViewBag.timeTableData = timeTableData;
            ViewBag.SubTitle = "Ngày" + ":" + fromDate + " - " + toDate;
            ViewBag.StudentName = student.Name;
            ViewBag.ClassName = student.Class;
            ViewBag.CourseName = courseBranch.Course.Name;
            return View();
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DeletedList()
        {
            ViewBag.IdBranch = new SelectList(db.Branches.ToList(), "Id", "Name");
            return View();
        }
        public ActionResult Loadlist_deleted(int IdBranch,string searchString)
        {
            var student = db.Students.Where(x => x.IdBranch == IdBranch && x.Enable==false);
            string str = "";
            int count = 0;
            if(!string.IsNullOrEmpty(searchString))
            {
                student = student.Where(x=>x.Name.Contains(searchString));
            }
            foreach(var s in student)
            {
                count++;
                str += "<tr>"
                    +"<td class='text-center'>"+count+"</td>"
                    +"<td class='text-center'>"+s.Code+"</td>"
                    +"<td>"+s.Name+"</td>"
                    +"<td>"+s.Username+ "</td>"
                    + "<td class='text-center'>" +(s.DateOfBirth==null?"-":s.DateOfBirth.Value.ToString("dd/MM/yyyy"))+"</td>"
                    +"<td class='text-center'>" +s.Sex+"</td>"
                    +"<td>"+s.User.Name+"</td>"
                    +"<td class='text-end'>" +
                    "<a href='javascript:restore_Student("+s.Id+")' class='text-primary'><i class='ti ti-refresh'></i></a>" +
                    "</td>"
                    + "</tr>";
            }
            return Json(new {status="ok",str},JsonRequestBehavior.AllowGet);
        }
        public ActionResult Restore_student(int? Id)
        {
            if (Id == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy học viên cần khôi phục!" }, JsonRequestBehavior.AllowGet);
            }
            Student student = db.Students.Find(Id);
            if (student == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy học viên cần Khôi phục!" }, JsonRequestBehavior.AllowGet);
            }

            // Vô hiệu hóa học viên
            student.Enable = true;
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "ok", message = "Đã khôi phục thành công!" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EndingSoon()
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x => x.Enable == true).ToList(), "Id", "Name");
            var courses = db.Courses.OrderBy(x => x.Program.DisplayOrder)
                        .ThenBy(x => x.DisplayOrder)
                        .ToList();

            // Create a list to hold the select options, starting with "All Courses"
            var courseList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Tất cả khóa học" }
            };

            // Add the rest of the courses to the list
            courseList.AddRange(courses.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));

            // Create the SelectList with the course list
            ViewBag.IdCourse = new SelectList(courseList, "Value", "Text");
            return View();
        }
        public ActionResult Loadlist_endingSoon(int? IdBranch, int IdCourse)
        {
            if (IdBranch == null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            string str = "";
            DateTime today = DateTime.Now;
            DateTime thresholdDate = today.AddDays(14);

            // Query to get data from database
            var query = db.StudentJoinClasses
                .Where(x => x.Class.IdBranch == IdBranch
                            && x.Todate.HasValue
                            && x.Todate.Value <= thresholdDate);

            if (IdCourse!=0)
            {
                query = query.Where(x => x.IdCourse == IdCourse);
            }

            // Execute query and fetch data to memory
            var students = query
                .Select(x => new
                {
                    IdStudent = x.IdStudent,
                    IdRegistration = x.IdRegistration,
                    IdCourse = x.IdCourse,
                    CourseName = x.Course.Name,
                    CodeStudent = x.Student.Code,
                    NameStudent = x.Student.Name,
                    UsernameStudent = x.Student.Username,
                    DateOfBirth = x.Student.DateOfBirth,
                    Sex = x.Student.Sex, // Assuming this is a bool for simplicity
                    Fromdate = x.Fromdate,
                    Todate = x.Todate,
                    Status = x.Todate.HasValue && x.Todate.Value <= thresholdDate && x.Todate.Value > today ? "Sắp kết khóa" : "",
                    ClassStatus = x.Registration.RegistrationCourses.Any(m => m.StatusJoinClass == false) ? "Chờ xét lớp" : "Đã xét lớp"
                })
                .ToList() // Fetch data into memory
                .Select(s => new
                {
                    s.IdStudent,
                    s.IdRegistration,
                    s.IdCourse,
                    s.CourseName,
                    s.CodeStudent,
                    s.NameStudent,
                    s.UsernameStudent,
                    DateOfBirth = s.DateOfBirth.HasValue ? s.DateOfBirth.Value.ToString("dd/MM/yyyy") : "",
                    Sex = s.Sex, // Assuming Sex is a boolean where true = Male and false = Female
                    Fromdate = s.Fromdate.HasValue ? s.Fromdate.Value.ToString("dd/MM/yyyy") : "",
                    Todate = s.Todate.HasValue ? s.Todate.Value.ToString("dd/MM/yyyy") : "",
                    s.Status,
                    s.ClassStatus
                })
                .ToList();

            int count = 0;
            if (students.Count > 0)
            {
                foreach (var s in students)
                {
                    var rec = db.RegistrationCourses.Any(x=>x.Registration.IdStudent == s.IdStudent && x.StatusJoinClass==false);
                    string statusStudent = "";
                    if (rec)
                    {
                        statusStudent = "<span class='badge bg-success text-white'>Đã tái khóa</span>";
                    }
                    else
                    {
                        statusStudent = "<span class='badge bg-danger text-white'>Chưa tái khóa</span>";
                    }
                    count++;
                    str += "<tr>"
                        + "<td class='text-center'>" + count + "</td>"
                        + "<td class='text-center'>" + s.CodeStudent + "</td>"
                        + "<td>" + s.NameStudent + "</td>"
                        + "<td>" + s.UsernameStudent + "</td>"
                        + "<td class='text-center'>" + s.DateOfBirth + "</td>"
                        + "<td class='text-center'>" + s.Sex + "</td>"
                        + "<td>" + s.CourseName + "</td>"
                        + "<td class='text-center'>" + s.Todate + "</td>"
                        + "<td class='text-center'>" + statusStudent + "</td>"
                        + "</tr>";
                }
            }
            else
            {
                str = "<tr><td colspan='9' class='text-center'>Không có học viên sắp kết khóa</td></tr>";
            }

            var item = new
            {
                str
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BirthDayList()
        {
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true).ToList(), "Id", "Name");
            return View();
        }
        public ActionResult Loadlist_birthday(int? IdBranch,int Month)
        {
            if(IdBranch == null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            string str = "";
            var student = db.Students.Where(x=>x.IdBranch==IdBranch && x.DateOfBirth.Value.Month== Month).OrderBy(x=>x.DateOfBirth);
            if (student.Count() > 0)
            {
                foreach (var s in student)
                {
                    var check = db.RegistrationCourses.Any(x => x.StatusJoinClass == false && x.Registration.IdStudent == s.Id);
                    var checkStatus = db.StudentJoinClasses.Any(x => x.IdStudent == s.Id && x.Todate > DateTime.Now);
                    if (check || checkStatus)
                    {
                        str += "<li class='timeline-item d-flex position-relative overflow-hidden'>"
                                + "<div class='timeline-time text-dark flex-shrink-0 text-end'>"+s.DateOfBirth.Value.ToString("dd/MM/yyyy")+"</div>"
                                    + "<div class='timeline-badge-wrap d-flex flex-column align-items-center'>"
                                        + "<span class='timeline-badge border-2 border border-info flex-shrink-0 my-8'></span>"
                                        + "<span class='timeline-badge-border d-block flex-shrink-0'></span>"
                                    + "</div>"
                                + "<div class='timeline-desc fs-3 text-dark mt-n1 fw-semibold'>"+s.Name+"</div>"
                            + "</li>";
                    }
                }
            }
            else
            {
                str += "<li class='timeline-item d-flex position-relative overflow-hidden'>"
                                + "<div class='timeline-time text-dark flex-shrink-0 text-end'>0</div>"
                                    + "<div class='timeline-badge-wrap d-flex flex-column align-items-center'>"
                                        + "<span class='timeline-badge border-2 border border-info flex-shrink-0 my-8'></span>"
                                        + "<span class='timeline-badge-border d-block flex-shrink-0'></span>"
                                    + "</div>"
                                + "<div class='timeline-desc fs-3 text-dark mt-n1 fw-semibold'>Không có học viên sinh nhật trong tháng!</div>"
                            + "</li>";
            }
            var item = new { str};
            return Json(item, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Load_exchangeBranch(int IdStudent)
        {
            var cn = db.Branches.Where(x => x.Enable == true);
            string str = "";
            foreach (var items in cn)
            {
                str += "<option value='" + items.Id + "'>" + items.Name + "</option>";
            }
            return Json(new {str}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Onchange_exchangeBranch(int IdBranch)
        {
            var user = db.Users.Where(x => x.IdBranch == IdBranch && x.Enable == true);
            string strUser="";
            foreach (var items in user)
            {
                strUser += "<option value='" + items.Id + "'>" + items.Name + "</option>";
            }
            var item = new
            {
                strUser
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_exchangeBranch(int IdStudent, int IdBranch, int IdUser)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var student = db.Students.Find(IdStudent);
                    if (student == null)
                    {
                        return Json(new { status = "error", message = "Student not found!" }, JsonRequestBehavior.AllowGet);
                    }

                    // Update student details
                    student.IdBranch = IdBranch;
                    student.IdUser = IdUser;
                    db.Entry(student).State = EntityState.Modified;

                    // Fetch registrations and transactions only if needed
                    var registrations = db.Registrations.Where(x => x.IdStudent == IdStudent && x.IdBranch != IdBranch).ToList();
                    var transactions = db.Transactions.Where(x => x.IdStudent == IdStudent && x.IdBranch != IdBranch).ToList();

                    // Update registrations if there are any
                    if (registrations.Count > 0)
                    {
                        foreach (var reg in registrations)
                        {
                            reg.IdBranch = IdBranch;
                            db.Entry(reg).State = EntityState.Modified;
                        }
                    }

                    // Update transactions if there are any
                    if (transactions.Count > 0)
                    {
                        foreach (var tr in transactions)
                        {
                            tr.IdBranch = IdBranch;
                            db.Entry(tr).State = EntityState.Modified;
                        }
                    }

                    // Save all changes at once
                    db.SaveChanges();
                    transaction.Commit(); // Commit transaction

                    return Json(new { status = "ok", message = "Branch exchange completed successfully." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback transaction in case of error
                    return Json(new { status = "error", message = "An error occurred: " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult PrintSchedule(int? IdRegistration, int? IdCourse)
        {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());

            // Tìm thông tin lớp học và học viên đăng ký
            var joinClass = db.StudentJoinClasses
                              .FirstOrDefault(x => x.IdCourse == IdCourse && x.IdRegistration == IdRegistration);

            if (joinClass == null)
            {
                return Redirect("/error/e404");
            }

            int? IdClass = joinClass.IdClass;
            int? IdStudent = joinClass.IdStudent;

            // Lấy thông tin khóa học và chi nhánh
            var courseBranch = db.CourseBranches
                                 .FirstOrDefault(x => x.IdCourse == IdCourse && x.IdBranch == idbranch);



            // Lấy lịch nghỉ của cơ sở hiện tại
            var vacationSchedules = db.VacationSchedules
                                      .Where(vs => vs.IdBranch == idbranch)
                                      .ToList();

            var schedulesbyClass = db.Schedules
                .Where(x => x.IdClass == IdClass && (bool)x.Active).Include(x => x.Employee).ToList()
                .Select(x => new ClassAssignmentDTO
                {
                    DayOfWeek = scheduleHelper.GetDayName(x.IdWeek),
                    TeacherName = x.Employee.Name,
                    TimeSlot = scheduleHelper.GetTimeSlot((DateTime)x.FromHour, (DateTime)x.ToHour),
                    HourQuantity = scheduleHelper.GetHourQuantity((DateTime)x.FromHour, (DateTime)x.ToHour).ToString(),
                    RoomName = scheduleHelper.GetRoomName((int)IdClass,x.IdWeek)
                });

            List<TimeTableDTO> timeTableData = new List<TimeTableDTO>();

            // Lấy ngày bắt đầu và kết thúc từ joinClass
            DateTime fromDate = (DateTime)joinClass.Fromdate;
            DateTime toDate = (DateTime)joinClass.Todate;

            for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                // Kiểm tra ngày hiện tại có trùng lịch nghỉ không
                bool isVacation = vacationSchedules.Any(vs => vs.Fromdate <= date && vs.Todate >= date);

                if (isVacation)
                {
                    // Nếu ngày này trùng lịch nghỉ, bỏ qua và tiếp tục
                    continue;
                }

                var DayOfWeekVNCompared = scheduleHelper.ConvertEnglishDayToVietnamese(date.DayOfWeek.ToString());
                var scheduleMatched = schedulesbyClass.FirstOrDefault(x => x.DayOfWeek == DayOfWeekVNCompared);

                if (scheduleMatched != null)
                {
                    if (timeTableData.Count() < courseBranch.Sessons)
                    {
                        timeTableData.Add(new TimeTableDTO
                        {
                            DayOfWeek = DayOfWeekVNCompared,
                            Date = date.ToString("dd/MM/yyyy"),
                            TimeSlot = scheduleMatched.TimeSlot,
                            HourQuantity = scheduleMatched.HourQuantity,
                            TeacherName = scheduleMatched.TeacherName,
                            RoomName = scheduleMatched.RoomName
                        });
                    }
                }
            }

            // Lấy thông tin học viên và lớp học
            var student = db.Students.Find(IdStudent);
            var clas = db.Classes.Find(IdClass);

            // Gửi dữ liệu ra View để hiển thị thời khóa biểu
            ViewBag.timeTableData = timeTableData;
            ViewBag.SubTitle = "Ngày" + ": " + fromDate.ToString("dd/MM/yyyy") + " - " + toDate.ToString("dd/MM/yyyy");
            ViewBag.StudentName = student.Name;
            ViewBag.ClassName = clas.Name;
            ViewBag.CourseName = courseBranch.Course.Name;

            return View();
        }

        public ActionResult SendEmail(int id)
        {
            // Tạo model với dữ liệu cần thiết cho template
            var model = new
            {
                TenNguoiNhan = "Nguyen Van A + id:"+ id,
                LinkXacNhan = "https://yourwebsite.com/confirm?token=12345"
            };

            SendEmailHelper help = new SendEmailHelper();
            help.SendEmailWithTemplate("recipient@example.com", "Xác nhận tài khoản của bạn", model, "EmailTemplate_newStudent.html");
            return Content("Email đã được gửi!");
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
