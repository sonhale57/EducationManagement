using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using PagedList.Mvc;
using PagedList;
using SuperbrainManagement.Models;
using System.Threading.Tasks;
using SuperbrainManagement.Helpers;

namespace SuperbrainManagement.Controllers
{
    public class UsersController : Controller
    {
        private ModelDbContext db = new ModelDbContext();
        //CLASS RESPONSE DATA
        public class Innerjoin
        {
            public string Name { get; set; }
            public string code { get; set; }
            public int iduserAccount { get; set; }
            public List<Permissionse> Permissions { get; set; }

        }
        public class Permissionse
        {
            public int id { get; set; }
            public string code { get; set; }
            public string Name { get; set; }
            public bool IsRead { get; set; }
            public bool IsCreate { get; set; }
            public bool IsEdit { get; set; }
            public bool IsDelete { get; set; }
        }
        public class UserpermissionRes
        {
            public int IdPermission { get; set; }
            public int IsRead { get; set; }
            public int IsCreate { get; set; }
            public int IsDelete { get; set; }

            public int IsEdit { get; set; }
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string idBranch)
        {
            if (CheckUsers.iduser() == "")
            {
                return Redirect("/authentication");
            }
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

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var users = db.Users.Where(x=>x.Enable==true).ToList();

            if (!string.IsNullOrEmpty(idBranch))
            {
                users = users.Where(x => x.IdBranch == int.Parse(idBranch)).ToList();
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Username.ToLower().Contains(searchString.ToLower())).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.Name).ToList();
                    break;
                case "date":
                    users = users.OrderBy(s => s.Id).ToList();
                    break;
                case "name":
                    users = users.OrderBy(s => s.Name).ToList();
                    break;
                default:
                    users = users.OrderByDescending(s => s.Id).ToList();
                    break;
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);


            var pagedData = users.ToPagedList(pageNumber, pageSize);

            var pagedListRenderOptions = new PagedListRenderOptions();
            pagedListRenderOptions.FunctionToTransformEachPageLink = (liTag, aTag) =>
            {
                liTag.AddCssClass("page-item");
                aTag.AddCssClass("page-link");
                return liTag;
            };

