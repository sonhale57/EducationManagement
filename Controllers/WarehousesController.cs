using Antlr.Runtime.Misc;
using Microsoft.Ajax.Utilities;
using Mysqlx.Crud;
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
using System.Web.Services.Description;

namespace SuperbrainManagement.Controllers
{
    public class WarehousesController : Controller
    {
        public ModelDbContext db = new ModelDbContext();
        public ActionResult Index()
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            else
            {
                var branches = db.Branches.ToList();
                int idbranch = int.Parse(CheckUsers.idBranch());
                if (!CheckUsers.CheckHQ())
                {
                    branches = db.Branches.Where(x => x.Id == idbranch).ToList();
                }
                ViewBag.IdBranch = new SelectList(branches, "Id", "Name");
                return View();
            }
        }
        public ActionResult Receiptions(string idBranch)
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            else
            {
                var branches = db.Branches.ToList();
                int idbranch = int.Parse(CheckUsers.idBranch());
                if (!CheckUsers.CheckHQ())
                {
                    branches = db.Branches.Where(x => x.Id == idbranch).ToList();
                }
                ViewBag.IdBranch = new SelectList(branches, "Id", "Name");
                return View();
            }
        }
        public ActionResult Load_thekho(string idBranch, string sort, string searchString, DateTime fromdate, DateTime todate,int? Type)
        {
            string str = "";
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysort = "";
            string querysearch = "";
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and vt.Name like N'" + searchString + "' or vt.Code like N'" + searchString + "'";
            }
            if(Type == null)
            {
                Type = 1;
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "name":
                        querysort = " order by vt.Name";
                        break;
                    case "name_desc":
                        querysort = " order by vt.Name desc";
                        break;
                    case "date_desc":
                        querysort = " order by rec.Id desc";
                        break;
                    default:
                        querysort = " order by rec.Id";
                        break;
                }
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select vt.Image,vt.Code as ProductCode,vt.name,vt.Unit,pk.Price,pk.Amount,pk.TotalAmount,pk.type,pk.status,rec.Code as ReceiptionCode,rec.DateCreate,us.Name as Username"
                            + " from ProductReceiptionDetail pk inner join Product vt on vt.id = pk.IdProduct,WarehouseReceiption rec,[User] us"
                            + " where rec.id = pk.IdReceiption and us.Id = rec.IdUser and rec.DateCreate >= '" + fromdate + "' and rec.DateCreate <= '" + todate + "'" +querysearch
                            + " and rec.IdBranch = '" + idBranch + "' and rec.type='"+Type+"' "+querysort;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    double amount = Double.Parse(reader["Price"].ToString(), 0);
                    double totalamount = Double.Parse(reader["TotalAmount"].ToString(),0);
                    count++;
                    str += "<tr>"
                            + "<td class='text-center'>" + count + "</td>"
                            + "<td class='text-center'>" + reader["ProductCode"].ToString() + "</td>"
                            + "<td>" + reader["name"].ToString() + "</td>"
                            + "<td>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0} đ", amount) + "</td>"
                            + "<td class='text-center'>" + reader["Amount"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0} đ", totalamount) + "</td>"
                            + "<td class='text-center'>" + (reader["Type"].ToString() == "True" ? "Nhập kho" : "Xuất kho") + "</td>"
                            + "<td class='text-center'>" + reader["ReceiptionCode"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["DateCreate"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Username"].ToString() + "</td>"
                            + "</tr>";
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadlist(string idBranch, string sort, string searchString)
        {
            string str = "";
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysort = " order by p.Name";
            string querysearch = "";
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and (p.Name like N'" + searchString + "' or p.Code like N'" + searchString + "')";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "date":
                        querysort = " order by p.Id";
                        break;
                    case "date_desc":
                        querysort = " order by p.Id desc";
                        break;
                    case "name_desc":
                        querysort = " order by p.Name desc";
                        break;
                    default:
                        querysort = " order by p.Name";
                        break;
                }
            }
            int count = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryCat = "SELECT * From ProductCategory where Active=1 and Enable=1 order by DisplayOrder";

                SqlCommand commandCat = new SqlCommand(queryCat, connection);
                connection.Open();
                SqlDataReader readerCat = commandCat.ExecuteReader();

                while (readerCat.Read())
                {
                    str += "<tr>"
                        + "<td class='text-center text-success fw-bolder'>" + readerCat["Code"] + "</td>"
                        + "<td class='text-success fw-bolder' colspan=6>" + readerCat["Name"] + "</td>"
                        + "</tr>";
                    string query = "SELECT p.Id,p.Name,p.Image,p.Unit,p.Price,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + idBranch + "), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + idBranch + "), 0) AS Tonkho"
                                        + " FROM product p"
                                        + " where p.enable=1 and p.IdCategory=" + readerCat["Id"]
                                        + querysearch
                                        + querysort;
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        double dongia = Convert.ToInt32(reader["Price"].ToString());
                        count++;
                        str += "<tr>"
                                + "<td class='text-center'>" + count + "</td>"
                                + "<td>" + reader["code"].ToString() + "</td>"
                                + "<td> <img src='" + reader["Image"].ToString() + "' alt='' class='rounded-2 me-2' height='40'>" + reader["Name"].ToString() + "</td>"
                                + "<td class='text-center'>" + reader["Unit"].ToString() + "</td>"
                                + "<td class='text-end'>" + string.Format("{0:N0}",dongia) + "</td>"
                                + "<td class='text-center'>" + reader["Quota"].ToString() + "</td>"
                                + "<td class='text-center'>" + reader["Tonkho"].ToString() + "</td>"
                                + "</tr>";
                    }
                    reader.Close();
                }
                readerCat.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadlist_nhap()
        {
            string str = ""; int count = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Name,p.Unit,p.Price,p.Code,p.Quota,p.Image " 
                               + " FROM product p join ProductCategory pc on p.IdCategory = pc.Id" 
                               + " where pc.enable=1 and pc.active=1 and p.enable = 1 and p.active = 1 order by pc.DisplayOrder,p.Name";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    double amount = Double.Parse(reader["Price"].ToString(), 0);
                    count++;
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='w-25 align-content-center'> <img src='" + reader["Image"].ToString() + "' alt='' class='rounded-2 me-2' height='40'>" + reader["Name"].ToString() + "</td>"
                            + "<td class='w-25 align-content-center text-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='w-10 align-content-center text-center'>"
                            + "<input type='hidden' name='IdProduct_" + count + "' id='idproduct_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + reader["Id"].ToString() + "' class='form-control' onchange='javascript:update_thanhtien(" + count + ")'>"
                            + "<input type='text' name='Price_" + count + "' id='dongia_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + string.Format("{0:N0}", amount) + "' class='form-control text-end' onchange='javascript:update_thanhtien(" + count + ")'>"
                            + "</td>"
                            + "<td class='text-center w-5 align-content-center'><input type='text' name='Amount_" + count + "'  id='soluong_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' class='form-control text-center soluong' onchange='javascript:update_thanhtien(" + count + ")'></td>"
                            + "<td class='text-center w-10 align-content-center'><input type='text' name='TotalAmount_" + count + "'  id='thanhtien_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' class='form-control text-end' readonly></td>"
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
        public ActionResult Loadlist_xuat()
        {
            string str = ""; int count = 0;
            int idBranch = int.Parse(CheckUsers.idBranch());
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Image, p.Name,p.Unit,p.Price,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch = " + idBranch + "), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch = " + idBranch + "), 0) AS Tonkho"
                                        + " FROM product p"
                                        + " join ProductCategory pc on p.IdCategory=pc.Id"
                                        + " where p.enable=1 and p.active=1 and pc.Enable=1 and pc.Active=1"
                                        + " order by pc.DisplayOrder,p.Name";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count++;
                    double amount = Double.Parse(reader["Price"].ToString(), 0);
                    int tonkho = int.Parse(reader["Tonkho"].ToString());
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='w-25 align-content-center'> <img src='" + reader["Image"].ToString() + "' alt='' class='rounded-2 me-2' height='40'>" + reader["Name"].ToString() + "</td>"
                            + "<td class='w-25 align-content-center text-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='w-10 align-content-center text-center'>"
                            + "<input type='hidden' name='IdProduct_" + count + "' id='idproduct_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + reader["Id"].ToString() + "' class='form-control' onchange='javascript:update_thanhtienxuat(" + count + ")'>"
                            + "<input type='text' name='Price_" + count + "' id='dongia_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + string.Format("{0:N0}", amount) + "' class='form-control text-end' onchange='javascript:update_thanhtienxuat(" + count + ")'>"
                            + "</td>"
                            + "<td class='text-center w-5 align-content-center'><input type='text' name='Amount_" + count + "'  id='soluong_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' max='" + reader["Tonkho"].ToString() + "' class='form-control text-center soluong " + (tonkho > 0 ? "" : "disabled") + "' onchange='javascript:update_thanhtienxuat(" + count + ")' ></td>"
                            + "<td class='text-center w-10 align-content-center'><input type='text' name='TotalAmount_" + count + "'  id='thanhtien_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' class='form-control text-end' readonly></td>"
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
        string Getcode_receiption(bool type)
        {
            string loai = "";
            int idBranch = int.Parse(CheckUsers.idBranch());
            int nextCode = 0;
            if (type == false)
            {
                loai = "XK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == idBranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "NK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == idBranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(7, '0');
            var cn = db.Branches.Find(int.Parse(CheckUsers.idBranch()));
            string str = cn.Code + loai + code;

            return str;
        }
        [HttpPost]
        public ActionResult Submit_nhap(FormCollection Form)
        {
            string status = "ok";
            string Name = Form["Name"].ToString();
            string Phone = Form["Phone"].ToString();
            string Address = Form["Address"].ToString();
            string Description = Form["Description"].ToString();
            int Count = int.Parse(Form["countline"].ToString());
            decimal Tongtien = Decimal.Parse(Form["tongtien"].ToString().Replace(",", ""));
            if (Tongtien > 0)
            {
                var phieunhap = new WarehouseReceiption()
                {
                    DateCreate = DateTime.Now,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = int.Parse(CheckUsers.idBranch()),
                    TotalAmount = Tongtien,
                    Status = true,
                    Name = Name,
                    Phone = Phone,
                    Address = Address,
                    Description = Description,
                    Code = Getcode_receiption(true),
                    Credit = 0,
                    Debit = 0,
                    Active = true,
                    Type = true,
                    Enable = true,
                };
                db.WarehouseReceiptions.Add(phieunhap);
                db.SaveChanges();
                int warehouseReceiptionId = phieunhap.Id;
                for (int i = 1; i <= Count; i++)
                {
                    int idproduct = int.Parse(Form["IdProduct_" + i].ToString());
                    decimal dongia = Decimal.Parse(Form["Price_" + i].ToString().Replace(",", ""));
                    int soluong = int.Parse(Form["Amount_" + i].ToString());
                    Decimal thanhtien = Decimal.Parse(Form["TotalAmount_" + i].ToString().Replace(",", ""));

                    if (soluong > 0)
                    {
                        var details = new ProductReceiptionDetail()
                        {
                            IdReceiption = warehouseReceiptionId,
                            IdProduct = idproduct,
                            Amount = soluong,
                            Price = dongia,
                            TotalAmount = thanhtien,
                            Status = true,
                            Type = true,
                            Discount = 0
                        };
                        db.ProductReceiptionDetails.Add(details);
                        db.SaveChanges();
                    }

                }
            }
            else
            {
                status = "Không tìm thấy vật tư cần nhập";
            }
            var item = new
            {
                status = status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_xuat(FormCollection Form)
        {
            string status = "ok";
            string Name = Form["Name"].ToString();
            string Phone = Form["Phone"].ToString();
            string Address = Form["Address"].ToString();
            string Description = Form["Description"].ToString();
            int Count = int.Parse(Form["countline_xuat"].ToString());
            decimal Tongtien = Decimal.Parse(Form["tongtien_xuat"].ToString().Replace(",", ""));
            if (Tongtien > 0)
            {
                var phieuxuat = new WarehouseReceiption()
                {
                    DateCreate = DateTime.Now,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = int.Parse(CheckUsers.idBranch()),
                    TotalAmount = Tongtien,
                    Status = true,
                    Name = Name,
                    Phone = Phone,
                    Address = Address,
                    Description = Description,
                    Code = Getcode_receiption(false),
                    Credit = 0,
                    Debit = 0,
                    Active = true,
                    Type = false,
                    Enable = true,
                };
                db.WarehouseReceiptions.Add(phieuxuat);
                db.SaveChanges();
                int warehouseReceiptionId = phieuxuat.Id;
                for (int i = 1; i <= Count; i++)
                {
                    int idproduct = int.Parse(Form["IdProduct_" + i].ToString());
                    decimal dongia = Decimal.Parse(Form["Price_" + i].ToString().Replace(",", ""));
                    int soluong = int.Parse(Form["Amount_" + i].ToString());
                    Decimal thanhtien = Decimal.Parse(Form["TotalAmount_" + i].ToString().Replace(",", ""));

                    if (soluong > 0)
                    {
                        var details = new ProductReceiptionDetail()
                        {
                            IdReceiption = warehouseReceiptionId,
                            IdProduct = idproduct,
                            Amount = soluong,
                            Price = dongia,
                            TotalAmount = thanhtien,
                            Status = true,
                            Type = false,
                            Discount = 0
                        };
                        db.ProductReceiptionDetails.Add(details);
                        db.SaveChanges();
                    }

                }
            }
            else
            {
                status = "Không tìm thấy vật tư cần xuất";
            }
            var item = new
            {
                status = status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Statistics()
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            else
            {
                var branches = db.Branches.ToList();
                int idbranch = int.Parse(CheckUsers.idBranch());
                if (!CheckUsers.CheckHQ())
                {
                    branches = db.Branches.Where(x => x.Id == idbranch).ToList();
                }
                ViewBag.IdBranch = new SelectList(branches, "Id", "Name");
                return View();
            }
        }

    }
}