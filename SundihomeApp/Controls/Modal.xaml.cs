using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class Modal : ContentView
    {
		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpControl), null, BindingMode.TwoWay);
		public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

		public Modal()
        {
            InitializeComponent();
        }
        public void Show()
		{
            
		}
        public void Hide()
		{
            
		}
    }
}
