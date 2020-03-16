using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public delegate Task SomeEventHandler(object sender, EventArgs e);


    public partial class FilterPicker : StackLayout
    {
        private LookUpControl _lookUpControl;
        public event EventHandler PreOpen;
        public event EventHandler<LookUpChangeEvent> SelectedItemChange;
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FilterPicker), null, BindingMode.TwoWay);
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(FilterPicker), null, BindingMode.TwoWay, propertyChanged: ItemChanged);
        public object SelectedItem { get => (object)GetValue(SelectedItemProperty); set { SetValue(SelectedItemProperty, value); } }

        public static readonly BindableProperty NameDipslayProperty = BindableProperty.Create(nameof(SelectedItem), typeof(string), typeof(FilterPicker), null, BindingMode.OneWay);

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FilterPicker), null, BindingMode.TwoWay, null);
        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set { SetValue(ItemsSourceProperty, value); } }

        public BottomModal BottomModal { get; set; }

        public string NameDisplay { get => (string)GetValue(NameDipslayProperty); set { SetValue(NameDipslayProperty, value); } }

        public FilterPicker()
        {
            InitializeComponent();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += OnTapped;
            this.GestureRecognizers.Add(tap);

            this.lblText.SetBinding(Label.TextProperty, new Binding("Placeholder") { Source = this });
        }

        private async void OnTapped(object sender, EventArgs e)
        {
            if (_lookUpControl == null)
            {
                _lookUpControl = new LookUpControl();
                _lookUpControl.SetBinding(LookUpControl.ItemsSourceProperty, new Binding("ItemsSource") { Source = this });
                _lookUpControl.SetBinding(LookUpControl.SelectedItemProperty, new Binding("SelectedItem") { Source = this });
                _lookUpControl.SetBinding(LookUpControl.PlaceholderProperty, new Binding("Placeholder") { Source = this });
                _lookUpControl.NameDisplay = this.NameDisplay;
                _lookUpControl.BottomModal = this.BottomModal;
                _lookUpControl.SelectedItemChange += SelectedItemChange;
            }
            await _lookUpControl.OpenModal();
        }

        public void setActive()
        {
            string name = this.SelectedItem.GetType().GetProperty(this.NameDisplay)?.GetValue(this.SelectedItem, null)?.ToString();
            if (name != null)
            {
                lblText.Text = name;
                lblText.TextColor = (Color)App.Current.Resources["MainDarkColor"];
                lblIcon.TextColor = (Color)App.Current.Resources["MainDarkColor"];
            }
        }
        public void setUnActive()
        {
            lblText.Text = this.Placeholder;
            lblText.TextColor = Color.Black;
            lblIcon.TextColor = Color.Black;
        }

        private static void ItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            FilterPicker control = (FilterPicker)bindable;
            if (newValue != null)
            {
                control.setActive();
            }
            else
            {
                control.setUnActive();
            }
        }
    }
}
