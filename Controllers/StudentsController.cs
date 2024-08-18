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

        public ActionResult AddCourseProgramOfStudents(int IdStudent)
        {
            Student student = Connect.SelectSingle<Student>("select * from Student where Id='" + IdStudent + "'");
            Session["infoUser"] = student;
            return View(student);
        }
       
        public ActionResult RegistrationPrints()
        {
            Student student = Session["infoUser"] as Student;
            int idregistration = Convert.ToInt32(Session["IdRegistration"]);
           
            Registration registration = Connect.SelectSingle<Registration>("Select * from Registration where Id = '" + idregistration + "'");
            DataTable datatable = Connect.SelectAll("select course.Name,course.Price,rescourse.TotalAmount from Registration  res\r\ninner join RegistrationCourse rescourse\r\non rescourse.IdRegistration = res.Id\r\ninner join Course course \r\non course.Id = rescourse.IdCourse\r\nwhere res.Id = '" + idregistration+"'");
            var model = new
            {
                DataTableLoad = datatable
            };
            if (student != null)
            {
                Session["Name"] = student.Name;
                Session["code"] = registration.Code;
                Session["Datecreate"] = registration.DateCreate;
                Session["totalamount"] = registration.TotalAmount;
                Session["datatable"] = datatable;
            }
          
            return View(model);
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
        public string GenerateCode()
        {
            string prefix = "DK_HQ";

            // Generate a random number
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999); // Adjust range as needed

            // Generate the new code
            string newCode = $"{prefix}_{randomNumber}";

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

        public ActionResult getData(string IdRegistration)
        {
            DataTable dataTableCourse = Connect.SelectAll("SELECT cour.Name AS NameCourse, rescourse.IdCourse, res.Id, rescourse.Price, pro.Name AS NameProgram, res.Amount, rescourse.TotalAmount, res.Code, res.DateCreate, res.Discount FROM Registration res INNER JOIN RegistrationCourse rescourse ON rescourse.IdRegistration = res.Id INNER JOIN Course cour ON cour.Id = rescourse.IdCourse INNER JOIN Program pro ON pro.Id = cour.IdProgram WHERE res.Id = '" + IdRegistration + "'");
            DataTable dataTableProduct = Connect.SelectAll("SELECT resproduct.Discount,pro.Name, resproduct.Price, resproduct.TotalAmount, res.Id, resproduct.IdProduct FROM Registration res INNER JOIN RegistrationProduct resproduct ON res.Id = resproduct.IdRegistration INNER JOIN Product pro ON pro.Id = resproduct.IdProduct WHERE res.Id = '" + IdRegistration + "'");
            DataTable dataTableOther = Connect.SelectAll("select revenue.Name,revenue.Price,revenue.Discount,other.Amount,other.TotalAmount,res.Id,other.IdReference from Registration res inner join RegistrationOther other on other.IdRegistration = res.Id inner join RevenueReference revenue on revenue.Id = other.IdReference where other.IdRegistration = '"+IdRegistration+"'");
            Registration registration = Connect.SelectSingle<Registration>("SELECT * FROM Registration WHERE Id = '" + IdRegistration + "'");

            var data = new StringBuilder();
            var totalAmount = 0;
            var i = 0;

            List<DataRow> allRows = new List<DataRow>();
            allRows.AddRange(dataTableCourse.AsEnumerable());
            allRows.AddRange(dataTableProduct.AsEnumerable());
            allRows.AddRange(dataTableOther.AsEnumerable());

            foreach (DataRow row in allRows)
            {
                i++;
                string amountString = "";
                string discount = "";
                string name = "";
                string total = "";
                var idobject = 0;
                if (row.Table == dataTableCourse)
                {
                    amountString = string.Format("{0:N0} đ", row["TotalAmount"]);
                    discount = string.Format("{0:N0} đ", row["Discount"]);
                    name = row["NameProgram"].ToString() + "<hr>" + row["NameCourse"].ToString();
                    idobject = Convert.ToInt32(row["IdCourse"]);
                    total = amountString;
                }
                else if (row.Table == dataTableProduct)
                {
                    amountString = string.Format("{0:N0} đ", row["Price"]);
                    discount = string.Format("{0:N0} đ", row["Discount"]);
                    total = string.Format("{0:N0} đ", row["TotalAmount"]);
                    idobject = Convert.ToInt32(row["IdProduct"]);
                    name = row["Name"].ToString();
                }else if(row.Table == dataTableOther)
                {
                    amountString = string.Format("{0:N0} đ", row["Price"]);
                    discount = string.Format("{0:N0} đ", row["Discount"]);
                    total = string.Format("{0:N0} đ", row["TotalAmount"]);
                    idobject = Convert.ToInt32(row["IdReference"]);
                    name = row["Name"].ToString();
                }

                var newRow = "<tr>" +
                    "<td style='text-align:center;'>" + i + "</td>" +
                    "<td style='text-align:left;'>" + name + "</td>" +
                    "<td style='text-align:center;'>1</td>" +
                    "<td style='text-align:right;'>" + amountString + "</td>" +
                    "<td style='text-align:center;'>" + discount + "</td>" +
                    "<td style='text-align:right;'>" + total + "</td>" +
                    "<td style='text-align:center;'>" +
                    "<a href='#' class='btn btn-sm btn-danger ti-trash' onclick=\"deleteitem('" + idobject + "','" + row["Id"] + "')\" data-courseid='" + idobject + "' data-registrationid='" + row["Id"] + "'>" +
                    "<i class='bx bx-trash-alt font-size-18'></i>" +
                    "</a>" +
                    "</td>" +
                    "</tr>";

                data.Append(newRow);

                if (row.Table == dataTableCourse)
                {
                    totalAmount += Convert.ToInt32(row["TotalAmount"]);
                }
                else if (row.Table == dataTableProduct)
                {
                    totalAmount += Convert.ToInt32(row["TotalAmount"]);
                }else if(row.Table == dataTableOther)
                {
                    totalAmount += Convert.ToInt32(row["TotalAmount"]);
                }
            }

            var result = new
            {
                datalist = data.ToString(),
                TotalAmount = totalAmount,
                DateCreate = Convert.ToDateTime(registration.DateCreate).ToString("dd/MM/yyyy"),
                idRegistrations = registration.Id,
                Code = registration.Code
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public class listProduct
        {
           public int isChecked {  get; set; }
           public int idpro {  get; set; }
        }
    
        [HttpPost] 
        public ActionResult SaveRegistration(int? IdRegistration,int type, int? IdObject,int? price,int? totalamount,int? amount,string Description,int? Discount)
        {
            string iduser = CheckUsers.iduser();
            Student student = Session["infoUser"] as Student;

            Registration registration = new Registration();
            registration = Connect.SelectSingle<Registration>("select * from Registration where Id='" + IdRegistration + "'");
             // Create new Registration
            if(registration == null)
            {
                Registration NewRegistration = new Registration();
                NewRegistration.IdStudent = student.Id;
                NewRegistration.IdUser = Convert.ToInt32(iduser);
                NewRegistration.Amount = amount;
                NewRegistration.Code = GenerateCode();
                NewRegistration.TotalAmount = totalamount;
                NewRegistration.DateCreate = DateTime.Now;
                NewRegistration.Status = true;
                NewRegistration.Enable = true;
                NewRegistration.Description = Description;
                NewRegistration.IdBranch = student.IdBranch;
                db.Registrations.Add(NewRegistration);
                db.SaveChanges();
                registration = NewRegistration;
                Session["IdRegistration"] = registration.Id;
            }
            //Check type luồng Product,Course,Order
            switch (type)
            {
                case 1:
                    RegistrationProduct registrationProduct = Connect.SelectSingle<RegistrationProduct>("select * from RegistrationProduct where IdProduct = '"+IdObject+"'  and IdRegistration = '"+registration.Id+"'");
                    if(registrationProduct != null)
                    {
                        return Json(null);

                    }else
                    {
                        RegistrationProduct NewregistrationProduct = new RegistrationProduct();
                        NewregistrationProduct.IdRegistration = registration.Id;
                        NewregistrationProduct.IdProduct = Convert.ToInt32(IdObject);
                        NewregistrationProduct.Status = true;
                        NewregistrationProduct.Price = price;
                        NewregistrationProduct.Amount = amount;
                        NewregistrationProduct.TotalAmount = totalamount;
                        NewregistrationProduct.Discount = Discount;
                        db.RegistrationProducts.Add(NewregistrationProduct);
                        db.SaveChanges();
                        return Json(NewregistrationProduct.IdRegistration);
                    }
                case 2:
                    RegistrationCourse registrationCourse = Connect.SelectSingle<RegistrationCourse>("select * from RegistrationCourse  where IdRegistration = '"+registration.Id+"'  AND IdCourse = '"+IdObject+"'"); 
                    if(registrationCourse != null)
                    {
                        return Json(null);
                    }else
                    {
                        RegistrationCourse NewregistrationCourse = new RegistrationCourse();
                        NewregistrationCourse.Status = true;
                        NewregistrationCourse.Price = price;
                        NewregistrationCourse.Amount = amount;
                        NewregistrationCourse.TotalAmount = totalamount;
                        NewregistrationCourse.IdRegistration = registration.Id;
                        NewregistrationCourse.IdCourse  = Convert.ToInt32(IdObject);
                        NewregistrationCourse.Discount = Discount;
                        NewregistrationCourse.StatusExchangeCourse = false;
                        NewregistrationCourse.StatusExtend = false;
                        NewregistrationCourse.StatusJoinClass=false;
                        NewregistrationCourse.StatusReserve=false;
                        db.RegistrationCourses.Add(NewregistrationCourse);
                        db.SaveChanges();
                        return Json(NewregistrationCourse.IdRegistration);
                    }
                case 3:
                    RegistrationOther RegistrationOther = Connect.SelectSingle<RegistrationOther>("select * from RegistrationOther where IdRegistration = '"+registration.Id+"' and IdReference = '"+IdObject+"'");
                    if(RegistrationOther != null)
                    {
                        return Json(null);
                    }else
                    {
                        RegistrationOther NewRegistrationOther = new RegistrationOther();
                        NewRegistrationOther.IdRegistration = registration.Id;
                        NewRegistrationOther.IdReference = Convert.ToInt32(IdObject);
                        NewRegistrationOther.Discount = Discount;
                        NewRegistrationOther.Status = "1";
                        NewRegistrationOther.Price = price;
                        NewRegistrationOther.Amount = amount;
                        NewRegistrationOther.TotalAmount = totalamount;
                        db.RegistrationOthers.Add(NewRegistrationOther);
                        db.SaveChanges();
                        return Json(NewRegistrationOther.IdRegistration);
                    }
                default:
                    return Json(null);
            }

        }
        public ActionResult getDataComboxOther(int? IdOther)
        {
            List<RevenueReference> revenueReferences = Connect.Select<RevenueReference>("select * from RevenueReference");

            var str = "";
            var totalamount = 0;
            if (IdOther != 0)
            {
                RevenueReference product = Connect.SelectSingle<RevenueReference>("select * from RevenueReference where Id = '" + IdOther + "'");
                totalamount = Convert.ToInt32(product.Price);
                
            }
            else
            {
                foreach (var items in revenueReferences)
                {
                    str += "<option value='" + items.Id + "' data-name='" + items.Name + "'>" + items.Name + "</option>";
                }
                totalamount = Convert.ToInt32(revenueReferences[0].Price);
            }

            var item = new
            {
                str,
                totalamount
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
                string query = "select course.Id as IdCourse,re.id as IdRegistration,re.Code,course.Name as NameCourse,c.Name as NameClass,joinclass.Sessions,joinclass.DateCreate,joinclass.Fromdate,joinclass.Todate,us.Name as NameUser from StudentJoinClass joinclass inner join Registration re on re.id=joinclass.IdRegistration,Course course,Class c,[User] us where course.Id = joinclass.IdCourse and c.id=joinclass.IdClass and us.id=joinclass.IdUser and re.IdStudent=" + idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("test:" +reader["NameClass"].ToString());   
                    count++;
                    str +="<tr>"
                        + "<td>" + count + "</td>"
                        + "<td>" + reader["Code"].ToString() + "</td>"
                        + "<td>" + reader["NameCourse"].ToString() + "</td>"
                        + "<td>" + reader["NameClass"].ToString() + "</td>"
                        + "<td>" + DateTime.Parse(reader["Fromdate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                        + "<td>" + DateTime.Parse(reader["Todate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                        + "<td class='text-center'>0/" + reader["Sessions"].ToString() + "</td>"
                        + "<td>" + DateTime.Parse(reader["DateCreate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
                        + "<td class='text-center'>"+(DateTime.Parse(reader["DateCreate"].ToString())>DateTime.Now?"<i class='ti ti-circle-check text-danger' title='Đã kết khóa'></i>": "<i class='ti ti-circle-check text-success' title='Đang học'></i>") +"</td>"
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
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select s.Id,re.Id as IdRegistration,re.Code as Code,rec.IdCourse,course.Name as NameCourse ,re.DateCreate,rec.Status, rec.TotalAmount,rec.StatusExchangeCourse,rec.StatusJoinClass,rec.StatusExtend,rec.StatusReserve from Student s inner join registration re on re.IdStudent= s.id inner join RegistrationCourse rec on rec.IdRegistration = re.id, Course course where course.id= rec.IdCourse and s.id=" + idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Status = reader["status"],
                    // StatusExchangeCourse = reader["statusexchangecourse"].ToString(),
                    // StatusJoinClass= reader["StatusJoinClass"].ToString(),
                    // StatusExtend = reader["StatusExtend"].ToString()
                    double amount = Double.Parse(reader["TotalAmount"].ToString());
                    count++;
                    str +="<tr>"
                        +"<td>"+count+"</td>"
                        +"<td>"+ reader["Code"].ToString() + "</td>"
                        +"<td>"+reader["NameCourse"].ToString() + "</td>"
                        +"<td>"+ string.Format("{0:N0} đ", amount) + "</td>"
                        +"<td>"+ reader["DateCreate"].ToString() + "</td>"
                        +"<td class='text-center'>"+ (Convert.ToBoolean(reader["Status"]) ? "<i class='ti ti-circle-check text-success'></i>" : "Chưa thanh toán") + "</td>"
                        +"<td class='text-center'>"+ (Convert.ToBoolean(reader["StatusJoinClass"]) ? "<span class='text-success fw-bolder'>Đã xét lớp</span>" : "Chờ xét lớp") + "</td>"
                        + "<td class='text-end'>"
                        + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                        +"<i class=\"ti ti-dots-vertical\"></i>"
                        +"</a>"
                        +"<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                        +"<li><a class=\"dropdown-item\" href='javascript:Xetlop_modal(" + reader["IdRegistration"] + "," + reader["IdCourse"] +")'><i class=\"ti ti-eye-check\"></i> Xét vào lớp</a></li>"
                        +"</ul>"
                        + "</td>"
                        + "</tr>";
                }
                reader.Close();
            }
            var data = new {str};
            return Json(data,JsonRequestBehavior.AllowGet); 
        }
        public ActionResult Load_xetlop(int idRegistration,string idCourse,int idStudent) {
            string str = "";
            string registrionCode =db.Registrations.Find(idRegistration).Code;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select rec.IdCourse,c.Name,rec.Status from RegistrationCourse rec inner join Registration re on re.id= rec.IdRegistration, Course c where c.id=rec.IdCourse and re.IdStudent = "+idStudent;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["IdCourse"].ToString()==idCourse)
                    {
                        str += "<option value='" + reader["IdCourse"] +"' data-status='" + reader["Status"] +"' selected>" + reader["Name"] +"</option>";
                    }
                    else
                    {
                        str += "<option value='" + reader["IdCourse"] +"' data-status='" + reader["Status"] +"'>" + reader["Name"] +"</option>";
                    }
                    
                }
                reader.Close();
            }
            var item = new { 
                str,registrionCode
            };
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
                        +"<td>"+ reader["Voucher"] + "</td>"
                        +"<td>"+ reader["Description"] + "</td>"
                        +"<td>" + DateTime.Parse(reader["DateCreate"].ToString()).ToString("dd/MM/yyyy hh:mm tt") + "</td>"
                        +"<td>"+ reader["Name"] + "</td>"
                        + "<td class='text-center'>" + (Convert.ToBoolean(reader["Type"])?"Nạp":"Xuất")+"</td>"
                        + "</tr>";
                }
                str += "<tr class='bg-light'><td colspan=5 class='text-end'>Số dư:</td><td class='text-end'>"+(nhap-xuat)+"</td></tr>";
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
        public ActionResult getDataComboxProduct(int? idproduct)
        {
            List<Product> products = Connect.Select<Product>("select * from Product");
            var str = "";
            var number = 0;
            var totalamount = 0;
            if (idproduct != 0)
            {
                Product product = Connect.SelectSingle<Product>("select * from Product where Id = '" + idproduct + "'");
                totalamount = Convert.ToInt32(product.Price);
                number = Convert.ToInt32(product.NumberOfPackage);

            }
            else
            {
                foreach (var items in products)
                {

                    str += "<option value='" + items.Id + "' data-name='" + items.Name + "' data-inventory='" + ProductReceiptionDetailsController.GetInventory(items.Id) + "'>" + items.Name + "</option>";
                }

                totalamount = Convert.ToInt32(products[0].Price);
                number = Convert.ToInt32(products[0].NumberOfPackage);
            }

            var item = new
            {
                str,
                totalamount,
                number
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDataCombobox(int? IdProgram, int? type)
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
            List<Promotion> promotions = Connect.Select<Promotion>("Select * from Promotion where idBranch='"+idBranch+"' and todate>getdate() and active='1'");
            if (promotions.Count > 0)
            {
                foreach (var promotion in promotions)
                {
                    
                    strpromotion += "<option value='" + string.Format("{0:N0}", promotion.Value) + "' data-name='" + promotion.Name + "'>" + promotion.Name + "</option>";
                }
            }
            else
            {
                strpromotion += "<option value='0' data-name='--'>--</option>";
            }
            if (type == 1)
            {
                if (IdProgram == 0)
                {
                    sqlquery = " and c.IdProgram='" + programs[0].Id + "'";
                }
                else
                {
                    sqlquery = " and c.IdProgram='" + IdProgram + "'";
                }
            }
            else
            {

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
                strpromotion,
                type=type
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

                string querykho = "select p.Id,p.Name,p.Unit,p.Price,pc.Amount" +
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
                    strtable += "<tr>"
                        + "<td class='text-center'></td>"
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
        public ActionResult GetDataComboboxx(int? IdProgram,int? IdCourse,int? type)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            var item = new
            {
                courses = new List<Course>(),
                programs = new List<Program>()
            };

            // Lấy danh sách tất cả các chương trình
            List<Program> programs = Connect.Select<Program>("SELECT * FROM Program");
            List<Promotion> promotions = Connect.Select<Promotion>("Select * from Promotion where idBranch='"+idBranch+"' and todate>getdate() and active='1'");
            List<Product> products = Connect.Select<Product>("select * from Product");
            var priceCourse = 0;
            // Nếu không có IdProgram được chọn, lấy danh sách khóa học của chương trình đầu tiên
            if (IdProgram == 0 && type == 1)
            {
                // Lấy danh sách khóa học của chương trình đầu tiên
                    if (programs.Count > 0)
                    {
                        List<Course> courses = Connect.Select<Course>("SELECT * FROM Course WHERE IdProgram = '" + programs[0].Id + "'");
                        priceCourse = Convert.ToInt32(courses[0].Price);
                        item.programs.AddRange(programs);
                        item.courses.AddRange(courses);
                    }
                
            }
            else if(IdProgram != 0 && type == 1)
            {

                    // Nếu có IdProgram được chọn, lấy danh sách khóa học của chương trình tương ứng
                    List<Course> courses = Connect.Select<Course>("SELECT * FROM Course WHERE IdProgram = '" + IdProgram + "'");
                    priceCourse = Convert.ToInt32(courses[0].Price);
                    item.courses.AddRange(courses);

            
            }else if (type == 2) 
            {
                if (IdCourse != 0)
                {
                    Course course = Connect.SelectSingle<Course>("select * from Course where Id = '" + IdCourse + "'");
                    priceCourse = Convert.ToInt32(course.Price);
                }
            }
          
            // Tạo chuỗi HTML cho các dropdown chương trình
            var strpro = "";
            if (item.programs.Count > 0)
            {
                foreach (var pro in item.programs)
                {
                    strpro += "<option value='" + pro.Id + "' data-name='" + pro.Name + "'>" + pro.Name + "</option>";
                }
            }
            var strcour = "";
            foreach (var course in item.courses)
            {
                strcour += "<option value='" + course.Id + "' data-name='" + course.Name + "'>" + course.Name + "</option>";
            }

            var strpromotion = "";
            foreach(var promotion in promotions)
            {
                strpromotion += "<option value='" + promotion.Id + "' data-name='" + promotion.Name + "'>" + promotion.Name + "</option>";
            }
            var strTable = "";
            foreach (var itemtable in products)
            {
                strTable += "<tr class='listproduct'>"
                      + "<th><input class='idproduct' onclick='updateCheckboxValue(this)'  type='checkbox' value=''/><input class='idpro' value='"+itemtable.Id+"' hidden/></th>"
                    + "<th>" + itemtable.Name + "</th>"
                     + "<th>" + itemtable.Price + "</th>"
                       + "<th>" + itemtable.Price + "</th>"
                    + "</tr>";
            }
            // Tạo đối tượng để chứa chuỗi HTML của dropdown chương trình và khóa học
            var strcombobox = new
            {
                strpro,
                strcour,
                strTable,
                strpromotion,
                priceCourse =  priceCourse,
                type = type
                // Đây là để giữ chỗ, bạn có thể thêm logic để tạo chuỗi HTML cho dropdown khóa học tương ứng
            };

            return Json(strcombobox, JsonRequestBehavior.AllowGet);
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
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name");
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Image,Code,DateOfBirth,Sex,Username,Password,Enable,School,Class,Description,ParentName,Phone,Email,ParentDateOfBirth,City,District,Address,Relationship,Job,Facebook,Hopeful,Known,IdMKT,IdBranch,PowerScore,Balance,Presenter,Status,Power,StatusStudy")] Student student, HttpPostedFileBase file)
        {
            int idBranch= Convert.ToInt32(CheckUsers.idBranch());
            int IdUser = Convert.ToInt32(CheckUsers.iduser());
            if (ModelState.IsValid)
            {
                if (Check_availidStudent(student.Name, student.Phone, student.Email) == "ok")
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
                    student.Username=Get_usernameStudent(student.Name,student.Phone);
                    student.Password = "taptrung";
                    student.DateCreate = DateTime.Now;
                    student.IdUser = IdUser;
                    student.Enable = true;
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "<div class='alert alert-danger'>" + Check_availidStudent(student.Name,student.Phone,student.Email)+"</div>";
                }
            }
            return View(student);
        }
        public string Get_usernameStudent(string Name,string Phone) {
            ConvertText convi = new ConvertText();
            string username = convi.fixtex(Name) + Phone;
            var student  = db.Students.SingleOrDefault(x=>x.Username== username);
            var st = db.Students.Where(x=>x.Username.Contains(username)).Count();
            if(student == null)
            {
                return username;
            }
            return username+"_"+st;
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
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", student.IdBranch);
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
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", student.IdBranch);
            ViewBag.IdMKT = new SelectList(db.MKTCampaigns, "Id", "Name", student.IdMKT);
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit_editStudent(int Id, string Code, string Name, DateTime DateOfBirth, string Sex, string Username, string Password, string School, string Class, string Description, string ParentName, string Phone, string Email, DateTime ParentDateOfBirth, string City, string District, string Address, string Relationship, string Job, string Facebook, string Hopeful, string Known, int IdMKT, HttpPostedFileBase Image)
        {
            string status="", message="";
            var student = db.Students.Find(Id);
            if (student != null)
            {
                student.DateOfBirth = DateOfBirth;
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
                student.ParentDateOfBirth = ParentDateOfBirth;
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
                    Image.SaveAs(path);
                    student.Image = "/Uploads/Images/" + fileName;
                }
                status = "ok";
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                status = "Error";
                message = "Không tìm thấy học viên này!";
            }
            var item = new { status, message };
            return Json(item, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetSchedule(int idClass, string fromDate, string toDate)
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
                    timeTableData.Add(new TimeTableDTO
                    {
                        DayOfWeek = DayOfWeekVNCompared,
                        Date = date.ToString("dd/MM/yyyy"),
                        TimeSlot = scheduleMatched.TimeSlot,
                        HourQuantity = scheduleMatched.HourQuantity
                    }); ;
                }
            }

            var item = new
            {
                schedulesbyClass,
                timeTableData
            };

            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowTimeTableData(int? idClassShowTimeTbl, string fromDate, string toDate, int studentId)
        {
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
                    timeTableData.Add(new TimeTableDTO
                    {
                        DayOfWeek = DayOfWeekVNCompared,
                        Date = date.ToString("dd/MM/yyyy"),
                        TimeSlot = scheduleMatched.TimeSlot,
                        HourQuantity = scheduleMatched.HourQuantity
                    }); ;
                }
            }

            var student = db.Students.FirstOrDefault(x=> x.Id == studentId);

            ViewBag.timeTableData = timeTableData;
            ViewBag.SubTitle = "Ngày" + ":"+ fromDate + " - " + toDate;

            ViewBag.StudentName = student.Name;
            ViewBag.ClassName = student.Class;

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
        public ActionResult Loadlist_deleted()
        {
            return View();
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
