using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using SuperbrainManagement.Models;

namespace SuperbrainManagement.Controllers
{
    public class BranchesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Branches
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Loadlist(string searchString)
        {
            var configTimePayment = db.Configurations.OrderByDescending(x => x.Id).FirstOrDefault().TimePayment;
            string str = "";
            var cum = db.BranchGroups.Where(x=>x.Enable==true).ToList();
            int count = 0;
            int countcum = 0;
            foreach(var c in cum)
            {
                countcum++;
                str += "<tr>" 
                    +"<td  class='text-success fw-bolder text-center'>"+countcum+"</td>" 
                    +"<td colspan=7 class='text-success fw-bolder'>"+c.Name+"" 
                        +"<a href=\"/productcategories/edit/" + c.Id + "\" class=\"ms-2\"><i class=\"ti ti-edit text-primary fw-bolder\"></i></a>"
                        + "<a href=\"javascript:Delete_category(" + c.Id + ")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>"
                    +"</td>" 
                    +"</tr>";
                var cn = db.Branches.Where(x => x.IdGroup == c.Id);
                if (!string.IsNullOrEmpty(searchString))
                {
                    cn = cn.Where(x => x.Name.Contains(searchString) || x.Code.Contains(searchString));
                }
                foreach(var cn2 in cn)
                {
                    count++;
                    str += "<tr>" +
                        "<td class='text-center align-content-center'>"+count+"</td>" +
                        "<td class='text-center align-content-center'>" + cn2.Code+"</td>" +
                        "<td class='text-left align-content-center'>" + cn2.Name+"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.Phone != null ? cn2.Phone : "-") +"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.Email != null ? cn2.Email : "-") +"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.ContractExpire != null ? cn2.ContractExpire.Value.ToString("dd/MM/yyyy") : "-") +"</td>" +
                        "<td class='text-center align-content-center'>" + (cn2.StatusActiveOnline==true?"<span class='text-success'>Đã thanh toán</span>":"<span class='text-danger'>Chưa thanh toán</span>") + "</td>" +
                        "<td class='text-end align-content-center'>" +
                        "<a href='/branches/edit/"+cn2.Id+"'><i class='ti ti-edit text-primary'></i></a>" +
                        "<a href='javascript:Delete_branches("+cn2.Id+")' class='ms-1'><i class='ti ti-trash text-danger'></i></a>" +
                        "</td>" +
                        "</tr>";
                }
            }
            var item = new {
                str
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }

        int count_tinhphi(int idStudent)
        {
            var config = db.Configurations.OrderByDescending(x=>x.Id).FirstOrDefault().TimePayment;
            var join = db.StudentJoinClasses.Where(x => x.IdStudent == idStudent && x.Todate >= DateTime.Now).ToList();
            return join.Count;
        }
        int count_student(int idBranch) {
            var join = db.StudentJoinClasses.Where(x=>x.Student.Branch.Id == idBranch && x.Todate>=DateTime.Now).ToList();
            return join.Count;
        }
        public ActionResult List()
        {
            return View();
        }
        public ActionResult PaymentList()
        {
            // Lấy danh sách cấu hình và chuẩn hóa dữ liệu
            var config = db.Configurations
                           .OrderByDescending(x => x.Id).ToList();
            var list = new List<SelectListItem>();
            foreach(var item in config)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.TimePayment.Value.ToString("dd/MM/yyyy"),
                    Text = item.TimePayment.Value.AddMonths(1).ToString("MM/yyyy")
                });
            }
            // Gán ViewBag.Month với dữ liệu hoặc thông báo lỗi
            if (!config.Any())
            {
                ViewBag.Month = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Text = "Không tìm thấy dữ liệu", Value = "" }
                });
            }
            else
            {
                ViewBag.Month = new SelectList(list, "Value", "Text");
            }

            // Gán ViewBag.IdBranch với các chi nhánh đang hoạt động
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x => x.Enable == true), "Id", "Name");
            return View();
        }
        public ActionResult Loadlist_paymentList(int? IdBranch, DateTime? toDate)
        {
            // Khởi tạo chuỗi HTML
            string str = "";
            int stt = 0;

            // Gán mặc định IdBranch nếu không có giá trị
            if (IdBranch == null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }

            // Trả về lỗi nếu không có toDate
            if (toDate == null)
            {
                return Json(new { status = "error", message = "Lỗi cập nhật: Không tìm thấy thông tin kỳ phí!", str }, JsonRequestBehavior.AllowGet);
            }

            // Tính kỳ phí và khoảng thời gian lọc
            string kiphi = toDate.Value.AddMonths(1).ToString("MM/yyyy");
            DateTime fromDate = toDate.Value.AddMonths(-1);

            // Truy vấn danh sách học viên theo điều kiện
            var list = from hs in db.Students
                       join joinclass in db.StudentJoinClasses on hs.Id equals joinclass.IdStudent
                       join course in db.Courses on joinclass.IdCourse equals course.Id
                       where joinclass.Fromdate >= fromDate
                             && joinclass.Fromdate < toDate
                             && hs.IdBranch == IdBranch
                       select new
                       {
                           Name = hs.Name,
                           Code = hs.Code,
                           NameCourse = course.Name,
                           Fromdate = joinclass.Fromdate,
                           Todate = joinclass.Todate,
                           Updatetime = joinclass.DateCreate
                       };

            // Kiểm tra nếu không có dữ liệu
            if (!list.Any())
            {
                str = $"<tr><td colspan='8' class='text-center'>Không có dữ liệu tính phí của kỳ phí {kiphi}</td></tr>";
            }
            else
            {
                // Tạo chuỗi HTML từ dữ liệu
                foreach (var item in list)
                {
                    stt++;
                    str += "<tr>"
                         + $"<td class='text-center'>{stt}</td>"
                         + $"<td class='text-center'>{item.Code}</td>"
                         + $"<td class='text-start'>{item.Name}</td>"
                         + $"<td class='text-center'>{item.NameCourse}</td>"
                         + $"<td class='text-center'>{item.Fromdate:dd/MM/yyyy}</td>"
                         + $"<td class='text-center'>{item.Todate:dd/MM/yyyy}</td>"
                         + $"<td class='text-center'>{item.Updatetime:dd/MM/yyyy}</td>"
                         + "</tr>";
                }
            }
            str += $"<tr><td colspan=7>Tổng cộng: {stt} tài khoản.</td></tr>";
            // Trả về JSON chứa chuỗi HTML và kỳ phí
            return Json(new { status = "ok", str, kiphi }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Logo,Code,Name,CompanyName,Address,City,District,Ward,Map,Phone,Email,OtherEmail,TaxCode,Description,License,DisplayOrder,ContractNumber,ContractExpire,Enable,Active,DateExpire,StatusUsageBrand,DateExpireOnline,StatusActiveOnline,IdGroup,NumberInGroup,FreeTrainning,DateCreate,Unlock,GrandOpeningDay")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Branches.Add(branch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name", branch.IdGroup);
            return View(branch);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name", branch.IdGroup);
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Logo,Code,Name,CompanyName,Address,City,District,Ward,Map,Phone,Email,OtherEmail,TaxCode,Description,License,DisplayOrder,ContractNumber,ContractExpire,Enable,Active,DateExpire,StatusUsageBrand,DateExpireOnline,StatusActiveOnline,IdGroup,NumberInGroup,FreeTrainning,DateCreate,Unlock,GrandOpeningDay")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdGroup = new SelectList(db.BranchGroups, "Id", "Name", branch.IdGroup);
            return View(branch);
        }

    }
}
