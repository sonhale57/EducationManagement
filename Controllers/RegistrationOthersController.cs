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
    public class RegistrationOthersController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: RegistrationOthers s
        public async Task<ActionResult> Index()
        {
            var registrationOthers = db.RegistrationOthers.Include(r => r.Registration).Include(r => r.RevenueReference);
            return View(await registrationOthers.ToListAsync());
        }

        // GET: RegistrationOthers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistrationOther registrationOther = await db.RegistrationOthers.FindAsync(id);
            if (registrationOther == null)
            {
                return HttpNotFound();
            }
            return View(registrationOther);
        }

        // GET: RegistrationOthers/Create
        public ActionResult Create()
        {
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description");
            ViewBag.IdReference = new SelectList(db.RevenueReferences, "Id", "Code");
            return View();
        }

        // POST: RegistrationOthers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdRegistration,IdReference,Price,Discount,TotalAmount,Status,Amount")] RegistrationOther registrationOther)
        {
            if (ModelState.IsValid)
            {
                db.RegistrationOthers.Add(registrationOther);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", registrationOther.IdRegistration);
            ViewBag.IdReference = new SelectList(db.RevenueReferences, "Id", "Code", registrationOther.IdReference);
            return View(registrationOther);
        }

        // GET: RegistrationOthers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistrationOther registrationOther = await db.RegistrationOthers.FindAsync(id);
            if (registrationOther == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", registrationOther.IdRegistration);
            ViewBag.IdReference = new SelectList(db.RevenueReferences, "Id", "Code", registrationOther.IdReference);
            return View(registrationOther);
        }

        // POST: RegistrationOthers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdRegistration,IdReference,Price,Discount,TotalAmount,Status,Amount")] RegistrationOther registrationOther)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registrationOther).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", registrationOther.IdRegistration);
            ViewBag.IdReference = new SelectList(db.RevenueReferences, "Id", "Code", registrationOther.IdReference);
            return View(registrationOther);
        }

        // GET: RegistrationOthers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistrationOther registrationOther = await db.RegistrationOthers.FindAsync(id);
            if (registrationOther == null)
            {
                return HttpNotFound();
            }
            return View(registrationOther);
        }

        // POST: RegistrationOthers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RegistrationOther registrationOther = await db.RegistrationOthers.FindAsync(id);
            db.RegistrationOthers.Remove(registrationOther);
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
