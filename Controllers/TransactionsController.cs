using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Mysqlx.Crud;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class TransactionsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            else
            {
                var branches = db.Branches.ToList();
                int idbranch = int.Parse(CheckUsers.idBranch());
                if (!CheckUsers.CheckHQ())
                {
                    branches = db.Branches.Where(x => x.Id == idbranch).ToList();
                }
                ViewBag.IdBranch = new SelectList(branches, "Id", "Name");
                return View();
            }
        }
        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Loadlist_statistics()
        {
            return View();
        }
        public ActionResult Loadlist(string idBranch, string sort, string type, string searchString, DateTime fromdate, DateTime todate)
        {
            string str = "";
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysort = "";
            string querysearch = "";
            string querytype = "";
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and trans.Code like N'" + searchString + "' or trans.Description like N'" + searchString + "'";
            }
            if (!string.IsNullOrEmpty(type))
            {
                querytype = " and trans.Type ='" + type + "'";
            }
            else
            {
                querytype = " and trans.Type ='1'";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "name":
                        querysort = " order by trans.Code";
                        break;
                    case "name_desc":
                        querysort = " order by trans.Code desc";
                        break;
                    case "date_desc":
                        querysort = " order by trans.Id desc";
                        break;
                    default:
                        querysort = " order by trans.Id";
                        break;
                }
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select trans.Id,trans.Code,trans.Name,trans.Description,trans.Amount,trans.Discount,trans.TotalAmount,trans.DateCreate,trans.Image,trans.Type,trans.Status,trans.PaymentMethod,us.Name as Username"
                            + " from [Transaction] trans,[User] us"
                            + " where us.id = trans.IdUser and trans.DateCreate>='"+fromdate+"' and trans.DateCreate<='"+todate+"' and trans.IdBranch = " + idBranch + querytype + querysearch + querysort;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    double totalamount = Double.Parse(reader["TotalAmount"].ToString(), 0);
                    count++;
                    str += "<tr>"
                            + "<td class='text-center'>" + count + "</td>"
                            + "<td class='text-center'>" + reader["Code"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Name"].ToString() + "</td>"
                            + "<td>" + reader["Description"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0} đ", totalamount) + "</td>"
                            + "<td>" + reader["PaymentMethod"].ToString() + "</td>"
                            + "<td class='text-center'>" + (reader["Type"].ToString() == "True" ? "Phiếu thu" : "Phiếu chi") + "</td>"
                            + "<td class='text-center'>" + reader["DateCreate"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Username"].ToString() + "</td>"
                            + "<td class='text-end'><a href='javascript:Delete(" + reader["Id"] + ")' class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a></td>"
                            + "</tr>";
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        string Getcode_transaction(bool type)
        {
            string loai = "";
            var idbranch = int.Parse(CheckUsers.idBranch());
            int nextCode = 0;
            if (type == false)
            {
                loai = "PC";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "PT";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(idbranch);
            string str = cn.Code + loai + DateTime.Now.Year + code;

            return str;
        }
        [HttpPost]
        public ActionResult Submit_transaction(bool type,decimal sotien,string lydo,string hoten,string dienthoai,string diachi,string method,  HttpPostedFileBase file)
        {
            string status = "ok"; 
            string fileName = "";
            var transaction = new Transaction()
            {
                Code = Getcode_transaction(type),
                TotalAmount = sotien,
                Amount = 0,
                Name = hoten,
                Phone =dienthoai,
                Address = diachi,
                PaymentMethod = method,
                Discount = 0,
                Type = type,
                Status = true,
                DateCreate = DateTime.Now,
                Description = lydo,
                IdUser = Convert.ToInt32(CheckUsers.iduser()),
                IdBranch = Convert.ToInt32(CheckUsers.idBranch())
            };
            if (file != null && file.ContentLength > 0)
            {
                // Generate a unique file name
                fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                // Specify the path to save the file
                string _path = Path.Combine(Server.MapPath("~/Uploads/Transaction"), fileName);
                file.SaveAs(_path);
                transaction.Image = "/Uploads/Transaction/" + fileName;
            }
            db.Transactions.Add(transaction);
            db.SaveChanges();

            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        #region Default
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
        #endregion
    }
}