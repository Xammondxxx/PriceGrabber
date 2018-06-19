using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContainerPage : ContentPage
    {
        public ContainerPage(ContentView view)
        {
            InitializeComponent();
            SetContentView(view);
        }

        public void SetContentView(ContentView view)
        {
            Grid.Children.Insert(0, view);
        }

        public async void ShowActivityIndicator()
        {
            await Task.Delay(50);
            activityIndicator.IsVisible = true;
            activityIndicator.IsRunning = true;
        }

        public async void HideActivityIndicator()
        {
            await Task.Delay(50);
            activityIndicator.IsVisible = false;
            activityIndicator.IsRunning = false;
        }
    }
}