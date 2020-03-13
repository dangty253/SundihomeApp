using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class MyLiquidationListPage : ContentPage
    {
        public LiquidationFilterViewModel viewModel;
        public int CurrentIndex = 0;
        private int _type;

        public MyLiquidationListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new LiquidationFilterViewModel();
            viewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);

            ListViewThanhLy.ItemTemplate = new DataTemplate(typeof(Cells.LiquidationCells.LiquidationViewCell));

            Init();
        }
        public async void Init()
        {
            MessagingCenter.Subscribe<AddLiquidationPage>(this, "OnSaveItem", (sender) => viewModel.RefreshCommand.Execute(null));
            MessagingCenter.Subscribe<LiquidationDetailPage, Guid>(this, "OnDeleted", (sender, liquidationId) =>
            {
                if (this.viewModel.Data.Any(x => x.Id == liquidationId))
                {
                    var removeItem = this.viewModel.Data.Single(x => x.Id == liquidationId);
                    this.viewModel.Data.Remove(removeItem);
                }
            });
            ListViewThanhLy.ItemTapped += async (sender, e) =>
            {
                var item = e.Item as Liquidation;
                await Navigation.PushAsync(new LiquidationDetailPage(item.Id));
            };
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void FilterByStatus_Clicked(object sender, EventArgs e)
        {
            RadBorder radBoder = sender as RadBorder;
            if (radBoder.BackgroundColor == Color.FromHex("#eeeeee")) return;

            TapGestureRecognizer tapGestureRecognizer = radBoder.GestureRecognizers[0] as TapGestureRecognizer;

            if (tapGestureRecognizer.CommandParameter != null)
            {
                int Status = int.Parse(tapGestureRecognizer.CommandParameter.ToString());
                viewModel.FilterModel.Status = Status;
            }
            else
            {
                viewModel.FilterModel.Status = null;
            }
            loadingPopup.IsVisible = true;


            // inactive
            foreach (RadBorder item in StackLayoutFilter.Children)
            {
                item.BorderColor = Color.FromHex("#eeeeee");
                item.BackgroundColor = Color.White;
            }
            // active.
            radBoder.BackgroundColor = Color.FromHex("#eeeeee");
            radBoder.BorderColor = Color.FromHex("#aaaaaa");



            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void AddLiquidation_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new AddLiquidationPage());
        }

        public void Clicked_BtnSearch(object sender, EventArgs e)
        {
            viewModel.FilterModel.Keyword = MySearchBarLiquidationListPage.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChaned(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MySearchBarLiquidationListPage.Text))
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
