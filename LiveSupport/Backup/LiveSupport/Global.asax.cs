using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LiveSupport
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*path}", new { path = @"blog\/(.*)" });
            routes.MapRoute(
                        "Sitemap",
                        "sitemap.xml",
                        new { controller = "Home", action = "SiteMap" }
                        ).RouteHandler = new SingleCultureMvcRouteHandler();

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute(
                      "LogOnRoute",
                      "Account/LogOn",
                      new { controller = "Account", action = "LogOn" }
               ).RouteHandler = new SingleCultureMvcRouteHandler();


            foreach (Route r in routes)
            {
                if (!(r.RouteHandler is SingleCultureMvcRouteHandler))
                {
                    r.RouteHandler = new MultiCultureMvcRouteHandler();
                    r.Url = "{culture}/" + r.Url;
                    //Adding default culture 
                    if (r.Defaults == null)
                    {
                        r.Defaults = new RouteValueDictionary();
                    }
                    r.Defaults.Add("culture", Culture.en.ToString());

                    //Adding constraint for culture param
                    if (r.Constraints == null)
                    {
                        r.Constraints = new RouteValueDictionary();
                    }
                    r.Constraints.Add("culture", new CultureConstraint(Culture.en.ToString(), Culture.de.ToString(),Culture.bg.ToString()));
                }
            }

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

        }
    }
}