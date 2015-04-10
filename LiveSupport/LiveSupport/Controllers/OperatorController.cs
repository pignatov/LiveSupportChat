using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Models;
using System.Web.Security;
using System.Web.Routing;
using LiveSupport.Data;

namespace LiveSupport.Controllers
{
    public class OperatorController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        MopligEntities context;

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public OperatorController()
        {
            context = new MopligEntities();
        }
        
        //
        // GET: /Operator/

        public ActionResult Index()
        {
            var current_company = CompanyBO.GetCurrentCompany();

            var Operators = from i in context.Users
                            where i.companyID == current_company.ID
                            select new RegisterOperatorModel()
                            {
                                UserName = i.UserName,
                                UserID = i.UserId,
                                TotalSessions = i.Sessions.Count
                            };

            var ops = Operators.ToList();
            foreach (var oper in ops)
            {
                oper.FullName = UserProfile.GetUserProfile(oper.UserName).FullName;
            }
            ViewBag.OnlineOperators = LiveSupport.CountAvailableOperators(current_company.CodeName);
            return View(ops);
        }



        //
        // GET: /Operator/Details/5

        public ActionResult Details(string id)
        {
            var user = (from i in context.Users
                       where i.UserName == id
                       select i).FirstOrDefault();
            ViewBag.UserProfile = UserProfile.GetUserProfile(id);
            return View(user);
        }

        //
        // GET: /Operator/Create

        public ActionResult Create()
        {
            var current_company = CompanyBO.GetCurrentCompany();

            int number_of_operators = (from u in context.Users
                     where u.companyID == current_company.ID
                     select u).Count();

            var license = (from l in context.Licenses
                           orderby l.EndDate descending
                           select l).FirstOrDefault();

            ViewBag.LimitOfOperatorsReached = (license.MaxOperators <= number_of_operators);

            return View();
        } 

        //
        // POST: /Operator/Create

        [HttpPost]
        public ActionResult Create(RegisterOperatorModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password);
                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        Roles.AddUserToRoles(model.UserName, new string[] { "Operator" });

                        var current_company = CompanyBO.GetCurrentCompany();

                        User user = (from i in context.Users
                                     where i.UserName == model.UserName
                                     select i).FirstOrDefault();
                        
                        user.Company = current_company;
                        UserProfile profile = UserProfile.GetUserProfile(model.UserName);
                        profile.FullName = model.FullName;
                        profile.Save();
                        context.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Operator/Edit/5
 
        public ActionResult Edit(string id)
        {
            var user = (from i in context.Users
                        where i.UserName == id
                        select i).FirstOrDefault();
            ViewBag.UserProfile = UserProfile.GetUserProfile(id);
            return View(user);
        }

        //
        // POST: /Operator/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, User user, string FullName)
        {
            try
            {
                // TODO: Add update logic here
                UserProfile profile = UserProfile.GetUserProfile(id);
                profile.FullName = FullName;
                profile.Save();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Operator/Delete/5
 
        public ActionResult Delete(string id)
        {
            var supporter = (from i in context.Users
                            where i.UserName == id
                            select new RegisterOperatorModel()
                            {
                                UserName = i.UserName,
                                UserID = i.UserId,
                                FullName = ""
                            }).FirstOrDefault();
            return View(supporter);
        }

        //
        // POST: /Operator/Delete/5

        [HttpPost]
        public ActionResult Delete(string id, FormCollection collection)
        {
            try
            {
                System.Web.Security.Membership.DeleteUser(id); 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
