using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.Views.MoiGioiViews;
using Telerik.XamarinForms.Input;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;
using CompanyEntity = SundihomeApi.Entities.Company;
namespace SundihomeApp.Views.CompanyViews
{
    [QueryProperty("SelectedIndex","index")]
    public partial class CompanyProfileDetailPage : ContentPage
    {
        public string SelectedIndex
        {
            set
            {
                if (value == "0")
                {
                    ControlSegment.SetActive(0);
                    Segment_OnSelectedIndexChanged(null, EventArgs.Empty);
                }
                else if (value == "1")
                {
                    ControlSegment.SetActive(1);
                    Segment_OnSelectedIndexChanged(null, EventArgs.Empty);
                }
                else
                {
                    ControlSegment.SetActive(2);
                    Segment_OnSelectedIndexChanged(null, EventArgs.Empty);
                }
            }
        }
        private Guid _compnayId;
        private CompanyEntity Company { get; set; }

        public CompanyProfileDetailPage(Guid Id)
        {
            InitializeComponent();
            _compnayId = Id;
            Init();
        }

        public async void Init()
        {
            await LoadDetailCompany();
            this.Title = Company.Name;
            this.MainCompanyContentView.Content = new CompanyInfoContentView(this.Company);
            SetVisibleEditCompanyDetail();

            ControlSegment.ItemsSource = new List<string>() { Language.cong_ty, Language.san_pham, Language.hinh_anh };
            ControlSegment.SetActive(0);

            MessagingCenter.Subscribe<AddCompanyPage>(this, "OnSaveCompany", async (sender) =>
            {
                // chi co thong tin va hinh anh thay doi. cho nen set null lai.
                MainCompanyContentView.Content = null;
                MainImagesContentView.Content = null;
                await LoadDetailCompany(); // load lai thong tin cong ty

                // goi ham nay de quay ve tab thong tin cong ty
                //Company_Clicked(BtnCompany, EventArgs.Empty);
            });

            loadingPopup.IsVisible = false;
        }


        private void SetVisibleEditCompanyDetail()
        {
            if (string.IsNullOrWhiteSpace(UserLogged.CompanyId))
            {
                btnEmployeeRegister.IsVisible = true;
            }
        }

        public async Task LoadDetailCompany()
        {
            ApiResponse response = await ApiHelper.Get<CompanyEntity>(Configuration.ApiRouter.COMANY_GETBYID + $"/{_compnayId}");
            if (response.IsSuccess)
            {
                this.Company = response.Content as CompanyEntity;
            }
        }
        public void Segment_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            MainCompanyContentView.IsVisible = false;
            MainProductContentView.IsVisible = false;
            MainImagesContentView.IsVisible = false;

            var index = ControlSegment.GetCurrentIndex();
            if (index == 0)
            {
                if (this.MainCompanyContentView.Content==null)
                {
                    this.MainCompanyContentView.Content = new CompanyInfoContentView(this.Company);
                }
                MainCompanyContentView.IsVisible = true;
            }
            else if (index == 1)
            {
                if (this.MainProductContentView.Content==null)
                {
                    this.MainProductContentView.Content = new ProductContentView(_compnayId);
                }
                MainProductContentView.IsVisible = true;
            }
            else
            {
                if (this.MainImagesContentView.Content==null)
                {
                    if (this.Company.ImagesList != null)
                    {
                        var imageList = new string[this.Company.ImagesList.Length];
                        for (int i = 0; i < Company.ImagesList.Length; i++)
                        {
                            imageList[i] = ApiConfig.CloudStorageApiCDN + "/company/" + Company.ImagesList[i];
                        }

                        this.MainImagesContentView.Content = new CompanyImageListContentView(imageList);
                    }
                }
                MainImagesContentView.IsVisible = true;
            }
        }



        private async void EditCompany_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCompanyPage(Guid.Parse(UserLogged.CompanyId)));
        }

        private async void EmployeeRegister_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_de_gui_ho_so_dang_ky_nhan_vien, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }

            if (UserLogged.Type == 1)
            {
                //Dang ky nhan vien
                var confirm = await DisplayAlert(Language.xac_nhan, $"{Language.ban_co_chac_chan_muon_gui_ho_so_dang_nhan_vien_cho_cong_ty} {this.Company.Name}?", Language.dong_y, Language.tu_choi);
                if (!confirm) return;
                var responseRegisterEmployee = await ApiHelper.Post(ApiRouter.EMPLOYEE_REGISTER + this.Company.Id, null, true);
                if (responseRegisterEmployee.IsSuccess)
                {
                    await DisplayAlert("", Language.dang_ky_nhan_vien_thanh_cong, Language.dong);
                }
                else
                {
                    await DisplayAlert("", Language.ban_da_gui_ho_so_den_cong_ty_va_dang_cho_xet_duyet_vui_long_doi, Language.dong);
                }
            }
            else
            {
                var confirm = await DisplayAlert(Language.xac_nhan, Language.de_dang_ky_nha_vien_ban_phai_dang_ky_moi_gioi_truoc_ban_muon_dang_ky_moi_gioi_khong, Language.dong_y, Language.tu_choi);
                if (confirm == false) return;
                //Dang ky moi gioi
                if (ModalDangKyMoiGioi.Body == null)
                {
                    var dangKyMoiGioiContentView = new DangKyMoiGioiContentView(LookUpModal, Guid.Parse(UserLogged.Id));
                    dangKyMoiGioiContentView.OnSaved += async (object s, EventArgs e2) =>
                    {
                        await ModalDangKyMoiGioi.Hide();
                        EmployeeRegister_Clicked(null, EventArgs.Empty);
                    };
                    dangKyMoiGioiContentView.OnCancel += async (object ssender, EventArgs e2) => await ModalDangKyMoiGioi.Hide();
                    ModalDangKyMoiGioi.Body = dangKyMoiGioiContentView;
                    ModalDangKyMoiGioi.CustomCloseButton(dangKyMoiGioiContentView.Cancel_Clicked);
                }
                await ModalDangKyMoiGioi.Show();
            }
        }

        private async void OnShareData_Clicked(object sender, EventArgs e)
        {
            await CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = $"{ApiConfig.WEB_IP}company/{this.Company.Id}" });
        }
    }
}
