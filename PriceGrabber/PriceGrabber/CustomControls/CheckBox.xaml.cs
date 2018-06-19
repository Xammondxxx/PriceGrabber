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
    public partial class CheckBox : StackLayout
    {
        public event Action<bool> CheckedChanged;

        public CheckBox()
        {
            InitializeComponent();

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += Tgr_Tapped;
            imgAcceptConditionCheck.GestureRecognizers.Add(tgr);
            imgAcceptConditionRect.GestureRecognizers.Add(tgr);
        }

        private void Tgr_Tapped(object sender, EventArgs e)
        {
            if (imgAcceptConditionCheck == null) return;

            if (imgAcceptConditionCheck.Source == null)
                imgAcceptConditionCheck.Source = ImageSource.FromFile("LightGrayCheck.png");
            else
                imgAcceptConditionCheck.Source = null;
            

            CheckedChanged?.Invoke(imgAcceptConditionCheck.Source != null);
        }

        public void Check()
        {
            imgAcceptConditionCheck.Source = "LightGrayCheck.png";
        }

        public void UnCheck()
        {
            imgAcceptConditionCheck.Source = null;
        }

        public Label TextLabel
        {
            get { return lbText; }
        }

        
    }
}