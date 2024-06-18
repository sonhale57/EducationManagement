using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers
{
    public class HomeController : Controller
    {
        ModelDbContext db= new ModelDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Loadlist_thongbao()
        {
            string str = "";
            var list = db.Feeds.ToList();
            foreach(var item in list)
            {
                str += "<div class=\"col-md-12\">"
                    +"<div class=\"p-1 border-1\">"
                    +"<div class=\"row align-items-center\">"
                    +"<div class=\"col-2 text-end\">" 
                    +"<img src=\""+item.User.Employee.Image+"\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">" 
                    +"</div>" 
                    +"<div class=\"col-10\">"
                    +"<div class=\"overflow-hidden flex-nowrap\">"
                    +"<h6 class=\"mb-1\">" 
                    +"<a href=\"javascript:View_thongbao("+item.Id+")\" class=\"fw-bolder\">"+item.Name+"</a>" 
                    +"</h6>" 
                    +"<i class=\"text-muted d-block mb-2 small\">đăng bởi <span class='fw-bolder'>"+item.User.Name+"</span> - vào lúc: "+item.DateCreate+"</i>" 
                    +"</div>" 
                    +"</div>" 
                    +"</div>" 
                    +"</div>" 
                    +"</div> <hr class='bg-light'/>";
            }
            var json = new
            {
                str
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}