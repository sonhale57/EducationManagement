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
            var list = db.Feeds.Where(x=>x.Enable==true).OrderByDescending(x=>x.Id).ToList();
            foreach (var item in list)
            {
                str += "<li class=\"list-group-item border-0 d-flex justify-content-between mb-3 border-radius-lg\">" +
                            "<div class=\"d-flex align-items-center\">" +
                                "<div class=\"me-3 text-center\">" +
                                    "<img src=\"" + (item.User.Employee.Image == null ? "/Assets/images/profile/user-1.jpg" : item.User.Employee.Image) + "\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">" +
                                "</div>" +
                                "<div class=\"d-flex flex-column\">" +
                                    "<h6 class='mb-0 pb-0'><a href=\"javascript:View_thongbao(" + item.Id + ")\" class='text-muted fw-bolder'>" + item.Name + "</a></h6>" +
                                    "<small class=\"fst-italic\"><i class=\"ti ti-user-circle\" aria-hidden=\"true\"></i> " + item.User.Name + " , <i class=\"ti ti-clock\" aria-hidden=\"true\"></i> " + item.DateCreate.Value.ToString("dd/MM/yyyy") + "</small>" +
                                "</div>" +
                            "</div>" +
                            "<div class=\"d-flex\">" +
                                "<a href=\"javascript:Edit(" + item.Id + ")\" class=\"text-primary\"><i class=\"ti ti-edit\" aria-hidden=\"true\"></i></a>" +
                                "<a href=\"javascript:Delete(" + item.Id + ")\" class=\"text-danger ms-1\"><i class=\"ti ti-trash\" aria-hidden=\"true\"></i></a>" +
                            "</div>" +
                        "</li>";
            }
            var json = new
            {
                str
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]  // Tắt xác thực đầu vào cho hành động này
        public ActionResult Submit_savechange(int? Id,string action,string Name,string Description,DateTime Todate) {
            int iduser = Convert.ToInt32(CheckUsers.iduser());
            if (action == "create")
            {
                var f = new Feed()
                {
                    Name = Name,
                    Description = Description,
                    Todate = Todate,
                    DateCreate = DateTime.Now,
                    IdUser = iduser,
                    Active = true,
                    Enable = true,
                    IsPublic = true
                };
                db.Feeds.Add(f);
                db.SaveChanges();
                return Json(new {status="ok",message="Đã thêm thông báo thành công!"},JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (Id == null)
                {
                    return Json(new { status = "error", message = "Không tìm thấy thông báo cần cập nhật!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var f = db.Feeds.Find(Id);
                    f.Name = Name;
                    f.Description = Description;
                    f.Todate = Todate;
                    db.Entry(f).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { status = "ok", message = "Đã cập nhật thông báo thành công!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult GetFeed(int id) { 
            var f = db.Feeds.Find(id);
            if(f== null)
            {
                return Json(new { status = "error", message = "Không tìm thấy thông báo cần cập nhật!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {Id= f.Id, Name =f.Name, Description = f.Description,Todate = f.Todate.Value.ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Submit_delete(int id)
        {
            var f = db.Feeds.Find(id);
            if (f == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy thông báo cần cập nhật!" }, JsonRequestBehavior.AllowGet);
            }
            f.Enable = false;
            db.Entry(f).State= EntityState.Modified;
            db.SaveChanges();
            return Json(new { status="ok",message="Đã xóa thông báo thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}
