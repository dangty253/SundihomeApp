using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class ShowMoreLabel : ContentView
    {
        public static readonly BindableProperty MaxLinesProperty = BindableProperty.Create(nameof(MaxLines), typeof(int), typeof(LookUpControl), null, BindingMode.TwoWay);
        public int MaxLines { get => (int)GetValue(MaxLinesProperty); set => SetValue(MaxLinesProperty, value); }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(LookUpControl), null, BindingMode.TwoWay);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        public ShowMoreLabel()
        {
            InitializeComponent();
            this.Label.BindingContext = this;
            this.Label.SetBinding(Label.TextProperty, "Text");
            
        }
        private void OnShowMoreLabelClick(object sender, EventArgs e)
        {
            if(this.Label.MaxLines > 0) this.Label.MaxLines = 0;
            else this.Label.MaxLines = MaxLines;
        }
    }
}
