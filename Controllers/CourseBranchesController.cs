using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class CourseBranchesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: CourseBranches
        public ActionResult Index()
        {
            var courseBranches = db.CourseBranches.Include(c => c.Branch).Include(c => c.Course);
            return View(courseBranches.ToList());
        }

        // GET: CourseBranches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseBranch courseBranch = db.CourseBranches.Find(id);
            if (courseBranch == null)
            {
                return HttpNotFound();
            }
            return View(courseBranch);
        }

        // GET: CourseBranches/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code");
            return View();
        }

        // POST: CourseBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdBranch,IdCourse,PriceCourse,PriceAccount,PriceTest,Hour,Sessons,DiscountPrice,StatusDiscount,FromdateDiscount,TodateDiscount")] CourseBranch courseBranch)
        {
            if (ModelState.IsValid)
            {
                db.CourseBranches.Add(courseBranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", courseBranch.IdBranch);
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", courseBranch.IdCourse);
            return Redirect("/courses");
        }

        // GET: CourseBranches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseBranch courseBranch = db.CourseBranches.Find(id);
            if (courseBranch == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", courseBranch.IdBranch);
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", courseBranch.IdCourse);
            return View(courseBranch);
        }

        // POST: CourseBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdBranch,IdCourse,PriceCourse,PriceAccount,PriceTest,Hour,Sessons,DiscountPrice,StatusDiscount,FromdateDiscount,TodateDiscount")] CourseBranch courseBranch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseBranch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", courseBranch.IdBranch);
            ViewBag.IdCourse = new SelectList(db.Courses, "Id", "Code", courseBranch.IdCourse);
            return View(courseBranch);
        }

        // GET: CourseBranches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseBranch courseBranch = db.CourseBranches.Find(id);
            if (courseBranch == null)
            {
                return HttpNotFound();
            }
            return View(courseBranch);
        }

        // POST: CourseBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseBranch courseBranch = db.CourseBranches.Find(id);
            db.CourseBranches.Remove(courseBranch);
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
