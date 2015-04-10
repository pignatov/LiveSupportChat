using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Data;

namespace LiveSupport.Controllers
{ 
    [Authorize(Roles="Supervisor")]
    public class ResponseController : Controller
    {
        private MopligEntities db = new MopligEntities();

        //
        // GET: /Response/

        public ViewResult Index()
        {
            var current_company = CompanyBO.GetCurrentCompany();
            var responses = db.Responses.Where(r => r.CompanyID == current_company.ID).Include("Company");
            return View(responses.ToList());
        }

        //
        // GET: /Response/Details/5

        public ViewResult Details(int id)
        {
            Respons respons = db.Responses.Single(r => r.ID == id);
            return View(respons);
        }

        //
        // GET: /Response/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Response/Create

        [HttpPost]
        public ActionResult Create(Respons respons)
        {
            var current_company = CompanyBO.GetCurrentCompany();

            if (ModelState.IsValid)
            {
                respons.CompanyID = current_company.ID;
                db.Responses.AddObject(respons);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(respons);
        }
        
        //
        // GET: /Response/Edit/5
 
        public ActionResult Edit(int id)
        {
            Respons respons = db.Responses.Single(r => r.ID == id);
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "FullName", respons.CompanyID);
            return View(respons);
        }

        //
        // POST: /Response/Edit/5

        [HttpPost]
        public ActionResult Edit(Respons respons)
        {
            if (ModelState.IsValid)
            {
                db.Responses.Attach(respons);
                db.ObjectStateManager.ChangeObjectState(respons, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // ViewBag.CompanyID = new SelectList(db.Companies, "ID", "FullName", respons.CompanyID);
            return View(respons);
        }

        //
        // GET: /Response/Delete/5
 
        public ActionResult Delete(int id)
        {
            Respons respons = db.Responses.Single(r => r.ID == id);
            return View(respons);
        }

        //
        // POST: /Response/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Respons respons = db.Responses.Single(r => r.ID == id);
            db.Responses.DeleteObject(respons);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}