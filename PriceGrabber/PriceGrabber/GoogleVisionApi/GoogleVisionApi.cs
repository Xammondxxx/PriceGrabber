using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.GoogleVisionApi
{
    public class GoogleVision
    {
        static string UrlVisionApi = " https://vision.googleapis.com/v1/images:annotate?key=AIzaSyDy1xlJAviA5UbI_jc8K_JTcP7gVPXYHFU";


        public static async Task<Data.ResponseJson.RootObject> GetTextFromImage(byte[] image)
        {
            // Create new record 
            var str = JsonConvert.SerializeObject(Data.RequestJson.GetNewRequest(image));
            var content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");

            try
            {
                using (HttpClient cl = new HttpClient())
                {
                    var res = await cl.PostAsync(UrlVisionApi, content);
                    var resStr = await res.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Data.ResponseJson.RootObject>(resStr);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
