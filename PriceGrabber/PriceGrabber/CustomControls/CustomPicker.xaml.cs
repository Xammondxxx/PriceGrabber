using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PriceGrabber.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomPicker : StackLayout
    {
        public CustomPicker()
        {
            InitializeComponent();
        }

        public BasePicker PickerControl
        {
            get { return this.PickerCtrl; }
            set { this.PickerCtrl = value; }
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (PickerControl.Height != 0)
            {
                ArrowImage.HeightRequest = PickerControl.Height;
                ArrowImage.WidthRequest = PickerControl.Height;
            }
            base.LayoutChildren(x, y, width, height);
        }
    }
}