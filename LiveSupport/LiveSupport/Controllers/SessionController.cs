using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Data;


namespace LiveSupport.Controllers
{
    [Authorize(Roles="Supervisor, Operator")]
    public class SessionController : Controller
    {
        MopligEntities db;

        public SessionController()
        {
            db = new MopligEntities();
        }
        
        //
        // GET: /Session/

        [HttpGet]
        public ActionResult Index(Guid supporter)
        {
            int currentPage = 1;
            int pageSize = 5;

            ViewData["currentPage"] = currentPage;
            ViewData["pageSize"] = pageSize;

            return Index(supporter, currentPage, pageSize);

        }

        [HttpPost]
        public ActionResult Index(Guid supporter, int currentPage = 1, int pageSize = 10)
        {
            var supporter_sessions = from i in db.Sessions
                                     where i.UserID == supporter
                                     orderby i.StartDate descending
                                     select i;

            int totalRows = supporter_sessions.Count();
            ViewData["previousPage"] = currentPage - 1;
            ViewData["currentPage"] = currentPage;
            ViewData["nextPage"] = currentPage + 1;
            ViewData["lastPage"] = (int)Math.Ceiling((double)totalRows / (double)pageSize); ;

            ViewData.Model = supporter_sessions.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return View();
        }

        //
        // GET: /Session/Details/5

        public ActionResult Details(int id)
        {
            var session_messages = from i in db.Messages
                                   where i.SessionID == id
                                   select i;
            var session = (from i in db.Sessions
                              where i.ID == id
                              select i).FirstOrDefault();
            ViewBag.Session = session;

            ViewData.Model = session_messages.ToList();
            return View();
        }

        public static SelectList PageSizeSelectList()
        {
            var pageSizes = new List<string> {"5", "10", "20", "50", "100" };
            return new SelectList(pageSizes, "value");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
