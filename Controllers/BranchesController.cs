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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Loadlist()
        {
            var configTimePayment = db.Configurations.OrderByDescending(x => x.Id).FirstOrDefault().TimePayment;
            string str = "";
            var cum = db.BranchGroups.Where(x=>x.Enable==true).ToList();
            int count = 0;
            int countcum = 0;
            foreach(var c in cum)
            {
                countcum++;
                str += "<tr>" 
                    +"<td  class='text-success fw-bolder text-center'>"+countcum+"</td>" 
                    +"<td colspan=7 class='text-success fw-bolder'>"+c.Name+"" 
                        +"<a href=\"/productcategories/edit/" + c.Id + "\" class=\"ms-2\"><i class=\"ti ti-edit text-primary fw-bolder\"></i></a>"
                        + "<a href=\"javascript:Delete_category(" + c.Id + ")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                    +"</td>" 
                    +"</tr>";
                var cn = db.Branches.Where(x => x.IdGroup == c.Id);
                foreach(var cn2 in cn)
                {
                    count++;
                    str += "<tr>" +
                        "<td class='text-center align-content-center'>"+count+"</td>" +
                        "<td class='text-center align-content-center'>" + cn2.Code+"</td>" +
                        "<td class='text-left align-content-center'>" + cn2.Name+"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.Phone != null ? cn2.Phone : "-") +"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.Email != null ? cn2.Email : "-") +"</td>" +
                        //"<td class='text-center align-content-center'>" + (cn2.DateExpire != null ? cn2.DateExpire.Value.ToString("dd/MM/yyyy") : "-") +"</td>" +
                        //"<td class='text-center align-content-center'>" + (cn2.DateExpireOnline != null ? cn2.DateExpireOnline.Value.ToString("dd/MM/yyyy") : "-") +"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.ContractExpire != null ? cn2.ContractExpire.Value.ToString("dd/MM/yyyy") : "-") +"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.StatusActiveOnline==true?"<span class='text-success'>Đã thanh toán</span>":"<span class='text-danger'>Chưa thanh toán</span>") + "</td>" +
                        "<td class='text-end align-content-center'>" +
                        "<a href='/branches/edit/"+cn2.Id+"'><i class='ti ti-edit text-primary'></i></a>" +
                        "<a href='javascript:Delete_branches("+cn2.Id+")' class='ms-1'><i class='ti ti-trash text-danger'></i></a>" +
                        "</td>" +
                        "</tr>";
                }
            }
            var item = new {
                str
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }

        int count_tinhphi(int idStudent)
        {
            var config = db.Configurations.OrderByDescending(x=>x.Id).FirstOrDefault().TimePayment;
            var join = db.StudentJoinClasses.Where(x => x.IdStudent == idStudent && x.Todate >= DateTime.Now).ToList();
            return join.Count;
        }
        int count_student(int idBranch) {
            var join = db.StudentJoinClasses.Where(x=>x.Student.Branch.Id == idBranch && x.Todate>=DateTime.Now).ToList();
            return join.Count;
        }
        public ActionResult List(string sortOrder, string currentFilter, string searchString, int? page)
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

        public ActionResult PaymentList()
        {
            var config = db.Configurations.OrderByDescending(x=>x.Id).ToList();
            if (config == null)
            {
                ViewBag.Month = new SelectList(config, "Không tìm thấy dữ liệu");
            }
            ViewBag.Month = new SelectList(config, "TimePayment","TimePayment" );
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id","Name" );
            return View();
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
