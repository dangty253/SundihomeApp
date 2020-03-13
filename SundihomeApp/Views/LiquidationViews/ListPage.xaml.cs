using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public partial class ListPage : ContentPage
    {
        public LiquidationFilterViewModel viewModel;
        public bool isMyListPage = false;
        public int CurrentIndex = 0;
        private int _type;
        private List<LiquidationCategory> Categories;
        public ListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new LiquidationFilterViewModel();
            viewModel.FilterModel.Status = 0;
            Init();
        }
        public ListPage(LiquidationFilterModel filterModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new LiquidationFilterViewModel(filterModel);
            Init();
        }

        public async void Init()
        {
            ListViewThanhLy.ItemTemplate = new DataTemplate(typeof(Cells.LiquidationCells.LiquidationViewCell));
            MessagingCenter.Subscribe<AddLiquidationPage>(this, "OnSaveItem", (sender) => viewModel.RefreshCommand.Execute(null));
            MessagingCenter.Subscribe<LiquidationDetailPage, Guid>(this, "OnDeleted", (sender, liquidationId) =>
            {
                if (this.viewModel.Data.Any(x => x.Id == liquidationId))
                {
                    var removeItem = this.viewModel.Data.Single(x => x.Id == liquidationId);
                    this.viewModel.Data.Remove(removeItem);
                }
            });
            Categories = DependencyService.Get<IServices.ILiquidation.ILiquidationCategoryService>().GetLiquidations();
            BindableLayout.SetItemsSource(CategoriesStackLayout, Categories);
            ListViewThanhLy.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as Liquidation;

                if (isMyListPage)
                {
                    await Navigation.PushAsync(new AddLiquidationPage(item.Id));
                }
                else
                {
                    await Navigation.PushAsync(new LiquidationDetailPage(item.Id));
                }

            };

            InitCategoriesLayout();
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
                    Text = Language.see_all,
                }
            };

            var tapGestureRecognizer1 = new TapGestureRecognizer();
            tapGestureRecognizer1.Tapped += (s, e) => OnCategoryTapped(s, e);
            rb.GestureRecognizers.Add(tapGestureRecognizer1);
            CategoriesStackLayout.Children.Insert(0, rb);


            if (viewModel.FilterModel.CategoryId.HasValue) // set active
            {
                var category = this.Categories.SingleOrDefault(x => x.Id == viewModel.FilterModel.CategoryId.Value);
                int countCategories = Categories.Count;
                for (int i = 1; i < countCategories + 1; i++) // +1 vi co them thang tat ca.
                {
                    RadBorder radBorder = CategoriesStackLayout.Children[i] as RadBorder;
                    var stackLayout = radBorder.Content as StackLayout;
                    var label = stackLayout.Children[1] as Label;
                    if (label.Text == category.Name)
                    {
                        Device.BeginInvokeOnMainThread(() => SetCategoryActiveStyle(radBorder));
                        break;
                    }
                }
            }
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
                await Shell.Current.Navigation.PushAsync(new AddLiquidationPage());
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_dang_ky_de_duoc_dang_san_pham, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }

        }

        public void Clicked_BtnSearch(object sender, EventArgs e)
        {
            viewModel.FilterModel.Keyword = SearchBarLiquidation.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChaned(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBarLiquidation.Text))
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
