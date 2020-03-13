using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SundihomeApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FormLabelRequired : ContentView
    {
        #region Placeholder
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text),
                typeof(string),
                typeof(FormLabelRequired),
                "",
                BindingMode.TwoWay);
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
        #endregion

        public FormLabelRequired()
        {
            InitializeComponent();
            lbl.SetBinding(Label.TextProperty, new Binding("Text")
            {
                Source = this
            });
        }
    }
}