using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using LiveSupport.Models;
using LiveSupport.Data;
using CaptchaMVC;
using CaptchaMVC.Attribute;

namespace LiveSupport.Controllers
{
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        MopligEntities db;

        public AccountController()
        {
            db = new MopligEntities();
        }

        public ActionResult LicenseInfo()
        {
            License license = null;
            try
            {
                var current_company = CompanyBO.GetCurrentCompany();
                license = (from i in db.Licenses
                               where i.CompanyID == current_company.ID
                               orderby i.EndDate descending
                               select i).FirstOrDefault();

            }
            catch
            {
            }
            return View(license);

        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }


        public ActionResult ChangeCulture(Culture lang, string returnUrl)
        {
            if (returnUrl.Length >= 3)
            {
                returnUrl = returnUrl.Substring(3);
            }
            return Redirect("/" + lang.ToString() + returnUrl);
        }        
        
        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        public JsonResult IsUID_Available(string Username)
        {
            if (!MembershipService.UserExists(Username))
                return Json(true, JsonRequestBehavior.AllowGet);

            string suggestedUID = String.Format("{0} is not available.", Username);

            for (int i = 1; i < 100; i++)
            {
                string altCandidate = Username + i.ToString();
                if (!MembershipService.UserExists(altCandidate))
                {
                    suggestedUID = String.Format("{0} is not available. Try {1}.", Username, altCandidate);
                    break;
                }
            }
            return Json(suggestedUID, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpPost]
        // [CaptchaVerify("Captcha is not valid")]
        public ActionResult Register(RegisterModel model)
        {
            
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    Roles.AddUserToRoles(model.UserName, new string[] { "Supervisor", "Operator" });
                    
                    // New company
                    Company company = new Company();
                    company.FullName = model.CompanyName;
                    company.CodeName = model.CompanyName;
                    company.RegDate = DateTime.Now;
                    
                    User user = (from i in db.Users
                                    where i.UserName==model.UserName
                                    select i).FirstOrDefault();
                    user.Company = company;

                    // New trial license
                    License license = new License();
                    license.StartDate = DateTime.Now;
                    license.EndDate = license.StartDate.AddDays(14);
                    license.MaxOperators = 1;
                    license.TypeOfLicense = (int)License.LicenseType.Trial;
                    license.Company = company;

                    db.AddToLicenses(license);

                    UserProfile profile = UserProfile.GetUserProfile(model.UserName);
                    profile.FullName = model.FullName;
                    profile.Save();
                    db.SaveChanges();

                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    Helper.SendNotifyMessage(model.Email, "Moplig.com registration", String.Format(ControllerRes.Account.Register.NewRegistrationMessage, LiveSupportSettings.Settings.WebSite+"/en/Help/QuickStart"));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        [Authorize]
        public ViewResult Profile()
        {
            UserProfile profile = UserProfile.GetUserProfile(User.Identity.Name);
            var model = new UserProfileModel
            {
                FullName = profile.FullName,
                NotifyOnNewClient = profile.NotifyOnNewClient,
                NotifyOnNewOnlineClient = profile.NotifyOnNewOnlineClient
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Profile(UserProfileModel model)
        {
            if (ModelState.IsValid)
            {
                UserProfile profile = UserProfile.GetUserProfile(User.Identity.Name);
                profile.FullName = model.FullName;
                profile.NotifyOnNewClient = model.NotifyOnNewClient;
                profile.NotifyOnNewOnlineClient = model.NotifyOnNewOnlineClient;
                profile.Save();
                return RedirectToAction("Profile");
            }
            return View(model);
        }

    }
}
