using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Controllers
{
    public class AuthenticationController : Controller
    {
        ModelDbContext db = new ModelDbContext();
        // GET: Authentication
        public ActionResult Index()
        {
            if (CheckUsers.checkcookielogin())
            {
                return Redirect("/");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(string username ,string password)
        {
            MD5Hash md5 = new MD5Hash();
            HttpCookie login = new HttpCookie("check");
            string ip = Request.ServerVariables["REMOTE_ADDR"];
            string domain = Request.Url.GetLeftPart(UriPartial.Authority);
            password =md5.GetMD5Working(password);
            string pass = md5.mahoamd5(password.Replace("&^%$", ""));

            var user = db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
            {
                TempData["error"] = "<div class=\"alert alert-danger\" role=\"alert\">Không tìm thấy tài khoản này!</div>";
                return View();
            }

            if (user.Password != pass)
            {
                TempData["error"] = "<div class=\"alert alert-danger\" role=\"alert\">Sai mật khẩu!</div>";
                return View();
            }

            if (user.Expire < DateTime.Now)
            {
                TempData["error"] = "<div class=\"alert alert-danger\" role=\"alert\">Tài khoản đã hết hạn!</div>";
                return View();
            }

            if (user.Active==false)
            {
                TempData["error"] = "<div class=\"alert alert-danger\" role=\"alert\">Tài khoản đã bị khóa!</div>";
                return View();
            }

            if (user.Enable==false)
            {
                TempData["error"] = "<div class=\"alert alert-danger\" role=\"alert\">Tài khoản đã bị xóa!</div>";
                return View();
            }
            login["iduser"] = md5.Encrypt(user.Id.ToString());
            login["login"] = md5.Encrypt("Yvaphatco" + user.Id.ToString());
            login["browser"] = Request.Browser.Browser;
            login["IPAddress "] = Request.UserHostAddress;
            login.Expires = DateTime.Now.AddDays(2);
            Response.Cookies.Add(login);
            //LoginLog log = new LoginLog()
            //{
            //    Browser = Request.Browser.Browser,
            //    DateCreate = DateTime.Now,
            //    IPAddress = Request.UserHostAddress,
            //    IdUser = user.Id,
            //    Devide = (Request.Browser.IsMobileDevice ?"Mobile" : "PC")
            //};
            //db.LoginLogs.Add(log);
            //db.SaveChanges();
            return Redirect("/");
        }
        public ActionResult Forgot()
        {
            return View();
        }
        public ActionResult MyAccount()
        {
            return View();
        }
        public ActionResult Profile()
        {
            int IdUser  = int.Parse(CheckUsers.iduser());
            User user = db.Users.Find(IdUser);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            HttpCookie login = new HttpCookie("check");
            login["iduser"] = "";
            login["login"] = "";
            Response.Cookies.Add(login);
            return Redirect("/");
        }
    }
}