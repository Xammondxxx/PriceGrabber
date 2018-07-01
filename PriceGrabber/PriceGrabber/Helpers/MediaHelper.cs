using Plugin.Media;
using Plugin.Media.Abstractions;
using PriceGrabber.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Helpers
{
    public enum MediaAction
    {
        Unknown,
        PickPhotoFromGallery,
        TakePhotoFromCamera
    }

    public static class MediaHelper
    {
        static bool _gettingImage = false;
        public static async Task<MediaFile> GetMedia(MediaAction action = MediaAction.Unknown)
        {
            if (_gettingImage) return null;
            _gettingImage = false;
            MediaFile res = null;
            try
            {
                if (action == MediaAction.Unknown)
                {
                    var strAction = await App.Current.MainPage.DisplayActionSheet("Choose action".Localize(), "Cancel".Localize(), null, "Pick Photo".Localize(), "Take Photo".Localize());
                    await Task.Delay(300);
                    if (strAction == "Pick Photo".Localize())
                        action = MediaAction.PickPhotoFromGallery;
                    else if (strAction == "Take Photo".Localize())
                        action = MediaAction.TakePhotoFromCamera;
                }

                if (action == MediaAction.PickPhotoFromGallery)
                {
                    /*if (Device.RuntimePlatform == Device.Android)
                    {
                        p = await DependencyService.Get<IMediaHelper>().CheckGalleryPermissions();
                        await Task.Delay(300);
                        var stream = await DependencyService.Get<IMediaHelper>().PickImage();
                        if (stream == null) return;
                        byte[] image = (stream as MemoryStream)?.ToArray();
                        if (image?.Length > 50000)
                            image = DependencyService.Get<IMediaHelper>().ResizeImage((stream as MemoryStream).ToArray(), 200, 200);
                        if (image == null) return;
                        carPhotoArray = image.ToArray();
                        imgCar.Source = ImageSource.FromStream(() => new MemoryStream(image.ToArray()));
                    }
                    else
                        p = CrossMedia.Current.IsPickPhotoSupported;*/

                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await App.Current.MainPage.DisplayAlert("Gallery unavailable".Localize(), "You have no permissions for gallery".Localize(), "OK".Localize());
                        return null;
                    }
                    res = await CrossMedia.Current.PickPhotoAsync();

                    if (res == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Failure".Localize(), "Could not pick photo from gallery".Localize(), "OK".Localize());
                        return null;
                    }
                }
                else if (action == MediaAction.TakePhotoFromCamera)
                {
                    /*if (Device.RuntimePlatform == Device.Android)
                    {
                        p = await DependencyService.Get<IMediaHelper>().CheckCameraPermissions();
                        await Task.Delay(300);
                        /*var stream = await DependencyService.Get<IMediaHelper>().TakePhoto();
                        if (stream == null) return;

                        carPhotoArray = stream.ToArray();
                        imgCar.Source = ImageSource.FromStream(() => new MemoryStream(stream.ToArray()));
                    }
                    else
                        p = CrossMedia.Current.IsTakePhotoSupported;*/
                    if (!CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await App.Current.MainPage.DisplayAlert("Camera unavailable".Localize(), "You have no permissions for using camera".Localize(), "OK".Localize());
                        return null;
                    }
                    res = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "Sample.jpg"
                    });

                    if (res == null)
                    {
                        await App.Current.MainPage.DisplayAlert("Failure".Localize(), "Could not take foto from camera".Localize(), "OK".Localize());
                        return res;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error".Localize(), ex.Message, "OK".Localize());
                return null;
            }
            finally
            {
                _gettingImage = false;
            }
        }

    }
}
