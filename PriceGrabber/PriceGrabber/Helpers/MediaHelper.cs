using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Helpers
{
    public static class MediaHelper
    {
        static bool _gettingImage = false;
        public static async Task<MediaFile> GetMedia()
        {
            if (_gettingImage) return null;
            _gettingImage = false;
            MediaFile res = null;
            try
            {

                var action = await App.Current.MainPage.DisplayActionSheet("Выберите дейтвие:", "Отмена", null, "Выбрать фото", "Сделать фото");
                await Task.Delay(300);

                if (action == "Выбрать фото")
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
                        await App.Current.MainPage.DisplayAlert("Галерея недоступна", "Нет прав на доступ к галерее фотографий.", "OK");
                        return null;
                    }
                    res = await CrossMedia.Current.PickPhotoAsync();

                    if (res == null)
                    {
                        await App.Current.MainPage.DisplayAlert("", "Фото из галереи не получено!.", "OK");
                        return null;
                    }
                }
                else if (action == "Сделать фото")
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
                        await App.Current.MainPage.DisplayAlert("Камера недоступна", "Нет прав на доступ к камере.", "OK");
                        return null;
                    }
                    res = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "CarImage.jpg"
                    });

                    if (res == null)
                    {
                        await App.Current.MainPage.DisplayAlert("", "Фото с камеры не получено!.", "OK");
                        return res;
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", ex.Message, "OK");
                return null;
            }
            finally
            {
                _gettingImage = false;
            }
        }

    }
}
