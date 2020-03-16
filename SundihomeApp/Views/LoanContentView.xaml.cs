using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.BankViewModel;
using SundihomeApp.Views.BankViews;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class LoanContentView : AbsoluteLayout
    {
        public GoiVayFilterresultViewModel viewModel;
        private LoadingPopup loadingPopup;
        public LoanContentView(LoadingPopup LoadingPopup)
        {
            InitializeComponent();
            this.loadingPopup = LoadingPopup;
            this.BindingContext = viewModel = new GoiVayFilterresultViewModel();
            Init();
        }

        public LoanContentView()
        {
            InitializeComponent();

            loadingPopup = new LoadingPopup()
            {
                IsVisible = true
            };
            Children.Add(loadingPopup);

            this.BindingContext = viewModel = new GoiVayFilterresultViewModel();
            Init();
        }

        public async void Init()
        {
            MessagingCenter.Subscribe<AddLoanView>(this, "OnSave", async (senser) =>
            {
                await viewModel.LoadOnRefreshCommandAsync();
            });
            MessagingCenter.Subscribe<GoiVayDetailPage, Guid>(this, "OnDeleted", (e, id) =>
            {
                var removeItem = viewModel.Data.SingleOrDefault(x => x.Id == id);
                if (removeItem != null)
                {
                    viewModel.Data.Remove(removeItem);
                }
            });
            await viewModel.LoadData();
            await viewModel.LoadProvinceAsync();
            await viewModel.LoadBanksAsync();
            loadingPopup.IsVisible = false;
        }

        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as GoiVay;
            await Navigation.PushAsync(new GoiVayDetailPage(item.Id));
        }

        private async void Calculator_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalculatorPage());
        }

        private async void AddLoan_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }

            var resonse = await ApiHelper.Get<BankEmployee>(ApiRouter.BANK_EMPLOYEE_DETAIL + UserLogged.Id);
            if (resonse.IsSuccess && resonse.Content != null)
            {
                var view = new AddLoanView(LookUpModal);
                view.OnCancel += async (sender1, e1) => await MainCenterModal.Hide();
                view.OnSaved += async (sender1, e1) =>
                {
                    loadingPopup.IsVisible = true;
                    await MainCenterModal.Hide();
                    await viewModel.LoadOnRefreshCommandAsync();
                    loadingPopup.IsVisible = false;
                };
                MainCenterModal.Title = Language.dang_goi_vay;
                MainCenterModal.Body = view;
                await MainCenterModal.Show();
            }
            else
            {
                await Shell.Current.DisplayAlert("", Language.dang_ky_nhan_vien_ngan_hang, Language.dong);
                var dangKyNhanVienView = new DangKyNhanVienNganHangView(LookUpModal);
                dangKyNhanVienView.OnCancel += async (s, e2) => await ModalBankEmployeeRegister.Hide();
                dangKyNhanVienView.OnSaved += async (s, e2) =>
                {
                    await ModalBankEmployeeRegister.Hide();
                    ToastMessageHelper.ShortMessage(Language.dang_ky_nhan_vien_thanh_cong);
                    AddLoan_Clicked(sender, EventArgs.Empty);
                };
                ModalBankEmployeeRegister.Body = dangKyNhanVienView;
                await ModalBankEmployeeRegister.Show();
            }
        }

        public void Clicked_BtnSearch(object sender, EventArgs e)
        {
            viewModel._filterModel.Keyword = searchBar.Text;
            viewModel.RefreshCommand.Execute(null);
        }

        public void Search_TextChaned(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text))
            {
                if (!string.IsNullOrWhiteSpace(viewModel._filterModel.Keyword))
                {
                    viewModel._filterModel.Keyword = null;
                    viewModel.RefreshCommand.Execute(null);
                }
            }
        }

        public async void FilterDistrict_Changed(object sender, LookUpChangeEvent e)
        {
            loadingPopup.IsVisible = true;
            if (viewModel.District.Id == -1)
            {
                viewModel.District = null;
                viewModel._filterModel.DistrictId = null;
            }
            else viewModel._filterModel.DistrictId = viewModel.District.Id;

            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void FilterBank_Changed(object sender, LookUpChangeEvent e)
        {
            if (viewModel.Bank.Id == -1)
            {
                viewModel.Bank = null;
                viewModel._filterModel.BankId = null;
            }
            else viewModel._filterModel.BankId = viewModel.Bank.Id;

            loadingPopup.IsVisible = true;
            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }

        public async void FilterProvince_Changed(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            if (viewModel.Province.Id == -1)
            {
                viewModel.Province = null;
                viewModel._filterModel.ProvinceId = null;
            }
            else
            {
                viewModel._filterModel.ProvinceId = viewModel.Province.Id;
            }
            viewModel.District = null;
            await Task.WhenAll(viewModel.LoadDistrictAsync(), viewModel.LoadOnRefreshCommandAsync());
            loadingPopup.IsVisible = false;
        }

        public async void Clear_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            viewModel.Province = null;
            viewModel._filterModel.ProvinceId = null;
            viewModel.District = null;
            viewModel._filterModel.DistrictId = null;
            viewModel.Bank = null;
            viewModel._filterModel.BankId = null;
            viewModel.DistrictList.Clear();

            await viewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
        }
    }
}
