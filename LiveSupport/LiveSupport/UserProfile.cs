using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;

namespace LiveSupport
{
    public class UserProfile: ProfileBase
    {
        public static UserProfile GetUserProfile(string username)
        {
            return Create(username) as UserProfile;
        }

        public static UserProfile GetUserProfile()
        {
            return Create(Membership.GetUser().UserName) as UserProfile;
        }

        public string FullName
        {
            get { return this["FullName"] as string; }
            set { this["FullName"] = value; }
        }

        public bool? NotifyOnNewClient
        {
            get { return this["NotifyOnNewClient"] as bool?; }
            set { this["NotifyOnNewClient"] = value; }
        }

        public bool? NotifyOnNewOnlineClient
        {
            get { return this["NotifyOnNewOnlineClient"] as bool?; }
            set { this["NotifyOnNewOnlineClient"] = value; }
        }
    }
}