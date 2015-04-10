using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Reflection;

namespace LiveSupport.Controllers
{
    public class HomeController : Controller
    {
        [Sitemap]
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult Company()
        {
            return View();
        }

        [Sitemap]
        public ActionResult About()
        {
            return View();
        }

        public ActionResult ToS()
        {
            return View();
        }

        [Sitemap]
        public ActionResult FAQ()
        {
            return View();
        }

        [Sitemap]
        public ActionResult Features()
        {
            return View();
        }

        public ContentResult Sitemap()
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            const string url = "http://www.moplig.com/{0}/{1}/{2}";
            const string nonlocalized_url = "http://www.moplig.com/{0}/{1}";
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => t.IsClass == true && t.BaseType == typeof(System.Web.Mvc.Controller));
            IEnumerable<MethodInfo> methods = types.SelectMany(type => type.GetMethods());

            var cultureList = Enum.GetNames(typeof(Culture)).Cast<string>().ToList();
            // Adds empty culture setting if needed - e.g. http://moplig.com/Home/Index
            // cultureList.Insert(0,"");
                
            // containing Sitemap attribute and not containing Authorize attribute
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from i in methods
                    from culture in cultureList
                    where i.GetCustomAttributes(typeof(SitemapAttribute), true).Length > 0
                    && i.GetCustomAttributes(typeof(AuthorizeAttribute), true).Length == 0 
                    select
                        //Add ns to every element.
                    new XElement(ns + "url",
                      new XElement(ns + "loc",
                          string.Format(url, culture, i.DeclaringType.Name.Replace("Controller", ""), i.Name == "Index"? "": i.Name)),
                          new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                      new XElement(ns + "changefreq", "monthly"),
                      new XElement(ns + "priority", "0.5")
                      )
                    )
                  );
            return Content(sitemap.ToString(), "text/xml");

        }
    }
}
