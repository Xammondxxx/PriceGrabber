using PriceGrabber.Core;
using PriceGrabber.DependencyServices;
using PriceGrabber.Helpers;
using PriceGrabber.Pages.PriceGrabber;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages.LeadGenerator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeadGeneratorPage : CustomNavigationPage
    {
        public List<string> Countries = new List<string>() { "Russia", "US", "Poland" };

        public LeadGeneratorPage()
        {
            InitializeComponent();
            Initilize();
            AddNavigationPanel(null);
        }

        private void Initilize()
        {
            entryFirstName.Placeholder = "Contact's First Name...";
            entryLastName.Placeholder = "Contact's Last Name...";
            entryBusiness.Placeholder = "Name of Business...";
            entryEmail.Placeholder = "Contact's Email Address...";
            entryPhone.Placeholder = "Contact's Phone Number...";
            pkrCountry.PickerControl.PlaceHolder = "Country...";
            lbDevicesNumber.Text = "Number Of Devices:";
            lbConversation.Text = "Prior Conversation?";
            btnDone.Text = "Submit";
            entryComment.PlaceHolder = "Notes...";
            cbConverasationNo.TextLabel.Text = "No";
            cbConverasationYes.TextLabel.Text = "Yes";
            cbConverasationYes.Check();

            cbConverasationYes.CheckedChanged += (newV) => { if (newV) cbConverasationNo.UnCheck(); else cbConverasationNo.Check();};
            cbConverasationNo.CheckedChanged += (newV) => { if (newV) cbConverasationYes.UnCheck(); else cbConverasationYes.Check(); };

            SetCountries();
            SetNumberOfDevices();

            btnDone.Clicked += BtnDone_Clicked;

            var imageTapGR = new TapGestureRecognizer();
            imageTapGR.Tapped += GetNewImage;
            slImage.GestureRecognizers.Add(imageTapGR);
            imgContact.GestureRecognizers.Add(imageTapGR);
        }

        protected override Module GetModule()
        {
            return Module.LeadGenerator;
        }

        private async void GetNewImage(object sender, EventArgs e)
        {
            var newImage = await MediaHelper.GetMedia();
            if (newImage == null) return;
            MemoryStream stream = new MemoryStream();
            newImage.GetStream().CopyTo(stream);
            if (Device.RuntimePlatform == Device.Android && stream?.ToArray()?.Length > 50000)
            {
                var image = DependencyService.Get<IMediaHelper>().ResizeImage(stream.ToArray(), 300, 300);
                if (image == null) return;
                stream = new MemoryStream(image);
            }
            imgContact.Source = ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));
        }


        private void SetCountries()
        {
            pkrCountry.PickerControl.ItemsSource = Countries;
        }

        private void SetNumberOfDevices()
        {
            var devicesNumberList = new List<string>();
            for (int i = 0; i < 100; i++)
                devicesNumberList.Add(i.ToString());
            pkrNumberOfDevices.PickerControl.ItemsSource = devicesNumberList;
            pkrNumberOfDevices.PickerControl.SelectedIndex = 1;
        }

        private async void BtnDone_Clicked(object sender, EventArgs e)
        {
           await  App.Current.MainPage.DisplayAlert("Done", "Contact info was added!", "OK");
            App.Current.MainPage = new MainPage();
        }

    }
}