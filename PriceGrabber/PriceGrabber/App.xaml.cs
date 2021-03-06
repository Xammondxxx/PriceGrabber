﻿using PriceGrabber.Core;
using PriceGrabber.Pages;
using PriceGrabber.Helpers;
using PriceGrabber.Pages.PriceGrabber;
using PriceGrabber.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PriceGrabber.DependencyServices;
using PriceGrabber.Pages.LeadGenerator;

namespace PriceGrabber
{
    public partial class App : Application
    {
        public static App Instance { get; private set; }
        public App()
        {
            InitializeComponent();
            Instance = this;
            FontHelper.SetFonts(true);
            MainPage = new LoginPage();
            //MainPage = new MainPage();
            //MainPage = new AddProductPhotoPage(null, new Core.Data.PriceGrabberItem());
            //   RequestLocationPermission();
            //   Test();
            // var vw = new WebViewerView(CustomControls.BrowserType.NewLogin, new LoginPage());
            //  vw.Uri = "http://youtube.com";
            // MainPage = neTestw ContainerPage(vw);

        }

        private async void Test()
        {
            await Task.Delay(1000);
            MainPage = new AddressPage(null);
        }

        //TEST
        private void RequestLocationPermission()
        {
            DependencyService.Get<IAPIHelper>().RequestPermissions("Location");
        }

        private async void Test(ContentPage parent)
        {
            await  Task.Delay(1000);
            MainPage = new AddressPage(parent);
        }

        public bool BackButtonPressed()
        {
            if (MainPage is IBrowserParentPage page)
                return page.BackButtonPressed();
            return false;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
