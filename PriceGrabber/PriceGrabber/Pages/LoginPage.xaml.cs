using PriceGrabber.Core;
using PriceGrabber.CustomControls;
using PriceGrabber.DependencyServices;
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
            btnLogin.WidthRequest = StaticDeviceInfo.WidthDp * 0.35;
            btnLogin.Clicked += BtnLogin_Clicked;

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += GetSupportTapped;
            LblGetSupport.GestureRecognizers.Add(tapGesture);
            LblGetSupport.Text = "Get support";
        }

        private void GetSupportTapped(object sender, EventArgs e)
        {
            ShowBrowser("Support", "https://app-designer.itcs.hp.com/form/view/243?PullToRefresh=false");
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;
            await Task.Delay(100);
            await StartLogin();
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;
        }

        private async Task<bool> StartLogin()
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

        public void ShowBrowser(string title, string url, BrowserType type = BrowserType.Cached, bool hideTitle = false)
        {
            if (string.IsNullOrEmpty(url)) return;
            var urldata = UrlParser.Parse(url);
            if (string.IsNullOrEmpty(urldata.AbsoluteUrl) || !PGService.CheckConnection(true)) return;

            var browser = new WebViewerView(type, this, true, true, showTitlePanel: !hideTitle);
            browser.OnReturn += BrowserOnReturn;
            browser.Uri = urldata.AbsoluteUrl;
            browser.GoHome();
            browser.Title = title;
            browser.IsVisible = true;
        }

        private void BrowserOnReturn()
        {
            SetOrientation(Orientation.Portrait);
            App.Current.MainPage = new LoginPage();
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
                btnLogin.IsEnabled = false;
                btnLogin.BackgroundColor = Color.LightGray;
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
                    await LoginFinish(res.Item2);
                }
                else
                {
                   
                }
            }
            finally
            {
                _loginCallbackProcess = false;
            }
        }

        private async Task<bool> LoginFinish(string item2)
        {
            App.Current.MainPage = new MainPage();
            return true;
        }
    }
}