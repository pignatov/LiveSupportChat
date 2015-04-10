using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Data;

namespace LiveSupport.Controllers
{
    [Authorize(Roles = "Administrator, Operator")]     
    public class SupportController : Controller
    {
        MopligEntities db;

        public SupportController()
        {
            db = new MopligEntities();
        }
        
        //
        // GET: /Support/
        [Authorize(Roles = "Administrator, Operator")]  
        public ActionResult Answer()
        {
            if (LicenseBO.IsLicenseActive())
            {
                var company = CompanyBO.GetCurrentCompany();
                ViewBag.Company = company.CodeName;

                var responses = from i in db.Responses
                                where i.CompanyID == company.ID
                                select i;

                ViewBag.Responses = new SelectList(responses, "Value", "Value");
                ViewBag.Profile = (HttpContext.Profile as UserProfile);
                ViewBag.CompanyName = company.FullName;

                return View();
            }
            else
            {
                return RedirectToAction("LicenseExpired");
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LicenseExpired()
        {
            var license = LicenseBO.GetActiveLicense();
            return View(license);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
