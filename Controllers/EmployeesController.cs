using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using System.Threading.Tasks;
using SuperbrainManagement.Helpers;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Xml.Linq;
using System.IO;

namespace SuperbrainManagement.Controllers
{
    public class EmployeesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Employees

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string idBranch, string filterEnum)
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            else
            {
                var branches = db.Branches.Where(x => x.Enable == true).ToList();
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

                ViewBag.CurrentBranch = idBranch;

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                ViewBag.CurrentFilter = searchString;

                var employees = db.Employees.Where(x => x.Enable == true).ToList();
                if (!string.IsNullOrEmpty(filterEnum))
                {
                    employees = employees.Where(x => x.IsOfficial == Boolean.Parse(filterEnum)).ToList();
                    ViewBag.SelectedFilter = (filterEnum == "1" ? "Official" : "");
                    ViewBag.CurrentEnum = (filterEnum == "1" ? "True" : "False");
                }
                else
                {
                    employees = employees.Where(x => x.IsOfficial == true).ToList();
                    ViewBag.SelectedFilter = "Official";
                    ViewBag.CurrentEnum = "True";
                }

                if (!string.IsNullOrEmpty(idBranch))
                {
                    employees = employees.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
                }
                if (!string.IsNullOrEmpty(searchString))
                {
                    employees = employees.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        employees = employees.OrderByDescending(s => s.Name).ToList();
                        break;
                    case "date":
                        employees = employees.OrderBy(s => s.Id).ToList();
                        break;
                    case "name":
                        employees = employees.OrderBy(s => s.Name).ToList();
                        break;
                    default:
                        employees = employees.OrderByDescending(s => s.Id).ToList();
                        break;
                }
                int pageSize = 20;
                int pageNumber = (page ?? 1);


                var pagedData = employees.ToPagedList(pageNumber, pageSize);

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
        }
        public ActionResult Loadlist(bool IsOfficial, int? IdBranch, string searchString)
        {
            // Lấy ID của branch người dùng hiện tại
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());

            // Lọc danh sách nhân viên
            var emp = db.Employees.Where(x => x.Enable == true && x.IsOfficial == IsOfficial);

            // Nếu IdBranch không có giá trị, lấy IdBranch của user đăng nhập
            if (IdBranch == null)
            {
                IdBranch = idbranch;
            }

            // Lọc theo IdBranch
            emp = emp.Where(x => x.IdBranch == IdBranch);

            // Nếu searchString không rỗng, thực hiện tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                emp = emp.Where(x => x.Name.Contains(searchString));
            }

            // Biến chứa chuỗi HTML để render danh sách
            string str = "";
            int stt = 0;

            // Duyệt qua từng nhân viên và tạo hàng cho bảng
            foreach (var item in emp)
            {
                stt++;
                str += "<tr>"
                    + "<td class='text-center align-content-center'>" + stt + "</td>"
                    + "<td class='text-start align-content-center'>" + item.Name + "</td>"
                    + "<td class='text-center align-content-center'>" + item.Phone + "</td>"
                    + "<td class='text-center align-content-center'>" + item.Email + "</td>"
                    + "<td class='text-center align-content-center'>" + (item.IdPosition == null ? "-" : item.Position.Name) + "</td>"
                    + "<td class='text-center align-content-center'>" + (item.StartWork == null ? "-" : item.StartWork.Value.ToString("dd/MM/yyyy")) + "</td>"
                    + "<td class='text-center align-content-center'>" + (item.CertificateNumber == null ? "-" : item.CertificateNumber.ToString()) + "</td>"
                    + "<td class='text-end align-content-center'>"
                    + "<a href=\"/employees/edit/" + item.Id + "\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                    + "<a href=\"javascript:Delete(" + item.Id + ")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                    + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                    + "<i class=\"ti ti-dots-vertical\"></i>"
                    + "</a>"
                    + "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                    + "<li><a class=\"dropdown-item\" href=\"javascript:UserbyId(" + item.Id + ")\"><i class=\"ti ti-lock\"></i> Tài khoản đăng nhập</a></li>"
                    + "</ul>"
                    + "</td>"
                    + "</tr>";
            }

            // Trả về chuỗi HTML dạng JSON
            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }

        public static string GetLastName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            string[] nameParts = fullName.Split(' ');
            return nameParts[nameParts.Length - 1];
        }
        public static string GetLastThreeDigits(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 3)
            {
                return phoneNumber;
            }

            return phoneNumber.Substring(phoneNumber.Length - 3);
        }
        public ActionResult Submit_AddPosition(string Code, string Name, string Action)
        {
            string status = "ok";
            string message = "";
            var pc = db.Positions.SingleOrDefault(x => x.Code == Code && x.Enable == true);
            if (Action == "create")
            {
                if (pc == null)
                {
                    var c = new Position()
                    {
                        Code = Code,
                        Name = Name,
                        Enable = true
                    };
                    db.Positions.Add(c);
                    db.SaveChanges();
                    status = "ok";
                    message = "Đã thêm thành công!";
                }
                else
                {
                    status = "error";
                    message = "Đã tồn tại mã này!";
                }
            }
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_create(string Name, string Phone, string Email, string Sex, string Address, string Description, DateTime DateOfBirth, DateTime StartWork, string Gratuate, int? IdBranch, int IdPosition, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (IdBranch == null)
                    {
                        IdBranch = Convert.ToInt32(CheckUsers.idBranch());
                    }
                    var e = new Employee()
                    {
                        Name = Name,
                        Phone = Phone,
                        Email = Email,
                        Sex = Sex,
                        Address = Address,
                        Description = Description,
                        DateOfBirth = DateOfBirth,
                        StartWork = StartWork,
                        Gratuate = Gratuate,
                        IdBranch = IdBranch,
                        IdPosition = IdPosition,
                        DateCreate = DateTime.Now
                    };
                    // Xử lý việc lưu ảnh đại diện (nếu có)
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(imageFile.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Uploads/Images"), fileName);
                        imageFile.SaveAs(filePath);
                        e.Image = "/Uploads/Images/" + fileName;
                    }

                    // Thực hiện thêm mới nhân viên vào cơ sở dữ liệu
                    db.Employees.Add(e);
                    db.SaveChanges();

                    return Json(new { status = "ok", message = "Thành công: Đã thêm mới nhân sự thành công!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        status = "error",
                        message = "Lỗi cập nhật: " + ex.Message
                    });
                }
            }
            return Json(new { status = "error", message = "Lỗi cập nhật: Dữ liệu không hợp lệ." });
        }
        public JsonResult CreateUser(int IdEmployee, string Name, string Password)
        {
            try
            {
                // Băm mật khẩu bằng MD5
                MD5Hash md5 = new MD5Hash();
                Password = md5.GetMD5Working(Password);
                string pass = md5.mahoamd5(Password.Replace("&^%$", ""));

                // Lấy thông tin nhân sự từ bảng Employees
                var employee = db.Employees.Find(IdEmployee);
                if (employee == null)
                {
                    return Json(new { success = false, message = "Nhân viên không tồn tại." });
                }

                var IdBranch = employee.IdBranch;
                var Phone = employee.Phone;

                // Khởi tạo ConvertText để xử lý tên
                ConvertText convi = new ConvertText();
                string baseUsername = "GV_" + convi.fixtex(GetLastName(Name)) + GetLastThreeDigits(Phone);

                // Kiểm tra xem Username đã tồn tại hay chưa
                string newUsername = baseUsername;
                int count = 1;

                while (db.Users.Any(u => u.Username == newUsername))
                {
                    // Tạo Username mới nếu bị trùng
                    newUsername = baseUsername + count.ToString();
                    count++;
                }

                // Tạo user mới
                var newUser = new User()
                {
                    IdEmployee = IdEmployee,
                    Name = Name,
                    Username = newUsername, // Sử dụng Username mới tạo
                    Password = pass,
                    Enable = true,
                    Active = true,
                    DateCreate = DateTime.Now,
                    IdBranch = IdBranch,
                    Createby = Convert.ToInt32(CheckUsers.iduser()),
                    Expire = DateTime.Now.AddMonths(6)
                };

                // Thêm vào database và lưu thay đổi
                db.Users.Add(newUser);
                db.SaveChanges();

                return Json(new { success = true, message = "User đã được tạo thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Submit_adduser(int IdEmployee,string Username,string Name,string Password)
        {
            string status = "ok";
            string message = "ok";
            if (string.IsNullOrEmpty(Password))
            {
                message = "Không có mật khẩu!";
            }
            else
            {
                MD5Hash md5 = new MD5Hash();
                Password = md5.GetMD5Working(Password);
                string pass = md5.mahoamd5(Password.Replace("&^%$", ""));
                var user = db.Users.FirstOrDefault(u => u.IdEmployee == IdEmployee);
                var IdBranch = db.Employees.Find(IdEmployee).IdBranch;
                var Phone = db.Employees.Find(IdEmployee).Phone;
                ConvertText convi = new ConvertText();
                if (user == null)
                {
                    var newuser = new User()
                    {
                        IdEmployee = IdEmployee,
                        Name = Name,
                        Username = "GV_" + convi.fixtex(GetLastName(Name)) + GetLastThreeDigits(Phone),
                        Password = pass,
                        Enable = true,
                        Active = true,
                        DateCreate = DateTime.Now,
                        IdBranch = IdBranch,
                        Createby = Convert.ToInt32(CheckUsers.iduser()),
                        Expire = DateTime.Now.AddMonths(6)
                    };
                    db.Users.Add(newuser);
                    db.SaveChanges();
                    status = "Đã tạo tài khoản thành công!";

                }
                else
                {
                    user.Username = Username;
                    user.Password = pass;
                    db.Entry(user);
                    db.SaveChanges();
                    status = "Đã cập nhật tài khoản thành công!";
                }
            }
            
            var item = new {
                status,
                message
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete_Employee(int id)
        {
            string status, message;
            var pc = db.Employees.Find(id);
            var user = db.Users.FirstOrDefault(u => u.IdEmployee == id);
            if (user == null)
            {
                status = "error";
                message = "Không tồn tại nhân sự này!";
            }
            else
            {
                user.Enable = false;
                db.Entry(user); 
                db.SaveChanges();
            }
            pc.Enable = false;
            db.Entry(pc);
            db.SaveChanges();
            status = "ok";
            message = "Đã xóa thành công!";
            var item = new { status,message
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Confirm_official(int id)
        {
            string status, message;
            var pc = db.Employees.Find(id);
            if (pc == null)
            {
                status = "error";
                message = "Không tồn tại nhân sự này!";
            }
            pc.IsOfficial = true;
            db.Entry(pc);
            db.SaveChanges();
            status = "ok";
            message = "Đã cập nhật thành công!";
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_infoEmployee(int id)
        {
            var e = db.Employees.Find(id);
            string username = "",action="";
            var us = db.Users.FirstOrDefault(x=>x.IdEmployee==id);
            if (us == null)
            {
                username = "";
                action = "create";
            }
            else
            {
                action = "edit";
                username = us.Username;
            }
            var item = new { 
                name = e.Name,
                action,
                username,
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }

        #region default

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name");
            ViewBag.IdPosition = new SelectList(db.Positions, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,IdPosition,Description,Image,Phone,Email,CertificateNumber,DateCertificate,StartWork,IdBranch,IsOfficial,Enable,DateOfBirth,Address,Gratuate,Sex,DateCreate,DateStart,BasicSalary,BankName,BankAccountName,BankNumber,BankBranch")] Employee employee)
        {
            ConvertText convi = new ConvertText();
            int IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            if (ModelState.IsValid)
            {
                string Username = "GV_" + convi.fixtex(GetLastName(employee.Name)) + GetLastThreeDigits(employee.Phone);
                if (employee.IdBranch == null)
                {
                    employee.IdBranch = IdBranch;
                }
                employee.Enable = true;
                employee.IsOfficial = false;
                employee.DateCreate = DateTime.Now;
                db.Employees.Add(employee);
                if (db.Users.Any(x => x.Username == Username)) {
                    TempData["error"] = "<span class='alert alert-danger'>Đã tồn tại username của nhân sự này, vui lòng kiểm tra lại!</span>";
                }
                else
                {
                    var us = new User() {
                        Username = Username,
                        Password = "taptrung",
                        Expire = DateTime.Now.AddMonths(3),
                        DateCreate = DateTime.Now,
                        Active = true
                    };
                    db.Users.Add(us);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", employee.IdBranch);
            ViewBag.IdPosition = new SelectList(db.Positions, "Id", "Name", employee.IdPosition);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", employee.IdBranch);
            ViewBag.IdPosition = new SelectList(db.Positions, "Id", "Name", employee.IdPosition);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,IdPosition,Description,Image,Phone,Email,CertificateNumber,DateCertificate,StartWork,IdBranch,IsOfficial,Enable,DateOfBirth,Address,Gratuate,Sex,DateCreate,DateStart,BasicSalary,BankName,BankAccountName,BankNumber,BankBranch")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", employee.IdBranch);
            ViewBag.IdPosition = new SelectList(db.Positions, "Id", "Name", employee.IdPosition);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion
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
