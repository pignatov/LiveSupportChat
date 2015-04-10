using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveSupport.Data
{
    public interface ILoggedUser
    {
        string ClientID { get; set; }
        string Name { get; set; }
    }
}
