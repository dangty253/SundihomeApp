using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class MyFavoritePostListPage : ContentPage
    {
        public MyFavoritePostListPageViewModel viewModel;
        public MyFavoritePostListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new MyFavoritePostListPageViewModel();
            Init();
        }

        private async void Init()
        {
            lv.ItemTapped += Lv_ItemTapped;
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as SundihomeApi.Entities.Post;
            Shell.Current.Navigation.PushAsync(new PostDetailPage(post.Id));
        }
    }
}
