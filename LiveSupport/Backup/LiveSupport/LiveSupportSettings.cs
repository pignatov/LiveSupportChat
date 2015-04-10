using System;
using System.Configuration;

namespace LiveSupport
{
    public class LiveSupportSettings : ConfigurationSection
    {
        private static LiveSupportSettings settings
          = ConfigurationManager.GetSection("LiveSupport") as LiveSupportSettings;

        public static LiveSupportSettings Settings
        {
            get
            {
                return settings;
            }
        }

        [ConfigurationProperty("NotificationSender"
          , DefaultValue = "notify@moplig.com"
          , IsRequired = false)]
        public string NotificationSender
        {
            get { return (string)this["NotificationSender"]; }
            set { this["NotificationSender"] = value; }
        }

    }
}