using PriceGrabber.DependencyServices;
using System.Globalization;
using Xamarin.Forms;

namespace PriceGrabber.Helpers
{
    public static class UserHelper
    {
        public static Element GetUserImage()
        {
            /*var res = HPHData.ActiveUser?.Photo;
            if (Device.RuntimePlatform == Device.Android && res != null && res.Length > 50000)
                res = DependencyService.Get<IIOHelper>().ResizeImage(HPHData.ActiveUser.Photo, 200, 200);
            if (res == null) return ImageSource.FromFile("user_avatar.png");
            var ele = ImageSource.FromStream(() => new MemoryStream(res));
            return ele;*/
            return null;
        }

        public static CultureInfo CurrentCultureInfo(bool byDeviceSettings)
        {
            string lang = null;
            if (!byDeviceSettings)
            {
                lang = Core.Settings.SsoData?.Language;
                if (string.IsNullOrEmpty(lang) && !string.IsNullOrEmpty(Core.Settings.AuthToken))
                    lang = "en";
            }
            CultureInfo ci = null;
            if (!string.IsNullOrEmpty(lang) && !byDeviceSettings)
            {
                try
                {
                    lang = lang.Trim().Substring(0, 2)?.ToLower();
                    ci = new CultureInfo(lang);
                }
                catch
                {
                    ci = null;
                }
            }
            if (ci == null)
            {
                ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            return ci;
        }
    }
}
