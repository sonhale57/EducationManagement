﻿using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers
{
    public class OrdersController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Orders
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
        public ActionResult Loadlist(string idBranch,int status,string sort,string searchString)
        {
            string str = "";
            string idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysort = "";
            string querysearch = "";
            string queryBranch = "";
            if (idBranch == idbranch_hq)
            {
                queryBranch = "";
            }
            else
            {
                queryBranch = "and o.idbranch = ' "+idBranch+" ' ";
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and p.Name like N'" + searchString + "' or p.Code like N'" + searchString + "'";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
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
                string query = "select o.Id,o.Code,o.DateCreate,o.status,o.Phone,o.Address,o.Description,o.Enable,us.Name as Username,cs.Name as TenCoSo,(select sum(TotalAmount) from OrderDetail where IdOrder=o.Id) as tongtien "
                            + " from [Order] o,[User] us,Branch cs"
                            + " where o.IdBranch = cs.Id and us.id=o.IdUser and o.Status='"+status+"'" +queryBranch+"  order by o.Id desc";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    string badgeStatus = "";
                    if (reader["status"].ToString() == "1")
                    {
                        badgeStatus = "<span class='badge text-success bg-light'>Đơn hàng mới</span>";
                    }else if (reader["status"].ToString() == "2")
                    {
                        badgeStatus = "<span class='badge text-info bg-light'>Đã thanh toán</span>";
                    }
                    else if (reader["status"].ToString() == "3")
                    {
                        badgeStatus = "<span class='badge text-info bg-light'>Đã xác nhận</span>";
                    }
                    else if (reader["status"].ToString() == "4")
                    {
                        badgeStatus = "<span class='badge text-primary bg-light'>Đã đóng gói</span>";
                    }
                    else if (reader["status"].ToString() == "5")
                    {
                        badgeStatus = "<span class='badge text-primary bg-light'>Đang giao hàng</span>";
                    }
                    else if (reader["status"].ToString() == "6")
                    {
                        badgeStatus = "<span class='badge text-white bg-success'>Đã hoàn thành</span>";
                    }
                    else if (reader["status"].ToString() == "0")
                    {
                        badgeStatus = "<span class='badge text-white bg-light'>Đơn hàng hủy</span>";
                    }
                    str += "<div class=\"list-group-item list-group-item-action mt-2\" aria-current=\"true\">"
                                    +"<div class=\"d-flex w-100 justify-content-between\">"
                                        +"<p class=\"mb-1\"><i class=\"ti ti-shopping-cart\"></i> <a href='javascript:Status_Order(" + reader["Id"] +")' class='text-dark fw-bolder'>" + reader["Code"] +"</a> "+badgeStatus+"</p>"
                                        +"<div>"
                                            +"<a href=\"#\" class=\"btn btn-danger btn-sm ms-1\"><i class=\"ti ti-trash\"></i></a>"
                                            +"<a href=\"#\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>"
                                        +"</div>"
                                    +"</div>"
                                    + "<p class=\"mb-1\">Cơ sở: <b>" + reader["TenCoSo"] + "</b></p>"
                                     + "<div class=\"d-flex w-100 justify-content-between\">"
                                    + "<p class=\"mb-1\">Người đặt: <b>" + reader["Username"] +"</b></p>"
                                    + "<small>" + DateTime.Parse(reader["DateCreate"].ToString()) + "</small>"
                                    + "</div>"
                                    + (reader["Phone"].ToString()==""?"":"<p class=\"mb-1\">Số điện thoại: <b>" + reader["Phone"] +"</b></p>")
                                    + (reader["Address"].ToString()==""?"":"<p class=\"mb-1\">Địa chỉ giao hàng: <b>" + reader["Address"] +"</b></p>")
                                    + "<p class=\"mb-1\">Tổng tiền: <b>" + reader["tongtien"] +"</b></p>"
                                +"</div>";
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadlist_order()
        {
            string idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            string str = ""; int count = 0;
            int idBranch = int.Parse(CheckUsers.idBranch());
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Image, p.Name,p.Unit,p.Price,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch ="+idbranch_hq+"), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch ="+idbranch_hq+"), 0) AS Tonkho"
                                        + " FROM product p"
                                        + " where p.enable=1 "
                                        + " order by p.Name";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    count++;
                    int tonkho = int.Parse(reader["Tonkho"].ToString());
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='w-25 align-content-center'> <img src='" + reader["Image"].ToString() + "' alt='" + reader["Name"].ToString() + "' class='rounded-2 me-2' height='40'><span class='text-success'>" + reader["code"].ToString() + "</span> - " + reader["Name"].ToString() + "</td>"
                            + "<td class='w-25 align-content-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='w-10 align-content-center'>"
                            + "<input type='hidden' name='IdProduct_" + count + "' id='idproduct_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + reader["Id"].ToString() + "' class='form-control' onchange='javascript:update_thanhtien(" + count + ")'>"
                            + "<input type='text' name='Price_" + count + "' id='dongia_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + reader["Price"].ToString() + "' class='form-control' onchange='javascript:update_thanhtien(" + count + ")'>"
                            + "</td>"
                            + "<td class='text-center w-5 align-content-center'><input type='text' name='Amount_" + count + "'  id='soluong_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' max='" + reader["Tonkho"].ToString() + "' class='form-control soluong' onchange='javascript:update_thanhtien(" + count + ")' " + (tonkho > 0 ? "" : "Disabled") + "></td>"
                            + "<td class='text-center w-10 align-content-center'><input type='text' name='TotalAmount_" + count + "'  id='thanhtien_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' class='form-control' readonly></td>"
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
        public ActionResult LoadStatus_Order(int id)
        {
            string idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            string str = ""; int count = 0;
            string strCode = "",strTongtien="";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select p.Code as CodeProduct,p.Name,p.Unit,p.Image,od.Amount,od.Price,od.TotalAmount,o.Code as CodeOrder,o.Address,o.Phone,o.Description,o.Status,us.Name as Username,o.DateCreate,(select sum(TotalAmount) from OrderDetail where IdOrder=o.Id) as tongtien "
                                    + " from OrderDetail od inner join [Order] o on o.id = od.IdOrder, Product p,[User] us"
                                    +" where p.id=od.IdProduct and us.Id=o.IdUser and o.Id="+id;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                double tongtien = 0;
                while (reader.Read())
                {
                    count++;

                    double thanhtien = Double.Parse(reader["TotalAmount"].ToString());
                    tongtien += thanhtien;
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class=''> <img src='" + reader["Image"].ToString() + "' alt='" + reader["Name"].ToString() + "' class='rounded-2 me-2' height='40'><span class='text-success'>" + reader["CodeProduct"].ToString() + "</span> - " + reader["Name"].ToString() + "</td>"
                            + "<td class='align-content-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='align-content-end'>" + reader["Price"].ToString() + "</td>"
                            + "<td class='align-content-center'>" + reader["Amount"].ToString() + "</td>"
                            + "<td class='align-content-end'>" + reader["TotalAmount"].ToString() + "</td>"
                           + "</tr>";
                    strCode = reader["CodeOrder"].ToString();
                    strTongtien  = reader["tongtien"].ToString();
                }
                str += "<tr><td colspan=5 class='text-end'>Tổng tiền: </td><td>"+tongtien+"</td></tr>";
                reader.Close();
            }
            var item = new
            {
                str,
                strCode,
                strTongtien,
                count
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        string Getcode_order()
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            int nextCode = db.Orders.Count() + 1;
            string code = nextCode.ToString().PadLeft(7, '0');
            var cn = db.Branches.Find(idBranch);
            string str = cn.Code  + code;

            return str;
        }
        string Getcode_receiption(bool type)
        {
            string loai = "";
            int idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id;
            int idBranch = int.Parse(CheckUsers.idBranch());
            int nextCode = 0;
            if (type == false)
            {
                loai = "XK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == idbranch_hq && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "NK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == idbranch_hq && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(7, '0');
            var cn = db.Branches.Find(int.Parse(CheckUsers.idBranch()));
            string str = cn.Code + loai + code;

            return str;
        }
        [HttpPost]
        public ActionResult Submit_order(FormCollection Form)
        {
            int idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id;
            string status = "ok";
            string Name = Form["Name"].ToString();
            string Phone = Form["Phone"].ToString();
            string Address = Form["Address"].ToString();
            string Description = Form["Description"].ToString();
            int Count = int.Parse(Form["countline"].ToString());
            decimal Tongtien = Decimal.Parse(Form["tongtien"].ToString().Replace(",", ""));
            if (Tongtien > 0)
            {
                var order = new Order()
                {
                    DateCreate = DateTime.Now,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = int.Parse(CheckUsers.idBranch()),
                    Status = 1,
                    Phone = Phone,
                    Address = Address,
                    Description = Description,
                    Code = Getcode_order(),
                    Enable = true,
                };
                var receiption =new WarehouseReceiption(){ 
                    Name = Name,
                    Phone = Phone,
                    Address = Address,
                    Description = Description,
                    Code = Getcode_receiption(false),
                    Status= true,
                    Type=false,
                    Enable = true,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = idbranch_hq,
                    Credit=0,
                    Debit=0,
                    DateCreate = DateTime.Now,
                };
                db.Orders.Add(order);
                db.WarehouseReceiptions.Add(receiption);
                db.SaveChanges();
                int OrderId = order.Id;
                int ReceiptionId = receiption.Id;
                for (int i = 1; i <= Count; i++)
                {
                    int idproduct = int.Parse(Form["IdProduct_" + i].ToString());
                    decimal dongia = Decimal.Parse(Form["Price_" + i].ToString());
                    int soluong = int.Parse(Form["Amount_" + i].ToString());
                    Decimal thanhtien = Decimal.Parse(Form["TotalAmount_" + i].ToString().Replace(",", ""));
                    if (soluong > 0)
                    {
                        var details = new OrderDetail()
                        {
                            IdOrder = OrderId,
                            IdProduct = idproduct,
                            Amount = soluong,
                            Price = dongia,
                            TotalAmount = thanhtien,
                            Status = true
                        };
                        var product = new ProductReceiptionDetail() {
                            IdProduct = idproduct,
                            IdReceiption = ReceiptionId,
                            Amount = soluong,
                            Price= dongia,
                            TotalAmount = thanhtien,
                            Discount = 0,
                            Type= false
                        };
                        db.ProductReceiptionDetails.Add(product);
                        db.OrderDetails.Add(details);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                status = "Không tìm thấy vật tư cần đặt";
            }
            var item = new
            {
                status = status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_StatusOrder(int IdOrder,int Status,string Description,HttpPostedFileBase file)
        {
            string status = "ok";
            var order = db.Orders.Find(IdOrder);
            var orderStatus = new OrderStatus() { 
                IdOrder =IdOrder,
                Status = Status,
                Updatetime = DateTime.Now, 
                Description = Description,
                IdUser = int.Parse(CheckUsers.iduser()),
            };
            if (file != null && file.ContentLength > 0)
            {
                string _FileName = Path.GetFileName(file.FileName);
                string _path = Path.Combine(Server.MapPath("~/Uploads"), _FileName);
                file.SaveAs(_path);
                orderStatus.Image = "/Uploads/" + _FileName;
            }
            db.OrderStatus.Add(orderStatus);
            order.Status = Status;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            var item = new
            {
                status = status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
    }
}