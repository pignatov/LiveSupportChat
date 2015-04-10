using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LiveSupport.Data;
using System.IO;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace LiveSupport.Controllers
{

    public class CompanyController : Controller
    {
        MopligEntities db;

        public CompanyController()
        {
            db = new MopligEntities();
        }
        
        //
        // GET: /Company/

        [Authorize(Roles = "Supervisor")]
        public ActionResult Index()
        {
            Company c = CompanyBO.GetCurrentCompany();
            return View(c);
        }

        [Authorize(Roles = "Supervisor")]
        public ActionResult Logo()
        {
            return View();
        }

        [Authorize(Roles = "Supervisor")]
        public ActionResult Details()
        {
            Company company = CompanyBO.GetCurrentCompany();
            return View(company);
        }

        [Authorize(Roles = "Supervisor")]
        public ActionResult Buy()
        {
            Company company = CompanyBO.GetCurrentCompany();
            return View(company);
        }

        [HttpPost]
        [Authorize(Roles = "Supervisor")]
        public ActionResult Details(Company company)
        {
            if (ModelState.IsValid)
            {
                Company current_company = CompanyBO.GetCurrentCompany();
                current_company.FullName = company.FullName;
                db.SaveChanges();
            }
            return RedirectToAction("Details");
        }

        [Authorize(Roles = "Supervisor")]        
        public FileContentResult GetLogo()
        {
            System.Guid currentUserID = (System.Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
            var company = (from u in db.Users
                           where u.UserId == currentUserID
                           select u.Company).FirstOrDefault();

            if (company != null && company.logo != null)
            {
                return File(company.logo, "image/png", "logo.png");
            }
            else
            {
                byte[] contents = System.IO.File.ReadAllBytes(Server.MapPath("/images/internet-chat.png"));
                return File(contents, "image/png", "logo.png");
            }
        }

        public FileContentResult DisplayLogo(string company)
        {
            var c = (from u in db.Companies
                           where u.CodeName == company
                           select u).FirstOrDefault();

            if (c != null && c.logo != null)
            {
                return File(c.logo, "image/png", "logo.png");
            }
            else
            {
                byte[] contents = System.IO.File.ReadAllBytes(Server.MapPath("/images/internet-chat.png"));
                return File(contents,"image/png", "logo.png");
            }
        }

        [Authorize(Roles = "Supervisor")]        
        public ActionResult Upload()
        {
            foreach (string inputTagName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[inputTagName];
                if (file.ContentLength > 0)
                {
                    System.Guid currentUserID = (System.Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
                    var company = (from u in db.Users
                                        where u.UserId == currentUserID
                                        select u.Company).FirstOrDefault();

                    string mimeType = file.ContentType;
                    Stream fileStream = file.InputStream;
                    int fileLength = file.ContentLength;
                    byte[] fileData = new byte[fileLength];
                    fileStream.Read(fileData, 0, fileLength);

                    Image logo = Image.FromStream(fileStream);
                    logo = logo.Resize(64, 64, true);
                    MemoryStream ms = new MemoryStream();
                    logo.Save(ms, ImageFormat.Png);
                    company.logo = ms.ToArray();
                    
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Logo");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
