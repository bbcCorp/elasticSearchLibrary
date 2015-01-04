using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace elasticSearchLibrary.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "404",
                "404",
                new { controller = "Commons", action = "HttpStatus404" }
            );

            routes.MapRoute(
                name: "Library",
                url: "library/{action}/{id}",
                defaults: new { controller = "Book", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Search",
                url: "search/{action}",
                defaults: new { controller = "Search", action = "Index", q = UrlParameter.Optional, filter = UrlParameter.Optional }

            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Search", action = "Index" }
            );
        }
    }
}
