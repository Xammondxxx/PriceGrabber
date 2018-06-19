using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.DependencyServices
{
    public interface IWebCacheHelper
    {
        double GetCacheSize();
        void ClearCache();
        void ClearCookies();
    }
}
