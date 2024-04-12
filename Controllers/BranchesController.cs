using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class BranchesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Branches
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var branches = db.Branches.Include(b => b.BranchGroup);
            if (!string.IsNullOrEmpty(searchString))
            {
                branches = branches.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Code.ToLower().Contains(searchString.ToLower()) || x.BranchGroup.Name.ToLower().Contains(searchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    branches = branches.OrderByDescending(s => s.Name);
                    break;
                case "date":
                    branches = branches.OrderBy(s => s.Id);
                    break;
                case "date_desc":
                    branches = branches.OrderByDescending(s => s.Id);
                    break;
                case "group":
                    branches = branches.OrderBy(s => s.BranchGroup.Name);
                    break;
                default:
                    branches = branches.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 20; 
            int pageNumber = (page ?? 1); 
            var pagedData = branches.ToPagedList(pageNumber, pageSize);
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

        // GET: Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Branches/Create
        public ActionResult Create()
        {
            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name");
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Logo,Code,Name,CompanyName,Address,City,District,Ward,Map,Phone,Email,OtherEmail,TaxCode,Description,License,DisplayOrder,ContractNumber,ContractExpire,Enable,Active,DateExpire,StatusUsageBrand,DateExpireOnline,StatusActiveOnline,IdGroup,NumberInGroup,FreeTrainning,DateCreate,Unlock,GrandOpeningDay")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name", branch.IdGroup);
            return View(branch);
        }

        // GET: Branches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name", branch.IdGroup);
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Logo,Code,Name,CompanyName,Address,City,District,Ward,Map,Phone,Email,OtherEmail,TaxCode,Description,License,DisplayOrder,ContractNumber,ContractExpire,Enable,Active,DateExpire,StatusUsageBrand,DateExpireOnline,StatusActiveOnline,IdGroup,NumberInGroup,FreeTrainning,DateCreate,Unlock,GrandOpeningDay")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name", branch.IdGroup);
            return View(branch);
        }

        // GET: Branches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            db.Branches.Remove(branch);
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
