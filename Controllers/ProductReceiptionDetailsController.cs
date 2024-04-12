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
    public class ProductReceiptionDetailsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: ProductReceiptionDetails
        public ActionResult Index()
        {
            var productReceiptionDetails = db.ProductReceiptionDetails.Include(p => p.Product).Include(p => p.Supplier).Include(p => p.WarehouseReceiption);
            return View(productReceiptionDetails.ToList());
        }

        // GET: ProductReceiptionDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReceiptionDetail productReceiptionDetail = db.ProductReceiptionDetails.Find(id);
            if (productReceiptionDetail == null)
            {
                return HttpNotFound();
            }
            return View(productReceiptionDetail);
        }

        // GET: ProductReceiptionDetails/Create
        public ActionResult Create()
        {
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name");
            ViewBag.IdSupplier = new SelectList(db.Suppliers, "Id", "Code");
            ViewBag.IdReceiption = new SelectList(db.WarehouseReceiptions, "Id", "Code");
            return View();
        }

        // POST: ProductReceiptionDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdReceiption,IdProduct,Price,Discount,TotalAmount,Status,IdSupplier,Type,Amount")] ProductReceiptionDetail productReceiptionDetail)
        {
            if (ModelState.IsValid)
            {
                db.ProductReceiptionDetails.Add(productReceiptionDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productReceiptionDetail.IdProduct);
            ViewBag.IdSupplier = new SelectList(db.Suppliers, "Id", "Code", productReceiptionDetail.IdSupplier);
            ViewBag.IdReceiption = new SelectList(db.WarehouseReceiptions, "Id", "Code", productReceiptionDetail.IdReceiption);
            return View(productReceiptionDetail);
        }
        public static int GetInventory(int id)
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            ModelDbContext db = new ModelDbContext();
            int soluongnhap = 0, soluongxuat = 0;
            var ds = db.ProductReceiptionDetails.Where(x => x.IdProduct == id && x.Status==true && x.WarehouseReceiption.IdBranch==idBranch).ToList();
            foreach (var item in ds)
            {
                if (item.Type == true)
                {
                    soluongnhap += item.Amount;
                }
                else
                {
                    soluongxuat += item.Amount;
                }
            }

            return soluongnhap - soluongxuat;
        }
        // GET: ProductReceiptionDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReceiptionDetail productReceiptionDetail = db.ProductReceiptionDetails.Find(id);
            if (productReceiptionDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productReceiptionDetail.IdProduct);
            ViewBag.IdSupplier = new SelectList(db.Suppliers, "Id", "Code", productReceiptionDetail.IdSupplier);
            ViewBag.IdReceiption = new SelectList(db.WarehouseReceiptions, "Id", "Code", productReceiptionDetail.IdReceiption);
            return View(productReceiptionDetail);
        }

        // POST: ProductReceiptionDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdReceiption,IdProduct,Price,Discount,TotalAmount,Status,IdSupplier,Type,Amount")] ProductReceiptionDetail productReceiptionDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productReceiptionDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdProduct = new SelectList(db.Products, "Id", "Name", productReceiptionDetail.IdProduct);
            ViewBag.IdSupplier = new SelectList(db.Suppliers, "Id", "Code", productReceiptionDetail.IdSupplier);
            ViewBag.IdReceiption = new SelectList(db.WarehouseReceiptions, "Id", "Code", productReceiptionDetail.IdReceiption);
            return View(productReceiptionDetail);
        }

        // GET: ProductReceiptionDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReceiptionDetail productReceiptionDetail = db.ProductReceiptionDetails.Find(id);
            if (productReceiptionDetail == null)
            {
                return HttpNotFound();
            }
            return View(productReceiptionDetail);
        }

        // POST: ProductReceiptionDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductReceiptionDetail productReceiptionDetail = db.ProductReceiptionDetails.Find(id);
            db.ProductReceiptionDetails.Remove(productReceiptionDetail);
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
