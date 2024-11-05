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
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x=>x.Enable==true), "Id", "Name", idBranch);
            return View();
        }
        public ActionResult Loadlist(int? IdBranch)
        {
            int idbranchHQ = db.Branches.FirstOrDefault(x => x.Code.ToLower() == "hq").Id;
            var mkt = db.MKTCampaigns.Where(x=>x.Enable==true);
            if (IdBranch == null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            mkt = mkt.Where(x => x.IdBranch == IdBranch || (x.IdBranch==idbranchHQ && x.IsPublic==true)); 
            string str = "";int stt = 0;
            foreach (var item in mkt)
            {
                stt = stt + 1;
                str += "<tr>"
                    +"<td class='text-center align-content-center'>"+stt+"</td>"
                    + "<td class='text-start align-content-center'>" + item.Code+"</td>"
                    + "<td class='text-start align-content-center'>" + item.Name+"</td>"
                    + "<td class='text-center align-content-center'><span class='btn btn-sm btn-outline-success m-1'>" + db.Students.Count(x=>x.IdMKT==item.Id && x.IdBranch==IdBranch) + "</span></td>"
                    + "<td class='text-center align-content-center'>" + (item.IsPublic==true ? "<span class='text-success'>Công khai</span>" : "Riêng tư") + "</td>"
                   + "<td class='text-center align-content-center'>" + (item.Status==true ? "<label class=\"custom-control ios-switch\"><input type=\"checkbox\" class=\"ios-switch-control-input\" onchange=\"javascript: ChangeStatus(this)\" data-id=\"" + item.Id + "\" value=\"false\" checked><span class=\"ios-switch-control-indicator\"></span></label>" : "<label class=\"custom-control ios-switch\"><input type=\"checkbox\" class=\"ios-switch-control-input\" onchange=\"javascript: ChangeStatus(this)\" data-id=\"" + item.Id + "\" value=\"true\"><span class=\"ios-switch-control-indicator\"></span></label>") + "</td>"
                    + "<td class='text-end align-content-center'>"
                    + "<a href='javascript:Edit(" + item.Id+")' class='text-primary'><i class='ti ti-edit'></i> </a>" 
                    +"<a href='javascript:Delete(" + item.Id+ ")' class='text-danger'><i class='ti ti-trash'></i> </a>" 
                    +"</td>"
                    +"</tr>";
            }
            return Json(new {str}, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadInfoMKT(int id) {
            var mkt = db.MKTCampaigns.Find(id);
            if (mkt == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy chương trình MKT" },JsonRequestBehavior.AllowGet);
            }
            return Json(new {Code=mkt.Code,Name = mkt.Name,Status=mkt.Status, IsPublic = mkt.IsPublic},JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_savechange(int? Id,string action,string Code,string Name,bool? IsPublic,bool? Status) {
            int IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            int IdUser = Convert.ToInt32(CheckUsers.iduser());
            if (action == "create")
            {
                var mkt = new MKTCampaign() {
                    IdBranch = IdBranch,
                    Name = Name,
                    Code = Code,
                    DateCreate = DateTime.Now,
                    Status = Status,
                    Enable = true,
                    Fee=0,
                    IsPublic = IsPublic,
                    IdUser = IdUser
                };
                db.MKTCampaigns.Add(mkt);
                db.SaveChanges();
                return Json(new { status = "ok", message = "Đã thêm mới chương trình MKT!" },JsonRequestBehavior.AllowGet);
            }
            else
            {
                var m = db.MKTCampaigns.Find(Id);
                if (m == null)
                    return Json(new { status = "error", message = "Không tìm thấy chương trình MKT!" }, JsonRequestBehavior.AllowGet);
                else
                {
                    m.Status = Status;
                    m.Code = Code;
                    m.Name = Name;
                    m.IsPublic = IsPublic;
                    db.Entry(m).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { status = "ok", message = "Đã thêm mới chương trình MKT!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult Submit_delete(int id)
        {
            var mkt = db.MKTCampaigns.Find(id);
            if (mkt == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy chương trình MKT!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //kiểm tra 
                if (db.Students.Any(x => x.IdMKT == id))
                {
                    mkt.Enable = false;
                    db.Entry(mkt).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { status = "ok", message = "Đã cập nhật chương trình MKT!" }, JsonRequestBehavior.AllowGet);
                }
                db.MKTCampaigns.Remove(mkt);
                db.SaveChanges();
                return Json(new { status = "ok", message = "Đã xóa chương trình này." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult updateStatus(int id, bool status)
        {
            MKTCampaign mkt = db.MKTCampaigns.Find(id);
            if (mkt != null)
            {
                mkt.Status = status;
                db.SaveChanges();
                return Json(new {status="ok",message="Đã cập nhật thành công"},JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new {status="error",message="Lỗi cập nhật!"},JsonRequestBehavior.AllowGet);
            }
        }
    }
}
