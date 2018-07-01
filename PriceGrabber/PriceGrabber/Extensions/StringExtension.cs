using PriceGrabber.Helpers;
using System;
using System.Collections.Generic;
 using System.Linq;

namespace PriceGrabber.Extensions
{
    public static class StringExtension
    {
        public static Dictionary<string, string> UrlParseGetParams(this string url)
        {
            var q = url.IndexOf('?');
            if (q < 0) return null;
            var pars = url.Substring(q + 1).Split('&');
            return pars.Select(par => par.Split('=')).Where(v => v.Length == 2).ToDictionary(v => v[0], v => v[1]);
        }

        public static string ToCarouselTitle(this string title)
        {
            if (string.IsNullOrEmpty(title) || title.Length <= 10) return title;
            var res = title.Substring(0, 10);
            var n = res.LastIndexOf(' ');
            return n < 0 ? res : res.Substring(0, n);
        }

        public static string Localize(this string text, bool byDeviceSettings = false)
        {
            return LocalizeHelper.Localize(text, byDeviceSettings);
        }
    }
}
