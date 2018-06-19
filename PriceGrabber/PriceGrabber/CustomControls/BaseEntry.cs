using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.CustomControls
{
    public  class BaseEntry : Entry
    {
        public static readonly BindableProperty PaddingProperty =
        BindableProperty.Create("Padding", typeof(Thickness), typeof(BasePicker), defaultValue: new Thickness(20, 0, 0, 0));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public int FontSize { get; set; } = 15;


    }
}
