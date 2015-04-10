using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public partial class License
    {
        public enum LicenseType { Trial = 0, Regular = 1, Discounted = 2 };
    }
}