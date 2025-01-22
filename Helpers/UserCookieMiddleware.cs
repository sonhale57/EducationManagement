using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SuperbrainManagement.Helpers
{
    public class UserCookieMiddleware : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies["iduser"];
            if (cookie == null) // Nếu cookie không tồn tại
            {
                filterContext.Result = new RedirectResult("~/Authentication"); // Redirect đến trang đăng nhập
            }

            base.OnActionExecuting(filterContext);
        }
    }
}