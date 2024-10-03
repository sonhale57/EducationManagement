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
                return View();
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

        [HttpPost]
        public ActionResult Submit_adduser(int IdEmployee, string Username, string Password)
        {
            if (string.IsNullOrEmpty(Password))
            {
                return Json(new {status="error",message="Lỗi cập nhật: Không có mật khẩu!"}, JsonRequestBehavior.AllowGet);
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
                user.Username = Username;
                user.Password = pass;
                db.Entry(user);
                db.SaveChanges();
                return Json(new {status="ok",message="Thành công: Đã cập nhật thành công!"}, JsonRequestBehavior.AllowGet);
            }
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
            int IdUser = Convert.ToInt32(CheckUsers.iduser());

            if (ModelState.IsValid)
            {
                if (db.Employees.Any(x => x.Phone == employee.Phone || x.Email==employee.Email))
                {
                    TempData["error"] = "<script>showError('Lỗi cập nhật: Số điện thoại hoặc email đã tồn tại!',3000);</script>";
                    // Trả lại form với thông báo lỗi
                    ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", employee.IdBranch);
                    ViewBag.IdPosition = new SelectList(db.Positions, "Id", "Name", employee.IdPosition);
                    return View(employee);
                }
                // Tạo Username ban đầu
                string baseUsername = "GV_" + convi.fixtex(GetLastName(employee.Name)) + GetLastThreeDigits(employee.Phone);
                string Username = baseUsername;
                // Kiểm tra nếu Username đã tồn tại
                if (db.Users.Any(x => x.Username == Username))
                {
                    TempData["error"] = "<script>showError('Lỗi cập nhật: Username đã tồn tại!',3000);</script>";
                    // Trả lại form với thông báo lỗi
                    ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", employee.IdBranch);
                    ViewBag.IdPosition = new SelectList(db.Positions, "Id", "Name", employee.IdPosition);
                    return View(employee);
                }

                // Nếu IdBranch của nhân viên chưa được gán, sử dụng IdBranch của user hiện tại
                if (employee.IdBranch == null)
                {
                    employee.IdBranch = IdBranch;
                }

                // Thiết lập các giá trị mặc định cho nhân viên
                employee.Enable = true;
                employee.IsOfficial = false;
                employee.DateCreate = DateTime.Now;

                // Thêm nhân viên vào bảng Employees
                db.Employees.Add(employee);
                db.SaveChanges(); // Lưu để có thể lấy được Id của nhân viên

                MD5Hash md5 = new MD5Hash();
                string Password = md5.GetMD5Working("taptrung");
                Password = md5.mahoamd5(Password.Replace("&^%$", ""));
                // Tạo user mới và gán thông tin
                var us = new User()
                {
                    Username = Username,
                    Password = Password, // Mật khẩu mặc định, có thể thay đổi sau
                    Expire = DateTime.Now,
                    DateCreate = DateTime.Now,
                    Active = true,
                    Enable = true,
                    IdBranch = employee.IdBranch,
                    Name = employee.Name,
                    Createby = IdUser,
                    IdEmployee = employee.Id
                };

                // Thêm user vào bảng Users
                db.Users.Add(us);
                db.SaveChanges();

                // Điều hướng về trang danh sách nhân viên sau khi thêm thành công
                return RedirectToAction("Index");
            }

            // Hiển thị lại form nếu có lỗi trong quá trình xác thực
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
