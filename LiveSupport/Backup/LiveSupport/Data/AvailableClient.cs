using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public class AvailableClient: ILoggedUser
    {
        public string ClientID { get; set; }
        public string Company { get; set; }
        public LoggedOperator AssignedOperator { get; set; }
        public string Name { get; set; }
        public List<string> Messages { get; set; }
        public Browser Browser { get; set; }
        public int SessionID { get; set; }
        public bool InvitedToChat { get; set; }

        public AvailableClient(string aClientID, string aCompany, Browser aBrowser, string aName = "Anonymous")
        {
            this.ClientID = aClientID;
            this.Company = aCompany;
            this.Name = aName;
            this.Browser = aBrowser;
            this.Messages = new List<string>();
            this.InvitedToChat = false;
        }        
    }
}