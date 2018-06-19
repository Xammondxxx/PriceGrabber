using PriceGrabber.DependencyServices;
using PriceGrabber.Pages.LeadGenerator;
using PriceGrabber.Pages.PriceGrabber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Initilize();

            DependencyService.Get<IAPIHelper>().RequestPermissions("Location");
        }

        public void Initilize()
        {
            mainGrid.WidthRequest = StaticDeviceInfo.WidthDp * 0.8;
            mainGrid.HeightRequest = mainGrid.WidthRequest;
            btnGrabber.Clicked += BtnGrabber_Clicked;
            btnLeadGen.Clicked += BtnLeadGen_Clicked;  
        }

        private void BtnLeadGen_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new LeadGeneratorPage();
        }

        private void BtnGrabber_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new AddressPage(null);
        }

       
    }
}