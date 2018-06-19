using PriceGrabber.Core;
using PriceGrabber.DependencyServices;
using PriceGrabber.Droid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(APIHelper))]
namespace PriceGrabber.DependencyServices
{
    public class APIHelper : IAPIHelper
    {
        public string GetAppVersion()
        {
            return MainActivity.AppVerison;
        }

        public string GetDeviceModel()
        {
            return null;
        }

        public async void RequestPermissions(string permission)
        {
            switch (permission)
            {
                case "Location":
                    await RequestLocationsPermissions();
                    return;
            }
        }


        public async Task<bool> RequestLocationsPermissions()
        {
            MainActivity.Instance.RequestLocationPermissions();
            while (MainActivity.Instance.LocationPermissionsGranted == null)
            {
                await Task.Delay(50);
            }
            return MainActivity.Instance.LocationPermissionsGranted == true;
        }

        public void StartRequestLocation()
        {
            MainActivity.RequestLocation();
        }

        public void StopRequestLocation()
        {
            MainActivity.StopRequestLocation();
        }

        public GeoLocation GetCurrentLocation()
        {
            return MainActivity.GetCurrentLocation();
        }

    }
}
