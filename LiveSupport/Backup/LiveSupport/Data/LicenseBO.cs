using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public class LicenseBO
    {
        private static MopligEntities db;

        static LicenseBO()
        {
            db = new MopligEntities();
        }

        public static License GetActiveLicense()
        {
            var current_company = CompanyBO.GetCurrentCompany();
            var license = (from i in db.Licenses
                       where i.CompanyID == current_company.ID
                       orderby i.EndDate descending
                       select i).FirstOrDefault();
            return license;
        }

        public static bool IsLicenseActive()
        {
            License lic = GetActiveLicense();
            return (lic.EndDate > DateTime.Now);
        }
    }
}