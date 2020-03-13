using System;
using System.Collections;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class FloatingButtonGroup : ContentView
    {
        public event EventHandler ClickedEvent;
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(FloatingButtonGroup), null, BindingMode.TwoWay, null);
        public IEnumerable ItemsSource { get => (IEnumerable)GetValue(ItemsSourceProperty); set { SetValue(ItemsSourceProperty, value); } }

        private bool IsShow = false;

        public FloatingButtonGroup()
        {
            InitializeComponent();
            this.BindingContext = this;

            // tap ra ngaoi thit off option list.
            var tap = new TapGestureRecognizer();
            tap.NumberOfTapsRequired = 1;
            tap.Tapped += (o, e) =>
            {
                BtnShow_CLicked(BtnShow, EventArgs.Empty);
            };
            this.GestureRecognizers.Add(tap);
        }

        private void BtnShow_CLicked(object sender, EventArgs e)
        {
            if (!IsShow) // dang an 
            {
                this.BackgroundColor = Color.FromRgba(250, 250, 250, 0.9);

                AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));
                AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.All);
                MainScrollView.IsVisible = true;
                this.HorizontalOptions = LayoutOptions.Fill;

                IsShow = true;
                BtnShow.Text = "\uf00d";
                BtnShow.TextColor = Color.Black;
                BtnShow.BackgroundColor = Color.White;
                //BtnShow.FontFamily = Device.RuntimePlatform == Device.iOS ? "FontAwesome5Free-Solid" : "FontAwesome5Solid.otf#Regular";
            }
            else // dang hien thi
            {
                this.BackgroundColor = Color.Transparent;

                BtnShow.Text = "\uf129";
                BtnShow.TextColor = Color.White;
                BtnShow.BackgroundColor = Color.FromHex("#4266B2");
                IsShow = false;


                AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 1, 1, 64));
                AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.YProportional | AbsoluteLayoutFlags.WidthProportional);
                MainScrollView.IsVisible = false;
                this.HorizontalOptions = LayoutOptions.End;
            }
        }

        private void OnItem_Clicked(object sender, EventArgs e)
        {
            FloatButtonItem floatBtnItem = null;
            if (sender is Button)
            {
                floatBtnItem = (sender as Button).CommandParameter as FloatButtonItem;
            }
            else if (sender is StackLayout)
            {
                floatBtnItem = ((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as FloatButtonItem;
            }
            if (floatBtnItem == null) return;

            if (floatBtnItem.OnClickCommand != null)
            {
                floatBtnItem.OnClickCommand.Execute(null);
            }
            else if (floatBtnItem.OnClickeEvent != null)
            {
                floatBtnItem.OnClickeEvent.Invoke(this, EventArgs.Empty);
            }
            BtnShow_CLicked(this.BtnShow, EventArgs.Empty);
        }
    }
}
