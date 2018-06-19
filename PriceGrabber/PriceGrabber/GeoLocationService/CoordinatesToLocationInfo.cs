using Newtonsoft.Json;
using PriceGrabber.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.GeoLocationService
{
    public static class CoordinatesToLocationInfo
    {
        public static GeoLocation LastLocation = new GeoLocation(0,0);
        public static GoogleGeocodeResponse.RootObject LastResponse = null;

        public static async Task<Tuple<string, string>> GetCountryByLocation(GeoLocation loc)
        {

            var res = await GetLocationInfoByLocation(loc);
            if (res == null) return null;
            try
            {
                var info = res.results.LastOrDefault()?.address_components?.LastOrDefault();
                if(info != null)
                    return new Tuple<string, string>(info.long_name, info.short_name);
            }
            catch { }

            return null;
        }

        public static async Task<GoogleGeocodeResponse.RootObject> GetLocationInfoByLocation(GeoLocation loc)
        {
            if (LastLocation.Latitude == loc.Latitude && LastLocation.Longitude == loc.Longitude)
                return LastResponse;

            LastLocation = loc;
            dynamic result = null;
            using (var wc = new HttpClient())
            {
                var uri = new Uri(
                    String.Format(
                    "https://maps.googleapis.com/maps/api/geocode/json?latlng={0}&key={1}",
                    loc.Latitude.ToString().Replace(",", ".") + "," + loc.Longitude.ToString().Replace(",", "."),
                    Data.WebServiceApiKey));
                try
                {
                    var res = await wc.GetStringAsync(uri);

                    result = JsonConvert.DeserializeObject<GoogleGeocodeResponse.RootObject>(res);
                    LastResponse = result;
                }
                catch (Exception ex)
                {

                }
            }
            return result;

        }

    }
}
