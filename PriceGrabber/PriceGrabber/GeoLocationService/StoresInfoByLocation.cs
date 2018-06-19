using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using PriceGrabber.GeoLocationService;
using PriceGrabber.Core;

namespace PriceGrabber.GeoLocationService
{
  
    public class StoresInfoByLocation
    {
        const string Key = "AIzaSyBYQZ5j30i9N6H1jd8nhPoeKWY19HN8iok";
        const string NearByPlacesUrlByRadius = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0}&radius={1}&type={2}&key={3}";
        const string NearByPlacesUrlByDistance = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0}&rankby=distance&type={1}&key={2}";
        const string NearByPlacesUrlNextPage = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?pagetoken={0}&key={1}";
        const int PageResultsCount = 20;

        public static SearchType CurSearchType = StoresInfoByLocation.SearchType.Distance;
        public static int SearchRadius = 50;
        public static Types StoreType = Types.store;
        public static int MaxResults = 40;
        
        public static async Task<List<string>> GetStoresName(GeoLocation loc, bool includeCafe = false)
        {
            var res = await GetStoresWebApi(loc, includeCafe);
            if (res != null)
                return res.Select(x => x.name).ToList();

            return null;
        }


        public static async Task<List<FindStoresResponse.Result>> GetStoresWebApi(GeoLocation loc, bool includeCafe = false)
        {

            var res = await GetPlacesInfoWebApi(loc, includeCafe);
            List<FindStoresResponse.Result> ress = new List<FindStoresResponse.Result>();
            if (res != null)
            {
                ress.AddRange(res.results);
                var maxCount = MaxResults / PageResultsCount;
                var count = 1;
                while (res.next_page_token != null && count < maxCount)
                {
                    var nextRes =  await GetPlacesInfoWebApi(loc, includeCafe, res.next_page_token);
                    if (nextRes != null)
                        ress.AddRange(nextRes.results);
                    else break;
                    count++;
                }

                List<FindStoresResponse.Result> results = new List<FindStoresResponse.Result>();

                if (!includeCafe)
                {
                    foreach (var item in ress)
                    {
                        if (!item.types.Contains(Types.restaurant.ToString()) &&
                            !item.types.Contains(Types.cafe.ToString()))
                            results.Add(item);
                    }
                }
                else results.AddRange(ress);

                return results;
            }
            return null;

        }


        public static async Task<FindStoresResponse.RootObject> GetPlacesInfoWebApi(GeoLocation loc, bool includeCafe = false, string nextPageToken = null)
        {
            using (HttpClient cl = new HttpClient())
            {
                string url = null;

                switch (CurSearchType)
                {
                    case SearchType.Radius:
                        url = string.Format(NearByPlacesUrlByRadius,
                         loc.Latitude.ToString().Replace(",", ".") + "," + loc.Longitude.ToString().Replace(",", "."),
                         SearchRadius,
                         StoreType.ToString(),
                         Key
                         ); 
                        break;
                    case SearchType.Distance:
                        url = string.Format(NearByPlacesUrlByDistance,
                         loc.Latitude.ToString().Replace(",", ".") + "," + loc.Longitude.ToString().Replace(",", "."),
                         StoreType.ToString(),
                         Key
                         );
                        break;
                }
                
                if (nextPageToken != null)
                    url = string.Format(NearByPlacesUrlNextPage,
                     nextPageToken,
                     Key
                     );

                var jsonres = await cl.GetStringAsync(url);
                if (jsonres != null)
                    return JsonConvert.DeserializeObject<FindStoresResponse.RootObject>(jsonres);
            }
            return null;
        }



        public enum SearchType
        {
            Distance,
            Radius
        }

        public enum Types
        {
            store, //all store types
            hardware_store,
            electronics_store,
            department_store,
            convenience_store,
            clothing_store,
            pet_store,
            shoe_store,
            home_goods_store,
            furniture_store,
            restaurant,
            cafe
        }

    }
}
