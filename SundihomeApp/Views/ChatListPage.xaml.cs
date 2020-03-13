using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class ChatListPage : ContentPage
    {
        private ChatListPageViewModel viewModel;
        public ChatListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ChatListPageViewModel();
            Init();
        }

        private async void Init()
        {
            lv.ItemTapped += ItemTapped;
            await viewModel.LoadData();
            loadingPoup.IsVisible = false;
        }
        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var chatlistItem = e.Item as ChatListItem;
            await Navigation.PushAsync(new ChatPage(chatlistItem.PartnerId.ToLower()));
        }
    }
}
