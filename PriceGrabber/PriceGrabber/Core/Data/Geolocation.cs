using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core
{
    public class GeoLocation
    {
        public GeoLocation(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
