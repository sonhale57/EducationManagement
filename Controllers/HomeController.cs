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
            var list = db.Feeds.Take(5).ToList();
            foreach(var item in list)
            {
                str += "<div class=\"col-md-12\">"
                    +"<div class=\"p-1 border-1\">"
                    +"<div class=\"row align-items-center\">"
                    +"<div class=\"col-auto text-end\">" 
                    +"<img src=\""+item.User.Employee.Image+"\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle\">" 
                    +"</div>" 
                    +"<div class=\"col-10\">"
                    +"<div class=\"overflow-hidden flex-nowrap\">"
                    +"<h6 class=\"mb-1\">" 
                    +"<a href=\"javascript:View_thongbao("+item.Id+")\" class=\"fw-bolder\">"+item.Name+"</a>" 
                    +"</h6>" 
                    +"<i class=\"text-muted d-block mb-2 small\">đăng bởi <span class='fw-bolder'>"+item.User.Name+"</span> - vào lúc: "+item.DateCreate.Value.ToString("dd/MM/yyyy")+"</i>" 
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
        public ActionResult Load_thongbao(int id)
        {
            var list = db.Feeds.Find(id);
            var json = new
            {
                tieude =list.Name,
                ngay =list.DateCreate.Value.ToString("dd/MM/yyyy"),
                str = list.Description
            };
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Loadlist_vattu()
        {
            string str = "";
            string idBranch = CheckUsers.idBranch();
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Name,p.Image,p.Unit,p.Price,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + idBranch + "), 0) -"
                                    + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + idBranch + "), 0) AS Tonkho"
                                    + " FROM product p"
                                    + " where p.enable=1 ";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int dinhmuc = Convert.ToInt32(reader["Quota"]);
                    int tonkho = Convert.ToInt32(reader["tonkho"]);
                    if (tonkho <= dinhmuc)
                    {
                        str += "<div class=\"row align-items-center\">"
                            + "<div class=\"col-auto\">"
                            + "<img src=\"" + reader["Image"] + "\" alt=\"\" width=\"35\" height=\"35\" class=\"rounded-circle mt-0\">"
                            + "</div>"
                            + "<div class=\"col-10\">"
                            + "<div class=\"overflow-hidden flex-nowrap\">"
                            + "<h6 class=\"mb-1\">"
                            + "<a href=\"javascript:void(0)\" class=\"fw-bolder\">" +reader["Name"] + "</a>"
                            + "</h6>"
                            + "<i class=\"text-muted d-block mb-2 small\">Tồn kho: <span class='fw-bolder'>" + tonkho + "</span></i>"
                            + "</div>"
                            + "</div>"
                            + "</div> <hr class='bg-light'/>";
                    }
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

    }
}