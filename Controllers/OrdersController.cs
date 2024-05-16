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
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysort = "";
            string querysearch = "";
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
                string query = "select o.Code,o.DateCreate,o.status,o.Phone,o.Address,o.Description,o.Enable,us.Name as Username,(select sum(TotalAmount) from OrderDetail where IdOrder=o.Id) as tongtien "
                            + " from [Order] o,[User] us"
                            + " where us.id=o.IdUser and o.Status='"+status+"' and o.idbranch='"+idBranch+"' order by o.Id desc";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    str += "<div class=\"list-group-item list-group-item-action mt-2\" aria-current=\"true\">"
                                    +"<div class=\"d-flex w-100 justify-content-between\">"
                                        +"<p class=\"mb-1\"><i class=\"ti ti-shopping-cart\"></i> <b>" + reader["Code"] +"</b> - <small>" + DateTime.Parse(reader["DateCreate"].ToString())+"</small></p>"
                                        +"<div>"
                                            +"<a href=\"#\" class=\"btn btn-danger btn-sm\"><i class=\"ti ti-trash\"></i></a>"
                                            +"<a href=\"#\" class=\"btn btn-success btn-sm\"><i class=\"ti ti-printer\"></i></a>"
                                        +"</div>"
                                    +"</div>"
                                    +"<p class=\"mb-1\">Người đặt: " + reader["Username"] +"</p>"
                                    +"<p class=\"mb-1\">Số điện thoại: " + reader["Phone"] +"</p>"
                                    + (reader["Address"].ToString()==""?"":"<p class=\"mb-1\">Địa chỉ giao hàng: " + reader["Address"] +"</p>")
                                    +"<p class=\"mb-1\">Tổng tiền: " + reader["tongtien"] +"</p>"
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
    }
}