using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class TopModal : ContentView
    {
      
        public static readonly BindableProperty BodyProperty = BindableProperty.Create(nameof(Body), typeof(View), typeof(BottomModal), null, BindingMode.TwoWay);
        public View Body { get => (View)GetValue(BodyProperty); set => SetValue(BodyProperty, value); }

       
        // an hien tabbar o duoi 
        public bool IsToggleTabBar { get; set; }

        public TapGestureRecognizer BackgroudTap;

        public TopModal()
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
            BackgroudTap.Tapped -= BackgroudTap_Tapped;
            BackgroudTap.Tapped += customEvent;
        }

        private async void Hide_Clicked(object sender, EventArgs e)
        {
            await this.Hide();
        }

        public async Task Show()
        {
            this.IsVisible = true;
            await this.MainContent.ScaleTo(1, 130);
        }
        public async Task Hide()
        {
            await this.MainContent.ScaleTo(0, 130);
            this.IsVisible = false;
        }
    }
}
