using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Controls;
using SundihomeApp.Models;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class SearchResultPage : ContentPage
    {
        public readonly SearchPageResultViewModel viewModel;
        public SearchResultPage(FilterModel filterModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new SearchPageResultViewModel();
            viewModel.FilterModel = filterModel;
            Init();
        }
        public async void Init()
        {
            await viewModel.LoadData();
            lv.ItemTapped += Lv_ItemTapped;
            loadingPopup.IsVisible = false;
        }

        private async void Lv_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (viewModel.Data == null || viewModel.Data.Count == 0) return;
            var item = e.Item as Post;
            var count = viewModel.Data.Count;
            if (item == viewModel.Data[count - 1])
            {
                viewModel.Page += 1;
                await viewModel.LoadData();
            }
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as SundihomeApi.Entities.Post;
            Navigation.PushAsync(new PostDetailPage(post.Id));
        }

        public async void GoToMap_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new MapsPage(viewModel.FilterModel));
        }
    }
}
