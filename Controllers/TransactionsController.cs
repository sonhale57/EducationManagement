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
    public class TransactionsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Branch).Include(t => t.Order).Include(t => t.Registration).Include(t => t.Student).Include(t => t.User);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdOrder = new SelectList(db.Orders, "Id", "Code");
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description");
            ViewBag.IdStudent = new SelectList(db.Students, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DateCreate,IdUser,IdStudent,Description,Type,Amount,Discount,TotalAmount,Status,IdBranch,Image,Name,Phone,Email,Address,IdRegistration,IdOrder")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", transaction.IdBranch);
            ViewBag.IdOrder = new SelectList(db.Orders, "Id", "Code", transaction.IdOrder);
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", transaction.IdRegistration);
            ViewBag.IdStudent = new SelectList(db.Students, "Id", "Name", transaction.IdStudent);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", transaction.IdUser);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", transaction.IdBranch);
            ViewBag.IdOrder = new SelectList(db.Orders, "Id", "Code", transaction.IdOrder);
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", transaction.IdRegistration);
            ViewBag.IdStudent = new SelectList(db.Students, "Id", "Name", transaction.IdStudent);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", transaction.IdUser);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DateCreate,IdUser,IdStudent,Description,Type,Amount,Discount,TotalAmount,Status,IdBranch,Image,Name,Phone,Email,Address,IdRegistration,IdOrder")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", transaction.IdBranch);
            ViewBag.IdOrder = new SelectList(db.Orders, "Id", "Code", transaction.IdOrder);
            ViewBag.IdRegistration = new SelectList(db.Registrations, "Id", "Description", transaction.IdRegistration);
            ViewBag.IdStudent = new SelectList(db.Students, "Id", "Name", transaction.IdStudent);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", transaction.IdUser);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
