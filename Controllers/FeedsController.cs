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
    public class FeedsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Feeds
        public ActionResult Index()
        {
            var feeds = db.Feeds.Include(f => f.User);
            return View(feeds.ToList());
        }
        public ActionResult Loadlist_thongbao()
        {
            string str = "";
            var list = db.Feeds.ToList();
            foreach (var item in list)
            {
                str += "<div class=\"col-md-12\">"
                    + "<div class=\"p-1 border-1\">"
                    + "<div class=\"row align-items-center\">"
                    + "<div class=\"col-auto text-end\">"
                    + "<img src=\"" + (item.User.Employee.Image == null ? "/Assets/images/profile/user-1.jpg" : item.User.Employee.Image) + "\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">"
                    + "</div>"
                    + "<div class=\"col-10\">"
                    + "<div class=\"overflow-hidden flex-nowrap\">"
                    + "<h6 class=\"mb-1\">"
                    + "<a href=\"javascript:View_thongbao(" + item.Id + ")\" class=\"fw-bolder\">" + item.Name + "</a>"
                    + "</h6>"
                    + "<i class=\"text-muted d-block mb-2 small\">đăng bởi <span class='fw-bolder'>" + item.User.Name + "</span> - vào lúc: " + item.DateCreate + "</i>"
                    + "</div>"
                    + "</div>"
                    + "</div>"
                    + "</div>"
                    + "</div> <hr class='bg-light'/>";
            }
            var json = new
            {
                str
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        // GET: Feeds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }


        // GET: Feeds/Create
        public ActionResult Create()
        {
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Feeds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,DateCreate,IdUser,Enable,Active,Todate,IsPublic,Type")] Feed feed,string editor1)
        {
            if (ModelState.IsValid)
            {
                feed.Description = editor1;
                feed.DateCreate = DateTime.Now;
                feed.IdUser = Convert.ToInt32(CheckUsers.iduser());
                feed.Enable= true;
                feed.Active = true;
                feed.IsPublic = true;
                db.Feeds.Add(feed);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", feed.IdUser);
            return View(feed);
        }

        // GET: Feeds/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", feed.IdUser);
            return View(feed);
        }

        // POST: Feeds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,DateCreate,IdUser,Enable,Active,Todate,IsPublic,Type")] Feed feed)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feed).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", feed.IdUser);
            return View(feed);
        }

        // GET: Feeds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Feed feed = db.Feeds.Find(id);
            if (feed == null)
            {
                return HttpNotFound();
            }
            return View(feed);
        }

        // POST: Feeds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Feed feed = db.Feeds.Find(id);
            db.Feeds.Remove(feed);
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