            ViewBag.PagedListRenderOptions = pagedListRenderOptions;
            return View(pagedData);
        }

        // GET LIST PERMISSION
        [HttpGet]
        public ActionResult GetDataPermissionWithid(int idInput)
        {
            Console.WriteLine(idInput);
            Session["iduseraccount"] = idInput;
            List<UserPermission> userPermissions = Connect.Select<UserPermission>("select * from UserPermission where IdUser = '" + idInput + "'");
            List<Innerjoin> Model = new List<Innerjoin>();

            if (userPermissions.Count > 0)
            {
                Model = LoadDataPermission(idInput);
            }
            else
            {
                List<Permission> permissions = Connect.Select<Permission>("select * from Permission");
                // Create a new UserPermission object and set its properties
                foreach (Permission per in permissions)
                {
                    UserPermission newPermission = new UserPermission
                    {
                        IdUser = idInput, // Assuming IdUser is a property in your UserPermission entity
                        IdPermission = per.Id, // Assuming IdPermission is a property in your UserPermission entity
                        IsRead = false,
                        IsDelete = false,
                        IsCreate = false,
                        IsEdit = false,
                    };
                    // Add the new permission to the DataContext
                    db.UserPermissions.Add(newPermission);
                    db.SaveChanges();
                    // Submit changes to persist the new record to the database
                }
                Model = LoadDataPermission(idInput);
            }
            // Sử dụng model ở đây (ví dụ: lưu vào session, truyền vào view, ...)
            TempData["data"] = Model;
            return Json(Model, JsonRequestBehavior.AllowGet);
        }
        public List<Innerjoin> LoadDataPermission(int idInput)
        {
            List<Innerjoin> Model = new List<Innerjoin>();
            List<PermissionCategory> permissionCategories = Connect.Select<PermissionCategory>("select * from PermissionCategory");
            if (permissionCategories != null)
            {
                foreach (PermissionCategory permissionCategory in permissionCategories)
                {
                    Innerjoin innerjoin = new Innerjoin();
                    innerjoin.Name = permissionCategory.Name;
                    List<Permissionse> permissionseslist = new List<Permissionse>();
                    List<Permission> permissions = Connect.Select<Permission>("select * from Permission where IdPermissionCategory = '" + permissionCategory.Id + "' ");
                    foreach (Permission permission in permissions)
                    {
                        Permissionse permissionse = new Permissionse();
                        UserPermission userPermission = Connect.SelectSingle<UserPermission>("select * from UserPermission where IdUser = '" + idInput + "' and IdPermission = '" + permission.Id + "'");
                        permissionse.IsRead = (bool)userPermission.IsRead;
                        permissionse.IsCreate = (bool)userPermission.IsCreate;
                        permissionse.IsEdit = (bool)userPermission.IsEdit;
                        permissionse.IsDelete = (bool)userPermission.IsDelete;
                        permissionse.code = permission.Code;
                        permissionse.Name = permission.Name;
                        permissionse.id = permission.Id;
                        permissionseslist.Add(permissionse);
                    }

                    innerjoin.Permissions = permissionseslist;
                    Model.Add(innerjoin);
                }
            }
            return Model;
        }
        [HttpPost]
        public ActionResult SaveChange(List<UserpermissionRes> Permissions)
        {
            int id = Convert.ToInt32(Session["iduseraccount"]);
            List<UserPermission> users = Connect.Select<UserPermission>("select * from UserPermission where IdUser = '" + id + "'");
            if (users != null)
            {
                foreach (UserpermissionRes userPermission in Permissions)
                {
                    UserPermission userPermissionUpdate = db.UserPermissions.FirstOrDefault(x => x.IdUser == id && x.IdPermission == userPermission.IdPermission);

                    userPermissionUpdate.IsRead = userPermission.IsRead == 1 ? true : false;
                    userPermissionUpdate.IsDelete = userPermission.IsDelete == 1 ? true : false;
                    userPermissionUpdate.IsCreate = userPermission.IsCreate == 1 ? true : false;
                    userPermissionUpdate.IsEdit = userPermission.IsEdit == 1 ? true : false;
                    db.SaveChanges();
                }
            }
            var item = new { 
                str="ok"
            };
            return Json(item,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Load_infoEdit(int id)
        {
            var e = db.Employees.Find(id);
            string username = "", name = "";
            var us = db.Users.Find(id);
            if (us != null)
            {
                name = us.Name;
                username = us.Username;
            }
            var item = new
            {
                name,
                username
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_EditUser(int Id, string Password)
        {
            string status = "ok";
            string message = "ok";
            if (string.IsNullOrEmpty(Password))
            {
                message = "Không có mật khẩu!";
            }
            else
            {
                MD5Hash md5 = new MD5Hash();
                Password = md5.GetMD5Working(Password);
                string pass = md5.mahoamd5(Password.Replace("&^%$", ""));
                var user = db.Users.Find(Id);
                if (user == null)
                {
                    status = "error";
                    message = "Không tìm thấy User này!";
                }
                else
                {
                    user.Password = pass;
                    db.Entry(user);
                    db.SaveChanges();
                    status="ok";
                    message = "Đã cập nhật thành công!";
                }
            }
            var item = new { status, message };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Delete_Account(int id)
        {
            string status, message;
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                status = "error";
                message = "Không tìm thấy user này!";
                return HttpNotFound();
            }
            user.Enable = false;
            db.Entry(user);
            status = "ok";
            message = "Đã xóa thành công!";
            await db.SaveChangesAsync();
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Extend_Account(int id)
        {
            string status, message;
            var user = db.Users.Find(id);
            if (user == null)
            {
                status = "error";
                message = "Không tìm thấy user này!";
            }
            user.Expire = user.Expire.Value.AddMonths(6);
            db.Entry(user);
            status = "ok";
            message = "Đã gia hạn thành công!";
            db.SaveChanges();
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Submit_ChangePassword(string Password)
        {
            string status, message;
            int id = Convert.ToInt32(CheckUsers.iduser());
            var user = db.Users.Find(id);
            if (user == null)
            {
                status = "error";
                message = "Phiên đăng nhập đã kết thúc, vui lòng đăng nhập lại!";
            }
            MD5Hash md5 = new MD5Hash();
            Password = md5.GetMD5Working(Password);
            string pass = md5.mahoamd5(Password.Replace("&^%$", ""));
            user.Password = pass;
            db.Entry(user);
            status = "ok";
            message = "Đã cập nhật thành công!";
            db.SaveChanges();
            var item = new
            {
                status,
                message
            };
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        // GET: Users
        [HttpPost]
        public ActionResult updateStatus(int id, int status)
        {
            User user = db.Users.Find(id);
            if (user != null)
            {
                user.Active = status == 1;
                db.SaveChanges();
                return Json(user.Active);
            }
            else
            {
                return Json(false); 
            }
        }
    }
}
