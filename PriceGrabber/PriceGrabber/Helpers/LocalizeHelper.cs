using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Helpers
{
    class LocalizeHelper
    {
        public static string Localize(string text, bool byDeviceSettings = false)
        {
            return text;
            //return HPHData.LocalizableText(text, byDeviceSettings);
        }
    }
}
