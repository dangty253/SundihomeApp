using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class ToDayListPage : ContentPage
    {
        public LiquidationToDayFilterViewModel viewModel;
        private List<LiquidationCategory> Categories;
        public ToDayListPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new LiquidationToDayFilterViewModel();
            viewModel.FilterModel.Status = 0;

            ListViewThanhLy.ItemTemplate = new DataTemplate(typeof(Cells.LiquidationCells.ToDayViewCell));

            Init();
        }

        public async void Init()
        {
            MessagingCenter.Subscribe<AddToDayPage>(this, "OnSaveItem", (sender) => viewModel.RefreshCommand.Execute(null));
            MessagingCenter.Subscribe<ToDayDetailPage, Guid>(this, "OnDeleted", (sender, todayId) =>
            {
                if (this.viewModel.Data.Any(x => x.Id == todayId))
                {
                    var removeItem = this.viewModel.Data.Single(x => x.Id == todayId);
                    this.viewModel.Data.Remove(removeItem);
                }
            });
            MessagingCenter.Subscribe<PickerLiquidationPage>(this, "OnSaveItem", (sender) => viewModel.RefreshCommand.Execute(null));
            Categories = DependencyService.Get<IServices.ILiquidation.ILiquidationCategoryService>().GetLiquidations();
            BindableLayout.SetItemsSource(CategoriesStackLayout, Categories);
            InitCategoriesLayout();
            ListViewThanhLy.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as LiquidationToDay;
                await Navigation.PushAsync(new ToDayDetailPage(item.Id));
            };
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private void InitCategoriesLayout()
        {
            var rb = new RadBorder()
            {
                Style = (Style)this.Resources["RadBorderCategories"],
                BorderColor = BorderColorActive,
                BackgroundColor = BGColorActive,
                Content = new Label()
                {
                    Style = (Style)Resources["Category"],
                    Text = Language.tat_ca,
                }
            };

            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += (s, e) => OnCategoryTapped(s, e);
            rb.GestureRecognizers.Add(tapGestureRecognizer1);
            CategoriesStackLayout.Children.Insert(0, rb);
        }

        private Color BGColorActive = Color.FromHex("#eeeeee");
        private Color BorderColorActive = Color.FromHex("#aaaaaa");
        private Color BGColorInActive = Color.White;
        private Color BorderColorInActive = Color.FromHex("#eeeeee");
        private async void OnCategoryTapped(object sender, EventArgs e)
        {
            var categoryChoosed = sender as RadBorder;

            if (categoryChoosed.BackgroundColor == BGColorActive)
            {
                return;
            }

            loadingPopup.IsVisible = true;

            //set active style to category choosed
            SetCategoryActiveStyle(categoryChoosed);

            //get category choosed
            var tap = categoryChoosed.GestureRecognizers[0] as TapGestureRecognizer;


            //get list product by category choosed
            if (tap.CommandParameter == null)
            {
                //get all product
                viewModel.FilterModel.CategoryId = null;
            }
            else
            {
                int categoryId = (int)tap.CommandParameter;
                //get product by categoryId
                viewModel.FilterModel.CategoryId = categoryId;
            }

            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public void SetCategoryActiveStyle(RadBorder item)
        {
            foreach (RadBorder rad in CategoriesStackLayout.Children)
            {
                rad.BackgroundColor = BGColorInActive;
                rad.BorderColor = BorderColorInActive;
            }

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


        public async void AddLiquidation_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged)
            {
                var answer = await DisplayActionSheet(Language.dang_san_pham_trong_ngay, Language.huy, null, Language.chon_tu_san_pham_thanh_ly, Language.them_san_pham);
                if (answer == Language.chon_tu_san_pham_thanh_ly)
                {
                    await Navigation.PushAsync(new PickerLiquidationPage(false));
                }
                if (answer == Language.them_san_pham)
                {
                    await Navigation.PushAsync(new AddToDayPage());
                }
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_dang_ky_de_duoc_dang_san_pham, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }

        }
        public void Clicked_BtnSearch(object sender, EventArgs e)
        {
            viewModel.FilterModel.Keyword = SearchBarToDayLisPage.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChaned(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBarToDayLisPage.Text))
            {
                if (!string.IsNullOrWhiteSpace(viewModel.FilterModel.Keyword))
                {
                    viewModel.FilterModel.Keyword = null;
                    viewModel.RefreshCommand.Execute(null);
                }
            }
        }
    }
}
