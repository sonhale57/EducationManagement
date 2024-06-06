using SuperbrainManagement.Helpers;
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
    public class DonhangController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        private readonly VNPayHelper _vnPayHelper = new VNPayHelper();
        // GET: Donhang
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
        public ActionResult Payment(int orderId) 
        {
            var o = db.Orders.Find(orderId);
            var od = db.OrderDetails.Where(x => x.IdOrder==orderId);
            decimal tongtien = od.Sum(x => x.TotalAmount).Value;
            ViewBag.orderId = orderId;
            ViewBag.tongtien = tongtien;
            ViewBag.mota = "Đơn hàng " + o.Code;
            return View();
        }
      
        public ActionResult Loadlist(string idBranch, int status, string sort, string searchString)
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
                queryBranch = "and o.idbranch = ' " + idBranch + " ' ";
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
                            + " where o.IdBranch = cs.Id and us.id=o.IdUser and o.Status='" + status + "'" + queryBranch + "  order by o.Id desc";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                int count = 0;
                while (reader.Read())
                {
                    count++;
                    string badgeStatus = "";
                    string btnPayment = "";
                    if (reader["status"].ToString() == "1")
                    {
                        badgeStatus = "<span class='badge text-success bg-light'>Đơn hàng mới</span>";
                        btnPayment = "<a href='/payment/payment?orderId=" + reader["Id"] +"' class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-credit-card\"></i></a>";
                    }
                    else if (reader["status"].ToString() == "2")
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
                                    + "<div class=\"d-flex w-100 justify-content-between\">"
                                        + "<p class=\"mb-1\"><i class=\"ti ti-shopping-cart\"></i> <a href='javascript:Status_Order(" + reader["Id"] + ")' class='text-dark fw-bolder'>" + reader["Code"] + "</a> " + badgeStatus + "</p>"
                                        + "<div>"
                                            + btnPayment
                                            + "<a href=\"#\" class=\"btn btn-danger btn-sm ms-1\"><i class=\"ti ti-trash\"></i></a>"
                                            + "<a href=\"#\" class=\"btn btn-success btn-sm ms-1\"><i class=\"ti ti-printer\"></i></a>"
                                        + "</div>"
                                    + "</div>"
                                    + "<p class=\"mb-1\">Cơ sở: <b>" + reader["TenCoSo"] + "</b></p>"
                                     + "<div class=\"d-flex w-100 justify-content-between\">"
                                    + "<p class=\"mb-1\">Người đặt: <b>" + reader["Username"] + "</b></p>"
                                    + "<small>" + DateTime.Parse(reader["DateCreate"].ToString()) + "</small>"
                                    + "</div>"
                                    + (reader["Phone"].ToString() == "" ? "" : "<p class=\"mb-1\">Số điện thoại: <b>" + reader["Phone"] + "</b></p>")
                                    + (reader["Address"].ToString() == "" ? "" : "<p class=\"mb-1\">Địa chỉ giao hàng: <b>" + reader["Address"] + "</b></p>")
                                    + "<p class=\"mb-1\">Tổng tiền: <b>" + reader["tongtien"] + "</b></p>"
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
    }
}