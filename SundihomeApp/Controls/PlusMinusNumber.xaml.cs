using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class PlusMinusNumber : ContentView
    {
        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(int?), typeof(LookUpControl), null, BindingMode.TwoWay);
        public int? Value { get => (int?)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }

        public PlusMinusNumber()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.Android)
            {
                radBorder.BackgroundColor = Color.FromHex("#e0dede");
            }
            else
            {
                radBorder.BackgroundColor = Color.White;
            }
            LblValue.BindingContext = this;
        }

        public void GridProperty_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Height")
            {
                double height = (double)this.Height - 4;
                double radius = height / 2;
                radBorder.HeightRequest = height + 4;
                btnDec.WidthRequest = btnDec.HeightRequest = height;
                BtnInc.WidthRequest = BtnInc.HeightRequest = height;

                btnDec.CornerRadius = (int)radius;
                BtnInc.CornerRadius = (int)radius;
                radBorder.CornerRadius = (int)radius;
            }
        }

        private void Increase_Clicked(object sender, EventArgs e)
        {
            if (Value.HasValue)
            {
                Value += 1;
            }
            else
            {
                Value = 1;
            }
        }
        private void Decrease_Clicked(object sender, EventArgs e)
        {
            if (Value.HasValue && Value.Value > 1)
            {
                Value -= 1;
            }
            else
            {
                Value = null;
            }
        }
    }
}
