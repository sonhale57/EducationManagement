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
    public class RegistrationProductsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();
         
        // GET: RegistrationProducts
        public async Task<ActionResult> Index()
        {
            var registrationProducts = db.RegistrationProducts.Include(r => r.Product).Include(r => r.Promotion).Include(r => r.Registration);
            return View(await registrationProducts.ToListAsync());
        }

        // GET: RegistrationProducts/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistrationProduct registrationProduct = await db.RegistrationProducts.FindAsync(id);
            if (registrationProduct == null)
            {
                return HttpNotFound();
            }
            return View(registrationProduct);
        }

        // GET: RegistrationProducts/Create
        public ActionResult Create()
        {
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name");
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code");
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description");
            return View();
        }

        // POST: RegistrationProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdRegistration,IdProduct,Price,Discount,TotalAmount,Status,Amount,IsGift,IdPromotion")] RegistrationProduct registrationProduct)
        {
            if (ModelState.IsValid)
            {
                db.RegistrationProducts.Add(registrationProduct);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", registrationProduct.IdProduct);
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code", registrationProduct.IdPromotion);
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", registrationProduct.IdRegistration);
            return View(registrationProduct);
        }

        // GET: RegistrationProducts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistrationProduct registrationProduct = await db.RegistrationProducts.FindAsync(id);
            if (registrationProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", registrationProduct.IdProduct);
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code", registrationProduct.IdPromotion);
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", registrationProduct.IdRegistration);
            return View(registrationProduct);
        }

        // POST: RegistrationProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdRegistration,IdProduct,Price,Discount,TotalAmount,Status,Amount,IsGift,IdPromotion")] RegistrationProduct registrationProduct)
        {
            if (ModelState.IsValid)
            {
                db.Entry(registrationProduct).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", registrationProduct.IdProduct);
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code", registrationProduct.IdPromotion);
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", registrationProduct.IdRegistration);
            return View(registrationProduct);
        }

        // GET: RegistrationProducts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegistrationProduct registrationProduct = await db.RegistrationProducts.FindAsync(id);
            if (registrationProduct == null)
            {
                return HttpNotFound();
            }
            return View(registrationProduct);
        }

        // POST: RegistrationProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RegistrationProduct registrationProduct = await db.RegistrationProducts.FindAsync(id);
            db.RegistrationProducts.Remove(registrationProduct);
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
