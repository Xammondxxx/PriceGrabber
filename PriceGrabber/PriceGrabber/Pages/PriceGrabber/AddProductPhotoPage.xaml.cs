using PriceGrabber.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriceGrabber.DependencyServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PriceGrabber.Core.Data;

namespace PriceGrabber.Pages.PriceGrabber
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddProductPhotoPage : CustomNavigationPage
    {
        public List<string> Brands = new List<string>() { "HP", "Samsung", "Huawei" };
        private PriceGrabberItem PriceGrabberItem;

        public AddProductPhotoPage(ContentPage parent, PriceGrabberItem item) : base()
        {
            InitializeComponent();
            Initilize();
            PriceGrabberItem = item;
            AddNavigationPanel(parent);
        }


        private void Initilize()
        {
            pkrBrand.PickerControl.PlaceHolder = "Select Brand...";
            entryProductName.Placeholder = "Product Name...";
            entryPrice.Placeholder = "Price in local currency";
            entryPrice.Keyboard = Keyboard.Numeric;
            entryPrice.TextChanged += EntryPrice_TextChanged;
            entryComment.PlaceHolder = "Comments are welcome";
            btnDone.Text = "Submit";
            SetBrands();

            var imageTapGR = new TapGestureRecognizer();
            imageTapGR.Tapped += GetNewImage;
            slImage.GestureRecognizers.Add(imageTapGR);
            imgProduct.GestureRecognizers.Add(imageTapGR);

            btnDone.Clicked += BtnDone_Clicked;
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
            imgProduct.Source = ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));

            var imageText = await GoogleVisionApi.GoogleVision.GetTextFromImage(stream.ToArray());
            entryComment.Text = imageText?.responses?.FirstOrDefault()?.textAnnotations?.FirstOrDefault().description;
        }

        private void EntryPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(e.NewTextValue != null)
            {
                var decimalSeparatorIdx = e.NewTextValue.IndexOf(".");
                if (decimalSeparatorIdx == -1) return;
                var decimalPartWithDot = e.NewTextValue.Substring(decimalSeparatorIdx, e.NewTextValue.Length - decimalSeparatorIdx);
                if (decimalPartWithDot.Length > 3)
                    entryPrice.Text = e.OldTextValue;
            }
        }

        private void SetBrands()
        {
            pkrBrand.PickerControl.ItemsSource = Brands;
        }

        private async void BtnDone_Clicked(object sender, EventArgs e)
        {
            PriceGrabberItem.Comment = entryComment.Text;
            PriceGrabberItem.Product = new PriceGrabberProduct()
            {
                BrandName = pkrBrand.PickerControl.SelectedItem?.ToString(),
                Name = entryProductName.Text,
                Price = Converters.ToDecimalSafe(entryPrice.Text)
            };

            await App.Current.MainPage.DisplayAlert("Done", "Product info was added!", "OK");
            App.Current.MainPage = new MainPage();

        }
    }
}