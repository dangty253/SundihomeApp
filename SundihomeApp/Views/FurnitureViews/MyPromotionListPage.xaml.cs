using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Settings;
using Xamarin.Forms;
using static SundihomeApp.Controls.ScrollTab;
using SundihomeApp.ViewModels.FurnitureViewModels;
using SundihomeApp.Views.Furniture;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Configuration;

namespace SundihomeApp.Views.FurnitureViews
{
    public partial class MyPromotionListPage : ContentPage
    {
        private ListViewPageViewModel2<SundihomeApi.Entities.Furniture.FurnitureProduct> searchPageResultViewModel;
        private int currentIndex = 0;
        public string Keyword { get; set; }
        public MyPromotionListPageViewModel viewModel;
        public FilterFurnitureProductModel FilterModel { get; set; }
        private FurnitureProduct FurnitureUpdate { get; set; }
        public MyPromotionListPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new MyPromotionListPageViewModel();
            ModalUpdate.Body.BindingContext = viewModel;
            ScrollTab.ItemsSource = new List<string> { Language.tat_ca, Language.khuyen_mai, Language.het_han };

            Init();
        }

        public async void Init()
        {
            lv.ItemTapped += Lv_ItemTapped;
            await viewModel.LoadData();

            MessagingCenter.Subscribe<AddProductPage, bool>(this, "AddProduct", async (sender, isPromotion) =>
            {
                if (isPromotion) await viewModel.LoadOnRefreshCommandAsync();
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
            MessagingCenter.Subscribe<PromotionPage>(this, "AddProduct", async (sender) =>
            {
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
                viewModel.FilterModel.PromotionFilterType = null;
                await this.viewModel.LoadOnRefreshCommandAsync();
            }
            else if (e.Index == 1)
            {
                //viewModel.FilterModel.ProductStatus = 0;
                viewModel.FilterModel.PromotionFilterType = 0;
                await this.viewModel.LoadOnRefreshCommandAsync();
            }
            else
            {
                //viewModel.FilterModel.ProductStatus = 1;
                viewModel.FilterModel.PromotionFilterType = 1;
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
                ModalPopup.IsVisible = true;
                await ModalPopup.TranslateTo(0, 0, 150);
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
        public async void CloseModal_Clicked(object sender, EventArgs e)
        {
            await ModalPopup.TranslateTo(0, ModalPopup.Height, 50);
            ModalPopup.IsVisible = false;
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

        public async void OnBtnSaveAddContactNeed_Click(object sender, EventArgs e)
        {
            if (viewModel.PromotionPrice == null) await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_nhap_gia_giam, Language.dong);
            else
            {
                FurnitureUpdate.IsPromotion = true;
                FurnitureUpdate.PromotionFromDate = viewModel.PromotionFromDate;
                FurnitureUpdate.PromotionToDate = viewModel.PromotionToDate;
                FurnitureUpdate.PromotionPrice = viewModel.PromotionPrice;

                ApiResponse apiResponse = await ApiHelper.Put($"{ApiRouter.FURNITUREPRODUCT_ADD_UPDATE}/update", FurnitureUpdate, true);
                if (apiResponse.IsSuccess)
                {
                    await ModalUpdate.Hide();
                    viewModel.PromotionFromDate = DateTime.Now;
                    viewModel.PromotionToDate = DateTime.Now;
                    viewModel.PromotionPrice = null;
                    CloseModal_Clicked(null, EventArgs.Empty);
                    await viewModel.LoadOnRefreshCommandAsync();
                    await searchPageResultViewModel.LoadOnRefreshCommandAsync();
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_thanh_cong);
                }
                else
                {
                    ToastMessageHelper.ShortMessage(apiResponse.Message);
                }
            }
        }
        public async void OnBtnCancelAddContactNeed_Click(object sender, EventArgs e)
        {
            await ModalUpdate.Hide();
            //viewModel.CancelPopUpAddContactNeed();
        }
    }
}

