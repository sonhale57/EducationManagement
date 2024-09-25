using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using Microsoft.EntityFrameworkCore;
using SuperbrainManagement.Helpers;
using EntityState = System.Data.Entity.EntityState;
using System.Data.SqlClient;
using Microsoft.Ajax.Utilities;

namespace SuperbrainManagement.Controllers
{
    public class VacationSchedulesController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        public ActionResult Index()
        {
            ViewBag.IdBranch = new SelectList(db.Branches.Where(x => x.Enable == true), "Id", "Name");
            return View();
        }
        public ActionResult Loadlist(int? IdBranch)
        {
            string str = "";
            int stt = 0;
            if (IdBranch == null)
            {
                IdBranch = Convert.ToInt32(CheckUsers.idBranch());
            }
            var vacation = db.VacationSchedules.Where(x=>x.IdBranch == IdBranch).OrderByDescending(x=>x.Id);
            if (vacation.Count() == 0)
            {
                str = "<tr><td colspan=5>Không có dữ liệu lịch nghỉ</td></tr>";
            }
            foreach (var v in vacation)
            {
                stt++;
                str += "<tr>"
                    +"<td class='text-center'>"+stt+"</td>"
                    +"<td>"+v.Description+"</td>"
                    +"<td class='text-center'>"+v.Fromdate.Value.ToString("dd/MM/yyyy")+"</td>"
                    +"<td class='text-center'>" +v.Todate.Value.ToString("dd/MM/yyyy") +"</td>"
                    +"<td class='text-end'>" +
                    "<a href='javascript:Edit_vacation(" + v.Id+")' class='text-primary'><i class='ti ti-edit'></i> </a>" +
                    "<a href='javascript:Delete_vacation(" + v.Id+ ")' class='text-danger'><i class='ti ti-trash'></i> </a>" +
                    "</td>"
                    + "</tr>";
            }
            return Json(new {str},JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadedit_vacation(int id)
        {
            var c = db.VacationSchedules.Find(id);
            return Json(new { id = c.Id, name = c.Description, fromDate = c.Fromdate.Value.ToString("dd/MM/yyyy"),toDate=c.Todate.Value.ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_savechanges(string action, int? Id, string Name, DateTime Fromdate, DateTime Todate)
        {
            int iduser = Convert.ToInt32(CheckUsers.iduser());
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            string status = "", message = "";
            if (action == "create")
            {
                VacationSchedule vacationSchedule = new VacationSchedule()
                {
                    Description = Name,
                    DateCreate = DateTime.Now,
                    IdBranch = idbranch,
                    IdUser = iduser,
                    Fromdate = Fromdate,
                    Todate = Todate
                };
                db.VacationSchedules.Add(vacationSchedule);

                db.SaveChanges();
                status = "ok";
                message = "Đã thêm lịch nghỉ thành công!";
            }
            else
            {
                if (Id == null)
                {
                    return Json(new { status = "error", message = "Không tìm thấy lịch nghỉ." }, JsonRequestBehavior.AllowGet);
                }
                var vac = db.VacationSchedules.Find(Id);
                vac.Description = Name;
                vac.Fromdate = Fromdate;
                vac.Todate = Todate;
                db.Entry(vac).State = EntityState.Modified;
                db.SaveChanges();
                status = "ok";
                message = "Đã cập nhật lịch nghỉ thành công!";

            }
            var studentClasses = db.StudentJoinClasses.Where(s => s.Fromdate <= Todate && s.Todate >= Fromdate).ToList();

            foreach (var studentClass in studentClasses)
            {
                // Cộng thêm số ngày nghỉ vào lịch của học viên
                int vacationDays = (Todate - Fromdate).Days + 1; // Số ngày nghỉ
                studentClass.Todate = studentClass.Todate.Value.AddDays(vacationDays);
                // Lưu thay đổi
                db.Entry(studentClass).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_vacation(int id) {
            var vacation = db.VacationSchedules.Find(id);
            if (vacation == null)
            {
                return Json(new { status = "error", message = "Không tìm thấy lịch nghỉ!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //kiểm tra 
                var joinClass = db.StudentJoinClasses.Where(x => x.Fromdate > vacation.Fromdate && x.Todate < vacation.Todate);

                db.VacationSchedules.Remove(vacation);
                db.SaveChanges();
                return Json(new { status = "ok", message = "Đã xóa lịch nghỉ này." }, JsonRequestBehavior.AllowGet);
            }    
        }
        
    }
}
