using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public class Browser
    {
        public string UserAgent { get; set; }
        public string OS { get; set; }
        public string Name { get; set; }
        public string Version { get; set;}
        public string CurrentPage { get; set; }
        public string IP { get; set; }
        public string Country { get; set; }
        public DateTime LogOnTime { get; set; }
        public string LogOn
        {
            get
            {
                return LogOnTime.ToString("dd/MM/YYYY hh:mm:ss");
            }
        }
    }

    public class UserInfo
    {
        public string Name {get; set;}
        public string eMail { get; set; }
        public string Question { get; set; }
        public bool InvitedToChat { get; set; }
    }
}