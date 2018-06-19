using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PriceGrabber.Droid.DependencyServices;
using PriceGrabber.DependencyServices;
using System.Drawing;


[assembly: Xamarin.Forms.Dependency(typeof(MediaHelper))]
namespace PriceGrabber.Droid.DependencyServices
{
    public class MediaHelper : IMediaHelper
    {
        public byte[] ResizeImage(byte[] source, float maxWidth, float maxHeight)
        {

            try
            {
                {
                    var options = new Android.Graphics.BitmapFactory.Options()
                    {
                        InJustDecodeBounds = false,
                        InPurgeable = true,
                    };

                    using (var image = Android.Graphics.BitmapFactory.DecodeStream(new MemoryStream(source)))//(sourceFile, options))
                    {
                        if (image != null)
                        {
                            var sourceSize = new Size((int)image.GetBitmapInfo().Height, (int)image.GetBitmapInfo().Width);

                            var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);

                            byte[] res = null;
                            if (maxResizeFactor > 0.9)
                            {
                                return source;
                            }
                            else
                            {

                                var width = (int)(maxResizeFactor * sourceSize.Width);
                                var height = (int)(maxResizeFactor * sourceSize.Height);

                                using (var bitmapScaled = Android.Graphics.Bitmap.CreateScaledBitmap(image, height, width, true))
                                {
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        bitmapScaled.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 95, stream);
                                        res = stream.ToArray();
                                    }
                                    bitmapScaled.Recycle();
                                }
                            }

                            image.Recycle();
                            return res;

                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ResizeImage failed:" + "  " + ex.Message);
                return source;
            }
            return null;
        }
    }
}