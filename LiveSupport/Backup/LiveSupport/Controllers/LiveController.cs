using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Models;
using LiveSupport.Data;
using System.Net.Mail;

namespace LiveSupport.Controllers
{
    public class LiveController : Controller
    {

        MopligEntities db;

        public LiveController()
        {
            db = new MopligEntities();
        }
        //
        // GET: /Live/

        public ActionResult Status(string company)
        {
            ViewBag.SupportCompany = company;
            ViewBag.IsAvailableOperator = LiveSupport.ISAvailableOperator(company);
            if (Request.UrlReferrer != null)
            {
                ViewBag.Referer = Request.UrlReferrer.AbsoluteUri;
            }
            else
            {
                ViewBag.Referer = "Unknown";
            }

            return View();
        }

        public FileContentResult Image(string company)
        {
            string culture = RouteData.Values["culture"].ToString();
            string status_file;
            if (LiveSupport.ISAvailableOperator(company))
            {
                status_file = "~/images/{0}/online.png";
            }
            else
            {
                status_file = "~/images/{0}/offline.png";
            }

            byte[] contents = System.IO.File.ReadAllBytes(Server.MapPath(Url.Content(String.Format(status_file,culture))));
            string content_type = "image/png";
            return new FileContentResult(contents, content_type);
        }

        public JavaScriptResult Script()
        {
            string script = "window.onmessage=function(e) { if (e.data=='Invite'){  CallDiv();  }  };";

            script += "function deleteParentElement(n){n.parentNode.parentNode.removeChild(n.parentNode); }\n";
            script += "function deleteParentParentElement(n){n.parentNode.parentNode.parentNode.removeChild(n.parentNode.parentNode); }";

            script += "function CallDiv() { \n" +
                "var url = \"'https://moplig.com/en/Live/Ask?company=\"+_mop[0][1] + \"&referer='+ encodeURIComponent(document.URL)\";\n" +
            "var divTag = document.createElement('div');\n" +
                "divTag.id = 'divDyn';\n" +
                "divTag.style.cssText='position:fixed;right:1%; top:50%; background: none repeat scroll 0 0 #FFFFFF; border: 3px solid #2E5186; border-radius: 8px 8px 8px 8px; box-shadow: 0 1px 3px rgba(0, 0, 0, 0.35); line-height: 15px; margin-top: 5px; padding: 10px; z-index: 2001';\n" +
                "divTag.innerHTML = \"We are online. Would you like to chat with us?<br/>" +
                    "<div style='text-align:center'><a href=\\\"javascript:void(window.open(\"+url+\",'','width=590,height=400,left=0,top=0,resizable=yes,menubar=no,location=no,status=yes,scrollbars=yes'))\\\" onclick='deleteParentParentElement(this)'> Yes</a>" +
                    "&nbsp; <a href='#' onclick='deleteParentParentElement(this)'>No</a></div>\";\n" +
                "document.body.appendChild(divTag); }\n";

            return JavaScript(script);
        }

        public ActionResult Presence(string company)
        {
            if (!this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("moplig_user"))
            {
                HttpCookie cookie = new HttpCookie("moplig_user");
                cookie.Value = Guid.NewGuid().ToString();
                cookie.Expires = DateTime.Now.AddDays(7);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }

            string ipAddress = this.Request.ServerVariables["REMOTE_ADDR"];
            ViewBag.IPAddress = ipAddress;
            ViewBag.Company = company;
            if (Request.UrlReferrer != null)
            {
                ViewBag.CurrentPage = Request.UrlReferrer.AbsoluteUri;
            }
            else
            {
                ViewBag.CurrentPage = "Unknown";
            }
            return View();
        }

        public ActionResult Ask(string company, string referer)
        {
            if (!this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("moplig_user"))
            {
                HttpCookie cookie = new HttpCookie("moplig_user");
                cookie.Value = Guid.NewGuid().ToString();
                cookie.Expires = DateTime.Now.AddDays(7);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }

            ViewBag.SupportCompany = company;
            string ipAddress = this.Request.ServerVariables["REMOTE_ADDR"];
            ViewBag.IPAddress = ipAddress;
            if (referer != null)
            {
                ViewBag.CurrentPage = referer;
            }
            else
            {
                if (Request.UrlReferrer != null)
                {
                    ViewBag.CurrentPage = Request.UrlReferrer.AbsoluteUri;
                }
                else
                {
                    ViewBag.CurrentPage = "Unknown";
                }
            }

            if (LiveSupport.ISAvailableOperator(company))
            {
                return View();
            }
            else
            {
                return RedirectToAction("SendMail", new  { company=company, referer= referer });
            }
        }

        public ActionResult SendMail(string company, string referer)
        {
            EmailModel model = new EmailModel();
            model.Company = company;
            ViewBag.CurrentPage = referer;

            return View(model);
        }

        [HttpPost]
        public ActionResult SendMail(EmailModel model)
        {
            if (ModelState.IsValid){
                var user = (from i in db.Users
                           where i.Company.CodeName == model.Company
                           select i).FirstOrDefault();

                string email = System.Web.Security.Membership.GetUser(user.UserId).Email;
                Helper.SendNotifyMessage(new MailAddress(email), "Message from user", String.Format("User:{0}\nE-mail:{1}\nQuestion:{2}", model.Name, model.Email, model.Question));
                return RedirectToAction("Thanks");
            }
            return View();
        }

        public ActionResult Thanks()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}
