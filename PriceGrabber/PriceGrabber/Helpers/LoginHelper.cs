using PriceGrabber.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Helpers
{
    public static class LoginHelper
    {
        public static async Task<bool> CheckAuthTokenExpiration()
        {
            if (string.IsNullOrEmpty(Core.Settings.AuthToken))
                return false;
            if (Core.Settings.AuthTokenExpiredAt > DateTime.UtcNow && (Core.Settings.AuthTokenExpiredAt - DateTime.UtcNow).TotalMinutes > 15)
                return true;
            var res = await PGService.RefreshAuthToken(Core.Settings.AuthToken);
            if (!res.Item1)
                return false;

            Core.Settings.AuthToken = res.Item2;
            Core.Settings.AuthTokenExpiredAt = res.Item3;

            var ssoData = await PGService.GetSSOData();
            if (ssoData == null)
                return false;
            Core.Settings.SsoData = ssoData;

            //Task.Run(() => HPHService.SetDeviceInfo());
            return true;
        }
    }
}
