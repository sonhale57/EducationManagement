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
    public class DataSuggestsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: DataSuggests
        public ActionResult Index()
        {
            var dataSuggests = db.DataSuggests.Include(d => d.Branch).Include(d => d.User);
            return View(dataSuggests.ToList());
        }
        public JsonResult GetAutocompleteSuggestions(int type, string term)
        {
            // Tạo danh sách gợi ý cho từng type
            var suggestions = new List<string>();

                suggestions = db.DataSuggests
                                .Where(s => s.Type==type)
                                .Select(s => s.Name)
                                .ToList();
            
            // Tương tự cho các type khác...

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
        // GET: DataSuggests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSuggest dataSuggest = db.DataSuggests.Find(id);
            if (dataSuggest == null)
            {
                return HttpNotFound();
            }
            return View(dataSuggest);
        }

        // GET: DataSuggests/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: DataSuggests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Description,Type,Value,DisplayOrder,IdBranch,Enable,Active,DateCreate,IdUser")] DataSuggest dataSuggest)
        {
            if (ModelState.IsValid)
            {
                db.DataSuggests.Add(dataSuggest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", dataSuggest.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", dataSuggest.IdUser);
            return View(dataSuggest);
        }

        // GET: DataSuggests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSuggest dataSuggest = db.DataSuggests.Find(id);
            if (dataSuggest == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", dataSuggest.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", dataSuggest.IdUser);
            return View(dataSuggest);
        }

        // POST: DataSuggests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Description,Type,Value,DisplayOrder,IdBranch,Enable,Active,DateCreate,IdUser")] DataSuggest dataSuggest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dataSuggest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", dataSuggest.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", dataSuggest.IdUser);
            return View(dataSuggest);
        }

        // GET: DataSuggests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DataSuggest dataSuggest = db.DataSuggests.Find(id);
            if (dataSuggest == null)
            {
                return HttpNotFound();
            }
            return View(dataSuggest);
        }

        // POST: DataSuggests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DataSuggest dataSuggest = db.DataSuggests.Find(id);
            db.DataSuggests.Remove(dataSuggest);
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
