using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class CenterModal : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(BottomModal), null, BindingMode.TwoWay);
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly BindableProperty BodyProperty = BindableProperty.Create(nameof(Body), typeof(View), typeof(BottomModal), null, BindingMode.TwoWay);
        public View Body { get => (View)GetValue(BodyProperty); set => SetValue(BodyProperty, value); }

        public static readonly BindableProperty FooterProperty = BindableProperty.Create(nameof(Footer), typeof(View), typeof(BottomModal), null, BindingMode.TwoWay);
        public View Footer { get => (View)GetValue(FooterProperty); set => SetValue(FooterProperty, value); }

        // an hien tabbar o duoi 
        public bool IsToggleTabBar { get; set; }

        public TapGestureRecognizer BackgroudTap;

        public CenterModal()
        {
            InitializeComponent();
            this.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
            this.BindingContext = this;
            this.IsVisible = false;
            MainContent.Scale = 0;


            //
            
            BackgroudTap = new TapGestureRecognizer();
            BackgroudTap.Tapped += BackgroudTap_Tapped;
            this.GestureRecognizers.Add(BackgroudTap);
        }

        private async void BackgroudTap_Tapped(object sender, EventArgs e)
        {
            await this.Hide();
        }


        // su dung khi can custom lai nut close
        public void CustomCloseButton(EventHandler customEvent)
        {
            BtnClose.Clicked -= Hide_Clicked;
            BtnClose.Clicked += customEvent;

            BackgroudTap.Tapped -= BackgroudTap_Tapped;
            BackgroudTap.Tapped += customEvent;
        }

        private async void Hide_Clicked(object sender, EventArgs e)
        {
            await this.Hide();
        }

        public async Task Show()
        {
            if (IsToggleTabBar) Shell.SetTabBarIsVisible(this.Parent.Parent, false);
            this.IsVisible = true;
            await this.MainContent.ScaleTo(1, 130);
        }
        public async Task Hide()
        {
            if (IsToggleTabBar) Shell.SetTabBarIsVisible(this.Parent.Parent, true);
            await this.MainContent.ScaleTo(0, 130);
            this.IsVisible = false;
        }
    }
}
