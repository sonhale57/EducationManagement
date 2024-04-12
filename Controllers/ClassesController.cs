using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class ClassesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Classes
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string idBranch)
        {
            var branches = db.Branches.ToList();
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                branches = db.Branches.Where(x => x.Id == idbranch).ToList();
            }
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = branches.First().Id.ToString();
            }
            ViewBag.IdBranch = new SelectList(branches, "Id", "Name", idBranch);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var classes = db.Classes.ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                classes = classes.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                classes = classes.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    classes = classes.OrderByDescending(s => s.Name).ToList();
                    break;
                case "date":
                    classes = classes.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    classes = classes.OrderBy(s => s.Name).ToList();
                    break;
                default:
                    classes = classes.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = classes.ToPagedList(pageNumber, pageSize);

            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };

            ViewBag.PagedListRenderOptions = pagedListRenderOptions;
            return View(pagedData);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, int status)
        {
            Class classes = db.Classes.Find(id);
            if (classes != null)
            {
                classes.Active = status == 1;
                db.SaveChanges();
                return Json(classes.Active);
            }
            else
            {
                return Json(false);
            }
        }
        // GET: Classes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser,Enable,Active")] Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.DateCreate = DateTime.Now;
                @class.IdBranch =int.Parse(CheckUsers.idBranch()) ;
                @class.IdUser =int.Parse(CheckUsers.iduser());
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", @class.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", @class.IdUser);
            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", @class.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", @class.IdUser);
            return View(@class);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser,Enable,Active")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", @class.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", @class.IdUser);
            return View(@class);
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Class @class = db.Classes.Find(id);
            if (@class == null)
            {
                return HttpNotFound();
            }
            return View(@class);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
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
