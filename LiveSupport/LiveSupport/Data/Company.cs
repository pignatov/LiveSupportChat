using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveSupport.Data
{
    public partial class Company
    {
        public bool HasLogo
        {
            get { return (logo != null); }
        }
    }
}