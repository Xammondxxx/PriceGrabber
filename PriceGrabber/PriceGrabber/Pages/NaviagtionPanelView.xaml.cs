using PriceGrabber.Core;
using PriceGrabber.Core.Attributes;
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
	public partial class NavigationPanelView : ContentView
	{
        ContentPage ParentPage;
        ContentPage CurrentPage;

        public NavigationPanelView (Module module, ContentPage currentPage,  ContentPage parent)
		{
            ParentPage = parent;
            CurrentPage = currentPage;
            InitializeComponent();
            Initialize(module);
            AddToPage();
		}

        private void AddToPage()
        {
            Grid grid = new Grid();
            grid.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition(){ Height = 60},
                new RowDefinition(){ Height = new GridLength(1, GridUnitType.Star)},
            };
            grid.Children.Add(this);
            grid.Children.Add(CurrentPage.Content);
            Grid.SetRow(grid.Children[1], 1);
            CurrentPage.Content = grid;
        }

        private void Initialize(Module module)
        {
            lbTitle.Text = module.GetTitleAttribute<Module, ModuleTitleAttribute>(AttributeExtensions.Type.Field)?.ToString();

            if (ParentPage == null)
                imgHome.Source = "HomeImg.png";
            else imgHome.Source = "ArrowLeft.png";

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += HomeBtnTapped;
            imgHome.GestureRecognizers.Add(tgr);
        }

        private void HomeBtnTapped(object sender, EventArgs e)
        {
            if (ParentPage == null)
                App.Current.MainPage = new MainPage();
            else App.Current.MainPage = ParentPage;
        }
    }
}