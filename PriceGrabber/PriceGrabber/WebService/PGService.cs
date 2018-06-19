﻿using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using PriceGrabber.Core;
using PriceGrabber.DependencyServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.WebService
{
    public static class PGService
    {
        private enum RequestType
        {
            Get,
            Post,
            Put,
            GetHead,
            CheckETag
        }

        public enum ApiServerKind
        {
            Staging,
            Production
        }

        public static NativeMessageHandler HttpMessageHandler { get; private set; }
        public static NativeCookieHandler HttpCookieHandler { get; private set; }
        public static HttpClient HttpClient { get; private set; }

        public static string CurrentAuthToken;

#if PRODUCTION
        private const ApiServerKind ServerKind = ApiServerKind.Production;
#else
        private const ApiServerKind ServerKind = ApiServerKind.Staging;
#endif

        public static string ApiServerUrl => ServerKind == ApiServerKind.Staging ? PGConst.API_SERVER_URL_STAGING : PGConst.API_SERVER_URL_PROD;

        public static void Init()
        {
            if (HttpClient != null) return;
            HttpCookieHandler = new NativeCookieHandler();
            HttpMessageHandler = new NativeMessageHandler(true, false, HttpCookieHandler);
            HttpClient = new HttpClient(HttpMessageHandler, false);
            //httpClient = new HttpClient();
            //HttpClient.Timeout = TimeSpan.FromSeconds(7);
        }


        private static bool _isBusy;
        private static async Task<PGServiceResponse> ExecuteCommand(string url, StringContent content, RequestType reqType, bool useParams = true)
        {
            var res = new PGServiceResponse()
            {
                Result = false,
                Response = null,
                Error = null
            };

            if (string.IsNullOrEmpty(url)) return res;

            if (string.IsNullOrEmpty(CurrentAuthToken) && useParams)
            {
                var urldata = UrlParser.Parse(url);
                urldata.AddParam("CountryCode", DependencyService.Get<ILocalize>().GetCurrentCountry());
                urldata.AddParam("Language", DependencyService.Get<ILocalize>().GetCurrentLanguage());
                url = urldata.AbsoluteUrl;
            }

            res.Request = url;
            if (!CheckConnection()) return res;

            Init();

            while (_isBusy)
            {
                await Task.Delay(100);
            }

            try
            {
                _isBusy = true;
                Debug.WriteLine("{0} HttpClient Request: {1}", reqType, res.Request);
                Debug.WriteLine("CurrentAuthToken: {0}", CurrentAuthToken);
                 var watch = Stopwatch.StartNew();
                HttpResponseMessage response = null;
                var isCheckEtag = reqType == RequestType.CheckETag;

                switch (reqType)
                {
                    case RequestType.Post:
                        if (!string.IsNullOrEmpty(CurrentAuthToken))
                            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentAuthToken);
                        //response = await HttpClient.PostAsync(url, content);
                        response = await HttpClient.PostAsync(url, content).ConfigureAwait(false); //.Result;
                        res.Response = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        res.Result = response.IsSuccessStatusCode;
                        break;
                    case RequestType.Put:
                        if (!string.IsNullOrEmpty(CurrentAuthToken))
                            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentAuthToken);
                        //response = await HttpClient.PostAsync(url, content);
                        response = await HttpClient.PutAsync(url, content).ConfigureAwait(false);
                        res.Response = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        res.Result = response.IsSuccessStatusCode;
                        break;
                    case RequestType.Get:
                        if (!string.IsNullOrEmpty(CurrentAuthToken))
                            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentAuthToken);
                        response = await HttpClient.GetAsync(url).ConfigureAwait(false); 
                        res.Response = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var msg = response.StatusCode.ToString();
                        res.Result = response.IsSuccessStatusCode;
                        break;
                    case RequestType.GetHead:
                        var req = new HttpRequestMessage(HttpMethod.Head, url);
                        response = await HttpClient.SendAsync(req).ConfigureAwait(false);
                        res.Response = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        res.Result = response?.StatusCode == HttpStatusCode.OK;
                        break;
                    case RequestType.CheckETag:
                        if (!string.IsNullOrEmpty(CurrentAuthToken))
                            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentAuthToken);
                        req = new HttpRequestMessage(HttpMethod.Head, url);
                        response = await HttpClient.SendAsync(req).ConfigureAwait(false);
                        res.Response = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        res.ETag = response?.Headers?.GetValues("ETag").FirstOrDefault();
                        res.Result = response?.StatusCode == System.Net.HttpStatusCode.OK;
                        break;
                }
                watch.Stop();
                Debug.WriteLine("Elapsed: {0}", watch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                var msg = e.InnerException?.Message ?? e.Message;
                res.Error = msg;
                return res;
            }
            finally
            {
                _isBusy = false;
            }
            return res;
        }

        public static bool CheckConnection(bool showDialog = false)
        {
            var res = CrossConnectivity.Current.IsConnected;
            if (res) return true;
            if (showDialog)
            {
                App.Current.MainPage.DisplayAlert("Error", "Check your internet connection", "Return");
               /* DialogPage.Show(DialogType.Error, HPHData.LocalizableText("Error"), HPHData.LocalizableText("Check your internet connection"), new List<PopupPageButton>
                {
                    new PopupPageButton(PopupPageButtonType.Blue, HPHData.LocalizableText("Return"))
                });*/
            }
            return false;
        }


        //Tuple : <result, isUrl, content>
        public static async Task<Tuple<bool, bool, string>> GetHpIdRedirectLink()
        {
            // Fill request params
            var region = "";
#if TURKEY
            region = "TR";
#else
            try
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                if (ci.TwoLetterISOLanguageName.ToLower() == "tr")
                    region = "TR";
                else if (ci.Name.Split('-')[1].ToLower() == "tr")
                    region = "TR";
            }
            catch { }
#endif
            var str = JsonConvert.SerializeObject(new
            {
                Email = "hphero@nyjec.com",
                DeviceId = Settings.AppDeviceGuid.ToString(),
                Region = region
            });
            var content = new StringContent(str, System.Text.Encoding.UTF8, "application/json");

            // Execute request
            var response = await ExecuteCommand(ApiServerUrl + PGConst.API_NEW_AUTH, content, RequestType.Post, false);
            if (!response.Result) return new Tuple<bool, bool, string>(false, false, response.Response);
            var res = response.Response;

            if (res.Contains("url"))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(res);
                Settings.LogoutUrl = dict["logout_url"];
                return new Tuple<bool, bool, string>(true, true, dict["url"]);
            }
            return new Tuple<bool, bool, string>(false, false, response.Response);
        }

        //Tuple result, authtoken/errormessage 
        public static async Task<Tuple<bool, string>> GetNewAuthToken(string url)
        {
            // Execute request
            var response = await ExecuteCommand(url, null, RequestType.Get, false);
            var errMsg = "Authorization Error";
            Dictionary<string, string> content = null;
            try
            {
                content = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Response);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, errMsg);
            }

            if (response.Result)
            {
                var token = string.Empty;
                if (content.ContainsKey("AuthToken"))
                    token = content["AuthToken"];
                return string.IsNullOrWhiteSpace(token) ? new Tuple<bool, string>(false, errMsg) : new Tuple<bool, string>(true, token);
            }

            if (response.Response.Contains("Message"))
                errMsg = content["Message"];
            return new Tuple<bool, string>(false, errMsg);
        }

    }

}
