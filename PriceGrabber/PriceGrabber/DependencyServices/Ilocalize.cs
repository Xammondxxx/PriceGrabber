using System;
using System.Collections.Generic;
using System.Globalization;

namespace PriceGrabber.DependencyServices
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();
        string GetCurrentLanguage();
        string GetCurrentCountry();
        void SetLocale(CultureInfo ci);
    }
}
