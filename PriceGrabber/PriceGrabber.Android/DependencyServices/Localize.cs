using System;
using System.Globalization;
using System.Threading;
using Java.Util;
using Android.OS;
using Android.Content.Res;
using PriceGrabber.DependencyServices;
using PriceGrabber.Droid.DependencyServices;

[assembly: Xamarin.Forms.Dependency(typeof(Localize))]
namespace PriceGrabber.Droid.DependencyServices
{
    public class Localize : ILocalize
    {
        public string GetCurrentCountry()
        {
            try
            {
                Locale locale = Resources.System.Configuration.Locale;
                return locale.Country;
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
        }

        public CultureInfo GetCurrentCultureInfo()
        {
            try
            {
                Locale locale = Resources.System.Configuration.Locale;
                return new CultureInfo(locale.Language);
            }
            catch (Exception ex)
            {

                return new CultureInfo("en");
            }

            //return new CultureInfo("en");
        }

        public string GetCurrentLanguage()
        {
            try
            {
                Locale locale = Resources.System.Configuration.Locale;
                return locale.Language;
            }
            catch (Exception ex)
            {

                return string.Empty;
            }
        }

        public void SetLocale(CultureInfo ci)
        {
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}