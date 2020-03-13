using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class MenuItem : ContentView
    {
        public event EventHandler OnClicked;

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(MenuItem), null, BindingMode.TwoWay);
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(MenuItem), null, BindingMode.TwoWay);
        public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }

        public static readonly BindableProperty Font_FamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MenuItem), null, BindingMode.TwoWay);
        public string FontFamily { get => (string)GetValue(Font_FamilyProperty); set => SetValue(Font_FamilyProperty, value); }

        public MenuItem()
        {
            InitializeComponent();
            this.lblTitle.SetBinding(Label.TextProperty, new Binding("Title") { Source = this });
            this.lblIcon.SetBinding(Label.FontFamilyProperty, new Binding("FontFamily") { Source = this });
            this.lblIcon.SetBinding(Label.TextProperty, new Binding("Icon") { Source = this });

        }

        public void On_Clicked(object sender, EventArgs e)
        {
            this.OnClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
