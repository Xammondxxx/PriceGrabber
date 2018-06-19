using PriceGrabber.Core;
using PriceGrabber.CustomControls;
using PriceGrabber.DependencyServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WebViewerView : ContentView
    {
        public WebViewer Browser { get; private set; }
        public event Action OnTitleTripleTapped;
        public Orientation Orientation { get; private set; }
        private InitialSize Size { get; set; }
        private bool CanDoFullScreen { get; set; } = false;
        public static bool IsBackBtnLocked { get; set; } = false;

        public int DoFullScreenPeriod { get; set; } = 3000;  //3 sec
        
        public string ActualUrl => Browser?.ActualUrl;

        public WebViewerView(BrowserType type, IBrowserParentPage parentPage = null, bool withTopMargin = false, bool isExternal = false,
                             Orientation orientation = Orientation.Portrait, bool fullScreen = false, bool showTitlePanel = true)
        {
            InitializeComponent();

            this.Orientation = orientation;

            if (Device.RuntimePlatform == Device.iOS)
            {
                Browser = new WebViewer(orientation, fullScreen)
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                slWebContainer.Children.Add(Browser);
            }
            else
            {
                var container = new WebviewContainer()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                };
                container.WebViewer = new WebViewer(orientation, fullScreen);
                Browser = container.WebViewer;
                slWebContainer.Children.Add(container);
            }

            Browser.BrowserType = type;
            Browser.ParentPage = parentPage;
            Browser.IsExternal = isExternal;
            Browser.PropertyChanged += Browser_PropertyChanged;
            if (withTopMargin && Device.RuntimePlatform == Device.iOS)
                Content.Margin = new Thickness(0, 20, 0, 0);

            BindingContext = this;

        

            var tapGesture = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
            lblTitle.GestureRecognizers.Add(tapGesture);
            lblTitleView.GestureRecognizers.Add(tapGesture);
            tapGesture.Tapped += TapGesture_Tapped;

           // this.OnTitleTripleTapped += () => AppContentPage.Instance.WVTitleTripleClicked(this);

            // var tapGesture2= new TapGestureRecognizer();
            // lblTitle.GestureRecognizers.Add(tapGesture2);
            //  lblTitleView.GestureRecognizers.Add(tapGesture2);
            // tapGesture2.Tapped += TapGesture_Tapped1; ;
            if (!showTitlePanel)
            {
                rd0.Height = 0;
                lblTitleView.IsVisible = false;
            }
        }

        private void TapGesture_Tapped1(object sender, EventArgs e)
        {
            OnTitleTripleTapped?.Invoke();
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            OnTitleTripleTapped?.Invoke();
        }

        public void GoHome()
        {
            Browser.NeedCheckEtag = false;
            Browser.Action = "GoHome";
        }

        void Browser_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title") SetTitle();
        }

        void SetTitle()
        {
            var t = Title.Trim();
            if (t.Length > 0 && t[0] == '"') t = t.Substring(1);
            if (t.Length > 0 && t[t.Length - 1] == '"') t = t.Substring(0, t.Length - 1);
            lblTitle.Text = t;
            if (t.Length > 40) lblTitleView.Padding = new Thickness(StaticDeviceInfo.Height * 0.06 + 13, 0, 0, 0);
            else lblTitleView.Padding = new Thickness(0, 0, 0, 0);
        }

        public BrowserType BrowserType
        {
            get { return Browser.BrowserType; }
            set { Browser.BrowserType = value; }
        }

        public string Title
        {
            get { return Browser.Title; }
            set
            {
                if (Browser?.Title == value) return;
                Browser.Title = value;
                SetTitle();
            }
        }

        public string Uri
        {
            get { return Browser.Uri; }
            set { if (!string.IsNullOrEmpty(value.Trim())) Browser.Uri = value.Trim(); }
        }

        public bool PullToRefreshOff
        {
            get { return Browser.PullToRefreshOff; }
            set { Browser.PullToRefreshOff = value; }
        }

        public Dictionary<string, object> Params
        {
            get { return Browser.Params; }
            set { Browser.Params = value; }
        }

        public event Action OnReturn;

        void BackButtonClicked(object sender, EventArgs args)
        {
            if (IsBackBtnLocked)
            {
                IsBackBtnLocked = false;
                return;
            }

            CanDoFullScreen = false;
            OnReturn?.Invoke();
        }



       
        public void SetOrientation()
        {
            var depService = DependencyService.Get<IOrientationService>();
            if (depService.CurrentScreenOrientation != Orientation)
                depService.LockOrientation(Orientation);
        }
        
        protected override void InvalidateLayout()
        {
            base.InvalidateLayout();
        }
        
        public class InitialSize
        {
            public Rectangle? Container { get; set; }
            public Rectangle? View { get; set; }
        }

     
        Orientation CurOrientation = Orientation.All;
      

        private double ScreenHeight()
        {
            var statusBarHeigt = Device.RuntimePlatform == Device.iOS ? 20 : 0;
            var screenHeight = StaticDeviceInfo.Height - statusBarHeigt;
            return screenHeight;
        }

        private double ScreenWidht()
        {
            return StaticDeviceInfo.Width;
        }

        private double GetNavigationBarLandscapeSize()
        {
            double k = 0.12;
            if (Device.RuntimePlatform == Device.iOS)
                k = 0.06;
            var size = ScreenWidht() * k;
            return size;
        }


        private Orientation CurrentOrientaion()
        {
            return DependencyService.Get<IOrientationService>().CurrentScreenOrientation == Orientation.Landscape ? Orientation.Landscape : Orientation.Portrait;
        }

    }
}