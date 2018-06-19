using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using PriceGrabber.Core;

namespace PriceGrabber.CustomControls
{
    public class CustomMap : Map, IMap
    {
        public GeoLocation CurrentLocation { get; set; }

        public event Action<bool> LocationChanged;
        public event Action NeedGetLocationInfo;


        public void OnLocationChanged(GeoLocation newLoc, bool animate= true)
        {
            CurrentLocation = newLoc;
            LocationChanged?.Invoke(animate);
        }

        public void OnNeedGetLocationInfo(GeoLocation newLoc)
        {
            CurrentLocation = newLoc;
            NeedGetLocationInfo?.Invoke();
        }
    }
}
