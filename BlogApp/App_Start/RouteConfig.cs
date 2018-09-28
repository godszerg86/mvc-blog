using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlogApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Slugs",
                url: "post/{slug}",
                defaults: new { controller = "BlogPosts", action = "DetailsSlug", slug = UrlParameter.Optional }
                );

            routes.MapRoute(
              name: "Create",
              url: "create/",
              defaults: new { controller = "BlogPosts", action = "Create" }
              );

            routes.MapRoute(
                   name: "ContactMe",
                   url: "contact-me/",
                   defaults: new { controller = "BlogPosts", action = "ContactMe" }
                   );


            routes.MapRoute(
                name: "SlugsEdit",
                url: "post/edit/{slug}",
                defaults: new { controller = "BlogPosts", action = "EditSlug", slug = UrlParameter.Optional }
                );

            routes.MapRoute(
            name: "SlugsDelete",
            url: "post/delete/{slug}",
            defaults: new { controller = "BlogPosts", action = "DeleteSlug", slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "BlogPosts", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
