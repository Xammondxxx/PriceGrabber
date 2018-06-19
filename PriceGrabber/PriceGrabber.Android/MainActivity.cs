using System;
using Android.Util;
using Android.Content.Res;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android;
using Android.Webkit;
using System.Threading.Tasks;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using PriceGrabber.Core;
using Android.Locations;
using Plugin.Permissions;

namespace PriceGrabber.Droid
{
    [Activity(Label = "PriceGrabber", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ILocationListener
    {
        public const int LocationPermissions = 1, GalleryCode = 2, WriteExternalStorageCode = 3, WriteCalendarCode = 4, FileChooserCode = 5, ReadExternalStorageCode = 6, LocationCode = 7;
        public bool? LocationPermissionsGranted;

        public static bool? LocationAccess = null;
        public static bool? ReadExternalStorageAccess = null;

        static MainActivity instance;
        public static MainActivity Instance => instance;
        static string appVerison;
        public static string AppVerison => appVerison;

        LocationManager locManager;
        public static GeoLocation curLocation;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            SetMetrics();
            appVerison = ApplicationContext.PackageManager.GetPackageInfo(ApplicationContext.PackageName, 0).VersionName;
            instance = this;
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            LoadApplication(new App());
        }
       
        
        private void SetMetrics()
        {
            var metrics = Resources.DisplayMetrics;
            var size = GetScreenSizePx(metrics);
            var sizeDp = GetScreenSize(metrics);

            StaticDeviceInfo.Height = size.Height;
            StaticDeviceInfo.Width = size.Width;
            StaticDeviceInfo.HeightDp = sizeDp.Height;
            StaticDeviceInfo.WidthDp = sizeDp.Width;

        }

        private Xamarin.Forms.Size GetScreenSize(DisplayMetrics metrics)
        {
            return new Xamarin.Forms.Size(ConvertPixelsToDp(metrics.WidthPixels), ConvertPixelsToDp(metrics.HeightPixels));
        }

        private Xamarin.Forms.Size GetScreenSizePx(DisplayMetrics metrics)
        {
            return new Xamarin.Forms.Size(metrics.WidthPixels, metrics.HeightPixels);
        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        public void GetLocationPermission()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
                {
                    string[] perms =
                    {
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessFineLocation
                    };
                    Instance?.RequestPermissions(perms, LocationCode);
                    return;
                }
                LocationAccess = true;
                return;
            }
            LocationAccess = true;
        }

        public IValueCallback UploadMessage;
        public async void ChooseFile(IValueCallback filePathCallback, Intent intent)
        {
            GetReadPermission();
            while (ReadExternalStorageAccess == null)
                await Task.Delay(100);
            if (ReadExternalStorageAccess == false) return;

            // make sure there is no existing message
            if (UploadMessage != null)
            {
                UploadMessage.OnReceiveValue(null);
                UploadMessage = null;
            }

            UploadMessage = filePathCallback;

            try
            {
                StartActivityForResult(intent, FileChooserCode);
            }
            catch (ActivityNotFoundException e)
            {
                UploadMessage = null;
                Toast.MakeText(this, "Cannot open file chooser", ToastLength.Long).Show();
            }
        }

        public void GetReadPermission()
        {
            if (ReadExternalStorageAccess == true) return;
            try
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadExternalStorage }, ReadExternalStorageCode);
                }
                else ReadExternalStorageAccess = true;
            }
            catch (Exception e)
            {
            }
        }

        public GeoLocation GetLastKnownLocation()
        {
            try
            {
                var locManager = (LocationManager)Instance.GetSystemService(LocationService);
                var res = locManager.GetLastKnownLocation(LocationManager.GpsProvider);
                var res2 = locManager.GetLastKnownLocation(LocationManager.NetworkProvider);
                var res3 = locManager.GetLastKnownLocation(LocationManager.PassiveProvider);
                if (res != null)
                    return new GeoLocation(res.Latitude, res.Longitude);
                else
                  if (res2 != null)
                    return new GeoLocation(res2.Latitude, res2.Longitude);
                else
                  if (res3 != null)
                    return new GeoLocation(res3.Latitude, res3.Longitude);
            }
            catch(Exception ex) {

            }

            return new GeoLocation(0,0);
        }

        public void RequestLocationPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) != (int)Android.Content.PM.Permission.Granted ||
                    CheckSelfPermission(Manifest.Permission.AccessFineLocation) != (int)Android.Content.PM.Permission.Granted)
                {
                    string[] perms =
                    {
                        Manifest.Permission.AccessCoarseLocation,
                        Manifest.Permission.AccessFineLocation
                    };
                    Instance?.RequestPermissions(perms, LocationPermissions);
                    return;
                }
                LocationPermissionsGranted = true;
                return;
            }
            LocationPermissionsGranted = true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == LocationPermissions)
            {
                LocationPermissionsGranted = grantResults != null && grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted;
            }
        }


        public static async void RequestLocation()
        {
            curLocation = null;
            instance.locManager = (LocationManager)Instance.GetSystemService(LocationService);
            while (true)
            {
                try
                {
                    if (instance.locManager == null) break;
                    Instance.RunOnUiThread(() =>
                    {
                        instance.locManager.RequestSingleUpdate(LocationManager.NetworkProvider, instance, null);
                        instance.locManager.RequestSingleUpdate(LocationManager.GpsProvider, instance, null);
                        instance.locManager.RequestSingleUpdate(LocationManager.PassiveProvider, instance, null);
                    });
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(2000);
            }
            
        }

        public static void StopRequestLocation()
        {
            System.Diagnostics.Debug.WriteLine("StopRequestLocation: ");
            if (instance?.locManager != null)
            {
                Instance.locManager.RemoveUpdates(Instance);
                Instance.locManager = null;
            }
        }

        public static GeoLocation GetCurrentLocation()
        {
            return curLocation;
        }

        public void OnLocationChanged(Location location)
        {
            if (location.Accuracy < int.MaxValue && location != null && location.Latitude > 0 && location.Longitude > 0)
            {
                curLocation = new GeoLocation(location.Latitude, location.Longitude);
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }
    }
}

