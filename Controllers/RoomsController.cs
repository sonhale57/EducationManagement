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
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace SuperbrainManagement.Controllers
{
    public class RoomsController : Controller
    {
        private ModelDbContext db = new ModelDbContext();

        // GET: Rooms
        public ActionResult Index(string idBranch)
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
            var branches = db.Branches.Where(x => x.Enable == true).ToList();
            int idbranch = int.Parse(CheckUsers.idBranch());
            if (!CheckUsers.CheckHQ())
            {
                branches = db.Branches.Where(x => x.Id == idbranch).ToList();
            }
            ViewBag.IdBranch = new SelectList(branches, "Id", "Name", idBranch);

            return View();
        }
        public ActionResult Loadlist(int? IdBranch,string searchString) {
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            if (IdBranch == null)
            {
                IdBranch = idbranch;
            }

            string str = "";
            int stt = 0;
            var rooms = db.Rooms.Where(x => x.IdBranch == IdBranch);
            if (!string.IsNullOrEmpty(searchString))
            {
                rooms = rooms.Where(x => x.Name.Contains(searchString));
            }
            foreach (var c in rooms)
            {
                stt++;
                str += "<tr>"
                    + "<td class='text-center align-content-center'>" + stt + "</td>"
                    + "<td class='align-content-center'>" + c.Name + "</td>"
                    + "<td class='align-content-center'>" + c.Description + "</td>"
                    + "<td class='align-content-center text-center'><span class='btn btn-sm btn-outline-success'>" + db.Schedules.Where(x=>x.IdRoom==c.Id && x.Active==true).DistinctBy(x=>x.IdClass).Count() + "</span></td>"
                    + "<td class='align-content-center text-center'>" + c.User.Name + "</td>"
                    + "<td class='text-end align-content-center'>"
                    + "<a href=\"javascript:Edit_room(" + c.Id + ")\" class=\"me-1\"><i class=\"ti ti-edit text-primary\"></i></a>"
                    +"<a href=\"javascript:Delete_room(" + c.Id + ")\" class=\"me-1\"><i class=\"ti ti-trash text-danger\"></i></a>" 
                    + "</td>"
                    + "</tr>";
            }

            return Json(new { str }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Loadedit_room(int id)
        {
            var c = db.Rooms.Find(id);
            return Json(new { id = c.Id,code=c.Code, name = c.Name, description = c.Description }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_savechanges(string action, int? Id,string Code, string Name, string Description)
        {
            int iduser = Convert.ToInt32(CheckUsers.iduser());
            int idbranch = Convert.ToInt32(CheckUsers.idBranch());
            if (action == "create")
            {
                Room cla = new Room()
                {
                    Name = Name,
                    Code = Code,
                    Description = Description,
                    DateCreate = DateTime.Now,
                    IdBranch = idbranch,
                    IdUser = iduser
                };
                db.Rooms.Add(cla);
                db.SaveChanges();
                return Json(new { status="ok", message="Thành công: Đã thêm phòng học thành công!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var c = db.Rooms.Find(Id);
                if(c == null)
                {
                    return Json(new { status = "error", message = "Lỗi cập nhật: Không tìm thấy phòng học!" }, JsonRequestBehavior.AllowGet);
                }
                c.Name = Name;
                c.Code = Code;
                c.Description = Description;
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { status = "ok", message = "Thành công: Đã cập nhật phòng học thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.DateCreate = DateTime.Now;
                room.IdUser = int.Parse(CheckUsers.iduser());
                room.IdBranch = int.Parse(CheckUsers.idBranch());
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", room.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", room.IdUser);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,IdBranch,DateCreate,IdUser")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdBranch = new SelectList(db.Branches, "Id", "Logo", room.IdBranch);
            ViewBag.IdUser = new SelectList(db.Users, "Id", "Name", room.IdUser);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Delete_Room(int id)
        {
            var room = await db.Rooms.FindAsync(id);
            if (room == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra khóa ngoại
            var hasSchedules = db.Schedules.Any(s => s.IdRoom == id);

            if (hasSchedules)
            {
                return Json(new { status = "error", message = "Không thể xóa phòng này vì đang có lớp đang xét vào phòng này." }, JsonRequestBehavior.AllowGet);
            }

            db.Rooms.Remove(room);
            await db.SaveChangesAsync();

            return Json(new { status = "ok", message = "Thành công: Đã xóa phòng học thành công!" }, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
