using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public class LoggedOperator: ILoggedUser
    {
        public string ClientID { get; set; }
        public string Company { get; set; }
        public bool IsAvailable { get; set; }
        public List<LoggedClient> AssignedClients { get; set; }
        public string Name { get; set; }

        public LoggedOperator(string aClientID, string aCompany, string aName, bool aIsAvailable)
        {
            this.ClientID = aClientID;
            this.Company = aCompany;
            this.Name = aName;
            this.IsAvailable = aIsAvailable;
            this.AssignedClients = new List<LoggedClient>();
        }
    }
}