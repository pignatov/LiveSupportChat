using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveSupport.Controllers
{
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QuickStart()
        {
            return View();
        }

    }
}
