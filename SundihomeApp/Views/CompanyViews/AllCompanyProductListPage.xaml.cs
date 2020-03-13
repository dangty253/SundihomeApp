using System;
using System.Linq;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.Views.Furniture;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class AllCompanyProductListPage : ContentPage
    {
        public ListViewPageViewModel2<FurnitureProduct> viewModel;
        public AllCompanyProductListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new ListViewPageViewModel2<FurnitureProduct>()
            {
                PreLoadData = new Command(() =>
                {
                    viewModel.ApiUrl = $"api/furnitureproduct/companies?page={viewModel.Page}";
                })
            };
            Init();
        }
        public async void Init()
        {
            LvData.ItemTapped += LvData_ItemTapped;
            MessagingCenter.Subscribe<ViewModels.Furniture.ProductDetailPageViewModel, Guid>(this, "DeleteProduct", async (sender, PostId) =>
            {
                var deletedProduct = this.viewModel.Data.SingleOrDefault(x => x.Id == PostId);
                if (deletedProduct != null)
                    this.viewModel.Data.Remove(deletedProduct);
            });
            MessagingCenter.Subscribe<AddProductPage, FurnitureProduct>(this, "UpdateProduct", async (sender, product) =>
             {
                 this.viewModel.RefreshCommand.Execute(null);
             });
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void LvData_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var product = e.Item as FurnitureProduct;
            await Navigation.PushAsync(new ProductDetailPage(product.Id));
        }

        public async void AddPost_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged)
            {
                if (UserLogged.RoleId == 0)
                {
                    await Shell.Current.Navigation.PushAsync(new AddProductPage(Guid.Parse(UserLogged.CompanyId)) { Title = Language.them_san_pham });
                }
                else
                {
                    await Shell.Current.Navigation.PushAsync(new AddProductPage() { Title = Language.them_san_pham });
                }
            }
            else
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
        }
    }
}
