using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Data;
using LiveSupport.Areas.Admin.Models;

namespace LiveSupport.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StatsController : Controller
    {
        MopligEntities db;

        public StatsController()
        {
            db = new MopligEntities();
        }
        
        //
        // GET: /Admin/Stats/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Usage()
        {
            IEnumerable<CompanyInfoModel> q = from c in db.Companies
                    select new CompanyInfoModel(){
                           CompanyName = c.FullName,
                           MaxOperators = c.Licenses.OrderByDescending(a=>a.StartDate).FirstOrDefault().MaxOperators
                    };
            IEnumerable<CompanyInfoModel> cims = from c in q
                       select new CompanyInfoModel()
                       {
                           CompanyName = c.CompanyName,
                           MaxOperators = c.MaxOperators,
                           OnlineOperators = LiveSupport.CountAvailableOperators(c.CompanyName),
                           OnlineClients = LiveSupport.CountOnlineClients(c.CompanyName),
                           TotalSessions = (from i in db.Sessions
                                           where i.User.Company.FullName == c.CompanyName
                                           select i).Count()
                           
                       };
            return View(cims);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
