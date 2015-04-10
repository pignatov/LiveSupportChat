using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveSupport.Controllers
{
    public class PricingController : Controller
    {
        //
        // GET: /Pricing/

        [Sitemap]
        public ActionResult Index()
        {
            return View();
        }

    }
}
