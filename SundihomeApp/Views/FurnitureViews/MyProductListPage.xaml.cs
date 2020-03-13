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
using static SundihomeApp.Controls.ScrollTab;

namespace SundihomeApp.Views.Furniture
{
    public partial class MyProductListPage : ContentPage
    {
        public MyProductListPageViewModel viewModel;
        public MyProductListPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new MyProductListPageViewModel();

            //lv.ItemTemplate = new DataTemplate(typeof(Cells.FurnitureCells.ProductViewCell));

            Init();
        }

        public async void Init()
        {
            lv.ItemTapped += Lv_ItemTapped;
            await viewModel.LoadData();
            ScrollTab.ItemsSource = new List<string> {Language.tat_ca,Language.dang_ban_status,Language.ngung_ban };
            MessagingCenter.Subscribe<AddProductPage, bool>(this, "AddProduct", async (sender, isPromotion) =>
            {
                await viewModel.LoadOnRefreshCommandAsync();
            });
            MessagingCenter.Subscribe<AddProductPage, FurnitureProduct>(this, "UpdateProduct", async (sender, product) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModel.Data.Any(x => x.Id == product.Id))
                {
                    await viewModel.LoadOnRefreshCommandAsync();
                }
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<ViewModels.Furniture.ProductDetailPageViewModel, Guid>(this, "DeleteProduct", async (sender, arg) =>
            {
                loadingPopup.IsVisible = true;
                if (this.viewModel.Data.Any(x => x.Id == arg))
                {
                    var product = this.viewModel.Data.Single(x => x.Id == arg);
                    this.viewModel.Data.Remove(product);
                }
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<ProductDetailPage, Guid>(this, "UpdateProductStatus", async (sender, arg) =>
            {
                await viewModel.LoadOnRefreshCommandAsync();
            });

            MessagingCenter.Subscribe<PromotionPage>(this, "AddProduct", async (sender) => {
                loadingPopup.IsVisible = true;
                await viewModel.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
            });
            loadingPopup.IsVisible = false;
        }


        private void Lv_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as FurnitureProduct;
            Shell.Current.Navigation.PushAsync(new ProductDetailPage(item.Id));
        }

        private async void ScrollTabIndex_Change(object sender, SelectedIndexChangedEventArgs e)
        {
            if (e.Index == 0)
            {
                viewModel.FilterModel.ProductStatus = null;
                await this.viewModel.LoadOnRefreshCommandAsync();
            }
            else if (e.Index == 1)
            {
                viewModel.FilterModel.ProductStatus = 0;
                await this.viewModel.LoadOnRefreshCommandAsync();
            }
            else
            {
                viewModel.FilterModel.ProductStatus = 1;
                await this.viewModel.LoadOnRefreshCommandAsync();
            }
        }

        private async void FilterByStatus_Clicked(object sender, EventArgs e)
        {
            //RadBorder radBoder = sender as RadBorder;
            //if (radBoder.BackgroundColor == Color.FromHex("#eeeeee")) return;

            //TapGestureRecognizer tapGestureRecognizer = radBoder.GestureRecognizers[0] as TapGestureRecognizer;

            //if (tapGestureRecognizer.CommandParameter != null)
            //{
            //    int Status = int.Parse(tapGestureRecognizer.CommandParameter.ToString());
            //    //viewModel.FilterModel.Status = Status;
            //}
            //else
            //{
            //    //viewModel.FilterModel.Status = null;
            //}
            //loadingPopup.IsVisible = true;


            //// inactive
            //foreach (RadBorder item in StackLayoutFilter.Children)
            //{
            //    item.BorderColor = Color.FromHex("#eeeeee");
            //    item.BackgroundColor = Color.White;
            //}
            //// active.
            //radBoder.BackgroundColor = Color.FromHex("#eeeeee");
            //radBoder.BorderColor = Color.FromHex("#aaaaaa");



            //await viewModel.LoadOnRefreshCommandAsync();
            //loadingPopup.IsVisible = false;
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
