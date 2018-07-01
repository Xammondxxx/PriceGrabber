using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.Helpers
{
    public static class FontHelper
    {
        public static void SetFonts(bool byDeviceSettings)
        {
            try
            { 
                var ci = UserHelper.CurrentCultureInfo(byDeviceSettings);
                switch (ci?.TwoLetterISOLanguageName?.ToLower())
                {
                    case "en":
                    case null:
                        App.Current.Resources["MainFont"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified" : "HPSimplified_Lt.ttf#HP Simplified";
                        App.Current.Resources["MainFontItalic"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified" : "HPSimplified_It.ttf#HP Simplified";
                        App.Current.Resources["MainFontBold"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified" : "HPSimplified_Rg.ttf#HP Simplified";
                        App.Current.Resources["MainFontName"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified" : "HPSimplified_Lt.ttf";
                        break;
                    case "ru":
                        App.Current.Resources["MainFont"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W10 Light" : "cyrillic-light-ttf.ttf#HP Simplified W10 Light";
                        App.Current.Resources["MainFontItalic"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W10 Light" : "cyrillic-light-ttf.ttf#HP Simplified W10 Light";
                        App.Current.Resources["MainFontBold"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W10 Regular" : "cyrillic-regular-ttf.ttf#HP Simplified W10 Regular";
                        App.Current.Resources["MainFontName"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W10 Light" : "cyrillic-light-ttf.ttf";
                        break;
                    case "el":
                        App.Current.Resources["MainFont"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W15 Light" : "greek-light-ttf.ttf#HP Simplified W15 Light";
                        App.Current.Resources["MainFontItalic"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W15 Light" : "greek-light-ttf.ttff#HP Simplified W15 Light";
                        App.Current.Resources["MainFontBold"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified" : "greek-regular-ttf.ttf##HP Simplified W15 Regular";
                        App.Current.Resources["MainFontName"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W15 Light" : "greek-light-ttf.ttf";
                        break;
                    default:
                        App.Current.Resources["MainFont"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W02 Light" : "latin-e-light-ttf.ttf#HP Simplified W02 Light";
                        App.Current.Resources["MainFontItalic"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W02 Light" : "latin-e-light-ttf.ttf#HP Simplified W02 Light";
                        App.Current.Resources["MainFontBold"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W02 Regular" : "latin-e-regular-ttf.ttf#HP Simplified W02 Regular";
                        App.Current.Resources["MainFontName"] = Device.RuntimePlatform == Device.iOS ? "HP Simplified W02 Light" : "latin-e-light-ttf.ttf";
                        break;
                }                            
            }
            catch (Exception ex)
            {
                //GoogleAnalyticsHelper.SendException(Device.RuntimePlatform + ": Error setting fonts: " + ex.Message, false);
            }
        }
    }
}
