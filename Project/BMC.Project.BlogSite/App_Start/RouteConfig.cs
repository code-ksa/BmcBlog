using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BMC.Project.BlogSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Blog post route
            routes.MapRoute(
                name: "BlogPost",
                url: "blog/{title}",
                defaults: new { controller = "BlogPost", action = "Detail" }
            );

            // Category route
            routes.MapRoute(
                name: "Category",
                url: "category/{name}",
                defaults: new { controller = "Blog", action = "Category" }
            );

            // Blog listing route
            routes.MapRoute(
                name: "Blog",
                url: "blog",
                defaults: new { controller = "Blog", action = "Index" }
            );

            // Search route
            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "Search", action = "Results" }
            );

            // Account routes
            routes.MapRoute(
                name: "Account",
                url: "account/{action}",
                defaults: new { controller = "Account", action = "Login" }
            );

            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
