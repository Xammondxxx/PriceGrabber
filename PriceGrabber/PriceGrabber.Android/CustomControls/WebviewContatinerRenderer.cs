using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using PriceGrabber.Droid.CustomControls;
using PriceGrabber.CustomControls;
using Android.Support.V4.Widget;
using Android.Graphics;
using Android.Webkit;
using Android.Views.Animations;
using System.Timers;
using PriceGrabber.Droid.DependencyServices;
using System.Threading.Tasks;
using System.Net;
using PriceGrabber.WebService;

[assembly: Xamarin.Forms.ExportRenderer(typeof(WebviewContainer), typeof(WebviewContainerRenderer))]
namespace PriceGrabber.Droid.CustomControls
{
    public class WebviewContainerRenderer : ViewRenderer<WebviewContainer, View>
    {
        public static string AssetsPath = "file:///android_asset/";
        public static string LoginUrl = "file:///android_asset/Login.html";
        public static string ErrorLogUrl = "file:///android_asset/ErrorLog.html";

        public bool IsTitleReceived = false;
        public WebViewer WebViewer { get; private set; }
        public SwipeRefreshLayout RefreshControl { get; private set; }
        public WebView Native;
        WebViewDelegate webViewDelegate;
        WebChromeDelegate webChromeDelegate;
        string homeUrl;
        public string HomeUrl => homeUrl;
        public bool CanExecuteJScript { get; private set; }



        public readonly List<string> LoginUrls = new List<string>();


        public WebviewContainerRenderer()
        {
        }


        protected override void OnElementChanged(ElementChangedEventArgs<WebviewContainer> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                System.Diagnostics.Debug.WriteLine("WebViewRenderer DISPOSE");

            }


            if (e.NewElement != null)
            {
                WebViewer = (e.NewElement as WebviewContainer).WebViewer;

                WebViewer.OnRotate += WebViewer_OnRotate;

                webViewDelegate = new WebViewDelegate(this);
                webChromeDelegate = new WebChromeDelegate(this);

                Native = new WebView(this.Context);
                Native.SetWebViewClient(webViewDelegate);
                Native.SetWebChromeClient(webChromeDelegate);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                    Native.SetLayerType(LayerType.Hardware, null);
                else
                    Native.SetLayerType(LayerType.Software, null);
                Native.Settings.JavaScriptEnabled = true;
                Native.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                Native.Settings.AllowFileAccess = true;
                Native.Settings.DomStorageEnabled = true;
                Native.Settings.DatabaseEnabled = true;
                Native.Settings.AllowFileAccess = true;
                Native.DrawingCacheEnabled = true;
                Native.Settings.MediaPlaybackRequiresUserGesture = false;
                Native.Settings.LoadWithOverviewMode = true;
                Native.Settings.UseWideViewPort = true;
                Native.Settings.BuiltInZoomControls = false;
                Native.Settings.LoadWithOverviewMode = true;
                //Native.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;
                Native.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
                Native.ScrollbarFadingEnabled = true;
                Native.Settings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.SingleColumn);
                Native.Settings.SetAppCacheEnabled(true);
                Native.Settings.SetAppCacheMaxSize(1024 * 1024 * 32);
                Native.Settings.SetAppCachePath(MainActivity.Instance.CacheDir.AbsolutePath);
                Native.Settings.SetGeolocationEnabled(true);
                Native.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

                WebViewer.BaseUrl = WebViewer.Uri;
                //refreshControl = new CustomRefreshControl(this.Context, Native);
                RefreshControl = new SwipeRefreshLayout(this.Context);
                RefreshControl.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

                RefreshControl.SetBackgroundColor(Color.Transparent);
                RefreshControl.Refresh += RefreshControlRefresh;
                if (Native.Parent == null) RefreshControl.AddView(Native);
                //RefreshControl.SetDistanceToTriggerSync(50);
                RefreshControl.AddView(new LoadingOverlay(MainActivity.Instance.ApplicationContext, false, WebViewer.Orientation));
                SetNativeControl(RefreshControl);
                

