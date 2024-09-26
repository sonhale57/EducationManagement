using Microsoft.EntityFrameworkCore.Diagnostics;
using Org.BouncyCastle.Asn1.Ocsp;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace SuperbrainManagement.Controllers
{
    public class CheckUsers
    {
        public ModelDbContext db = new ModelDbContext();
        public static bool checkcookielogin()
        {
            try
            {
                var checkCookie = System.Web.HttpContext.Current.Request.Cookies["check"];
                if (checkCookie == null)
                {
                    return false;
                }

                string cookie = checkCookie["login"];
                string iduser = checkCookie["iduser"];

                if (string.IsNullOrEmpty(cookie) || string.IsNullOrEmpty(iduser))
                {
                    return false;
                }

                string ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                MD5Hash md5h = new MD5Hash();
                String check = md5h.Decrypt(cookie);
                iduser = md5h.Decrypt(iduser);
                if (check != (("Yvaphatco" + iduser)))
                {
                    return false;
                }
                else
                { return true; }
            }
            catch { return false; }
        }
        /// <summary>
        /// Lấy IdUser đăng nhập
        /// </summary>
        /// <returns></returns>
        public static string iduser()
        {
            try
            {
                MD5Hash md5 = new MD5Hash();
                string idlogin = "";
                if (System.Web.HttpContext.Current.Request.Cookies["check"] != null && System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"] != null)
                {
                    idlogin = md5.Decrypt(System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString());
                }
                return idlogin;
            }
            catch { return ""; }
        }

        /// <summary>
        /// Lấy IdUser đăng nhập
        /// </summary>
        /// <returns></returns>
        public static string idBranch()
        {
            try
            {
                MD5Hash md5 = new MD5Hash();
                string idUser = md5.Decrypt(System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString());
                if (idUser == "")
                {
                    return "0";
                }
                else
                {
                    ModelDbContext db = new ModelDbContext();
                    var us = db.Users.Find(int.Parse(idUser));
                    return us.IdBranch.ToString();
                }
            }
            catch { return "0"; }
        }

        public static bool CheckHQ()
        {
            try
            {
                MD5Hash md5 = new MD5Hash();
                string idUser = md5.Decrypt(System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString());
                if (idUser == "")
                {
                    return false;
                }
                else
                {
                    ModelDbContext db = new ModelDbContext();
                    var us = db.Users.Find(int.Parse(idUser));
                    if (us.Branch.Code.ToLower() == "hq")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch { return false; }
        }
        public static string CodeBranch()
        {
            try
            {
                MD5Hash md5 = new MD5Hash();
                string idUser = md5.Decrypt(System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString());
                if (idUser == "")
                {
                    return "";
                }
                else
                {
                    ModelDbContext db = new ModelDbContext();
                    var us = db.Users.Find(int.Parse(idUser));
                    return us.Branch.Code;
                }
            }
            catch { return ""; }
        }
        /// <summary>
        /// Code check phân quyền của user
        /// </summary>
        public static string checkRole(int IdpermissionCategory)
        {
            try
            {
                MD5Hash md5 = new MD5Hash();
                string iduser;
                if (System.Web.HttpContext.Current.Request.Cookies["check"] != null && System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"] != null)
                {
                    iduser = System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString();

                    iduser = md5.Decrypt(iduser.ToString());
                    List<Permission> permission = Connect.Select<Permission>("select * from Permission per where per.IdPermissionCategory = '" + IdpermissionCategory + "'");
                    if (permission != null)
                    {
                        foreach (Permission permissiones in permission)
                        {
                            List<UserPermission> userpermission = Connect.Select<UserPermission>("select * from UserPermission where IdPermission = '" + permissiones.Id + "' and IdUser = '" + iduser + "' ");
                            foreach (UserPermission user in userpermission)
                            {
                                if (user.IsRead == true || user.IsEdit == true || user.IsCreate == true || user.IsDelete == true)
                                {
                                    return "";
                                }
                            }
                        }
                    }
                    return "hideof"; // Trả về chuỗi "hideof" nếu không có quyền truy cập
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// Code check phân quyền của user
        /// </summary>
        public static string CheckHQ_Css()
        {
            ModelDbContext db = new ModelDbContext();
            int idbranch_hq = db.Branches.SingleOrDefault(x => x.Code.ToLower() == "hq").Id;
            try
            {
                MD5Hash md5 = new MD5Hash();
                var cookie = System.Web.HttpContext.Current?.Request.Cookies["check"];
                if (cookie != null)
                {
                    string iduser = cookie["iduser"]?.ToString();
                    if (!string.IsNullOrEmpty(iduser))
                    {
                        var idbranch_user = db.Users.Find(int.Parse(iduser)).IdBranch;
                        if (idbranch_user == idbranch_hq)
                        {
                            return "";
                        }
                        return "hideof"; // Trả về chuỗi "hideof" nếu không có quyền truy cập
                    }
                }
            }
            catch
            {
                return "";
            }

            return "";
        }

        public static string CheckPermission(string code, int role)
        {
            using (ModelDbContext db = new ModelDbContext())
            {
                try
                {
                    MD5Hash md5 = new MD5Hash();
                    string encryptedUserId = "";

                    // Kiểm tra cookie để lấy ID người dùng đã mã hóa
                    var userCookie = System.Web.HttpContext.Current.Request.Cookies["check"];
                    if (userCookie != null && userCookie["iduser"] != null)
                    {
                        encryptedUserId = md5.Decrypt(userCookie["iduser"].ToString());
                    }

                    if (string.IsNullOrEmpty(encryptedUserId)) { return "hideof"; }

                    int userId = Convert.ToInt32(encryptedUserId);
                    var permission = db.Permissions.FirstOrDefault(x => x.Code.ToLower() == code.ToLower());
                    if (permission == null) { return "hideof"; }

                    var userPermission = db.UserPermissions.FirstOrDefault(x => x.IdUser == userId && x.IdPermission == permission.Id);
                    if (userPermission == null) { return "hideof"; }

                    // Kiểm tra quyền theo vai trò
                    switch (role)
                    {
                        case 1:
                            if (!(bool)userPermission.IsRead) { return "hideof"; }
                            break;
                        case 2:
                            if (!(bool)userPermission.IsCreate) { return "hideof"; }
                            break;
                        case 3:
                            if (!(bool)userPermission.IsEdit) { return "hideof"; }
                            break;
                        case 4:
                            if (!(bool)userPermission.IsDelete) { return "hideof"; }
                            break;
                        default:
                            return "hideof";
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    // Log exception nếu cần thiết
                    return "";
                }
            }
        }


    }
}