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
        public ActionResult Index()
        {
            ViewBag.IdProduct = new SelectList(db.Products.Where(x=>x.Enable==true && x.Active==true), "Id", "Name");
            var programs = db.Programs.Where(x=>x.Enable==true).Include(p => p.User).OrderBy(x=>x.DisplayOrder);
            return View(programs.ToList());
        }
        [HttpPost]
        public ActionResult Submit_addProgram(string action,int? Id,string Code,string Name,int DisplayOrder,string Description)
        {
            string status = "ok";
            string message = "";
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
                    status = "ok";
                    message = "Đã thêm thành công!";
                }
                else
                {
                    status = "error";
                    message = "Đã tồn tại mã chương trình này!";
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
                status = "ok";
                message = "Đã cập nhật thành công!";
            }
            var item = new {
                message,
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
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
