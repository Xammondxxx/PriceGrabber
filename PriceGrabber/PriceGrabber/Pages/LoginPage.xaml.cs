using PriceGrabber.Core;
using PriceGrabber.CustomControls;
using PriceGrabber.DependencyServices;
using PriceGrabber.Helpers;
using PriceGrabber.Views;
using PriceGrabber.WebService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage, IBrowserParentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            BtnLogin.WidthRequest = StaticDeviceInfo.WidthDp * 0.35;
            BtnLogin.Clicked += BtnLoginClicked;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += GetSupportTapped;
            LblGetSupport.GestureRecognizers.Add(tapGesture);
            LblGetSupport.Text = "Get support";
        }

        private void GetSupportTapped(object sender, EventArgs e)
        {
            ShowBrowser("Support", "https://app-designer.itcs.hp.com/form/view/243?AppId=PriceGrabber&PullToRefresh=false");
        }

        protected override async void OnAppearing()
        {
            await Task.Delay(200);
            if (!string.IsNullOrWhiteSpace(Core.Settings.AuthToken))
                BtnLoginClicked(null, null);
        }

        private async void BtnLoginClicked(object sender, EventArgs e)
        {
            if (BtnLogin.Opacity < 1) return;
            if (!PGService.CheckConnection(true)) return;
            Indicator.IsRunning = true;
            Indicator.IsVisible = true;
            BtnLogin.Opacity = 0.4;
            if (!string.IsNullOrWhiteSpace(Core.Settings.AuthToken))
            {
                // Check if authtoken about to expired
                var res = await LoginHelper.CheckAuthTokenExpiration();
                if (!res)
                    LoginFailed();
                else
                    LoginFinish();
            }
            else
                LoginStart();
        }

        private async Task<bool> LoginStart()
        {
            
            DependencyService.Get<IWebCacheHelper>().ClearCookies();
            var res = await PGService.GetHpIdRedirectLink();

            if (res.Item1 && res.Item2)
            {
                var urldata = UrlParser.Parse(res.Item3);
                var webViewerView = new WebViewerView(
                    BrowserType.NewLogin,
                    this);
                webViewerView.Uri =  urldata.AbsoluteUrl;
                App.Current.MainPage = new ContainerPage(webViewerView);
                webViewerView.GoHome();
                webViewerView.Title = "";
                return true;
            }

            var msg = res.Item3;
            if (string.IsNullOrWhiteSpace(msg)) msg = "Unknown error";
            await App.Current.MainPage.DisplayAlert("Error", msg, "Close");

            return false;
        }

        private WebViewerView browser;
        public void ShowBrowser(string title, string url, BrowserType type = BrowserType.Cached, bool hideTitle = false)
        {
            if (string.IsNullOrEmpty(url)) return;
            var urldata = UrlParser.Parse(url);
            if (string.IsNullOrEmpty(urldata.AbsoluteUrl) || !PGService.CheckConnection(true)) return;

            if (browser == null)
            {
                browser = new WebViewerView(type, this, true, true, showTitlePanel: !hideTitle);
                browser.WidthRequest = Bounds.Width;
                browser.HeightRequest = Bounds.Height;
                browser.OnReturn += BrowserOnReturn;
            }
            browser.Uri = urldata.AbsoluteUrl;
            browser.GoHome();
            browser.Title = title;
            browser.IsVisible = true;
            GridLogin.IsVisible = false;
            SLContent.Children.Add(browser);

        }

        private void BrowserOnReturn()
        {
            browser.IsVisible = false;
            GridLogin.IsVisible = true;
        }

        private static void SetOrientation(Orientation orientation)
        {
            var depService = DependencyService.Get<IOrientationService>();
            if (orientation == Orientation.All)
                depService.ResetOrientation(depService.CurrentDeviceOrientation);
            else
                depService.LockOrientation(orientation);
        }

        bool successNavigating = false;
        public void DidStartNavigation(object sender, string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            Debug.WriteLine("LoginPage StartNavigating: {0}", url);
            successNavigating = url.Contains("callback?code");
            if (successNavigating) LoginCallback(url);
        }

        public void DidFinishNavigation(object sender, string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            successNavigating = url.Contains("callback?code");
            if (successNavigating)
            {
                LoginCallback(url);
                return;
            }
        }

        public void DidFailedNavigation(object sender, string url)
        {
        }

        public void ProgressNavigation(object sender, string url, int progress)
        {
        }

        public void SubLinkRequested(object sender, UrlData urlData)
        {
        }

        public void ExternalLinkRequested(object sender, string url)
        {
        }

        public void DocLinkRequested(object sender, string url)
        {
        }

        private bool _loginCallbackProcess;
        private bool _authTokenReceived;
        public async void LoginCallback(string callbackUrl)
        {
            try
            {
                BtnLogin.IsEnabled = false;
                BtnLogin.BackgroundColor = Color.LightGray;
            }
            catch(Exception ex) {

            }

            while (_loginCallbackProcess)
                await Task.Delay(200);

            (App.Current.MainPage as ContainerPage)?.ShowActivityIndicator();

            _loginCallbackProcess = true;
            try
            {
                if (string.IsNullOrWhiteSpace(callbackUrl) || _authTokenReceived) return;
               
                var res = await PGService.GetNewAuthToken(callbackUrl);
                if (res.Item1)
                {
                    _authTokenReceived = true;
                    Core.Settings.AuthToken = res.Item2;
                    Core.Settings.AuthTokenExpiredAt = res.Item3;
                    var ssoData = await PGService.GetSSOData();
                    if (ssoData == null)
                    {
                        LoginFailed();
                        return;
                    }
                    Core.Settings.SsoData = ssoData;
                    LoginFinish();
                }
                else
                {
                    LoginFailed();
                }
            }
            finally
            {
                _loginCallbackProcess = false;
            }
        }

        private void LoginFailed()
        {
            //await HPHService.SignOut();
            Core.Settings.AuthToken = string.Empty;
            Core.Settings.AuthTokenExpiredAt = DateTime.MinValue;
            Core.Settings.SsoData = null;
            _authTokenReceived = false;
            GridLogin.IsVisible = true;
            Indicator.IsRunning = false;
            Indicator.IsVisible = false;
            BtnLogin.Opacity = 1;
        }

        private void LoginFinish()
        {
            Indicator.IsRunning = false;
            Indicator.IsVisible = false;
            FontHelper.SetFonts(false);
            App.Current.MainPage = new MainPage();
        }

        public bool BackButtonPressed()
        {
            if (browser == null) return false;
            BrowserOnReturn();
            return true;
        }
    }
}