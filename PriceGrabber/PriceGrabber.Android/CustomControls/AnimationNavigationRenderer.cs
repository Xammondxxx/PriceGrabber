using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PriceGrabber.Droid.CustomControls;
using PriceGrabber.Pages.PriceGrabber;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer(typeof(CustomNavigationPage), typeof(AnimationNavigationRenderer))]
namespace PriceGrabber.Droid.CustomControls
{
    public class AnimationNavigationRenderer : PageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
           
        }

    }
}