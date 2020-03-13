using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.Furniture;
using SundihomeApp.ViewModels.FurnitureViewModels;
using SundihomeApp.Views.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class PromotionPage : ContentPage
    {
        private ListViewPageViewModel2<SundihomeApi.Entities.Furniture.FurnitureProduct> searchPageResultViewModel;
        private int currentIndex = 0;
        public string Keyword { get; set; }
        private PromotionViewModel viewModel;
        public FilterFurnitureProductModel FilterModel { get; set; }
        private FurnitureProduct FurnitureUpdate { get; set; }

        public PromotionPage()
        {
            InitializeComponent();
            //LV.ItemTemplate = new DataTemplate(typeof(Cells.FurnitureCells.ProductViewCell));
            
            this.BindingContext = viewModel = new PromotionViewModel();
            ModalUpdate.Body.BindingContext = viewModel;
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
                await viewModel.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<AddProductPage, bool>(this, "AddProduct", async (sender, isPromotion) =>
            {
                loadingPopup.IsVisible = true;
                if(isPromotion)
                    await viewModel.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<ViewModels.Furniture.ProductDetailPageViewModel, Guid>(this, "DeleteProduct", async (sender, productId) =>
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

            var action = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, Language.them_moi_san_pham, Language.chon_tu_san_pham_noi_that);
            if (action == Language.them_moi_san_pham)
            {
                await Navigation.PushAsync(new AddProductPage(true) { Title = Language.them_san_pham });
            }
            else if (action == Language.chon_tu_san_pham_noi_that)
            {
                loadingPopup.IsVisible = true;

                if (searchPageResultViewModel == null) // chua bat popup lan nao.
                {
                    this.ListView0.ItemTapped += ListView0_ItemTapped;
                    searchPageResultViewModel = new ListViewPageViewModel2<SundihomeApi.Entities.Furniture.FurnitureProduct>();




                    FilterModel = new FilterFurnitureProductModel();
                    FilterModel.ProductStatus = 0;
                    FilterModel.IsPromotion = false;
                    FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.FilterModel);

                    searchPageResultViewModel.PreLoadData = new Command(() =>
                            {
                                searchPageResultViewModel.ApiUrl = $"{Configuration.ApiRouter.FURNITUREPRODUCT_FILTER}?json={json}&page={searchPageResultViewModel.Page}";
                            });


                    this.ListView0.BindingContext = searchPageResultViewModel;
                }
                else
                {
                    Keyword = null;
                    ModalPopupSearchBar.Text = null;
                }
                await searchPageResultViewModel.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
                await ModalPickProduct.Show();
            }

        }
        private async void ListView0_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await ModalUpdate.Show();
            if (FurnitureUpdate == null)
            {

                viewModel.DateNow = DateTime.Now;
                viewModel.PromotionFromDate = DateTime.Now;
                viewModel.PromotionToDate = DateTime.Now;
            }
            FurnitureUpdate = e.Item as FurnitureProduct;

        }
 
        private void Search_Clicked(object sender, EventArgs e)
        {
            Keyword = ModalPopupSearchBar.Text ?? "";
            searchPageResultViewModel.RefreshCommand.Execute(null);
        }
        public void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // huy search va dieu kien saerch hien tai khac "" hoac emtpy thi moi chay lai. 
            if (string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(Keyword))
            {
                Search_Clicked(sender, EventArgs.Empty);
            }
        }

        public async void OnBtnSavePromotion_Click(object sender, EventArgs e)
        {
            if (viewModel.PromotionPrice == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_nhap_gia_giam, Language.dong);
            else
            {
                FurnitureProduct product = new FurnitureProduct();
                product.Name = FurnitureUpdate.Name;
                product.ParentCategoryId = FurnitureUpdate.ParentCategoryId;
                product.CategoryId = FurnitureUpdate.CategoryId;
                product.CreatedById = Guid.Parse(UserLogged.Id);
                product.Price = FurnitureUpdate.Price;
                product.Status = FurnitureUpdate.Status;
                product.Model = FurnitureUpdate.Model;
                product.Origin = FurnitureUpdate.Origin;
                product.Guarantee = FurnitureUpdate.Guarantee;
                product.Description = FurnitureUpdate.Description;
                product.Address = FurnitureUpdate.Address;
                
                product.Category = FurnitureUpdate.Category;
                product.Company = FurnitureUpdate.Company;
                product.CompanyId = FurnitureUpdate.CompanyId;
                product.AvatarUrl = FurnitureUpdate.AvatarUrl;
                product.CreatedBy = FurnitureUpdate.CreatedBy;
                product.DistrictId = FurnitureUpdate.DistrictId;
                product.Images = FurnitureUpdate.Images;
                product.Model = FurnitureUpdate.Model;
                product.ProvinceId = FurnitureUpdate.ProvinceId;
                product.Street = FurnitureUpdate.Street;
                product.WardId = FurnitureUpdate.WardId;
                product.Videos = FurnitureUpdate.Videos;

                product.ProductStatus = FurnitureUpdate.ProductStatus;
                product.IsPromotion = true;
                product.PromotionFromDate = viewModel.PromotionFromDate;
                product.PromotionToDate = viewModel.PromotionToDate;
                product.PromotionPrice = viewModel.PromotionPrice;

                

                ApiResponse apiResponse = await ApiHelper.Post($"{ApiRouter.FURNITUREPRODUCT_ADD_UPDATE}", product, true);
                if (apiResponse.IsSuccess)
                {
                    await ModalUpdate.Hide();
                    viewModel.PromotionFromDate = DateTime.Now;
                    viewModel.PromotionToDate = DateTime.Now;
                    viewModel.PromotionPrice = null;
                    await viewModel.LoadOnRefreshCommandAsync();
                    ToastMessageHelper.ShortMessage(Language.dang_thanh_cong);
                    MessagingCenter.Send<PromotionPage>(this, "AddProduct");
                    await ModalPickProduct.Hide();
                }
                else
                {
                    ToastMessageHelper.ShortMessage(apiResponse.Message);
                }
            }
        }
        public async void OnBtnCancelAddPromotion_Click(object sender, EventArgs e)
        {
            await ModalUpdate.Hide();
            //viewModel.CancelPopUpAddContactNeed();
        }
    }
}

