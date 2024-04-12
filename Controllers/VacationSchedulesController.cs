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
using Microsoft.EntityFrameworkCore;

namespace SuperbrainManagement.Controllers
{
    public class VacationSchedulesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: VacationSchedules
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page,string idBranch)
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
            
            var vacation = db.VacationSchedules.ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                vacation = vacation.Where(x => x.IdBranch==int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                vacation = vacation.Where(x => x.Description.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    vacation = vacation.OrderByDescending(s => s.Description).ToList();
                    break;
                case "date":
                    vacation = vacation.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    vacation = vacation.OrderBy(s => s.Description).ToList();
                    break;
                default:
                    vacation = vacation.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = vacation.ToPagedList(pageNumber, pageSize);

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

        // GET: VacationSchedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationSchedule vacationSchedule = db.VacationSchedules.Find(id);
            if (vacationSchedule == null)
            {
                return HttpNotFound();
            }
            return View(vacationSchedule);
        }

        // GET: VacationSchedules/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: VacationSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DateCreate,IdUser,IdBranch,Fromdate,Todate,Description")] VacationSchedule vacationSchedule)
        {
            if (ModelState.IsValid)
            {
                vacationSchedule.IdBranch = int.Parse(CheckUsers.idBranch());
                vacationSchedule.IdUser =int.Parse(CheckUsers.iduser());
                vacationSchedule.DateCreate = DateTime.Now;
                db.VacationSchedules.Add(vacationSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", vacationSchedule.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", vacationSchedule.IdUser);
            return View(vacationSchedule);
        }

        // GET: VacationSchedules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationSchedule vacationSchedule = db.VacationSchedules.Find(id);
            if (vacationSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", vacationSchedule.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", vacationSchedule.IdUser);
            return View(vacationSchedule);
        }

        // POST: VacationSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DateCreate,IdUser,IdBranch,Fromdate,Todate,Description")] VacationSchedule vacationSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vacationSchedule).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", vacationSchedule.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", vacationSchedule.IdUser);
            return View(vacationSchedule);
        }

        // GET: VacationSchedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VacationSchedule vacationSchedule = db.VacationSchedules.Find(id);
            if (vacationSchedule == null)
            {
                return HttpNotFound();
            }
            return View(vacationSchedule);
        }

        // POST: VacationSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VacationSchedule vacationSchedule = db.VacationSchedules.Find(id);
            db.VacationSchedules.Remove(vacationSchedule);
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
