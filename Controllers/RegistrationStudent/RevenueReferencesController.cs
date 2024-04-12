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
    public class RevenueReferencesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: RevenueReferences s
        public async Task<ActionResult> Index()
        {
            var revenueReferences = db.RevenueReferences.Include(r => r.Branch).Include(r => r.User);
            return View(await revenueReferences.ToListAsync());
        }

        // GET: RevenueReferences/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevenueReference revenueReference = await db.RevenueReferences.FindAsync(id);
            if (revenueReference == null)
            {
                return HttpNotFound();
            }
            return View(revenueReference);
        }

        // GET: RevenueReferences/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: RevenueReferences/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Name,Price,Discount,StatusDiscount,DateCreate,IdUser,IdBranch,IsPublic")] RevenueReference revenueReference)
        {
            if (ModelState.IsValid)
            {
                db.RevenueReferences.Add(revenueReference);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", revenueReference.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", revenueReference.IdUser);
            return View(revenueReference);
        }

        // GET: RevenueReferences/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevenueReference revenueReference = await db.RevenueReferences.FindAsync(id);
            if (revenueReference == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", revenueReference.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", revenueReference.IdUser);
            return View(revenueReference);
        }

        // POST: RevenueReferences/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Name,Price,Discount,StatusDiscount,DateCreate,IdUser,IdBranch,IsPublic")] RevenueReference revenueReference)
        {
            if (ModelState.IsValid)
            {
                db.Entry(revenueReference).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", revenueReference.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", revenueReference.IdUser);
            return View(revenueReference);
        }

        // GET: RevenueReferences/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RevenueReference revenueReference = await db.RevenueReferences.FindAsync(id);
            if (revenueReference == null)
            {
                return HttpNotFound();
            }
            return View(revenueReference);
        }

        // POST: RevenueReferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RevenueReference revenueReference = await db.RevenueReferences.FindAsync(id);
            db.RevenueReferences.Remove(revenueReference);
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
