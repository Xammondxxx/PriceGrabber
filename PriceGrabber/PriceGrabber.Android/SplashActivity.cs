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

namespace PriceGrabber.Droid
{
    [Activity(Label = "PriceGrabber", Theme = "@style/Theme.Splash", Icon = "@drawable/icon", MainLauncher = false, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);
            System.Threading.ThreadPool.QueueUserWorkItem(o => LoadActivity());

        }

        private void LoadActivity()
        {
            System.Threading.Thread.Sleep(5000); // Simulate a long loading process on app      
            StartActivity(typeof(MainActivity));
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_loading);

            Android.Graphics.Drawables.AnimationDrawable animation = (Android.Graphics.Drawables.AnimationDrawable)imageView.Drawable;

            animation.Start();
        }
    }
}