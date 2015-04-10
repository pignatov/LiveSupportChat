using LiveSupport.Data;
using LiveSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

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

            if (!Roles.RoleExists("Administrator")) Roles.CreateRole("Administrator");
            if (!Roles.RoleExists("Supervisor")) Roles.CreateRole("Supervisor");
            if (!Roles.RoleExists("Operator")) Roles.CreateRole("Operator");

            if (Roles.GetUsersInRole("Administrator").Count() == 0)
            {
                // Създаване на администртор
                MembershipCreateStatus createStatus = ( new AccountMembershipService()).CreateUser("Administrator", "123456", "changeMail");
                Roles.AddUserToRole("Administrator", "Administrator");
                using (var db = new MopligEntities())
                {
                    // New company
                    Company company = new Company();
                    company.FullName = "Moplig-Owner";
                    company.CodeName = "moplig-owner";
                    company.RegDate = DateTime.Now;

                    User user = (from i in db.Users
                                 where i.UserName == "Administrator"
                                 select i).FirstOrDefault();
                    user.Company = company;

                    // New paid license
                    License license = new License();
                    license.StartDate = DateTime.Now;
                    license.EndDate = license.StartDate.AddDays(9999);
                    license.MaxOperators = 1;
                    license.TypeOfLicense = (int)License.LicenseType.Regular;
                    license.Company = company;

                    db.AddToLicenses(license);

                    db.SaveChanges();
                }
            }

        }
    }
}