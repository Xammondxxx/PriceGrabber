using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PriceGrabber.Core;
using PriceGrabber.CustomControls;
using PriceGrabber.Droid.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace PriceGrabber.Droid.CustomControls
{
    public class CustomMapRenderer : MapRenderer
    {
        static GoogleMap native;
        CustomMap map;
        bool firstTime = true;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (map != null)
                {
                    map.PropertyChanged -= OnElementPropertyChanged;
                    native.CameraChange -= Native_CameraChange;
                    native = null;

                }
            }

            if (e.NewElement != null)
            {
                map = e.NewElement as CustomMap;
                map.PropertyChanged += OnElementPropertyChanged;
                map.LocationChanged += Map_LocationChanged;
                if (native == null)
                    Control.GetMapAsync(this);
            }

        }

        private void Map_LocationChanged(bool animate)
        {
            if (map?.CurrentLocation == null || map.CurrentLocation.Latitude == 0) return;

            var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(map.CurrentLocation.Latitude, map.CurrentLocation.Longitude), 15);

            if(animate)
                  native.AnimateCamera(cameraUpdate);
            else native.MoveCamera(cameraUpdate);
        }

        protected override void OnMapReady(GoogleMap googleMap)
        {
            native = googleMap;

            if (native != null)
            {
                //native.MapClick += Native_MapClick;
                native.CameraChange += Native_CameraChange;
               // native.SetOnMarkerClickListener(this);
                native.UiSettings.RotateGesturesEnabled = false;
                native.UiSettings.CompassEnabled = false;
                try
                {
                    native.MyLocationEnabled = false;
                }
                catch { }
                native.UiSettings.MyLocationButtonEnabled = false;
                native.UiSettings.MapToolbarEnabled = false;
                native.UiSettings.ZoomControlsEnabled = true;
            }
        }

        private void Native_CameraChange(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            if (firstTime)
            {
                firstTime = false;
                var loc = map?.CurrentLocation;
                if (loc == null)
                    loc = MainActivity.Instance.GetLastKnownLocation();
                if (loc == null)
                    loc = new GeoLocation(0, 0);
                var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(new LatLng(loc.Latitude, loc.Longitude), 15);
                native.MoveCamera(cameraUpdate);
            }
            else LocationChanged(e.Position.Target);
        }


        private async void LocationChanged(LatLng location)
        {
            map.OnNeedGetLocationInfo(new GeoLocation(location.Latitude, location.Longitude));
        }  
    }
}