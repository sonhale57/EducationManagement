using Microsoft.EntityFrameworkCore.Diagnostics;
using Org.BouncyCastle.Asn1.Ocsp;
using SuperbrainManagement.Models;
using System;
using System.Collections.Generic;
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
                string cookie = System.Web.HttpContext.Current.Request.Cookies["check"]["login"].ToString();
                string iduser = System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString();
                string ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                MD5Hash md5h = new MD5Hash();
                String check = md5h.Decrypt(cookie.ToString());
                iduser = md5h.Decrypt(iduser.ToString());
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
                string idlogin = md5.Decrypt(System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString());
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
                if(idUser == "")
                {
                    return "0";
                }else
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
                string iduser = System.Web.HttpContext.Current.Request.Cookies["check"]["iduser"].ToString();
                if(iduser=="") { return iduser; }
                else
                {
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
            }
            catch
            {
                return "";
            }
        }
    }
}