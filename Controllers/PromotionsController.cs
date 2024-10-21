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
    public class PromotionsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Promotions
        public ActionResult Index()
        {
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name");
            return View();
        }

        public ActionResult Loadlist(int? IdBranch,string searchString) 
        { 
            if(IdBranch == null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            var promo = db.Promotions.Where(x=>x.IdBranch == IdBranch);
            if (!string.IsNullOrEmpty(searchString))
            {
                promo = promo.Where(x=>x.Name.Contains(searchString)|| x.Code.Contains(searchString));
            }
            string str = "";int stt = 0;
            if (promo == null)
            {
                str = "<tr><td colspan=7 class='text-center'>Không có dữ liệu ưu đãi</td></tr>";
            }
            else
            {
                foreach(var item in promo)
                {
                    stt++;
                    str += "<tr>"
                        +"<td class='text-center'>"+stt+"</td>"
                        +"<td class='text-start'>"+item.Code+"</td>"
                        +"<td class='text-start'>"+item.Name+"</td>"
                        +"<td class='text-center'>"+string.Format("{0:N0}",item.Value)+"</td>"
                        +"<td class='text-center'>"+(item.Type==1?"<span class='badge bg-light text-success'>Khóa học</span>":"<span class='badge bg-light text-primary'>Vật tư</span>")+ "</td>"
                        +"<td class='text-center'>"+(db.RegistrationCourses.Count(x=>x.IdPromotion==item.Id)+ db.RegistrationProducts.Count(x=>x.IdPromotion==item.Id)) +"</td>"
                        + "<td class='text-center'>"+(item.Active==true?(item.Todate>DateTime.Now?"<span class='badge bg-success'>Đang diễn ra</span>":"<span class='badge bg-danger'>Đã kết thúc</span>"):"<span class='badge bg-danger'>Không kích hoạt</span>")+"</td>"
                        +"<td class='text-center'>"+item.Todate.Value.ToString("dd/MM/yyyy")+"</td>"
                        +"<td class='text-end'>"
                            +"<a href=\"/promotions/edit/" + item.Id + "\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                            + "<a href=\"/javascript:Delete(" + item.Id  + ")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                        + "</td>"
                        +"</tr>";
                }
            }
            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }
        // GET: Promotions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // GET: Promotions/Create
        public ActionResult Create()
        {
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name");
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Promotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,Name,Type,IdBranch,DateCreate,IdUser,Fromdate,Todate,Active,Value")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                db.Promotions.Add(promotion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", promotion.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", promotion.IdUser);
            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", promotion.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", promotion.IdUser);
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Type,IdBranch,DateCreate,IdUser,Fromdate,Todate,Active,Value")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promotion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Name", promotion.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", promotion.IdUser);
            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotions.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Promotion promotion = db.Promotions.Find(id);
            db.Promotions.Remove(promotion);
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
