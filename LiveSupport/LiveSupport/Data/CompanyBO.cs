using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public class CompanyBO
    {
        private static MopligEntities context;

        static CompanyBO()
        {
            context = new MopligEntities();
        }

        public static Company GetCurrentCompany()
        {

            System.Guid currentUserID = (System.Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
            var company = (from u in context.Users
                           where u.UserId == currentUserID
                           select u.Company).FirstOrDefault();

            return company;
        }

    }
}