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
using PagedList.Mvc;
using PagedList;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;

namespace SuperbrainManagement.Controllers.RegistrationStudent
{
    public class ProductsController : Controller
    {
        public ModelDbContext db = new ModelDbContext();

        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.IdCategory = new SelectList(db.ProductCategories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Description,Code,Image,Price,DiscountPrice,StatusDiscount,IdCategory,IsCore,Unit,Quota,NumberOfPackage,UnitOfPackage,Inventory,IsFixed,IsSale,IdSupplier,PowerScore")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Active = true;
                product.Enable= true;
                product.DateCreate = DateTime.Now;
                product.IdUser = int.Parse(CheckUsers.iduser());
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdCategory = new SelectList(db.ProductCategories, "Id", "Name", product.IdCategory);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", product.IdUser);
            return Redirect("/productcategories");
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCategory = new SelectList(db.ProductCategories, "Id", "Name", product.IdCategory);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Code,Image,Price,DiscountPrice,StatusDiscount,IdCategory,IsCore,Unit,Quota,NumberOfPackage,UnitOfPackage,Inventory,IsFixed,IsSale,IdSupplier,PowerScore")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdCategory = new SelectList(db.ProductCategories, "Id", "Name", product.IdCategory);
            return Redirect("/productcategories");
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult updateStatus(int id, int status)
        {
            Product product = db.Products.Find(id);
            if (product != null)
            {
                product.Active = status == 1;
                db.SaveChanges();
                return Json(product.Active);
            }
            else
            {
                return Json(false);
            }
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
