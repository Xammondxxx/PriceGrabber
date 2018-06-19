using PriceGrabber.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.Pages.PriceGrabber
{
    public class CustomNavigationPage : ContentPage
    {
        public ContentPage ParentPage { get; private set; }
        public NavigationPanelView NavigationPanel { get; private set; }

        public CustomNavigationPage()
        {
          
        }

        protected virtual void AddNavigationPanel(ContentPage parent)
        {
            ParentPage = parent;
            NavigationPanel = new NavigationPanelView(GetModule(), this, parent);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected virtual Module GetModule()
        {
            return Module.PriceGrabber;
        }
    }
}
