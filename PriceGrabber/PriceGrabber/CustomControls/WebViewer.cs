using PriceGrabber.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.CustomControls
{
    public enum BrowserType
    {
        Standard,
        Cached,
        Login,
        ErrorLog,
        Background,
        NewLogin,
    }

    public enum Orientation
    {
        Portrait,
        Landscape,
        All             //<--only for CurrentScreenOrientation
    }

    public class WebViewer : WebView
    {
        

        public WebViewer(Orientation orientation = Orientation.Portrait, bool fullScreen = false)
        {
            this.Orientation = orientation;
            this.IsFullScreen = fullScreen;
        }

        public BrowserType BrowserType { get; set; }
        public bool IsPortalLogout { get; set; }
        public bool IsExternal { get; set; }
        public bool IsCarousel { get; set; }
        public bool IsFullScreen { get; set; }
        public bool NeedCheckEtag { get; set; } = true;
        public string LateUrl { get; set; }
        public string ActualUrl { get; set; }
        public string BaseUrl { get; set; }
        public Orientation Orientation { get; private set; }

        public void ApplyLateUrl()
        {
            if (string.IsNullOrEmpty(LateUrl)) return;
            Uri = LateUrl;
            LateUrl = null;
        }

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: nameof(Uri),
            returnType: typeof(string),
            declaringType: typeof(WebViewer),
            defaultValue: default(string));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public static readonly BindableProperty ActionProperty = BindableProperty.Create(
            propertyName: nameof(Action),
            returnType: typeof(string),
            declaringType: typeof(WebViewer),
            defaultValue: default(string));

        public string Action
        {
            get { return (string)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(WebViewer),
            defaultValue: default(string));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public IBrowserParentPage ParentPage;
        public bool PullToRefreshOff { get; set; }
        public Dictionary<string, object> Params { get; set; }

        public event Action OnReturn;
        public event Action OnRotate;


        public void RotateWebViewer()
        {
            OnRotate?.Invoke();
        }

        public void OnBackButtonTapped(object sender, EventArgs args)
        {
            OnReturn?.Invoke();
        }

      
    }
}
