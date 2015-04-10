using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public class LoggedClient : ILoggedUser
    {
        public string ClientID { get; set; }
        public string Company { get; set; }
        public LoggedOperator AssignedOperator { get; set; }
        public UserInfo Info { get; set; }
        public List<string> Messages { get; set; }
        public Browser Browser { get; set; }
        public int SessionID { get; set; }
        public string Name
        {
            get { return this.Info.Name; }
            set {}
        }

        public LoggedClient(string aClientID, string aCompany, Browser aBrowser, UserInfo aInfo)
        {
            this.ClientID = aClientID;
            this.Company = aCompany;
            this.Info = aInfo;
            this.Browser = aBrowser;
            this.Messages = new List<string>();
        }

    }
}