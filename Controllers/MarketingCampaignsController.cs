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

namespace SuperbrainManagement.Controllers.RegistrationStudent
{
    public class MarketingCampaignsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: MarketingCampaigns
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

            var mKTCampaigns = db.MKTCampaigns.ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                mKTCampaigns = mKTCampaigns.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                mKTCampaigns = mKTCampaigns.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    mKTCampaigns = mKTCampaigns.OrderByDescending(s => s.Name).ToList();
                    break;
                case "date":
                    mKTCampaigns = mKTCampaigns.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    mKTCampaigns = mKTCampaigns.OrderBy(s => s.Name).ToList();
                    break;
                default:
                    mKTCampaigns = mKTCampaigns.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = mKTCampaigns.ToPagedList(pageNumber, pageSize);

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


        // GET: MarketingCampaigns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MKTCampaign mKTCampaign = db.MKTCampaigns.Find(id);
            if (mKTCampaign == null)
            {
                return HttpNotFound();
            }
            return View(mKTCampaign);
        }

        // GET: MarketingCampaigns/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo");
            ViewBag.IdBranch = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: MarketingCampaigns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,DateCreate,IdUser,IdBranch,Enable,Status,IsPublic,Fee")] MKTCampaign mKTCampaign)
        {
            if (ModelState.IsValid)
            {
                db.MKTCampaigns.Add(mKTCampaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", mKTCampaign.IdBranch);
            ViewBag.IdBranch = new SelectList(db.Users, "Id", "Name", mKTCampaign.IdBranch);
            return View(mKTCampaign);
        }

        // GET: MarketingCampaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MKTCampaign mKTCampaign = db.MKTCampaigns.Find(id);
            if (mKTCampaign == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", mKTCampaign.IdBranch);
            ViewBag.IdBranch = new SelectList(db.Users, "Id", "Name", mKTCampaign.IdBranch);
            return View(mKTCampaign);
        }

        // POST: MarketingCampaigns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,DateCreate,IdUser,IdBranch,Enable,Status,IsPublic,Fee")] MKTCampaign mKTCampaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mKTCampaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", mKTCampaign.IdBranch);
            ViewBag.IdBranch = new SelectList(db.Users, "Id", "Name", mKTCampaign.IdBranch);
            return View(mKTCampaign);
        }

        // GET: MarketingCampaigns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MKTCampaign mKTCampaign = db.MKTCampaigns.Find(id);
            if (mKTCampaign == null)
            {
                return HttpNotFound();
            }
            return View(mKTCampaign);
        }

        // POST: MarketingCampaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MKTCampaign mKTCampaign = db.MKTCampaigns.Find(id);
            db.MKTCampaigns.Remove(mKTCampaign);
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
