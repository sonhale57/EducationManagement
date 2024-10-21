using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class ProgramsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Programs
        public ActionResult Index(string searchString)
        {
            ViewBag.IdProduct = new SelectList(db.Products.Where(x=>x.Enable==true && x.Active==true), "Id", "Name");
            return View();
        }
        public ActionResult Loadlist(string searchString)
        {
            string str = "";
            var programs = db.Programs.Where(x => x.Enable == true);
            if (programs == null)
            {
                str = "<tr colspan=7 class='text-center'>Không có dữ liệu khóa học</tr>";
            }
            foreach(var item in programs.OrderBy(x => x.DisplayOrder))
            {
                str += "<tr>" 
                    + "<td class='text-success text-center fw-bolder'>"+item.Code+"</td>"
                    + "<td class='text-success fw-bolder' colspan=5>"+item.Name 
                    +"<a href=\"javascript:Edit_Program("+item.Id+")\" class=\"ms-2\"><i class=\"ti ti-edit text-primary fw-bolder\"></i></a>" 
                    +"<a href=\"javascript:Delete_Program("+item.Id+")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a></td>"
                    + "</tr>";
                var courses = db.Courses.Where(x=>x.IdProgram==item.Id);
                if (!string.IsNullOrEmpty(searchString))
                {
                    courses = courses.Where(x=>x.Name.Contains(searchString));
                }
                foreach(var course in courses.OrderBy(x=>x.DisplayOrder))
                {
                    str += "<tr>"
                        +"<td class='text-center'>"+course.Code+"</td>"
                        +"<td class='text-left'>"+course.Name+"</td>"
                        +"<td class='text-center'>"+course.Price+"</td>"
                        +"<td class='text-center'>"+course.Sessions+"</td>"
                        +"<td class='text-center'>"+course.Hours+"</td>"
                        + "<td class='text-end'>"
                            + "<a href=\"/courses/edit/" + course.Id + "\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                            + "<a href=\"/javascript:Delete_Course(" + course.Id + ")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                            + "<a class=\"text-warning\" id=\"dropdownMenuButton\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">"
                            + "<i class=\"ti ti-dots-vertical\"></i>"
                            + "</a>"
                            + "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenuButton\">"
                            + "<li><a class=\"dropdown-item\" href=\"javascript:Load_ProductCourse(" + item.Id + ")\"><i class=\"ti ti-lock\"></i> Cài đặt vật tư</a></li>"
                            + "</ul>"
                        + "</td>"
                        + "</tr>";
                }
            }
            return Json(new { str },JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_addProgram(string action,int? Id,string Code,string Name,int? DisplayOrder,string Description)
        {
            if(action == "create")
            {
                var program = db.Programs.SingleOrDefault(x => x.Code == Code);
                if (program == null)
                {
                    var p = new Program()
                    {
                        DateCreate = DateTime.Now,
                        Code = Code,
                        Name = Name,
                        IdUser = int.Parse(CheckUsers.iduser()),
                        Description = Description,
                        Enable = true,
                        DisplayOrder = DisplayOrder,
                        IsTest = true
                    };
                    db.Programs.Add(p);
                    db.SaveChanges();
                        return Json(new {status = "ok", message = "Thành công: Đã thêm mới thành công!"}, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new {status = "error", message = "Lỗi cập nhật: Đã tồn tại mã chương trình này!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else if(action == "edit")
            {
                var p = db.Programs.Find(Id);
                p.Description = Description;
                p.Name = Name;
                p.Code = Code;
                p.DisplayOrder = DisplayOrder;
                db.Entry(p);
                db.SaveChanges();
            }
            return Json(new {status = "ok", message = "Thành công: Đã cập nhật thành công!"}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadedit_Program(int id) {
            var p = db.Programs.Find(id);
            var item = new { 
                name = p.Name,
                code= p.Code,
                displayorder = p.DisplayOrder,
                description = p.Description
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        #region default
        // GET: Programs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }
            return View(program);
        }
        // GET: Programs/Create
        public ActionResult Create()
        {
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Programs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Description,DisplayOrder,IsTest,Enable,DateCreate,IdUser")] Program program)
        {
            if (ModelState.IsValid)
            {
                program.DateCreate = DateTime.Now;
                program.IdUser = int.Parse(CheckUsers.iduser());
                db.Programs.Add(program);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", program.IdUser);
            return View(program);
        }

        // GET: Programs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Program program = db.Programs.Find(id);
            if (program == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", program.IdUser);
            return View(program);
        }

        // POST: Programs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Description,DisplayOrder,IsTest,Enable,DateCreate,IdUser")] Program program)
        {
            if (ModelState.IsValid)
            {
                db.Entry(program).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", program.IdUser);
            return View(program);
        }

        public ActionResult Submit_delete(int id)
        {
            var program = db.Programs.Find(id);
            if(program == null)
            {
                return Json(new {status="error",message="Không tìm thấy chương trình cần xóa"},JsonRequestBehavior.AllowGet);
            }
            var course = db.Courses.Where(x=>x.IdProgram == id);
            if (course.Any())
            {
                var sJoinClass= db.RegistrationCourses.Any(x=>x.Course.IdProgram==id);
                if (sJoinClass)
                {
                    return Json(new { status = "error", message = "Chương trình không thể xóa, đang có khóa học của chương trình này được đăng ký!" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    program.Enable = false;
                    db.Entry(program).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { status = "ok", message = "Đã ngừng kích hoạt chương trình này!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                db.Programs.Remove(program);
                db.SaveChanges();
                return Json(new {status="ok",message="Đã xóa chương trình thành công!"},JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var c = await db.Programs.FindAsync(id);
            if (c == null)
            {
                return HttpNotFound();
            }

            db.Programs.Remove(c);
            await db.SaveChangesAsync();

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
        
    }
}
