﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.CustomControls
{
    public class BasePicker : Picker
    {
        public event Action<string> PlaceHolderChanged;

        public static readonly BindableProperty PaddingProperty =
          BindableProperty.Create("Padding", typeof(Thickness), typeof(BasePicker), defaultValue: new Thickness(20, 0, 0, 0));

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public int FontSize { get; set; } = 15;
        public bool HasTitle { get; set; } = false;

        string placeHolder;
        public string PlaceHolder
        {
            get { return placeHolder; }
            set
            {
                placeHolder = value;
                OnPlaceHolderChanged();
            }
        }


        private void OnPlaceHolderChanged()
        {
            PlaceHolderChanged?.Invoke(PlaceHolder);
        }
    }
}
