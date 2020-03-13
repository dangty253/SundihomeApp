using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.BankViewModel;
using Xamarin.Forms;

namespace SundihomeApp.Views.BankViews
{
    public partial class MyGoiVayListPage : ContentPage
    {
        public MyGoiVayListPageViewModel viewModel;
        public MyGoiVayListPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new MyGoiVayListPageViewModel();
            viewModel._filterModel.EmployeeId = Guid.Parse(UserLogged.Id);
            Init();
        }

        public async Task LoadInfo()
        {
            var response = await ApiHelper.Get<BankEmployee>(ApiRouter.BANK_EMPLOYEE_DETAIL + UserLogged.Id);
            if (response.IsSuccess)
            {
                var BankEmployee = response.Content as BankEmployee;
                spanBankName.Text = BankEmployee.Bank.Name;
                spanKhuVuc.Text = BankEmployee.Address;
                imgAvatar.Source = ApiConfig.CloudStorageApiCDN + "/bank/goivay/bank_logo/" + BankEmployee.Bank.Logo;
            }
        }

        public async void Init()
        {
            var tapped = new TapGestureRecognizer()
            {
                NumberOfTapsRequired = 1,
            };
            tapped.Tapped += async (sender, e) =>
            {
                loadingPopup.IsVisible = true;
                var view = new DangKyNhanVienNganHangView(bottomModal, Guid.Parse(UserLogged.Id));
                view.OnCancel += async (s, e2) => await modalEditInfo.Hide();
                view.OnSaved += async (s, e2) =>
                {
                    await LoadInfo();
                    await modalEditInfo.Hide();
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_thong_tin_thanh_cong);
                };
                modalEditInfo.Body = view;
                loadingPopup.IsVisible = false;
                await modalEditInfo.Show();
            };
            grAccountName.GestureRecognizers.Add(tapped);

            MessagingCenter.Subscribe<GoiVayDetailPage, Guid>(this, "OnDeleted", (e, id) =>
            {
                var removeItem = viewModel.Data.SingleOrDefault(x => x.Id == id);
                if (removeItem != null)
                {
                    viewModel.Data.Remove(removeItem);
                }
            });
            await viewModel.LoadData();
            await LoadInfo();
            loadingPopup.IsVisible = false;
        }
        private async void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as GoiVay;
            await Navigation.PushAsync(new GoiVayDetailPage(item.Id));
        }
        private async void AddLoan_Clicked(object sender, EventArgs e)
        {
            var view = new AddLoanView(bottomModal);
            view.OnCancel += async (sender1, e1) => await ModalAddLoan.Hide();
            view.OnSaved += async (sender1, e1) =>
            {
                loadingPopup.IsVisible = true;
                await ModalAddLoan.Hide();
                await viewModel.LoadOnRefreshCommandAsync();
                loadingPopup.IsVisible = false;
            };
            ModalAddLoan.Body = view;
            await ModalAddLoan.Show();
        }
    }
}
