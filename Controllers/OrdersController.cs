using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.BuilderProperties;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SuperbrainManagement.Controllers
{
    public class OrdersController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Orders
        public ActionResult Index(string idBranch)
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
                if (string.IsNullOrEmpty(idBranch))
                {
                    idBranch = branches.First().Id.ToString();
                }
                ViewBag.IdBranch = new SelectList(branches, "Id", "Name", idBranch);
                return View();
            }
        }
        public ActionResult Loadlist_Count(string IdBranch) 
        {
            string IdBranch_HQ = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            var list = db.Orders.ToList();
            int count0=0,count1=0,count2=0,count3= 0,count4 = 0,count5 = 0,count6 = 0;
            if (string.IsNullOrEmpty(IdBranch))
            {
                //Dành cho cơ sở
                int idbranch = Convert.ToInt32(CheckUsers.idBranch());
                list = db.Orders.Where(x=>x.IdBranch == idbranch).ToList();
            }
            else
            {
                if(IdBranch_HQ == IdBranch)
                {
                    // Load toàn bộ danh sách
                    list = db.Orders.ToList();
                }
                else
                {
                    //Load của cơ sở
                    int idbranch = Convert.ToInt32(IdBranch);
                    list = db.Orders.Where(x => x.IdBranch == idbranch).ToList();
                }
            }
            foreach(var i in list)
            {
                switch(i.Status)
                {
                    case 0:
                        count0++;
                        break;
                    case 1:
                        count1++;
                        break;
                    case 2:
                        count2++;
                        break;
                    case 3:
                        count3++;
                        break;
                    case 4:
                          count4++;
                        break;
                    case 5:
                        count5++;
                        break;
                    case 6:
                        count6++;
                        break;
                }
            }
            var item = new {
                count0, count1, count2, count3, count4, count5, count6
            };
            return Json(item,JsonRequestBehavior.AllowGet); 
        }
        public ActionResult Loadlist(string idBranch, int? status, string searchString)
        {
            string str = "";
            string idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            if (string.IsNullOrEmpty(idBranch))
            {
                idBranch = CheckUsers.idBranch();
            }
            string querysearch = "";
            string queryBranch = "";
            if (idBranch == idbranch_hq)
            {
                queryBranch = "";
            }
            else
            {
                queryBranch = "and o.idbranch = ' " + idBranch + " ' ";
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                querysearch = " and o.Code like N'" + searchString + "'";
            }
            else
            {
                querysearch = " and o.Status='"+status+"'";
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select o.Id,o.Code,o.DateCreate,o.status,o.Phone,o.Address,o.Description,o.Enable,us.Name as Username,cs.Name as TenCoSo,(select sum(TotalAmount) from OrderDetail where IdOrder=o.Id) as tongtien "
                            + " from [Order] o,[User] us,Branch cs"
                            + " where o.IdBranch = cs.Id and us.id=o.IdUser " + queryBranch + querysearch+ " order by o.Id desc";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    string badgeStatus = "";
                    string strbtn = "";
                    if (reader["status"].ToString() == "1")
                    {
                        badgeStatus = "<span class='badge text-success bg-light'>Đơn hàng mới</span>";
                        strbtn = "<a href=\"javascript:Cancel_Order(" + reader["Id"] + ")\" class=\"btn btn-danger btn-sm ms-1\"><i class=\"ti ti-trash\"></i></a>" 
                                + "<a href=\"/Orders/PrintOrder?IdOrder=" + reader["Id"] +"\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "2")
                    {
                        badgeStatus = "<span class='badge text-info bg-light'>Đã thanh toán</span>";
                        strbtn = "<a href=\"/Orders/PrintOrder?IdOrder=" + reader["Id"] +"\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "3")
                    {
                        badgeStatus = "<span class='badge text-info bg-light'>Đã xác nhận</span>";
                        strbtn = "<a href=\"/Orders/PrintOrder?IdOrder=" + reader["Id"] +"\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "4")
                    {
                        badgeStatus = "<span class='badge text-primary bg-light'>Đã đóng gói</span>";
                        strbtn = "<a href=\"/Orders/PrintOrder?IdOrder=" + reader["Id"] +"\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "5")
                    {
                        badgeStatus = "<span class='badge text-primary bg-light'>Đang giao hàng</span>";
                        strbtn = "<a href=\"/Orders/PrintOrder?IdOrder=" + reader["Id"] +"\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "6")
                    {
                        badgeStatus = "<span class='badge text-white bg-success'>Đã hoàn thành</span>";
                        strbtn = "<a href=\"/Orders/PrintOrder?IdOrder=" + reader["Id"] +"\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "0")
                    {
                        badgeStatus = "<span class='badge text-white bg-light'>Đơn hàng hủy</span>";
                        strbtn = "";
                    }
                    double tongtien = Double.Parse(reader["tongtien"].ToString());
                    str += "<div class='card mb-2'><div class=\"card-header\" style='padding:5px 20px!important;'>"
                                    + "<div class=\"d-flex w-100 justify-content-between\">"
                                        + "<p class=\"mb-1\"><i class=\"ti ti-shopping-cart\"></i> <a href='javascript:Status_Order(" + reader["Id"] + ")' class='text-dark fw-bolder'>" + reader["Code"] + "</a> " + badgeStatus + "</p>"
                                        + "<div>"
                                            +strbtn
                                        + "</div>"
                                    + "</div>"
                                + "</div>"
                                + "<div class='card-body' style='padding:5px 20px!important;'>"
                                    + "<p class=\"mb-1\">Cơ sở: <b>" + reader["TenCoSo"] + "</b></p>"
                                    + "<div class=\"justify-content-between\">"
                                    + "<p class=\"mb-1\">Người đặt: <b>" + reader["Username"] + "</b></p>"
                                    + "</div>"
                                    + (reader["Phone"].ToString() == "" ? "" : "<p class=\"mb-1\">Số điện thoại: <b>" + reader["Phone"] + "</b></p>")
                                    + (reader["Address"].ToString() == "" ? "" : "<p class=\"mb-1\">Địa chỉ giao hàng: <b>" + reader["Address"] + "</b></p>")
                                 + "</div>"
                                 + "<div class='align-content-center'  style='padding:0 20px!important;'>"
                                    + "<div class='d-flex w-100 justify-content-between'>"
                                        + "<p>Thời gian đặt hàng: <i>" + DateTime.Parse(reader["DateCreate"].ToString()) + "</i></p>"
                                        + "<p>Tổng tiền: <b>" + string.Format("{0:N0} đ", tongtien) + "</b></p>"
                                    + "</div>"
                                + "</div>"
                           + "</div>";
                }
                reader.Close();
            }
            var item = new
            {
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadCount(string IdBranch)
        {
            string IdBranch_HQ = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            int count_new = 0, count_cancel = 0, count_payment = 0, count_confirm = 0, count_package = 0, count_delivery = 0, count_complete = 0;
            var list = db.Orders.Where(x=>x.IdBranch ==Convert.ToInt32(IdBranch)).ToList();
            
            if (string.IsNullOrEmpty(IdBranch))
            {
                //Kiểm tra tài khoản đang đăng nhập => Cơ sở
                IdBranch = CheckUsers.idBranch();
                 if (IdBranch == IdBranch_HQ)
                {
                    list = db.Orders.ToList();
                }
                else
                {
                    int idbranch = Convert.ToInt32(IdBranch);
                    list = db.Orders.Where(x => x.IdBranch == idbranch).ToList();
                }
            }
            else
            {
                int idbranch = Convert.ToInt32(IdBranch);
                list = db.Orders.Where(x => x.IdBranch == idbranch).ToList();
            }
           
            foreach(var i  in list)
            {
                if (i.Status == 1)
                {
                    count_new++;
                } else if (i.Status == 2)
                {
                    count_payment++;
                } else if (i.Status == 3)
                {
                    count_confirm++;
                } else if (i.Status == 4)
                { 
                    count_package++; 
                }else if(i.Status == 5)
                {
                    count_delivery++;
                }else if(i.Status == 6)
                {
                    count_complete++;
                }
                else { count_cancel++; }    
            }
            var item = new
            {
                count_new,
                count_payment,
                count_confirm,
                count_package,
                count_delivery,
                count_complete,
                count_cancel
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public string Status_timeline(int IdOrder)
        {
            var orderStatus = db.OrderStatus.Where(x => x.IdOrder == IdOrder);
            var order = db.Orders.Find(IdOrder);
            string str = "<div class=\"text_circle done\">"
                                + "<div class=\"circle\">"
                                + "<h4>Đơn hàng mới</h4>"
                                + "<p>" + order.DateCreate.Value.ToString("dd/MM/yyyy") + "</p>"
                                + "</div>"
                                + "<a href=\"javascript:void(0)\" class=\"tvar\"><span data-toggle=\"popover\" title=\"Đơn hàng mới\" data-trigger=\"hover\" data-placement=\"top\" data-content=\"" + order.Description + "\"></span></a>"
                                + "</div>";
            if (orderStatus.Count() > 0)
            {
                foreach (var item in orderStatus)
                {
                    if (item.Status == 2)
                    {
                        str += "<div class=\"text_circle done\">"
                                    + "<div class=\"circle\">"
                                    + "<h4>Đã thanh toán</h4>"
                                    + "<p>" + item.Updatetime.Value.ToString("dd/MM/yyyy") + "</p>"
                                    + "</div>"
                                    + "<a href=\"" + item.Image + "\" class=\"tvar\" target='_blank'><span data-toggle=\"popover\" title=\"Đã thanh toán\" data-trigger=\"hover\" data-placement=\"top\" data-content=\"" + item.Description + "\"></span></a>"
                                    + "</div>";
                    }
                    if (item.Status == 3)
                    {
                        str += "<div class=\"text_circle done\">"
                                    + "<div class=\"circle\">"
                                    + "<h4>Đã xác nhận</h4>"
                                    + "<p>" + item.Updatetime.Value.ToString("dd/MM/yyyy") + "</p>"
                                    + "</div>"
                                    + "<a href=\"" + item.Image + "\" class=\"tvar\" target='_blank'><span data-toggle=\"popover\" title=\"Đã thanh toán\" data-trigger=\"hover\" data-placement=\"top\" data-content=\"" + item.Description + "\"></span></a>"
                                    + "</div>";
                    }
                    if (item.Status == 4)
                    {
                        str += "<div class=\"text_circle done\">"
                                    + "<div class=\"circle\">"
                                    + "<h4>Đã đóng gói</h4>"
                                    + "<p>" + item.Updatetime.Value.ToString("dd/MM/yyyy") + "</p>"
                                    + "</div>"
                                    + "<a href=\"" + item.Image + "\" class=\"tvar\" target='_blank'><span data-toggle=\"popover\" title=\"Đã đóng gói\" data-trigger=\"hover\" data-placement=\"top\" data-content=\"" + item.Description + "\"></span></a>"
                                    + "</div>";
                    }
                    if (item.Status == 5)
                    {
                        str += "<div class=\"text_circle done\">"
                                    + "<div class=\"circle\">"
                                    + "<h4>Đang giao hàng</h4>"
                                    + "<p>" + item.Updatetime.Value.ToString("dd/MM/yyyy") + "</p>"
                                    + "</div>"
                                    + "<a href=\"" + item.Image + "\" class=\"tvar\" target='_blank'><span data-toggle=\"popover\" title=\"Đang giao hàng\" data-trigger=\"hover\" data-placement=\"top\" data-content=\"" + item.Description + "\"></span></a>"
                                    + "</div>";
                    }
                    if (item.Status == 6)
                    {
                        str += "<div class=\"text_circle done\">"
                                    + "<div class=\"circle\">"
                                    + "<h4>Đã hoàn thành</h4>"
                                    + "<p>" + item.Updatetime.Value.ToString("dd/MM/yyyy") + "</p>"
                                    + "</div>"
                                    + "<a href=\"" + item.Image + "\" class=\"tvar\" target='_blank'><span data-toggle=\"popover\" title=\"Đã hoàn thành\" data-trigger=\"hover\" data-placement=\"top\" data-content=\"" + item.Description + "\"></span></a>"
                                    + "</div>";
                    }
                }
            }
            return str;
        }
        public ActionResult Loadlist_order()
        {
            int IdUser = Convert.ToInt32(CheckUsers.iduser());
            var user = db.Users.Find(IdUser);

            string idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            string str = ""; int count = 0;
            int idBranch = int.Parse(CheckUsers.idBranch());
            var cn = db.Branches.Find(idBranch);
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT p.Id,p.Image, p.Name,p.Unit,p.Price,p.NumberOfPackage,p.UnitOfPackage,p.Code,p.Quota,COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '1' AND re.IdBranch =" + idbranch_hq + "), 0) -"
                                        + " COALESCE((SELECT SUM(Amount) FROM ProductReceiptionDetail d INNER JOIN WarehouseReceiption re ON re.id = d.IdReceiption WHERE d.IdProduct = p.Id AND d.Type = '0' AND re.IdBranch =" + idbranch_hq + "), 0) AS Tonkho"
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
                    double dongia = Double.Parse(reader["Price"].ToString());
                    double heso = int.Parse(reader["NumberOfPackage"].ToString());
                    double dongiaban = dongia * heso;
                    str += "<tr>"
                            + "<td class='text-center align-content-center'>" + count + "</td>"
                            + "<td class='w-25 align-content-center'> <img src='" + reader["Image"].ToString() + "' alt='" + reader["Name"].ToString() + "' class='rounded-2 me-2' height='40'><span class='text-success'>" + reader["code"].ToString() + "</span> - " + reader["Name"].ToString() + "</td>"
                            + "<td class='w-25 text-center'>" + reader["UnitOfPackage"].ToString() + "</td>"
                            + "<td class='w-10 align-content-center'>"
                            + "<input type='hidden' name='NumberOfPackage_" + count + "' id='NumberOfPackage_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + reader["NumberOfPackage"].ToString() + "' class='form-control'>"
                            + "<input type='hidden' name='IdProduct_" + count + "' id='idproduct_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + reader["Id"].ToString() + "' class='form-control' onchange='javascript:update_thanhtien(" + count + ")'>"
                            + "<input type='text' name='Price_" + count + "' id='dongia_" + count + "' data-id='" + reader["Id"].ToString() + "' value='" + string.Format("{0:N0}", dongiaban) + "' class='form-control text-end' onchange='javascript:update_thanhtien(" + count + ")' readonly>"
                            + "</td>"
                            + "<td class='text-center w-5 align-content-center'><input type='text' name='Amount_" + count + "'  id='soluong_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' max='" + reader["Tonkho"].ToString() + "' class='form-control soluong text-center' onchange='javascript:update_thanhtien(" + count + ")' ></td>"
                            + "<td class='text-center w-10 align-content-center'><input type='text' name='TotalAmount_" + count + "'  id='thanhtien_" + count + "' data-id='" + reader["Id"].ToString() + "' value='0' class='form-control text-end' readonly></td>"
                            + "</tr>";
                }
                reader.Close();
            }
            var item = new
            {
                str,
                count,
                name= user.Name,
                phone = cn.Phone,
                address= cn.Address.ToString()
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadStatus_Order(int id)
        {
            string idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id.ToString();
            string str = ""; int count = 0;
            string strCode = "", strTongtien = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select p.Code as CodeProduct,p.Name,p.Unit,p.Image,od.Amount,od.Price,od.TotalAmount,o.Code as CodeOrder,o.Address,o.Phone,o.Description,o.Status,us.Name as Username,o.DateCreate,(select sum(TotalAmount) from OrderDetail where IdOrder=o.Id) as tongtien "
                                    + " from OrderDetail od inner join [Order] o on o.id = od.IdOrder, Product p,[User] us"
                                    + " where p.id=od.IdProduct and us.Id=o.IdUser and o.Id=" + id;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                double tongtien = 0;
                while (reader.Read())
                {
                    count++;
                    double dongia = Double.Parse(reader["Price"].ToString());
                    double thanhtien = Double.Parse(reader["TotalAmount"].ToString());
                    tongtien += thanhtien;
                    str += "<tr>"
                            + "<td class='text-center'>" + count + "</td>"
                            + "<td class=''> <img src='" + reader["Image"].ToString() + "' alt='" + reader["Name"].ToString() + "' class='rounded-2 me-2' height='40'><span class='text-success'>" + reader["CodeProduct"].ToString() + "</span> - " + reader["Name"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0}", dongia) + "</td>"
                            + "<td class='text-center'>" + reader["Amount"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0}", thanhtien) + "</td>"
                           + "</tr>";
                    strCode = reader["CodeOrder"].ToString();
                    strTongtien = reader["tongtien"].ToString();
                }
                str += "<tr><td colspan=5 class='text-end'>Tổng tiền: </td><td class='text-end'>" + string.Format("{0:N0}", tongtien) + "</td></tr>";
                reader.Close();
            }
            string strSelect = "";
            string strStatus = "";
            var status = db.Orders.Find(id).Status + 1;
            switch (status)
            {
                case 2:
                    strSelect += "<option value='2' selected>Thanh toán đơn hàng</option>";
                    strStatus = "2";
                    break;
                case 3:
                    strSelect += "<option value='3' selected>Xác nhận đơn hàng</option>";
                    strStatus = "3";
                    break;
                case 4:
                    strSelect += "<option value='4' selected>Đã đóng gói</option>";
                    strStatus = "4";
                    break;
                case 5:
                    strSelect += "<option value='5' selected>Đang giao hàng</option>";
                    strStatus = "5";
                    break;
                case 6:
                    strSelect += "<option value='6' selected>Đã hoàn thành</option>";
                    strStatus = "6";
                    break;
            }
            string strTimeline = Status_timeline(id);
            var item = new
            {
                status,
                str,
                strCode,
                strSelect,
                strTongtien,
                strTimeline,
                count,
                strStatus,
                isHQ =CheckUsers.CheckHQ()
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        string Getcode_order()
        {
            int idBranch = int.Parse(CheckUsers.idBranch());
            int nextCode = db.Orders.Count() + 1;
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(idBranch);
            string str = cn.Code +DateTime.Now.Year+ code;

            return str;
        }
        /// <summary>
        /// Mã xuất kho của HEAD QUATER
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string Getcode_receiption(bool type)
        {
            string loai = "";
            int idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id;
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
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(idbranch_hq);
            string str = cn.Code + loai +DateTime.Now.Year+ code;

            return str;
        }
        string Getcode_receiption(bool type, int IdBranch)
        {
            string loai = "";
            int nextCode = 0;
            if (type == false)
            {
                loai = "XK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == IdBranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "NK";
                nextCode = db.WarehouseReceiptions.Where(x => x.IdBranch == IdBranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(7, '0');
            var cn = db.Branches.Find(IdBranch);
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
                var receiption = new WarehouseReceiption()
                {
                    Name = Name,
                    Phone = Phone,
                    Address = Address,
                    Description = Description,
                    Code = Getcode_receiption(false),
                    Status = true,
                    Type = false,
                    Enable = true,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = idbranch_hq,
                    Credit = 0,
                    Debit = 0,
                    DateCreate = DateTime.Now,
                };
                db.Orders.Add(order);
                db.WarehouseReceiptions.Add(receiption);
                db.SaveChanges();
                int OrderId = order.Id;
                int ReceiptionId = receiption.Id;
                for (int i = 1; i <= Count; i++)
                {
                    int heso = int.Parse(Form["NumberOfPackage_" + i].ToString());
                    int idproduct = int.Parse(Form["IdProduct_" + i].ToString());
                    decimal dongia = Decimal.Parse(Form["Price_" + i].ToString());
                    int soluong = int.Parse(Form["Amount_" + i].ToString());
                    int soluongxuat = soluong * heso;
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
                        var product = new ProductReceiptionDetail()
                        {
                            IdProduct = idproduct,
                            IdReceiption = ReceiptionId,
                            Amount = soluongxuat,
                            Price = dongia,
                            TotalAmount = thanhtien,
                            Discount = 0,
                            Type = false
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
        public ActionResult Submit_StatusOrder(int IdOrder, int Status, string Description, HttpPostedFileBase file)
        {
            string status = "ok"; string fileName = "";
            var order = db.Orders.Find(IdOrder);

            var orderStatus = new OrderStatus()
            {
                IdOrder = IdOrder,
                Status = Status,
                Updatetime = DateTime.Now,
                Description = Description,
                IdUser = int.Parse(CheckUsers.iduser()),
            };
            if (file != null && file.ContentLength > 0)
            {
                // Generate a unique file name
                fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                fileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                // Specify the path to save the file
                string _path = Path.Combine(Server.MapPath("~/Uploads/Orders"), fileName);
                file.SaveAs(_path);
                orderStatus.Image = "/Uploads/Orders/" + fileName;
            }
            if (Status == 2)
            {
                Update_Transaction(IdOrder, fileName);
            }
            if (Status == 6)
            {
                Update_inventory(IdOrder);
            }
            db.OrderStatus.Add(orderStatus);
            order.Status = Status;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();


            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Cancel_Order(int id)
        {
            string status = "ok";
            var order = db.Orders.Find(id);
            Update_inventoryHQ(id);
            order.Status = 0;
            db.Entry(order);
            db.SaveChanges();
            var item = new
            {
                status
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        string Getcode_transaction(bool type,int IdOrder)
        {
            string loai = "";
            var idbranch = db.Orders.Find(IdOrder).IdBranch;
            int nextCode = 0;
            if (type == false)
            {
                loai = "PC";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == false).Count() + 1;
            }
            else if (type == true)
            {
                loai = "PT";
                nextCode = db.Transactions.Where(x => x.IdBranch == idbranch && x.Type == true).Count() + 1;
            }
            string code = nextCode.ToString().PadLeft(5, '0');
            var cn = db.Branches.Find(idbranch);
            string str = cn.Code + loai +DateTime.Now.Year+ code;

            return str;
        }
        public void Update_Transaction(int IdOrder, string Image)
        {
            var tongtien = db.OrderDetails.Where(x => x.IdOrder == IdOrder).Sum(x => x.TotalAmount);
            int IdUser = int.Parse(CheckUsers.iduser());
            int IdBranch = int.Parse(CheckUsers.idBranch());
            string code = db.Orders.Find(IdOrder).Code;
            var transaction = new Transaction()
            {
                Code = Getcode_transaction(false, IdOrder),
                Type = false,
                IdUser = IdUser,
                DateCreate = DateTime.Now,
                Amount = tongtien,
                IdBranch = IdBranch,
                Status = true,
                Description = "Thanh toán đơn hàng " + code,
                IdOrder = IdOrder,
                PaymentMethod ="chuyenkhoan",
                Name = "Head Quater",
                Image = Image,
                Discount = 0,
                TotalAmount = tongtien
            };
            db.Transactions.Add(transaction);
            db.SaveChanges();
        }
        public void Update_inventory(int IdOrder)
        {
            var order = db.Orders.Find(IdOrder);
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select o.IdBranch,o.Id as IdOrder,o.Code,od.IdProduct,p.Name,p.NumberOfPackage,od.Amount,(od.Amount * p.NumberOfPackage) as Soluong,p.Price,od.TotalAmount" +
                            " from [Order] o inner join OrderDetail od on o.Id = od.IdOrder,Product p" +
                            " where p.Id = od.IdProduct and o.id =" + IdOrder;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var receiption = new WarehouseReceiption()
                {
                    Name = "Head Quater",
                    Phone = "",
                    Address = "",
                    Description = "Nhập từ đơn hàng " + order.Code,
                    Code = Getcode_receiption(true, order.IdBranch.Value),
                    Status = true,
                    Type = true,
                    Enable = true,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = order.IdBranch,
                    Credit = 0,
                    Debit = 0,
                    DateCreate = DateTime.Now,
                };
                db.WarehouseReceiptions.Add(receiption);
                db.SaveChanges();
                int ReceiptionId = receiption.Id;
                while (reader.Read())
                {
                    int soluong = int.Parse(reader["Soluong"].ToString());
                    decimal dongia = Decimal.Parse(reader["Price"].ToString());
                    decimal thanhtien = decimal.Parse(reader["TotalAmount"].ToString());

                    var product = new ProductReceiptionDetail()
                    {
                        IdProduct = int.Parse(reader["IdProduct"].ToString()),
                        IdReceiption = ReceiptionId,
                        Amount = soluong,
                        Price = dongia,
                        TotalAmount = thanhtien,
                        Discount = 0,
                        Type = true
                    };
                    db.ProductReceiptionDetails.Add(product);
                    db.SaveChanges();
                }
                reader.Close();
            }
        }
        //Update cộng kho khi hủy
        public void Update_inventoryHQ(int IdOrder)
        {
            int idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id;
            var order = db.Orders.Find(IdOrder);
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select o.IdBranch,o.Id as IdOrder,o.Code,od.IdProduct,p.Name,p.NumberOfPackage,od.Amount,(od.Amount * p.NumberOfPackage) as Soluong,p.Price,od.TotalAmount" +
                            " from [Order] o inner join OrderDetail od on o.Id = od.IdOrder,Product p" +
                            " where p.Id = od.IdProduct and o.id =" + IdOrder;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                var receiption = new WarehouseReceiption()
                {
                    Name = "Head Quater",
                    Phone = "",
                    Address = "",
                    Description = "Nhập từ đơn hàng hủy " + order.Code,
                    Code = Getcode_receiption(true, idbranch_hq),
                    Status = true,
                    Type = true,
                    Enable = true,
                    IdUser = int.Parse(CheckUsers.iduser()),
                    IdBranch = idbranch_hq,
                    Credit = 0,
                    Debit = 0,
                    DateCreate = DateTime.Now,
                };
                db.WarehouseReceiptions.Add(receiption);
                db.SaveChanges();
                int ReceiptionId = receiption.Id;
                while (reader.Read())
                {
                    int soluong = int.Parse(reader["Soluong"].ToString());
                    decimal dongia = Decimal.Parse(reader["Price"].ToString());
                    decimal thanhtien = decimal.Parse(reader["TotalAmount"].ToString());    

                    var product = new ProductReceiptionDetail()
                    {
                        IdProduct = int.Parse(reader["IdProduct"].ToString()),
                        IdReceiption = ReceiptionId,
                        Amount = soluong,
                        Price = dongia,
                        TotalAmount = thanhtien,
                        Discount = 0,
                        Type = true
                    };
                    db.ProductReceiptionDetails.Add(product);
                    db.SaveChanges();
                }
                reader.Close();
            }
        }

        public ActionResult PrintOrder(int IdOrder)
        {
            ViewBag.IdOrder = IdOrder;
            return View();
        }
        public ActionResult Loadlist_PrintOrder(int id) 
        {
            string str = "",strCode="",strTongtien="";
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select p.Code as CodeProduct,p.Name,p.Unit,p.Image,od.Amount,od.Price,od.TotalAmount,o.Code as CodeOrder,o.Address,o.Phone,o.Description,o.Status,us.Name as Username,o.DateCreate,(select sum(TotalAmount) from OrderDetail where IdOrder=o.Id) as tongtien "
                                    + " from OrderDetail od inner join [Order] o on o.id = od.IdOrder, Product p,[User] us"
                                    + " where p.id=od.IdProduct and us.Id=o.IdUser and o.Id=" + id;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                double tongtien = 0;
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    double dongia = Double.Parse(reader["Price"].ToString());
                    double thanhtien = Double.Parse(reader["TotalAmount"].ToString());
                    tongtien += thanhtien;
                    str += "<tr>"
                            + "<td class='text-center'>" + count + "</td>"
                            + "<td class=''>" + reader["Name"].ToString() + "</td>"
                            + "<td class='text-center'>" + reader["Unit"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0}", dongia) + "</td>"
                            + "<td class='text-center'>" + reader["Amount"].ToString() + "</td>"
                            + "<td class='text-end'>" + string.Format("{0:N0}", thanhtien) + "</td>"
                           + "</tr>";
                    strCode = reader["CodeOrder"].ToString();
                }
                str += "<tr><td colspan=5 class='text-end'>Tổng tiền: </td><td class='text-end'>" + string.Format("{0:N0}", tongtien) + "</td></tr>";
                strTongtien = string.Format("{0:N0}", tongtien);
                reader.Close();
            }
            var order = db.Orders.Find(id);
            string strCoso = order.Branch.Name;
            string strTen =order.User.Name;
            string strDiachi=order.Address.ToString();
            string strdienthoai = order.Phone.ToString();
            string strDate = order.DateCreate.Value.ToString("dd/MM/yyyy");
            var item = new { 
                str, strCode,strTongtien,
                strCoso, strTen, strDiachi,strdienthoai,strDate
            };
            return Json(item,JsonRequestBehavior.AllowGet); 
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
        public ActionResult Load_Statistics(DateTime Fromdate,DateTime Todate,int IdBranch)
        {
            string str = "";
            int IdBranchHQ = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq"&&x.Enable==true).Id;
            var list=db.Orders.Where(x=>x.DateCreate>=Fromdate&&x.DateCreate<=Todate);
            if(IdBranch != IdBranchHQ) //Khác HQ
            {
                list= list.Where(x=>x.IdBranch==IdBranch);
            }
            int count=0;
            foreach (var i in list)
            {
                count++;
                string trangthai="";
                switch ((int)i.Status)
                {
                    case 0:
                        trangthai = "Đơn hàng hủy";
                        break;
                    case 1:
                        trangthai = "Đơn hàng mới";
                        break;
                    case 2:
                        trangthai = "Đơn hàng thanh toán";
                        break;
                    case 3:
                        trangthai = "Đơn hàng đã xác nhận";
                        break;
                    case 4:
                        trangthai = "Đơn hàng đóng gói";
                        break;
                    case 5:
                        trangthai = "Đơn hàng đang giao";
                        break;
                    case 6:
                        trangthai = "Đơn hàng hoàn thành";
                        break;
                }
                var sum = db.OrderDetails.Where(x => x.IdOrder == i.Id).Sum(x=>x.TotalAmount);
                str += "<tr>"
                    +"<td class='text-center'>" +count+"</td>"
                    +"<td>"+i.Code+"</td>"
                    +"<td>"+i.Branch.Name+"</td>"
                    +"<td class='text-center'>"+i.DateCreate.Value.ToString("dd/MM/yyyy")+"</td>"
                    +"<td class='text-center'>"+trangthai+"</td>"
                    + "<td class='text-end'>"+string.Format("{0:N0} đ",sum)+"</td>"
                    + "</tr>";
            }
            var item = new { 
                str
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DetailStatistics()
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
        public ActionResult Load_DetailStatistics(DateTime Fromdate, DateTime Todate, int IdBranch)
        {
            string str = "";
            string querycn = "";
            int IdBranchHQ = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq" && x.Enable == true).Id;
            var list = db.Orders.Where(x => x.DateCreate >= Fromdate && x.DateCreate <= Todate);

            if (IdBranch != IdBranchHQ) //Khác HQ
            {
                querycn = " and o.IdBranch=" + IdBranch;
            }
            int count = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["ModelDbContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "select o.Code,p.Name as NameProduct,od.Price,p.Unit,od.Amount,od.TotalAmount,b.Name as NameBranch,o.DateCreate,o.Status"
                                    + " from[Order] o"
                                    + " join OrderDetail od on od.IdOrder = o.Id"
                                    + " join Product p on p.Id = od.IdProduct"
                                    + " join Branch b on b.Id = o.IdBranch"
                                    + " where o.DateCreate between ' " + Fromdate + " ' and  ' " + Todate + " ' " + querycn;
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    count++;
                    str += "<tr>"
                        + "<td class='text-center'>" + count + "</td>"
                        + "<td class='text-center'>" + reader["Code"] + "</td>"
                        + "<td class='text-center'>" + reader["NameProduct"] + "</td>"
                        + "<td class='text-center'>" + reader["Unit"] + "</td>"
                        + "<td class='text-end'>" + reader["Price"] + "</td>"
                        + "<td class='text-center'>" + reader["Amount"] + "</td>"
                        + "<td class='text-end'>" + reader["TotalAmount"] + "</td>"
                        + "<td class='text-center'>" + reader["NameBranch"] + "</td>"
                        + "<td class='text-center'>" + DateTime.Parse(reader["DateCreate"].ToString()).ToString("dd/MM/yyyy") + "</td>"
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
    }
}