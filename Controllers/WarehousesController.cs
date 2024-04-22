using Microsoft.Ajax.Utilities;
using PagedList;
using PagedList.Mvc;
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
    public class WarehousesController : Controller
    {
        public ModelDbContext db = new ModelDbContext();
        public ActionResult Index(string idBranch)
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
            return View();
        }
        public ActionResult Loadlist(string idBranch,string sort,string searchString)
        {
            string str = "";
            if(string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysort = "";
            string querysearch = "";
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and p.Name like N'"+searchString+"' or p.Code like N'"+searchString+"'";
            }
            if(!string.IsNullOrEmpty(sort))
            {
                switch(sort)
                {
                    case "name":
                        querysort = " order by p.Name";
                        break;
                    case "name_desc":
                        querysort = " order by p.Name desc";
                        break;
                    case "date_desc":
                        querysort = " order by p.Id desc";
                        break;
                    default:
                        querysort = " order by p.Id";
                        break;
                }
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Name,p.Unit,p.Price,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = "+ idBranch + "), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = "+idBranch+"), 0) AS Tonkho"
                                        + " FROM product p"
                                        +" where p.enable=1 "
                                        + querysearch
                                        + querysort;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    str += "<tr>"
                            + "<td class'text-center'>" + count + "</td>"
                            + "<td class='text-center'>" + reader["code"].ToString() + "</td>"
                            + "<td>" + reader["Name"].ToString() + "</td>"
                            + "<td>" + reader["Unit"].ToString() + "</td>"
                            + "<td>" + reader["Price"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Quota"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Tonkho"].ToString() + "</td>"
                            +"<td></td>"
                            + "</tr>";
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadlist_nhap()
        {
            string str = "";int count = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Name,p.Unit,p.Price,p.Code,p.Quota,p.Image FROM product p"
                                        + " where p.enable=1 ";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    count++;
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='w-25 align-content-center'> <img src='" + reader["Image"].ToString() +"' alt='" + reader["Name"].ToString() +"' class='rounded-2 me-2' height='40'><span class='text-success'>" +reader["code"].ToString() +"</span> - "+ reader["Name"].ToString() + "</td>"
                            + "<td class='w-25 align-content-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='w-10 align-content-center'> <input type='text' name='Price' id='dongia_" + reader["Id"].ToString() +"' data-id='" + reader["Id"].ToString() +"' value='" + reader["Price"].ToString() + "' class='form-control' onchange='javascript:update_thanhtien(" + reader["Id"] +")'></td>"
                            + "<td class='text-center w-5 align-content-center'><input type='text' name='Amount'  id='soluong_" + reader["Id"].ToString() +"' data-id='" + reader["Id"].ToString() + "' value='0' class='form-control soluong' onchange='javascript:update_thanhtien(" + reader["Id"] +")'></td>"
                            + "<td class='text-center w-10 align-content-center'><input type='text' name='TotalAmount'  id='thanhtien_" + reader["Id"].ToString() +"' data-id='" + reader["Id"].ToString() +"' value='0' class='form-control' readonly></td>"
                            + "</tr>";
                }
                reader.Close();
            }
            var item = new
            {
                str,
                count
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}