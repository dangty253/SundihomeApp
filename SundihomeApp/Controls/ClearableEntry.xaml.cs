using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class ClearableEntry : ContentView
    {
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(LookUpControl), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(LookUpControl), null, BindingMode.TwoWay);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        public static readonly BindableProperty EntryKeyboardProperty = BindableProperty.Create(nameof(EntryKeyboard), typeof(Xamarin.Forms.Keyboard), typeof(LookUpControl), Xamarin.Forms.Keyboard.Text, BindingMode.TwoWay);
        public Xamarin.Forms.Keyboard EntryKeyboard { get => (Xamarin.Forms.Keyboard)GetValue(EntryKeyboardProperty); set => SetValue(EntryKeyboardProperty, value); }

        public ClearableEntry()
        {
            InitializeComponent();
            this.Entry.BindingContext = this;
            this.Entry.SetBinding(Entry.PlaceholderProperty, "Placeholder");
            this.Entry.SetBinding(Entry.TextProperty, "Text");
            this.Entry.SetBinding(Entry.KeyboardProperty, "EntryKeyboard");
            this.BtnClear.SetBinding(Button.IsVisibleProperty, new Binding("Text") { Source = this, Converter = new Converters.NullToHideConverter() });
        }

        public void Clear_Clicked(object sender, EventArgs e)
        {
            this.Text = null;
        }
    }
}
