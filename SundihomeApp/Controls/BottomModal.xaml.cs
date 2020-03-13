using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SundihomeApp.Controls
{
    public partial class BottomModal : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(BottomModal), null, BindingMode.TwoWay);
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }

        public static readonly BindableProperty ModalContentProperty = BindableProperty.Create(nameof(ModalContent), typeof(View), typeof(BottomModal), null, BindingMode.TwoWay);
        public View ModalContent { get => (View)GetValue(ModalContentProperty); set => SetValue(ModalContentProperty, value); }

        public BottomModal()
        {
            InitializeComponent();
            ModalPopupBackground.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            this.BindingContext = this;
        }
        public async Task Show()
        {
            this.IsVisible = true;
            ModalPopup.IsVisible = true;
            await ModalPopup.TranslateTo(0, 0, 150);
        }
        public async Task Hide()
        {
            this.IsVisible = false;
            await ModalPopup.TranslateTo(0, ModalPopup.Height, 50);
            ModalPopup.IsVisible = false;
        }
        private async void Hide_Tapped(object sender, EventArgs e)
        {
            await Hide();
        }
    }
}
