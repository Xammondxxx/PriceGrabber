using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PriceGrabber.Core
{
    public interface IMap
    {
        GeoLocation CurrentLocation { get; set; }
        void OnLocationChanged(GeoLocation newLoc, bool animate = true);
        void OnNeedGetLocationInfo(GeoLocation newLoc);

        event Action NeedGetLocationInfo;

    }
}
