using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Areas.Admin.Models
{
    public class CompanyInfoModel
    {
        public string CompanyName { get; set; }
        public int? OnlineOperators { get; set; }
        public int? MaxOperators { get; set; }
        public int? OnlineClients { get; set; }
        public int? TotalSessions { get; set; }
    }
}