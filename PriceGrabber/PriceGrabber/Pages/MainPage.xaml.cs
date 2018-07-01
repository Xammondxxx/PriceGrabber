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
            Initialize();

        }

        public void Initialize()
        {
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += LeadGeneratorClicked;
            FrameLeadGenerator.GestureRecognizers.Add(tgr);
            ImageLeadGenerator.GestureRecognizers.Add(tgr);
            tgr = new TapGestureRecognizer();
            tgr.Tapped += PriceGrabberClicked;
            FramePriceGrabber.GestureRecognizers.Add(tgr);
            ImagePriceGrabber.GestureRecognizers.Add(tgr);
            tgr = new TapGestureRecognizer();
            tgr.Tapped += ProfileClicked;
            ImgProfile.GestureRecognizers.Add(tgr);
        }

        private void LeadGeneratorClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new LeadGeneratorPage();
        }

        private void PriceGrabberClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new AddressPage(null);
        }

        private void ProfileClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new ProfilePage();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DependencyService.Get<IAPIHelper>().RequestPermissions("Location");
        }


    }
}