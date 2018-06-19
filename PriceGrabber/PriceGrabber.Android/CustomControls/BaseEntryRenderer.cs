using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using PriceGrabber.CustomControls;
using PriceGrabber.Droid.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BaseEntry), typeof(BaseEntryRenderer))]
namespace PriceGrabber.Droid.CustomControls
{
    public class BaseEntryRenderer  : EntryRenderer
    {
        public BaseEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            base.OnElementChanged(e);
            var entry = e.NewElement as BaseEntry;

            if (Control != null)
            {
                Control.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)entry.FontSize);
                Control.Hint = entry.Placeholder;
                Control.Background = Resources.GetDrawable(Resource.Drawable.CornerEntry);
                Control.Gravity = GravityFlags.CenterVertical;
                if(entry.Keyboard == Keyboard.Numeric)
                        Control.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagSigned | Android.Text.InputTypes.NumberFlagDecimal;
                
                if (entry.Padding != null)
                {
                    var density = Resources.DisplayMetrics.Density;
                    this.Control.SetPadding(
                         (int)(entry.Padding.Left * density),
                         (int)(entry.Padding.Top * density),
                         (int)(entry.Padding.Right * density),
                         (int)(entry.Padding.Bottom * density));
                }
            }
        }
    }
}