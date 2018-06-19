using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PriceGrabber.Helpers
{
    public class Converters
    {
        public static decimal ToDecimalSafe(string value)
        {
            if (value == null) return 0;

            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";
                return Convert.ToDecimal(value.Replace(",", "."), nfi);
            }
            catch { return 0; }
        }
    }
}
