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
    public class TrainingCoursesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: TrainingCourses
        public ActionResult Index()
        {
            var trainingCourses = db.TrainingCourses.Include(t => t.TrainingType).Include(t => t.User);
            return View(trainingCourses.ToList());
        }

        // GET: TrainingCourses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Create
        public ActionResult Create()
        {
            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Code");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: TrainingCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Description,Fromdate,Todate,Enable,Active,ResgistrationDeadline,Price,Number,IdType,IdUser,DateCreate")] TrainingCourse trainingCourse)
        {
            if (ModelState.IsValid)
            {
                db.TrainingCourses.Add(trainingCourse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Code", trainingCourse.IdType);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", trainingCourse.IdUser);
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Code", trainingCourse.IdType);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", trainingCourse.IdUser);
            return View(trainingCourse);
        }

        // POST: TrainingCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Description,Fromdate,Todate,Enable,Active,ResgistrationDeadline,Price,Number,IdType,IdUser,DateCreate")] TrainingCourse trainingCourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainingCourse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdType = new SelectList(db.TrainingTypes, "Id", "Code", trainingCourse.IdType);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", trainingCourse.IdUser);
            return View(trainingCourse);
        }

        // GET: TrainingCourses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            if (trainingCourse == null)
            {
                return HttpNotFound();
            }
            return View(trainingCourse);
        }

        // POST: TrainingCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainingCourse trainingCourse = db.TrainingCourses.Find(id);
            db.TrainingCourses.Remove(trainingCourse);
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
