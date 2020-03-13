using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class ProductListPage : ContentPage
    {
        private int currentIndex = 0;
        public ProductListPageViewModel viewModel;
        private Guid _parentCategoryId;

        public Color BGColorActive = Color.FromHex("#026294");
        public Color TextColorActive = Color.White;
        public Color BGColorInActive = Color.White;
        public Color TextColorInActive = Color.FromHex("#444");

        public ProductListPage(Guid parentCategoryId)
        {
            InitializeComponent();
            _parentCategoryId = parentCategoryId;
            BindingContext = viewModel = new ProductListPageViewModel(_parentCategoryId);
            lv.ItemTemplate = new DataTemplate(typeof(Cells.FurnitureCells.ProductViewCell));
            Init();
        }

        public async void Init()
        {
            lv.ItemTapped += Lv_ItemTapped;
            MessagingCenter.Subscribe<AddProductPage, bool>(this, "AddProduct", async (sender, isPromotion) =>
            {

                await viewModel.LoadData();
            });
            MessagingCenter.Subscribe<AddProductPage, FurnitureProduct>(this, "UpdateProduct", async (sender, product) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModel.Data.Any(x => x.Id == product.Id))
                {
                    await viewModel.LoadData();
                }
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
            await Task.WhenAll(viewModel.GetCategories(),
                viewModel.LoadData());
            InitCategoriesLayout();
            loadingPopup.IsVisible = false;
        }

        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as FurnitureProduct;
            Shell.Current.Navigation.PushAsync(new ProductDetailPage(item.Id));
        }

        public void InitCategoriesLayout()
        {
            var rb = new RadBorder()
            {
                Style = (Style)this.Resources["RadBorderCategories"],
                BorderColor = TextColorActive,
                BackgroundColor = BGColorActive,
                Content = new Label()
                {
                    Style = (Style)Resources["Category"],
                    TextColor = TextColorActive,
                    Text= Language.tat_ca,
                }
            };

            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += (s, e) => OnCategoryTapped(s, e);
            rb.GestureRecognizers.Add(tapGestureRecognizer1);
            CategoriesStackLayout.Children.Insert(0, rb);
        }

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
            inactiveItem.BorderColor = TextColorInActive;
            (inactiveItem.Content as Label).TextColor = TextColorInActive;

            this.currentIndex = CategoriesStackLayout.Children.IndexOf(categoryChoosed);

            SetCategoryActiveStyle(categoryChoosed);

            //get category choosed
            var tap = categoryChoosed.GestureRecognizers[0] as TapGestureRecognizer;
            var category = tap.CommandParameter as FurnitureCategory;

            //get list product by category choosed
            if (category == null)
            {
                //get all product

                viewModel.FilterModel.CategoryId = null;
            }
            else
            {
                //get product by categoryId
                viewModel.FilterModel.CategoryId = category.Id;
            }

            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public void SetCategoryActiveStyle(RadBorder item)
        {
            //set style
            item.BackgroundColor = BGColorActive;
            item.BorderColor = TextColorActive;
            (item.Content as Label).TextColor = TextColorActive;

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
    }
}
