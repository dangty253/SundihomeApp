using System;
using System.Collections.Generic;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.ViewModels.Furniture;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class FurnitureCategoryPage : ContentPage
    {
        public CategoryPageViewModel viewModel;
        public FurnitureCategoryPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new CategoryPageViewModel();
            Init();
        }

        public async void Init()
        {
            CollectionViewCategories.FlowItemsSource = await viewModel.GetCategories();
            loadingPopup.IsVisible = false;
        }

        public async void OnCategoryTapped(object sender, EventArgs e)
        {
            var item = sender as StackLayout;
            var tap = item.GestureRecognizers[0] as TapGestureRecognizer;
            var selectedCategory = tap.CommandParameter as FurnitureCategory;
            await Navigation.PushAsync(new ProductListPage(selectedCategory.Id) { Title = selectedCategory.Name });
        }
    }
}
