using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core
{
    public static class Settings
    {
        static ISettings AppSettings
        {
            get { if (CrossSettings.IsSupported) return CrossSettings.Current; return null; }

        }

        #region Setting Constants
        private const string AppDeviceGuidKey = "AppDeviceGuid";
        private const string LogoutUrlKey = "LogoutUrl";
        #endregion

        static Guid appDeviceGuid = Guid.Empty;
        public static Guid AppDeviceGuid
        {
            get
            {
                if (appDeviceGuid != Guid.Empty) return appDeviceGuid;
                appDeviceGuid = AppSettings.GetValueOrDefault(AppDeviceGuidKey, Guid.Empty);
                if (appDeviceGuid == Guid.Empty)
                {
                    appDeviceGuid = Guid.NewGuid();
                    AppSettings.AddOrUpdateValue(AppDeviceGuidKey, appDeviceGuid);
                }
                return appDeviceGuid;
            }
        }

        static string logoutUrl = string.Empty;
        public static string LogoutUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(logoutUrl)) return logoutUrl;
                logoutUrl = AppSettings.GetValueOrDefault(LogoutUrlKey, default(string));
                return logoutUrl;
            }
            set
            {
                if (logoutUrl == value) return;
                logoutUrl = value;
                AppSettings.AddOrUpdateValue(LogoutUrlKey, logoutUrl);
            }
        }
    }
}
