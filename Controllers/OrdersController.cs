using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
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
        public ActionResult Loadlist_Orders()
        {
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}