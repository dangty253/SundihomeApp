using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using SundihomeApp.Views.Furniture;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class ProductListContentView : ContentView
    {
        public ListViewPageViewModel2<FurnitureProduct> viewModel;
        private bool _isOwner = false;
        private Guid _companyId;
        public string Keyword;
        public ProductListContentView(Guid companyId, bool IsMyCompnay)
        {
            InitializeComponent();
            _companyId = companyId;
            this.BindingContext = viewModel = new ListViewPageViewModel2<FurnitureProduct>();
            viewModel.PreLoadData = new Command(() =>
            {
                string Url = $"{ApiRouter.FURNITUREPRODUCT_GET_PRODUCT_COMPANY}/{_companyId}?page={viewModel.Page}";
                if (!string.IsNullOrWhiteSpace(Keyword))
                {
                    Url += $"&keyword={Keyword}";
                }
                viewModel.ApiUrl = Url;
            });
            Init();
        }
        public async void Init()
        {
            lvSanPham.ItemTapped += lvSanPham_ItemTapped;
            await viewModel.LoadData();
            if (UserLogged.RoleId == 0 && Guid.Parse(UserLogged.CompanyId) == _companyId)
            {
                _isOwner = true;
                StackButton.IsVisible = true;
            }
            loadingPopup.IsVisible = false;
        }

        private async void lvSanPham_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as FurnitureProduct;
            if (_isOwner)
            {
                string[] options = { Language.xem_chi_tiet, Language.chinh_sua };
                string action = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, options);

                if (action == options[0])
                {
                    await Navigation.PushAsync(new ProductDetailPage(post.Id));
                }
                else if (action == options[1])
                {
                    await Shell.Current.Navigation.PushAsync(new AddProductPage(post.Id, false) { Title = post.Name });
                }
            }
            else
            {
                await Navigation.PushAsync(new ProductDetailPage(post.Id));
            }
        }

        private async void MyCompnayItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as FurnitureProduct;
            await Navigation.PushAsync(new AddProductPage(post.Id, true) { Title = Language.cap_nhat_san_pham });
        }

        /// <summary>
        /// chi hien thi khi la owner.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void AddProduct_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AddProductPage(Guid.Parse(UserLogged.CompanyId)) { Title = Language.them_san_pham });
        }

        public void Search_Pressed(object sender, EventArgs e)
        {
            Keyword = searchBar.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text) && !string.IsNullOrWhiteSpace(Keyword))
            {
                Keyword = null;
                viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
