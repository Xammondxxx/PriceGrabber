using PriceGrabber.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core
{
    public interface IBrowserParentPage
    {
        void DidStartNavigation(object sender, string url);
        void DidFinishNavigation(object sender, string url);
        void DidFailedNavigation(object sender, string url);
        void ProgressNavigation(object sender, string url, int progress);
        void SubLinkRequested(object sender, UrlData urlData);
        void ExternalLinkRequested(object sender, string url);
        void DocLinkRequested(object sender, string url);
    }
}
