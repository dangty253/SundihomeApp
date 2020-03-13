using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class FurnitureProductFilterResultPage : ContentPage
    {
        private int currentIndex = 0;
        private FilterFurnitureProductViewModel viewModel;
        public FurnitureProductFilterResultPage(FilterFurnitureProductModel filterMoel)
        {
            InitializeComponent();
            LV.ItemTemplate = new DataTemplate(typeof(Cells.FurnitureCells.ProductViewCell));
            this.Title = Language.ket_qua_loc;
            this.BindingContext = viewModel = new FilterFurnitureProductViewModel(filterMoel);
            Init();
        }

        public FurnitureProductFilterResultPage()
        {
            InitializeComponent();
            LV.ItemTemplate = new DataTemplate(typeof(Cells.FurnitureCells.ProductViewCell));
            this.Title = Language.san_pham;
            this.BindingContext = viewModel = new FilterFurnitureProductViewModel();
            Init();
        }


        private async void LV_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as FurnitureProduct;
            await Navigation.PushAsync(new ProductDetailPage(item.Id));
        }

        private async void Init()
        {
            LV.ItemTapped += LV_ItemTapped;

            MessagingCenter.Subscribe<AddProductPage, FurnitureProduct>(this, "UpdateProduct", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                await viewModel.LoadData();
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<ProductDetailPageViewModel, Guid>(this, "DeleteProduct", async (sender, productId) =>
            {
                loadingPopup.IsVisible = true;
                var product = this.viewModel.Data.SingleOrDefault(x => x.Id == productId);
                if (product != null)
                {
                    this.viewModel.Data.Remove(product);
                }
                loadingPopup.IsVisible = false;
            });

            await viewModel.LoadCategories();
            InitCategoriesLayout();
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }
        public void InitCategoriesLayout()
        {
            var rb = new RadBorder()
            {
                Style = (Style)this.Resources["RadBorderCategories"],
                BorderColor = BorderColorActive,
                BackgroundColor = BGColorActive,
                Content = new Label()
                {
                    Style = (Style)Resources["Category"],
                    Text= Language.tat_ca,
                }
            };

            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += (s, e) => OnCategoryTapped(s, e);
            rb.GestureRecognizers.Add(tapGestureRecognizer1);
            CategoriesStackLayout.Children.Insert(0, rb);
        }

        public void SetCategoryActiveStyle(RadBorder item)
        {
            //set style
            item.BackgroundColor = BGColorActive;
            item.BorderColor = BorderColorActive;

            //set scroll position
            var itemX = item.X;
            var itemWidth = item.Width;
            var center = Application.Current.MainPage.Width / 2;
            double x = 0;
            if (itemX > center)
            {
                x = itemX - center + itemWidth / 2 + 10;
            }
            scroll.ScrollToAsync(x, scroll.ScrollY, true);

        }
        private Color BGColorActive = Color.FromHex("#eeeeee");
        private Color BorderColorActive = Color.FromHex("#aaaaaa");
        private Color BGColorInActive = Color.White;
        private Color BorderColorInActive = Color.FromHex("#eeeeee");
        public async void OnCategoryTapped(object sender, EventArgs e)
        {
            var categoryChoosed = sender as RadBorder;

            if (categoryChoosed.BackgroundColor == BGColorActive)
            {
                return;
            }

            loadingPopup.IsVisible = true;

            //set all category style to inactive
            RadBorder inactiveItem = (RadBorder)this.CategoriesStackLayout.Children[currentIndex];
            inactiveItem.BackgroundColor = BGColorInActive;
            inactiveItem.BorderColor = BorderColorInActive;

            this.currentIndex = CategoriesStackLayout.Children.IndexOf(categoryChoosed);
            //set active style to category choosed
            SetCategoryActiveStyle(categoryChoosed);

            //get category choosed
            var tap = categoryChoosed.GestureRecognizers[0] as TapGestureRecognizer;
            Guid categoryId;
            if( tap.CommandParameter != null) categoryId = (Guid)tap.CommandParameter;

            //get list product by category choosed
            if (tap.CommandParameter == null)
            {
                viewModel.FilterModel.ParentCategoryId = null;
            }
            else
            {
                viewModel.FilterModel.ParentCategoryId = categoryId;
            }
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void OnSearchPressed(object sender, EventArgs e)
        {
            this.viewModel.FilterModel.Keyword = searchBar.Text;
            await this.viewModel.LoadOnRefreshCommandAsync();
        }

        public async void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                if (!string.IsNullOrWhiteSpace(viewModel.FilterModel.Keyword))
                {
                    this.viewModel.FilterModel.Keyword = null;
                    await this.viewModel.LoadOnRefreshCommandAsync();
                }
            }
        }
        public async void AddProduct_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new AddProductPage() { Title = Language.them_san_pham });
        }
    }
}
