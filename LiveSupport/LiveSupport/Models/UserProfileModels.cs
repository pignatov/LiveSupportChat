using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LiveSupport.Models
{
    public class UserProfileModel
    {
        [DisplayName("Full Name")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [DisplayName("Notify on new clients")]
        public bool? NotifyOnNewClient { get; set; }

        [DisplayName("Notify on new online clients")]
        public bool? NotifyOnNewOnlineClient { get; set; }
    }
}