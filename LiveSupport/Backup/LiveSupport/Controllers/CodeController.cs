using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LiveSupport.Data;

namespace LiveSupport.Controllers
{
    public class CultureInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    //[Authorize(Roles = "Administrator, Operator")]      
    public class CodeController : Controller
    {

        public static SelectList CultureSelectList()
        {
            var cultureIndexes = new List<CultureInfo> { new CultureInfo(){Name = "English", Value = "en"},
                new CultureInfo(){Name = "German", Value = "de"},
                new CultureInfo(){Name = "Bulgarian", Value = "bg"},
            };
            return new SelectList(cultureIndexes, "Value", "Name");
        }
        
        //
        // GET: /Code/
        [Authorize(Roles = "Administrator, Operator")]  
        public ActionResult Snippet()
        {
            System.Guid currentUserID = (System.Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
            MopligEntities entities = new MopligEntities();
            var company_code = (from u in entities.Users
                          where u.UserId == currentUserID
                          select u.Company.CodeName).FirstOrDefault();
            ViewBag.CodeSnippet1 = String.Format("<iframe src='https://moplig.com/en/Live/Status?company={0}'></iframe>", company_code);

            ViewBag.CodeSnippet2 = String.Format("<a href=\"javascript:void(window.open('https://moplig.com/en/Live/Ask?company={0}&referer='+ encodeURIComponent(document.URL),'','width=590,height=400,left=0,top=0,resizable=yes,menubar=no,location=no,status=yes,scrollbars=yes'))\"> " +
                "<img src='https://moplig.com/en/Live/Image?company={0}'></a>", company_code);


            string script = "<script type=\"text/javascript\">\n" +
            "var _mop = _mop || [];\n" +
            "_mop.push(['company', '"+ company_code + "']);\n" +

            "(function () {\n" +
                "var mo = document.createElement('script'); mo.type = 'text/javascript'; mo.async = true;\n" +
                "mo.src = 'https://moplig.com/en/Live/Script';\n" +
                "var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(mo, s);\n" +
            "})();\n" +

            "</script>\n";

            ViewBag.CodeSnippet3 = script + String.Format("<iframe src='https://moplig.com/en/Live/Presence?company={0}' style='visibility:hidden;display:none'></iframe>", company_code);

            return View();
        }

    }
}
