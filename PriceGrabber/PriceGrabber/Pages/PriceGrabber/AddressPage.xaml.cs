using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriceGrabber.Core;
using PriceGrabber.Core.Data;
using PriceGrabber.CustomControls;
using PriceGrabber.DependencyServices;
using PriceGrabber.GeoLocationService;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages.PriceGrabber
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressPage : CustomNavigationPage
    {
        public IMap Map { get; set; }

        public List<string> Countries = new List<string>() { "Russia", "US", "Poland" };

        private string CountryPickerPlacHolder =>  "Country...";

        public AddressPage(ContentPage parent) : base()
        {
            InitializeComponent();
            FindCurrentLocation();
            Initilize();
            InitilizeMap();
            AddNavigationPanel(parent);
        }


        private void InitilizeMap()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                Map = new CustomMap() { MapType = MapType.Street, };
                cvMap.Content = Map as CustomMap;
            }
            else
            {
                //  Map = new CustomMapView();
                // cvMap.Content = Map as CustomMapView;

            }
            Map.NeedGetLocationInfo += GetLocationInfo;
        }
        

        void Initilize()
        {
            pkrCountry.PickerControl.PlaceHolder = CountryPickerPlacHolder;
            entryStore.Placeholder = "Store Name...";
            lbLocation.Text = "Location:";
            btnNext.Text = "Next Step";
            btnNext.Clicked += BtnNext_Clicked;
            SetCountries();
        }

        protected override Module GetModule()
        {
            return Module.PriceGrabber;
        }

        private void FindCurrentLocation()
        {

        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();

            StartFindLocation();
          

        }

        private void SetCountries()
        {
            pkrCountry.PickerControl.ItemsSource = Countries;
        }

        private async void StartFindLocation()
        {
            imgPin.IsVisible = false;
            actIndicator.IsRunning = true;
            actIndicator.IsVisible = true;

            var apiService = DependencyService.Get<IAPIHelper>();
            apiService.StartRequestLocation();

            int attemptsCount = 0;
            GeoLocation curLocation = null;
            while (curLocation == null && attemptsCount < 100)
            {
                curLocation = apiService.GetCurrentLocation();
                await Task.Delay(50);
                attemptsCount++;
            }

            Map.OnLocationChanged(curLocation);
            apiService.StopRequestLocation();
        }

        private void StopFindLocation()
        {
            imgPin.IsVisible = true;
            actIndicator.IsRunning = false;
            actIndicator.IsVisible = false;
        }

        private void BtnNext_Clicked(object sender, EventArgs e)
        {
            PriceGrabberItem item = new PriceGrabberItem();
            item.Country = pkrCountry.PickerControl.SelectedItem?.ToString();
            item.Location = Map.CurrentLocation;
            item.StoreName = entryStore.Text;

            App.Current.MainPage = new AddProductPhotoPage(this, item);
        }

        private async void GetLocationInfo()
        {
            bool isCountryDone = false, isStoreDone = false;

            //get country
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () => {
                var res = await  CoordinatesToLocationInfo.GetCountryByLocation(Map.CurrentLocation);
                int countryIdx = -1;
                if (res != null)
                {
                    countryIdx = Countries.FindIndex(x => x.ToLower() == res.Item1?.ToLower() || x == res.Item2?.ToLower());
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        pkrCountry.PickerControl.SelectedIndex = countryIdx;
                    });
                }
                if(countryIdx == -1)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        pkrCountry.PickerControl.SelectedIndex = -1;
                        pkrCountry.PickerControl.PlaceHolder = CountryPickerPlacHolder;
                    });
                isCountryDone = true;
                Task.Delay(1);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            //get store
            isStoreDone = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () => {
                var res = await StoresInfoByLocation.GetStoresName(Map.CurrentLocation);
                if (res?.FirstOrDefault() != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        entryStore.Text = res.FirstOrDefault();
                    });
                }
                isStoreDone = true;
                Task.Delay(1);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            //fishish loading indicator if needed
            if (actIndicator.IsVisible)
            {
                while (!isStoreDone || !isCountryDone)
                    await Task.Delay(50);

                StopFindLocation();
            }
            isStoreDone = isCountryDone = false;
        }
    }
}