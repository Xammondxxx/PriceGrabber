using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using PriceGrabber.Droid.DependencyServices;
using Android.Content.PM;
using PriceGrabber.DependencyServices;
using PriceGrabber.CustomControls;

[assembly: Xamarin.Forms.Dependency(typeof(OrientationService))]
namespace PriceGrabber.Droid.DependencyServices
{
    class OrientationService : IOrientationService
    {
        public Orientation CurrentScreenOrientation => ToCustomOrientation(MainActivity.Instance.Resources.Configuration.Orientation);
        public Orientation CurrentDeviceOrientation => ToOrientation(MainActivity.Instance?.WindowManager?.DefaultDisplay?.Rotation);

        public void LockOrientation(Orientation newOrienation)
        {
            MainActivity.Instance.RequestedOrientation = ToAndroidOrientation(newOrienation);
        }

        public void ResetOrientation(Orientation baseOrienation)
        {
            MainActivity.Instance.RequestedOrientation = ToAndroidOrientation(baseOrienation);
            MainActivity.Instance.RequestedOrientation = ScreenOrientation.Unspecified;
        }


        public Orientation ToOrientation(SurfaceOrientation? orientation)
        {
            switch (orientation)
            {
                case SurfaceOrientation.Rotation0:
                case SurfaceOrientation.Rotation180:
                    return Orientation.Portrait;
                case SurfaceOrientation.Rotation90:
                case SurfaceOrientation.Rotation270:
                    return Orientation.Landscape;
                default:
                    return Orientation.Portrait;
            }
        }

        public ScreenOrientation ToAndroidOrientation(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Portrait:
                    return ScreenOrientation.Portrait;
                case Orientation.Landscape:
                    return ScreenOrientation.Landscape;
                default:
                    return ScreenOrientation.Unspecified;
            }
        }

        public Orientation ToCustomOrientation(Android.Content.Res.Orientation orientation)
        {
            switch (orientation)
            {
                case Android.Content.Res.Orientation.Portrait:
                    return Orientation.Portrait;
                case Android.Content.Res.Orientation.Landscape:
                    return Orientation.Landscape;
                default:
                    return Orientation.Portrait;
            }
        }
    }
}