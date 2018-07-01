using Plugin.Media;
using Plugin.Media.Abstractions;
using PriceGrabber.DependencyServices;
using PriceGrabber.Extensions;
using PriceGrabber.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = Core.Settings.SsoData;
            if (Device.RuntimePlatform == Device.Android)
            {
                LblInsignia.Margin = new Thickness(0, -1, 0, 1);
            }
            PhotoBtn.Margin = new Thickness(14, 6, -4, 8);
            UpdateContent();
        }

        public void UpdateContent()
        {
            //if (HPHData.ActiveUser.Photo == null)
                ImgPhoto.Source = "user_avatar";
            /*else
            {
                var res = HPHData.ActiveUser.Photo;
                if (Device.RuntimePlatform == Device.Android && res != null && res.Length > 50000)
                    res = DependencyService.Get<IIOHelper>().ResizeImage(HPHData.ActiveUser.Photo, 200, 200);
                if (res == null)
                    imgPhoto.Source = "user-avatar";
                else
                    imgPhoto.Source = ImageSource.FromStream(() => new MemoryStream(res));
            }
            if (Device.RuntimePlatform == Device.Android)
            {
                //    photoBtn.Margin = new Thickness(0);

            }*/
            Localize();
        }

        void Localize()
        {
            LblPersonalDetails.Text = "Personal Details".Localize();
            LblWorkPhone.Text = "Work Phone".Localize();
            LblMobilephone.Text = "Mobile Phone".Localize();
            LblLanguage.Text = "Language".Localize();
            LblCompanyDetails.Text = "Company Details".Localize();
            LblCompany.Text = "Company".Localize();
            LblCompanyPhone.Text = "Company Phone".Localize();
            LblCountry.Text = "Country".Localize();
            LblCity.Text = "City".Localize();
            LblAddress.Text = "Address".Localize();
            BtnUpdateDetails.Text = "Update Details".Localize();

            try
            {
#if TURKEY
                lblInsignia.Text = HPHData.LocalizableText("Download HP Star Insignia");
#else
                if (Core.Settings.SsoData?.Language.Trim().Substring(0, 2)?.ToLower() == "tr")
                    LblInsignia.Text = "Download HP Star Insignia".Localize();
                else
                    LblInsignia.Text = "Download HP Hero Insignia".Localize();
#endif
            }
            catch
            {
                LblInsignia.Text = "Download HP Hero Insignia".Localize();
            }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            ImgPhoto.WidthRequest = ImgPhoto.Height;
        }

        async void BtnPickPhotoClicked(object sender, EventArgs args)
        {
            //if (Core.Settings.SimulationMode) return;
            try
            {
                var image = await MediaHelper.GetMedia(MediaAction.PickPhotoFromGallery);
                if (image != null)
                {
                    UpdateUserPhoto(image?.GetStream());
                    image.Dispose();
                }
            }
            catch (Exception e)
            {
                //Log.Add(e, "Profile.OnBtnPickPhotoClicked");
            }
        }

        async void BtnTakePhotoClicked(object sender, EventArgs args)
        {
            //if (Core.Settings.SimulationMode) return;
            try
            {
                var image = await MediaHelper.GetMedia(MediaAction.TakePhotoFromCamera);
                if (image != null)
                {
                    UpdateUserPhoto(image?.GetStream());
                    image.Dispose();
                }
            }
            catch (Exception e)
            {
                //Log.Add(e, "Profile.OnBtnPickPhotoClicked");
            }
        }

        void UpdateUserPhoto(Stream stream)
        {
            if (stream == null) return;
            using (var memstream = new MemoryStream())
            {
                stream.CopyTo(memstream);
                if (Device.RuntimePlatform == Device.Android && (stream as MemoryStream)?.ToArray()?.Length > 50000)
                {
                    var image = DependencyService.Get<IMediaHelper>().ResizeImage(memstream.ToArray(), 256, 256);
                    if (image == null) return;
                    stream = new MemoryStream(image);
                }
                //Core.Settings.Photo = photo;
            }
            UpdateContent();
            /*MainPage.Instance.RefreshMenu();
            if (!Core.Settings.SimulationMode)
            {
                await Task.Run(() => HPHLogin.SetUserPhoto());
                HPHData.SaveActiveUser();
            }*/
        }

        void BtnDownloadClicked(object sender, EventArgs args)
        {
        }

        private void BtnUpdateDetailsClicked(object sender, EventArgs e)
        {

        }
    }
}