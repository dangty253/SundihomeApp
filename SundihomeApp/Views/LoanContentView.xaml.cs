using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.BankViewModel;
using SundihomeApp.Views.BankViews;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class LoanContentView : ContentView
    {
        public GoiVayFilterresultViewModel viewModel;

        private LookUpControl LookUpControlProvince;
        private LookUpControl LookUpControlDistrict;
        private LookUpControl LookUpControlBank;

        public LoanContentView()
        {
            InitializeComponent();
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
                var view = new AddLoanView(bottomModal);
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
                var dangKyNhanVienView = new DangKyNhanVienNganHangView(bottomModal);
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

        public async void FilterProvince_Click(object sender, EventArgs e)
        {
            if (LookUpControlProvince == null)
            {
                await viewModel.LoadProvinceAsync();
                LookUpControlProvince = new LookUpControl();
                LookUpControlProvince.ItemsSource = viewModel.ProvinceList;
                LookUpControlProvince.SelectedItemChange += async (s, e2) =>
                {
                    loadingPopup.IsVisible = true;
                    if (viewModel.Province.Id == -1)
                    {
                        LabelProvince.Text = Language.tinh_thanh;
                        LabelProvince.TextColor = Color.Black;
                        viewModel.Province = null;
                        viewModel._filterModel.ProvinceId = null;
                    }
                    else
                    {
                        viewModel._filterModel.ProvinceId = viewModel.Province.Id;
                        LabelProvince.Text = viewModel.Province.Name;
                        LabelProvince.TextColor = Color.FromHex("#026294");
                    }

                    LabelDistrict.Text = Language.quan_huyen;
                    await viewModel.LoadDistrictAsync();
                    viewModel.District = null;
                    viewModel._filterModel.DistrictId = null;
                    LabelDistrict.TextColor = Color.Black;
                    await viewModel.LoadOnRefreshCommandAsync();
                    loadingPopup.IsVisible = false;
                };
                LookUpControlProvince.BottomModal = bottomModal;
                LookUpControlProvince.NameDisplay = "Name";
                LookUpControlProvince.Placeholder = Language.tinh_thanh;
                LookUpControlProvince.SetBinding(LookUpControl.SelectedItemProperty, new Binding("Province") { Source = viewModel });
            }

            await LookUpControlProvince.OpenModal();
        }
        public async void FilterDistric_Click(object sender, EventArgs e)
        {
            if (LookUpControlDistrict == null)
            {
                await viewModel.LoadDistrictAsync();
                LookUpControlDistrict = new LookUpControl();
                LookUpControlDistrict.ItemsSource = viewModel.DistrictList;
                LookUpControlDistrict.SelectedItemChange += async (s, e2) =>
                {
                    if (viewModel.District.Id == -1)
                    {
                        LabelDistrict.Text = Language.quan_huyen;
                        LabelDistrict.TextColor = Color.Black;
                        viewModel.District = null;
                        viewModel._filterModel.DistrictId = null;
                    }
                    else
                    {
                        viewModel._filterModel.DistrictId = viewModel.District.Id;
                        LabelDistrict.Text = viewModel.District.Name;
                        LabelDistrict.TextColor = Color.FromHex("#026294");
                    }
                    loadingPopup.IsVisible = true;

                    await viewModel.LoadOnRefreshCommandAsync();
                    loadingPopup.IsVisible = false;
                };
                LookUpControlDistrict.BottomModal = bottomModal;
                LookUpControlDistrict.NameDisplay = "Name";
                LookUpControlDistrict.Placeholder = Language.quan_huyen;
                LookUpControlDistrict.SetBinding(LookUpControl.SelectedItemProperty, new Binding("District") { Source = viewModel });
            }
            await LookUpControlDistrict.OpenModal();
        }
        public async void FilterBank_Click(object sender, EventArgs e)
        {
            if (LookUpControlBank == null)
            {
                await viewModel.LoadBanksAsync();
                LookUpControlBank = new LookUpControl();
                LookUpControlBank.ItemsSource = viewModel.BankList;
                LookUpControlBank.SelectedItemChange += async (s, e2) =>
                {
                    if (viewModel.Bank.Id == -1)
                    {
                        LabelType.Text = Language.loai_bat_dong_san;
                        LabelType.TextColor = Color.Black;
                        viewModel.Bank = null;
                        viewModel._filterModel.BankId = null;
                    }
                    else
                    {
                        viewModel._filterModel.BankId = viewModel.Bank.Id;
                        LabelType.Text = viewModel.Bank.Name;
                        LabelType.TextColor = Color.FromHex("#026294");
                    }
                    loadingPopup.IsVisible = true;
                    await viewModel.LoadOnRefreshCommandAsync();
                    loadingPopup.IsVisible = false;
                };
                LookUpControlBank.BottomModal = bottomModal;
                LookUpControlBank.NameDisplay = "FullName";
                LookUpControlBank.Placeholder = Language.loai_bat_dong_san;
                LookUpControlBank.SetBinding(LookUpControl.SelectedItemProperty, new Binding("Bank") { Source = viewModel });
            }
            await LookUpControlBank.OpenModal();
        }

        public async void Clear_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;
            LabelProvince.Text = Language.tinh_thanh;
            LabelDistrict.Text = Language.quan_huyen;
            LabelType.Text = Language.ngan_hang;

            LabelProvince.TextColor = Color.Black;
            LabelDistrict.TextColor = Color.Black;
            LabelType.TextColor = Color.Black;
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
