using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers.RegistrationStudent
{
    public class ProductCoursesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: ProductCourse ss
        public async Task<ActionResult> Index()
        {
            var productCourses = db.ProductCourses.Include(p => p.Course).Include(p => p.Product).Include(p => p.User);
            return View(await productCourses.ToListAsync());
        }

        // GET: ProductCourses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCourse productCourse = await db.ProductCourses.FindAsync(id);
            if (productCourse == null)
            {
                return HttpNotFound();
            }
            return View(productCourse);
        }

        // GET: ProductCourses/Create
        public ActionResult Create()
        {
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code");
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: ProductCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DateCreate,IdUser,IdCourse,IdProduct,Amount,Enable,Status")] ProductCourse productCourse)
        {
            if (ModelState.IsValid)
            {
                db.ProductCourses.Add(productCourse);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", productCourse.IdCourse);
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productCourse.IdProduct);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", productCourse.IdUser);
            return View(productCourse);
        }

        // GET: ProductCourses/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCourse productCourse = await db.ProductCourses.FindAsync(id);
            if (productCourse == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", productCourse.IdCourse);
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productCourse.IdProduct);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", productCourse.IdUser);
            return View(productCourse);
        }

        // POST: ProductCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DateCreate,IdUser,IdCourse,IdProduct,Amount,Enable,Status")] ProductCourse productCourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productCourse).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", productCourse.IdCourse);
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productCourse.IdProduct);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", productCourse.IdUser);
            return View(productCourse);
        }

        // GET: ProductCourses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCourse productCourse = await db.ProductCourses.FindAsync(id);
            if (productCourse == null)
            {
                return HttpNotFound();
            }
            return View(productCourse);
        }

        // POST: ProductCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductCourse productCourse = await db.ProductCourses.FindAsync(id);
            db.ProductCourses.Remove(productCourse);
            await db.SaveChangesAsync();
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
