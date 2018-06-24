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
    public partial class EntryPicker : StackLayout
    {
        public EntryPicker()
        {
            InitializeComponent();
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += ArrowImageTapped;
            ArrowImage.GestureRecognizers.Add(tgr);

            PickerCtrl.SelectedIndexChanged += PickerCtrl_SelectedIndexChanged;
        }

        private void PickerCtrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var idx = PickerCtrl.SelectedIndex;
            var txt = PickerCtrl.Items[idx];
            EntryCtrl.Text = txt;
        }

        private void ArrowImageTapped(object sender, EventArgs e)
        {
            if (PickerControl.IsFocused)
                PickerControl.Unfocus();
            else
                PickerControl.Focus();
        }

        public BasePicker PickerControl
        {
            get { return this.PickerCtrl; }
            set { this.PickerCtrl = value; }
        }

        public BaseEntry EntryControl
        {
            get { return this.EntryCtrl; }
            set { this.EntryCtrl = value; }
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