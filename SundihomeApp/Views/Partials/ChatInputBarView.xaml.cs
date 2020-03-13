using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.Partials
{
    public partial class ChatInputBarView : ContentView
    {
        public event EventHandler PickPost;
        public event EventHandler PickFurnitureProduct;
        public event EventHandler PickerLiquidationPost;

        public ChatInputBarView()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS)
            {
                this.SetBinding(HeightRequestProperty, new Binding("Height", BindingMode.OneWay, null, null, null, chatTextInput));
            }
        }
        public void Handle_Completed(object sender, EventArgs e)
        {
            (this.Parent.Parent.BindingContext as ChatPageViewModel).OnSendCommand.Execute(null);
            chatTextInput.Focus();
        }

        public async void OpenOptionLeft_Clicked(object sender, EventArgs e)
        {
            int PICK_POST = 1;
            int PICK_FURNITUREPRODUCT = 2;
            int PICK_LIQUIDATION_POST = 3;
            IDictionary<int, string> keys = new Dictionary<int, string>()
            {
                {PICK_POST,Language.chon_bat_dong_san },
                {PICK_FURNITUREPRODUCT,Language.chon_tu_san_pham_noi_that },
                {PICK_LIQUIDATION_POST,Language.chon_tu_san_pham_thanh_ly}
            };


            string result = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, keys.Values.ToArray());
            if (result == keys[PICK_POST])
            {
                PickPost?.Invoke(this, EventArgs.Empty);
            }
            else if (result == keys[PICK_FURNITUREPRODUCT])
            {
                PickFurnitureProduct?.Invoke(this, EventArgs.Empty);
            }
            else if (result == keys[PICK_LIQUIDATION_POST])
            {
                PickerLiquidationPost?.Invoke(this, EventArgs.Empty);
            }
            //chatTextInput.Focus();
        }

        public void UnFocusEntry()
        {
            chatTextInput?.Unfocus();
        }
        public void OnSwipe(object sender, EventArgs e)
        {
            UnFocusEntry();
        }
    }
}
