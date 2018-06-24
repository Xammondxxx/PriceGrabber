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
            pkrBrand.EntryControl.Placeholder = "Brand...";
            pkrProductName.EntryControl.Placeholder = "Product Name...";
            pkrPrice.EntryControl.Placeholder = "Price in local currency";
            pkrPrice.EntryControl.Keyboard = Keyboard.Numeric;
            pkrPrice.EntryControl.TextChanged += EntryPrice_TextChanged;
            entryComment.PlaceHolder = "Comments are welcome";
            btnDone.Text = "Submit";

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
                var image = DependencyService.Get<IMediaHelper>().ResizeImage(stream.ToArray(), 1024, 1024);
                if (image == null) return;
                stream = new MemoryStream(image);
            }
            imgProduct.Source = ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));

            RecognizeImageText(stream.ToArray());
        }

        private async void RecognizeImageText(byte[] image)
        {
            var imageText = await GoogleVisionApi.GoogleVision.GetTextFromImage(image);

            var recWords = imageText?.responses?.FirstOrDefault()?.textAnnotations;
            if ((recWords?.Count ?? 0) > 0)
            {
                pkrBrand.PickerControl.Items.Clear();
                pkrProductName.PickerControl.Items.Clear();
                pkrPrice.PickerControl.Items.Clear();
                for (int i = 1; i < recWords.Count; i++)
                {
                    var txt = recWords[i].description;
                    Decimal.TryParse(txt, out decimal price);
                    pkrBrand.PickerControl.Items.Add(txt);
                    pkrProductName.PickerControl.Items.Add(txt);
                    if (price > 0)
                        pkrPrice.PickerControl.Items.Add(price.ToString());
                }
            }
            //entryComment.Text = recWords[0].description;
        }

        private void EntryPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(e.NewTextValue != null)
            {
                var decimalSeparatorIdx = e.NewTextValue.IndexOf(".");
                if (decimalSeparatorIdx == -1) return;
                var decimalPartWithDot = e.NewTextValue.Substring(decimalSeparatorIdx, e.NewTextValue.Length - decimalSeparatorIdx);
                if (decimalPartWithDot.Length > 3)
                    pkrPrice.EntryControl.Text = e.OldTextValue;
            }
        }

        private async void BtnDone_Clicked(object sender, EventArgs e)
        {
            PriceGrabberItem.Comment = entryComment.Text;
            PriceGrabberItem.Product = new PriceGrabberProduct()
            {
                BrandName = pkrBrand.PickerControl.SelectedItem?.ToString(),
                Name = pkrProductName.EntryControl.Text,
                Price = Converters.ToDecimalSafe(pkrPrice.EntryControl.Text)
            };

            await App.Current.MainPage.DisplayAlert("Done", "Product info was added!", "OK");
            App.Current.MainPage = new MainPage();

        }
    }
}