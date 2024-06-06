using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SuperbrainManagement
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            // Thêm route cho Payment
            routes.MapRoute(
            name: "Payment",
            url: "payment/{action}/{id}",
            defaults: new { controller = "Payment", action = "Index", id = UrlParameter.Optional }
        );
        }
    }
}
