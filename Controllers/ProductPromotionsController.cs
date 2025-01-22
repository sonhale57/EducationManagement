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
    public class ProductPromotionsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: ProductPromotions s
        public async Task<ActionResult> Index()
        {
            var productPromotions = db.ProductPromotions.Include(p => p.Product).Include(p => p.Promotion);
            return View(await productPromotions.ToListAsync());
        }

        // GET: ProductPromotions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPromotion productPromotion = await db.ProductPromotions.FindAsync(id);
            if (productPromotion == null)
            {
                return HttpNotFound();
            }
            return View(productPromotion);
        }

        // GET: ProductPromotions/Create
        public ActionResult Create()
        {
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name");
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code");
            return View();
        }

        // POST: ProductPromotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdPromotion,IdProduct,Amount")] ProductPromotion productPromotion)
        {
            if (ModelState.IsValid)
            {
                db.ProductPromotions.Add(productPromotion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productPromotion.IdProduct);
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code", productPromotion.IdPromotion);
            return View(productPromotion);
        }

        // GET: ProductPromotions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPromotion productPromotion = await db.ProductPromotions.FindAsync(id);
            if (productPromotion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productPromotion.IdProduct);
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code", productPromotion.IdPromotion);
            return View(productPromotion);
        }

        // POST: ProductPromotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IdPromotion,IdProduct,Amount")] ProductPromotion productPromotion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productPromotion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productPromotion.IdProduct);
            ViewBag.IdPromotion = new SelectList(db.Promotions, "Id", "Code", productPromotion.IdPromotion);
            return View(productPromotion);
        }

        // GET: ProductPromotions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPromotion productPromotion = await db.ProductPromotions.FindAsync(id);
            if (productPromotion == null)
            {
                return HttpNotFound();
            }
            return View(productPromotion);
        }

        // POST: ProductPromotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductPromotion productPromotion = await db.ProductPromotions.FindAsync(id);
            db.ProductPromotions.Remove(productPromotion);
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
