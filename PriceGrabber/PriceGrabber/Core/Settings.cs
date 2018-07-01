using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using PriceGrabber.Core.Data;
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
        private const string AuthTokenKey = "AuthToken";
        private const string AuthTokenExpiredAtKey = "AuthTokenExpiredAt";
        private const string AppDeviceGuidKey = "AppDeviceGuid";
        private const string SsoDataKey = "SsoData";
        private const string PhotoKey = "Photo";
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

        static string authToken = string.Empty;
        public static string AuthToken
        {
            get
            {
                if (!string.IsNullOrEmpty(authToken)) return authToken;
                authToken = AppSettings.GetValueOrDefault(AuthTokenKey, default(string));
                return authToken;
            }
            set
            {
                if (authToken == value) return;
                authToken = value;
                AppSettings.AddOrUpdateValue(AuthTokenKey, authToken);
            }
        }

        static DateTime authTokenExpiredAt = DateTime.MinValue;
        public static DateTime AuthTokenExpiredAt
        {
            get
            {
                if (authTokenExpiredAt > DateTime.MinValue) return authTokenExpiredAt;
                authTokenExpiredAt = AppSettings.GetValueOrDefault(AuthTokenExpiredAtKey, DateTime.MinValue);
                return authTokenExpiredAt;
            }
            set
            {
                if (authTokenExpiredAt == value) return;
                authTokenExpiredAt = value;
                AppSettings.AddOrUpdateValue(AuthTokenExpiredAtKey, authTokenExpiredAt);
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

        static SsoData ssoData = null;
        public static SsoData SsoData
        {
            get
            {
                if (ssoData != null) return ssoData;
                ssoData = JsonConvert.DeserializeObject<SsoData>(AppSettings.GetValueOrDefault(SsoDataKey, default(string)));
                return ssoData;
            }
            set
            {
                ssoData = value;
                AppSettings.AddOrUpdateValue(SsoDataKey, JsonConvert.SerializeObject(ssoData));
            }
        }
    }
}