                if (string.IsNullOrEmpty(homeUrl)) homeUrl = WebViewer.Uri;
                WebViewer.PropertyChanged += (sender, ea) =>
                {
                    if (ea.PropertyName == "Action")
                    {
                        switch (WebViewer.Action)
                        {
                            case "GoHome":
                                GoHome(WebViewer.NeedCheckEtag);
                                break;
                        }
                        WebViewer.Action = string.Empty;
                    }
                    else if (ea.PropertyName == "Uri")
                    {
                        LoadUrl();
                    }
                };
                LoadUrl();
            }

        }

        private void WebViewer_OnRotate()
        {
            if (Native == null) return;
        }

       

        void GoHome(bool needCheckEtag = false)
        {
            if (string.IsNullOrEmpty(Native?.Url)) return;
            var naturl = UrlParser.Parse(Native.Url);
            if (naturl.AbsoluteUrl == homeUrl) return;
            WebViewer.Uri = homeUrl;
            LoadUrl(needCheckEtag: needCheckEtag);
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (RefreshControl != null && ManageNativeControlLifetime)
                {
                    RefreshControl.OnFocusChangeListener = null;
                    RefreshControl.RemoveView(Native);
                    RefreshControl.Refresh -= RefreshControlRefresh;
                    RefreshControl.RemoveFromParent();
                    Native.SetWebViewClient(null);
                    Native.SetWebChromeClient(null);
                    Native = null;
                    webViewDelegate = null;
                    webChromeDelegate = null;
                    RefreshControl = null;
                    WebViewer = null;
                }
                _disposed = true;
            }

            base.Dispose(disposing);
        }


        public void SubLinkRequested(UrlData urlData)
        {
            if (WebViewer?.ParentPage != null)
                WebViewer.ParentPage.SubLinkRequested(WebViewer, urlData);
        }

        public void ExternalLinkRequested(string url)
        {
            if (WebViewer?.ParentPage != null)
                WebViewer.ParentPage.ExternalLinkRequested(WebViewer, url);
        }

        public void DocLinkRequested(string url)
        {
            if (WebViewer?.ParentPage != null)
                WebViewer.ParentPage.DocLinkRequested(WebViewer, url);
        }

        public async void LoadUrl(bool fullRefresh = false, bool needCheckEtag = true)
        {
            if (Native == null || WebViewer == null) return;

            try
            {
                if (WebViewer.BrowserType == BrowserType.NewLogin)
                {
                    if (WebViewer.Uri == null) return;
                    Native.LoadUrl(WebViewer.Uri);
                    return;
                }

                if (needCheckEtag)
                    if (!fullRefresh && WebViewer.BrowserType != BrowserType.Background)
                    {
                        ShowLoadingOverlay();
                      //  fullRefresh = await PGService.IsWebPageContentChanged(WebViewer?.Uri);
                    }

                string baseUrl = string.Empty, query = string.Empty;
                if (WebViewer == null) return;
                if (WebViewer.BrowserType == BrowserType.Standard || WebViewer.BrowserType == BrowserType.Cached || WebViewer.BrowserType == BrowserType.Background)
                {
                    var urldata = UrlParser.Parse(WebViewer.Uri);
                    if (string.IsNullOrEmpty(homeUrl)) homeUrl = urldata.AbsoluteUrl;
                    /*  Native.Settings.CacheMode = HPHService.CheckConnection() ?
                          fullRefresh || (urldata?.Params != null && urldata.Params.ContainsKey("RefreshOnReturn")) ?
                              CacheModes.NoCache :
                              CacheModes.Normal :
                          CacheModes.CacheOnly;*/

                    Native.Settings.CacheMode = PGService.CheckConnection() && (fullRefresh || (urldata?.Params != null && urldata.Params.ContainsKey("RefreshOnReturn")))
                        ? CacheModes.Normal
                        : CacheModes.CacheElseNetwork;
                    var check = Native.Settings.CacheMode;
                    if (string.IsNullOrEmpty(urldata.AbsoluteUrl)) return;

                    if (urldata.AbsoluteUrl.StartsWith(PGService.ApiServerUrl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(PGService.CurrentAuthToken))
                        {
                            var headers = new Dictionary<string, string>
                            {
                                { "Authorization", "Bearer " + PGService.CurrentAuthToken }
                            };
                            Native.LoadUrl(urldata.AbsoluteUrl, headers);
                        }
                        else
                        {
                            var loc = new Localize();
                            urldata.AddParam("CountryCode", loc.GetCurrentCountry());
                            urldata.AddParam("Language", loc.GetCurrentLanguage());
                            Native.LoadUrl(urldata.AbsoluteUrl);
                        }
                    }
                    else
                    {
                        /*if (urldata.Url.Contains("id.hp.com"))
                            SetHpIdCookie(urldata.Url);*/
                        Native.LoadUrl(urldata.AbsoluteUrl);
                    }
                }

                else if (WebViewer.BrowserType == BrowserType.Login || WebViewer.BrowserType == BrowserType.ErrorLog)
                {
                    CanExecuteJScript = true;
                    switch (WebViewer.BrowserType)
                    {
                        case BrowserType.Login:
                            Native.LoadUrl(WebViewer.Uri);
                            break;
                        case BrowserType.ErrorLog:
                            Native.LoadUrl(ErrorLogUrl);
                            break;
                    }
                }
            }
            catch { }
        }

        bool hideLoadingSpinner;
        void RefreshControlRefresh(object sender, EventArgs e)
        {
            if (WebViewer?.PullToRefreshOff ?? true)
            {
                if (RefreshControl != null)
                    RefreshControl.Refreshing = false;
                return;
            }
            WebViewer.Uri = homeUrl;
            hideLoadingSpinner = true;
            LoadUrl(true);
        }

        public void RequestLoadStarted(string url)
        {
            //System.Diagnostics.Debug.WriteLine("RequestLoadStarted " + url);
            if (WebViewer != null) ShowLoadingOverlay();
            if (WebViewer?.ParentPage != null)
                WebViewer.ParentPage.DidStartNavigation(WebViewer, url);
            requestFailed = false;
        }

        public void RequestLoadFinished(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            //System.Diagnostics.Debug.WriteLine("RequestLoadFinished " + url);
            if (!url.Contains("ErrorLog.html"))
            {
                CloseLoadingOverlay();
                if (RefreshControl != null)
                    RefreshControl.Refreshing = false;
                if (!requestFailed) CloseErrorOverlay();
            }
            

            if (WebViewer.ParentPage != null)
            {
                WebViewer.ParentPage.DidFinishNavigation(WebViewer, url);
                if (CanExecuteJScript)
                {
                    ExecuteJScript();
                }
            }
        }

        bool requestFailed;
        public void RequestLoadFailed(string url, string error)
        {
      
            CloseLoadingOverlay();
            if (RefreshControl != null)
                RefreshControl.Refreshing = false;
            if (WebViewer?.ParentPage != null)
                WebViewer.ParentPage.DidFailedNavigation(WebViewer, url);
            ShowErrorOverlay(url, error);
            requestFailed = true;
        }

        LoadingOverlay loadingOverlay;
        bool didStart;
        public void ShowLoadingOverlay()
        {
            if (WebViewer?.BrowserType == BrowserType.Login || WebViewer?.BrowserType == BrowserType.Background || Native == null) return;
            if (!didStart)
            {
                if (loadingOverlay == null)
                    loadingOverlay = new LoadingOverlay(MainActivity.Instance.ApplicationContext, hideLoadingSpinner, WebViewer.Orientation);
                if (loadingOverlay.Parent == null)
                    Native.AddView(loadingOverlay);
                //((ViewGroup)loadingOverlay.Parent).RemoveView(loadingOverlay);
                loadingOverlay.SetSpinnerVisible(!hideLoadingSpinner);
                didStart = true;
            }
        }

        public void CloseLoadingOverlay()
        {
            if (WebViewer?.BrowserType == BrowserType.Login || WebViewer?.BrowserType == BrowserType.Background || Native == null) return;
            if (loadingOverlay != null)
                Native.RemoveView(loadingOverlay);
            hideLoadingSpinner = false;
            didStart = false;
        }

        ErrorOverlay errorOverlay;
        public void ShowErrorOverlay(string url, string error)
        {
            if (WebViewer?.BrowserType == BrowserType.Login || WebViewer?.BrowserType == BrowserType.Background || Native == null) return;
            if (errorOverlay == null)
                errorOverlay = new ErrorOverlay(MainActivity.Instance.ApplicationContext);
            errorOverlay.Init(url, error);
            if (errorOverlay.Parent == null)
                Native.AddView(errorOverlay);
            errorOverlay.Show();
        }

        public void CloseErrorOverlay()
        {
            if (WebViewer?.BrowserType == BrowserType.Login || WebViewer?.BrowserType == BrowserType.Background || Native == null) return;
            if (errorOverlay != null)
                Native.RemoveView(errorOverlay);
        }

        public void ExecuteJScript()
        {
            if (WebViewer == null || Native == null || !CanExecuteJScript) return;

            string script = string.Empty;
            if (WebViewer.BrowserType == BrowserType.Login)
            {
                try
                {
                    //   var esm = WebViewer.Params["Esm"];
                    var login = WebViewer.Params["Login"];
                    //  var password = WebViewer.Params["Password"];
                    script = string.Format("post('{0}','{1}','{2}');", null, login, null);
                }
                catch (Exception ex)
                {
                    script = string.Empty;
                    Console.WriteLine("Parsing login jscript error: " + ex);

                }
            }
            else if (WebViewer.BrowserType == BrowserType.ErrorLog)
            {
                try
                {
                    var action = WebViewer.Params["Link"];
                    var deviceId = WebViewer.Params["DeviceId"];
                    var platform = WebViewer.Params["Platform"];
                    var appVersion = WebViewer.Params["AppVersion"];
                    var text = "";// WebViewer.Params["ErrorLog"];

                    script = string.Format("post('{0}','{1}','{2}','{3}','{4}');", action, deviceId, platform, appVersion, text);
                }
                catch
                {
                    script = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(script))
                Native.EvaluateJavascript(script, null);
            CanExecuteJScript = false;
        }

        public bool IsValidTitle(string title)
        {
            if (title == null) return false;

            return !title.Contains("{{") && !title.Contains("}}") && !title.Contains("_");
        }



        public class WebViewDelegate : WebViewClient
        {
            const string JS_STOP_PROGRESS = "try { SmartPortal.UI.Screen.loading(false,true) } catch(err) {}";
            //public bool IsTrackingStartFinishEvents { get; set; } = true;

            public WebViewDelegate()
            {

            }

            public WebViewDelegate(WebviewContainerRenderer _renderer)
            {
                Renderer = _renderer;
            }

            WeakReference renderer;
            WebviewContainerRenderer Renderer
            {
                get
                {
                    if (!renderer.IsAlive) return null;
                    return (WebviewContainerRenderer)renderer.Target;
                }
                set
                {
                    if (value == null) renderer = null;
                    else renderer = new WeakReference(value);
                }
            }

            public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
            {
                var s = request.RequestHeaders.ToList();
                return base.ShouldInterceptRequest(view, request);
            }

            public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
            {
                
                Renderer?.RequestLoadStarted(url);

                base.OnPageStarted(view, url, favicon);
            }


            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                GetHeaders(view, url);
               // var elapsed = NetActivityLog.Finish(url);
                if (url.Contains("http"))
                {
                 //   GoogleAnalyticsHelper.SendTiming(elapsed, "UrlRequest", url, "Loading");
                }
                Renderer?.RequestLoadFinished(url);

                //if (!IsTrackingStartFinishEvents) return;
                var webViewer = Renderer?.WebViewer;
                if (webViewer != null)
                    webViewer.ActualUrl = url;

                if (webViewer?.ParentPage != null)
                {
                    //webViewer.ParentPage.DidFinishNavigation(webViewer, url);


                    if (string.IsNullOrEmpty(webViewer?.Title))
                    {
                        try
                        {
                            Renderer.IsTitleReceived = false;
                            WaitingTitle();
                            /* Renderer.Native.EvaluateJavascript(
                                 "document.title",
                                 new JavaScriptResult(Renderer));*/

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Cannot evaluate jscript: document.title ", ex.Message);
                        }
                    }

                    if (Renderer?.CanExecuteJScript == true)
                    {
                        Renderer?.ExecuteJScript();
                    }

                    var vt = webViewer.Title;
                    if (string.IsNullOrEmpty(vt)) vt = url;
                    //GoogleAnalyticsHelper.SendView(vt);
                }

                base.OnPageFinished(view, url);
              
            }

            private void GetHeaders(WebView view, string url)
            {
              /*  //check if webpage was laoded from cache or not
                var callBackHandler = new JavaScriptResult(Renderer, JavaScriptResult.Type.HeadersInfo);
                callBackHandler.Url = url;
                view.EvaluateJavascript(@"(function() { 
                                        var req = new XMLHttpRequest();
                                        req.open('GET', document.location, false);
                                        req.send(null);
                                        var headers = req.getAllResponseHeaders().toLowerCase();
                                        return  headers;})()",
                                            callBackHandler);*/

            }

            private async void WaitingTitle()
            {
                try
                {
                    int maxIterations = 40, currentIterations = 0;
                    while (!Renderer.IsTitleReceived || currentIterations >= maxIterations)
                    {
                        Renderer.Native.EvaluateJavascript(
                                        "document.title",
                                        new JavaScriptResult(Renderer));
                        currentIterations++;
                        await Task.Delay(100);
                    }
                }
                catch { }
            }

            string _url;
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
             
                if (Renderer.WebViewer.BrowserType == BrowserType.NewLogin)
                {
                    Renderer?.LoginUrls.Add(url);
                    if (url.Contains("callback?code"))
                    {
                        //Renderer?.SaveHpIdCookie();
                        return true;
                    }
                }


                _url = url.ToString();
                if (string.IsNullOrEmpty(_url) ||
                    //_url.Contains("about:blank") ||
                    _url.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)) return true;
                //Debug.WriteLine("ShouldStartLoad Pre: " + _url);
                var urlData = UrlParser.Parse(_url);



                if (urlData.AbsoluteUrl.ToLower().Contains(".pdf") || urlData.AbsoluteUrl.ToLower().Contains(".ics"))
                {
                    if (PGService.CheckConnection(true))
                        //MainActivity.Instance.GetWritePermission();
                        Renderer?.DocLinkRequested(urlData.AbsoluteUrl);
                    return true;
                }
                else if (urlData.AbsoluteUrl.ToLower().Contains(".mp4"))
                {
                    if (PGService.CheckConnection(true))
                    {
                        Intent intent = new Intent(Intent.ActionView);
                        intent.SetDataAndType(Android.Net.Uri.Parse(url), "video/*");
                        view.Context.StartActivity(intent);
                    }
                    return true;
                }

                _url = urlData.AbsoluteUrl;

                var webViewer = Renderer?.WebViewer;
                if (webViewer == null) return false;

                if (!urlData.Url.Contains(PGService.ApiServerUrl) &&
                    !webViewer.IsExternal && (webViewer.BrowserType == BrowserType.Standard || webViewer.BrowserType == BrowserType.Cached))
                {
                    if (PGService.CheckConnection(true) && webViewer.ParentPage != null)
                        Renderer?.ExternalLinkRequested(urlData.AbsoluteUrl);
                    view.EvaluateJavascript(JS_STOP_PROGRESS, null);
                    return true;
                }
                
                System.Diagnostics.Debug.WriteLine("ShouldStartLoad: " + _url);
                if (webViewer.BrowserType != BrowserType.Login)
                    webViewer.Uri = _url;
                return false;
            }

            public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
            {
                base.OnReceivedError(view, request, error);
                System.Diagnostics.Debug.WriteLine("OnReceivedError {0} for request {1}", error.Description, request.Url.ToString());
                if (request.Url.ToString() == Renderer?.HomeUrl)
                    Renderer?.RequestLoadFailed(request.Url.ToString(), error.Description);
            }

            public override void OnReceivedHttpError(WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
            {
                var error = "HTTP error " + errorResponse.StatusCode + " " + errorResponse.ReasonPhrase;
                base.OnReceivedHttpError(view, request, errorResponse);
                System.Diagnostics.Debug.WriteLine("OnReceivedHttpError [" + error + "] for request " + request.Url.ToString());
                if (request.Url.ToString() == Renderer?.HomeUrl)
                    Renderer?.RequestLoadFailed(request.Url.ToString(), error);
            }

            public override void OnReceivedSslError(WebView view, SslErrorHandler handler, Android.Net.Http.SslError error)
            {
                var err = "SSL error " + error;
                base.OnReceivedSslError(view, handler, error);
                System.Diagnostics.Debug.WriteLine("OnReceivedSslError {0}", error.Url);
                if (error.Url.ToString() == Renderer?.HomeUrl)
                    Renderer?.RequestLoadFailed(_url, err);
            }

            public override void DoUpdateVisitedHistory(WebView view, string url, bool isReload)
            {
                base.DoUpdateVisitedHistory(view, url, isReload);
            }


            class JavaScriptResult : Java.Lang.Object, IValueCallback
            {

                public enum Type
                {
                    Title,
                    HeadersInfo
                }

                WeakReference renderer;
                WebviewContainerRenderer Renderer
                {
                    get
                    {
                        if (!renderer.IsAlive) return null;
                        return (WebviewContainerRenderer)renderer.Target;
                    }
                    set
                    {
                        if (value == null) renderer = null;
                        else renderer = new WeakReference(value);
                    }
                }

                public Type CurrentType { get; set; }
                public string Url { get; set; }

                public JavaScriptResult(WebviewContainerRenderer renderer, Type type = Type.Title)
                {
                    Renderer = renderer;
                    CurrentType = type;
                }

                public void OnReceiveValue(Java.Lang.Object value)
                {
                    switch (CurrentType)
                    {
                        case Type.Title: SetTitle(value); break;
                        case Type.HeadersInfo: SetNetActivity(value); break;
                    }
                }

                public void SetTitle(Java.Lang.Object value)
                {
                    try
                    {

                        var title = (string)value;
                        var webViewer = Renderer?.WebViewer;
                        if (!string.IsNullOrEmpty(title?.Replace("\"", string.Empty)) && webViewer != null)
                            if (Renderer.IsValidTitle(title))
                            {
                                webViewer.Title = title;
                                Renderer.IsTitleReceived = true;
                            }
                    }
                    catch { }
                }

                public void SetNetActivity(Java.Lang.Object value)
                {
                    if (value == null) return;

                    var headersData = (string)value;
                    try
                    {
                        var strValue = value.ToString();
                        var values = strValue.Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);

                        var strDate = values?.FirstOrDefault(x => x.Contains("date"))?.Replace("date:", string.Empty)?.Replace("gmt", string.Empty).Replace("\"", string.Empty)?.Trim();
                        DateTime? date = null;
                        if (strDate != null)
                        {
                            date = DateTime.ParseExact(strDate, "ddd, dd MMM yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                            date = date.Value.ToLocalTime();
                        }
                        string etag = values?.FirstOrDefault(x => x.Contains("etag"))?.Replace("etag:", string.Empty)?.Trim() ?? "";

                    }
                    catch { }
                }
            }


            class JavaScriptResultTest : Java.Lang.Object, IValueCallback
            {
                WeakReference renderer;
                WebviewContainerRenderer Renderer
                {
                    get
                    {
                        if (!renderer.IsAlive) return null;
                        return (WebviewContainerRenderer)renderer.Target;
                    }
                    set
                    {
                        if (value == null) renderer = null;
                        else renderer = new WeakReference(value);
                    }
                }

                public JavaScriptResultTest(WebviewContainerRenderer renderer)
                {
                    Renderer = renderer;
                }

                public void OnReceiveValue(Java.Lang.Object value)
                {
                    var title = (string)value;
                    if (title?.Contains("translate") == false)
                        Renderer.IsTitleReceived = true;
                }
            }
        }

        public class WebChromeDelegate : WebChromeClient
        {
            WeakReference renderer;
            WebviewContainerRenderer Renderer
            {
                get
                {
                    if (!renderer.IsAlive) return null;
                    return (WebviewContainerRenderer)renderer.Target;
                }
                set
                {
                    if (value == null) renderer = null;
                    else renderer = new WeakReference(value);
                }
            }

            public WebChromeDelegate(WebviewContainerRenderer _renderer)
            {
                Renderer = _renderer;
            }

            public WebChromeDelegate()
            {

            }

            public override void GetVisitedHistory(IValueCallback callback)
            {
                base.GetVisitedHistory(callback);
            }

            public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
            {
                return base.OnJsAlert(view, url, message, result);
            }

            public override bool OnCreateWindow(WebView view, bool isDialog, bool isUserGesture, Message resultMsg)
            {
                return base.OnCreateWindow(view, isDialog, isUserGesture, resultMsg);
            }

            public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
            {
                return base.OnJsConfirm(view, url, message, result);
            }

            public override bool OnJsPrompt(WebView view, string url, string message, string defaultValue, JsPromptResult result)
            {
                return base.OnJsPrompt(view, url, message, defaultValue, result);
            }

            public override void OnHideCustomView()
            {
                base.OnHideCustomView();
            }

            public override void OnShowCustomView(View view, WebChromeClient.ICustomViewCallback callback)
            {
                base.OnShowCustomView(view, callback);
            }

            public override void OnProgressChanged(WebView view, int newProgress)
            {
                base.OnProgressChanged(view, newProgress);
                var webViewer = Renderer?.WebViewer;
                if (webViewer?.ParentPage != null)
                    webViewer.ParentPage.ProgressNavigation(webViewer, view.Url, newProgress);
            }

            public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
            {
                if (MainActivity.Instance == null) return false;
                Intent intent = fileChooserParams.CreateIntent();
                MainActivity.Instance.ChooseFile(filePathCallback, intent);
                return true;
            }

            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
            {
                // Always grant permission since the app itself requires location
                // permission and the user has therefore already granted it
                callback.Invoke(origin, true, false);
            }
        }

        public class LoadingOverlay : Android.Widget.RelativeLayout
        {
            Android.Widget.ProgressBar spinner;
            TextView label;
            ImageView image;

            public LoadingOverlay(Context context, bool withoutSpinner = false, PriceGrabber.CustomControls.Orientation orientation = PriceGrabber.CustomControls.Orientation.Portrait) : base(context)
            {
                Color mainColor = Color.LightBlue;
                try
                {
                    mainColor = ((Xamarin.Forms.Color)Xamarin.Forms.Application.Current.Resources["LightBlue"]).ToAndroid();
                }
                catch { }
                this.Alpha = 0.75f;
                this.SetBackgroundColor(mainColor);

                var contentHeight = (int)StaticDeviceInfo.Height;
                var contentWidth = (int)StaticDeviceInfo.Width;
                if (orientation == PriceGrabber.CustomControls.Orientation.Landscape)
                {
                    contentHeight = contentHeight + contentWidth;
                    contentWidth = contentHeight - contentWidth;
                    contentHeight = contentHeight - contentWidth;
                }

                //	SetMinimumWidth(contentWidth);
                //SetMinimumHeight(contentHeight);

                this.LayoutParameters = new ViewGroup.LayoutParams(-1, -1);

                LayoutParams lp;
                image = new ImageView(this.Context);
               // image.SetImageResource(Resource.Drawable.insigniaStar);
                image.SetMinimumHeight(contentHeight / 2);
                image.SetMinimumWidth(contentWidth / 2);

                lp = new LayoutParams(contentHeight / 2, contentWidth / 2);
                lp.AddRule(LayoutRules.CenterInParent);
                AddView(image, lp);



                spinner = new ProgressBar(this.Context);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    spinner.SetForegroundGravity(GravityFlags.CenterVertical | GravityFlags.CenterHorizontal);
                spinner.SetMinimumWidth(100);
                spinner.SetPadding(0, 0, 0, 50);
                spinner.IndeterminateDrawable.SetColorFilter(Color.White, PorterDuff.Mode.SrcIn);
                lp = new LayoutParams(100, 150);
                lp.AddRule(LayoutRules.CenterInParent);
                /*ViewGroup.LayoutParams vlp = new ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MatchParent,
                    ViewGroup.LayoutParams.MatchParent);*/

                if (!withoutSpinner && spinner.Parent == null)
                    AddView(spinner, lp);

                label = new TextView(this.Context); // { Text = HPHData.LocalizableText("Loading data") };
                label.Gravity = GravityFlags.Center;
                label.SetPadding(0, 100, 0, 0);
                label.SetTextColor(Color.White);
                label.SetText("Loading data", TextView.BufferType.Normal);
                try
                {
                    var tf = Typeface.CreateFromAsset(MainActivity.Instance.Assets, "HPSimplified_Lt.ttf");
                    label.SetTypeface(tf, TypefaceStyle.Normal);
                }
                catch { }
                lp = new LayoutParams(300, 150);
                lp.AddRule(LayoutRules.CenterInParent);
                if (!withoutSpinner && label.Parent == null)
                    AddView(label, lp);
            }


            public override void Draw(Canvas canvas)
            {
                base.Draw(canvas);

            }

            public void SetSpinnerVisible(bool visible)
            {
                if (spinner != null) spinner.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
                if (label != null) label.Visibility = visible ? ViewStates.Visible : ViewStates.Invisible;
            }

            public void Hide()
            {
                AlphaAnimation animation = new AlphaAnimation(1f, 0f);
                animation.Duration = 300;
                //   animation.StartOffset = 0;
                animation.FillAfter = true;
                this.StartAnimation(animation);

            }
        }

        public class ErrorOverlay : Android.Widget.RelativeLayout
        {
            TextView lbCaption, lbErCodeCaption, lbErrorCode, lbErrorMessage;
            ImageView image;

            public ErrorOverlay(Context context) : base(context)
            {
                this.Alpha = 1f;
                this.SetBackgroundColor(Android.Graphics.Color.White);

                var contentHeight = (int)StaticDeviceInfo.Height;
                var contentWidth = (int)StaticDeviceInfo.Width;

                SetMinimumWidth(contentWidth);
                SetMinimumHeight(contentHeight);

                image = new ImageView(this.Context);
                //image.SetImageResource(Resource.Drawable.Repair);
                image.SetMinimumHeight(100);
                image.SetMaxHeight(100);
                image.SetMinimumWidth(100);
                image.SetMaxWidth(100);
                image.SetPadding((int)(contentWidth / 3), 0, (int)(contentWidth / 3), (int)(contentHeight / 2.7));
                //   image.SetScaleType(ImageView.ScaleType.FitStart);
                //   LayoutParams par2 = new LayoutParams(contentHeight, contentWidth);
                //   par2.AddRule(LayoutRules.CenterHorizontal);

                ViewGroup.LayoutParams par = new ViewGroup.LayoutParams(
                     ViewGroup.LayoutParams.MatchParent,
                     ViewGroup.LayoutParams.MatchParent);
                AddView(image, par);


                lbCaption = new TextView(this.Context);
                lbCaption.SetText("We are sorry, but the app\nencountered a problem", TextView.BufferType.Normal);
                lbCaption.SetPadding(0, (int)(contentHeight / 2.3), 0, 0);
                lbCaption.TextSize = 17;
                SetLabelSettings(lbCaption);
                lbCaption.Gravity = GravityFlags.CenterHorizontal;

                lbErCodeCaption = new TextView(this.Context);
                lbErCodeCaption.SetText("Error Code", TextView.BufferType.Normal);
                lbErCodeCaption.SetPadding(0, (int)(contentHeight / 1.8), 0, 0);
                lbErCodeCaption.TextSize = 17;
                SetLabelSettings(lbErCodeCaption);
                lbErCodeCaption.Gravity = GravityFlags.CenterHorizontal;

                lbErrorCode = new TextView(this.Context);
                lbErrorCode.SetText("", TextView.BufferType.Normal);
                lbErrorCode.SetPadding(0, (int)(contentHeight / 1.7), 0, 0);
                lbErrorCode.TextSize = 21;
                SetLabelSettings(lbErrorCode);
                lbErrorCode.Gravity = GravityFlags.CenterHorizontal;

                lbErrorMessage = new TextView(this.Context);
                lbErrorMessage.SetText("", TextView.BufferType.Normal);
                lbErrorMessage.SetPadding(0, (int)(contentHeight / 1.4) + 0, 0, 0);
                lbErrorMessage.TextSize = 17;
                SetLabelSettings(lbErrorMessage);
                lbErrorMessage.Gravity = GravityFlags.CenterHorizontal;

                AddView(lbCaption, par);
                AddView(lbErCodeCaption, par);
                AddView(lbErrorCode, par);
                AddView(lbErrorMessage, par);
            }

            private void SetLabelSettings(TextView label)
            {
                label.SetTypeface(Android.Graphics.Typeface.CreateFromAsset(MainActivity.Instance.Assets, "HPSimplified_Lt.ttf"), TypefaceStyle.Normal);
                label.SetTextColor(Android.Graphics.Color.Gray);

            }

            public void Init(string url, string error)
            {
                var urlData = UrlParser.Parse(url);
                lbErrorCode.Text = ((int)(DateTime.UtcNow - DateTime.MinValue).TotalDays).ToString() +
                    (!string.IsNullOrEmpty(urlData.ViewId) ? "-" + urlData.ViewId : string.Empty);
                lbErrorMessage.Text = error;
            }

            public void Show()
            {
                this.Alpha = 1f;
            }

            public void Hide()
            {
                AlphaAnimation animation = new AlphaAnimation(1f, 0f)
                {
                    Duration = 300,
                    //   animation.StartOffset = 0;
                    FillAfter = true
                };
                this.StartAnimation(animation);
            }
        }
    }
}