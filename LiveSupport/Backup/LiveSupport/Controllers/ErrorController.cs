using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LiveSupport.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult HttpError()
        {
            return View();
        }

        public ActionResult Http404()
        {
            return View();
        }

        public ActionResult Http500()
        {
            return View();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}
