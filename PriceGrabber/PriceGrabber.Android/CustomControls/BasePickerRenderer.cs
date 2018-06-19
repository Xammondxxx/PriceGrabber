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

[assembly: ExportRenderer(typeof(BasePicker), typeof(BasePickerRenderer))]
namespace PriceGrabber.Droid.CustomControls
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class BasePickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            var picker = e.NewElement as BasePicker;

            if (Control != null)
            {
                if (picker.HasTitle)
                    Control.Text = picker.Title;
                Control.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)picker.FontSize);
                Control.Hint = picker.PlaceHolder;
                Control.InputType = InputTypes.TextFlagNoSuggestions;
                Control.Background = this.Resources.GetDrawable(Resource.Drawable.CornerEntry);
                if (picker.Padding != null)
                {
                    var density = Resources.DisplayMetrics.Density;
                    this.Control.SetPadding(
                         (int)(picker.Padding.Left * density),
                         (int)(picker.Padding.Top * density),
                         (int)(picker.Padding.Right * density),
                         (int)(picker.Padding.Bottom * density));
                }
                picker.PlaceHolderChanged += Picker_PlaceHolderChanged;
            }

        }

        private void Picker_PlaceHolderChanged(string obj)
        {
            if(Control != null)
                 Control.Hint = obj;
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}