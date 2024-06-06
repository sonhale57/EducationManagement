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
    public class RoomsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Rooms
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

            var room = db.Rooms.ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                room = room.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                room = room.Where(x => x.Description.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    room = room.OrderByDescending(s => s.Description).ToList();
                    break;
                case "date":
                    room = room.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    room = room.OrderBy(s => s.Description).ToList();
                    break;
                default:
                    room = room.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = room.ToPagedList(pageNumber, pageSize);

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


        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.DateCreate = DateTime.Now;
                room.IdUser =int.Parse(CheckUsers.iduser()) ;
                room.IdBranch = int.Parse(CheckUsers.idBranch());
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", room.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", room.IdUser);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", room.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", room.IdUser);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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
