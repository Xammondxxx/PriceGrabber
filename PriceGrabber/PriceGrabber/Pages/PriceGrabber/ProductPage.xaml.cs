using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.Pages.PriceGrabber
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductPage : CustomNavigationPage
    {
        public ProductPage(ContentPage parent) : base()
        {
			InitializeComponent ();
		}
	}
}