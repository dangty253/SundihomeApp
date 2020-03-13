using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities.CompanyEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class AddCompanyPage : ContentPage
    {
        public AddCompanyPageViewModel viewModel;
        public Guid _id;
        public AddCompanyPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddCompanyPageViewModel();
            Init();
            InitAdd();
        }
        public AddCompanyPage(Guid Id)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddCompanyPageViewModel();
            _id = Id;
            Init();
            InitUpdate();

        }
        public async void Init()
        {
            this.modalAddHistory.Body.BindingContext = viewModel;
            this.modalAddHistory.CustomCloseButton(CloseModalHistory_Clicked);

            //await viewModel.GetLichSuPhatTrienCongTyAsync(_id);
            //await viewModel.GetThanhTuuCongTyAsync(_id);
            //await viewModel.GetNhanVienUuTuCongTyAsync(_id);

            await Task.WhenAll(viewModel.GetLichSuPhatTrienCongTyAsync(_id),
                viewModel.GetThanhTuuCongTyAsync(_id),
                viewModel.GetNhanVienUuTuCongTyAsync(_id));

            BindableLayout.SetItemsSource(ItemSourceHistory, viewModel.LichSuPhatTrienCongTyList);
            BindableLayout.SetItemsSource(ItemSourceSucceed, viewModel.ThanhTuuCongTyList);
            BindableLayout.SetItemsSource(collectionViewList, viewModel.NhanVienUuTuCongTyList);
        }
        public async void InitAdd()
        {
            await viewModel.GetProviceAsync();
            viewModel.TitleCompany = Language.dang_ky_cong_ty;
            loadingPopup.IsVisible = false;
        }

        public async void InitUpdate()
        {
            stThongTinBoSung.IsVisible = true;
            var apiResponse = await ApiHelper.Get<AddCompanyModel>(ApiRouter.COMANY_GETBYID + "/" + _id);
            if (apiResponse.IsSuccess == false) return;

            var model = apiResponse.Content as AddCompanyModel;
            viewModel.AddCompanyModel = model;

            //set title
            viewModel.TitleCompany = Language.cap_nhat_cong_ty;

            await Task.WhenAll(
                viewModel.GetProviceAsync(),
                viewModel.GetDistrictAsync(),
                viewModel.GetWardAsync()
                );
            //set lai dia chi
            viewModel.AddCompanyModel.Province = viewModel.ProvinceList.Single(x => x.Id == model.ProvinceId);
            viewModel.AddCompanyModel.District = viewModel.DistrictList.Single(x => x.Id == model.DistrictId);
            viewModel.AddCompanyModel.Ward = viewModel.WardList.Single(x => x.Id == model.WardId);


            //set lai loai cong ty
            viewModel.AddCompanyModel.LoaiCongTy = viewModel.ListLoaiCongTy.Single(x => x.Id == model.NganhNgheId);

            viewModel.AddCompanyModel.LogoFullUrl = ApiConfig.CloudStorageApiCDN + "/company/" + viewModel.AddCompanyModel.Logo;

            if (string.IsNullOrEmpty(model.Images) == false)
            {
                string[] imageList = model.Images.Split(',');
                foreach (var image in imageList)
                {
                    viewModel.Media.Add(new MediaFile()
                    {
                        PreviewPath = ImageHelper.GetImageUrl("company", image),
                    });
                }
            }
            loadingPopup.IsVisible = false;
        }

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            //chon tinh/thanh
            if (viewModel.AddCompanyModel.Province != null)
            {
                viewModel.AddCompanyModel.ProvinceId = viewModel.AddCompanyModel.Province.Id;
            }
            else
            {
                viewModel.AddCompanyModel.Province = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.AddCompanyModel.District = null;
            viewModel.AddCompanyModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            //chon quan/huyen
            if (viewModel.AddCompanyModel.District != null)
            {
                viewModel.AddCompanyModel.DistrictId = viewModel.AddCompanyModel.District.Id;
            }
            else
            {
                viewModel.AddCompanyModel.District = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.AddCompanyModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void AddHistory_Tapped(object sender, EventArgs e)
        {
            modalAddHistory.Title = Language.lich_su_phat_trien;
            await modalAddHistory.Show();
        }
        public async void AddSucceed_Tapped(object sender, EventArgs e)
        {
            modalAddHistory.Title = Language.thanh_tuu_dat_duoc;
            await modalAddHistory.Show();
        }


        public async void Save_Click(object sender, EventArgs e)
        {
            //kiem tra null
            if (viewModel.AddCompanyModel.Name == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_ten_cong_ty, Language.dong);
                return;
            }
            if (viewModel.AddCompanyModel.ShortName == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_ten_rut_gon, Language.dong);
                return;
            }
            if (string.IsNullOrEmpty(viewModel.AddCompanyModel.MST))
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_ma_so_thue, Language.dong);
                return;
            }

            //kiem tra ton tai dau (,) trong ma so thue
            string[] boolMST = viewModel.AddCompanyModel.MST.Split(',');
            if (viewModel.AddCompanyModel.MST.Contains(",") || viewModel.AddCompanyModel.MST.Contains("."))
            {
                await DisplayAlert(Language.thong_bao, Language.ma_so_thue_chua_dung_vui_long_kiem_tra_lai, Language.dong);
                return;
            }
            if (viewModel.AddCompanyModel.Province == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_tinh_thanh, Language.dong);
                return;
            }
            if (viewModel.AddCompanyModel.District == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_quan_huyen, Language.dong);
                return;
            }
            if (viewModel.AddCompanyModel.Ward == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_phuong_xa, Language.dong);
                return;
            }
            if (viewModel.AddCompanyModel.LoaiCongTy == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_loai_cong_ty, Language.dong);
                return;
            }
            if (string.IsNullOrEmpty(viewModel.AddCompanyModel.Logo))
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_logo, Language.dong);
                return;
            }


            loadingPopup.IsVisible = true;

            MultipartFormDataContent form = new MultipartFormDataContent();
            string[] imageList = new string[viewModel.Media.Count];
            for (int i = 0; i < viewModel.Media.Count; i++)
            {
                var item = viewModel.Media[i];
                // chua upload. upload roi link = null 
                if (string.IsNullOrEmpty(item.Path) == false) // co link la co chon tu dien thoai.
                {
                    imageList[i] = $"{Guid.NewGuid().ToString()}.jpg";
                    var stream = new MemoryStream(File.ReadAllBytes(item.Path));
                    var content = new StreamContent(stream);
                    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "files" + i,
                        FileName = imageList[i]
                    };
                    form.Add(content);
                }
                else
                {
                    imageList[i] = item.PreviewPath.Replace(ApiConfig.CloudStorageApiCDN + "/company/", "");
                }
            }

            if (imageList.Count() != 0)
            {
                viewModel.AddCompanyModel.Images = string.Join(",", imageList);
            }
            else
            {
                viewModel.AddCompanyModel.Images = null;
            }

            //gan
            viewModel.AddCompanyModel.ProvinceId = viewModel.AddCompanyModel.Province.Id;
            viewModel.AddCompanyModel.DistrictId = viewModel.AddCompanyModel.District.Id;
            viewModel.AddCompanyModel.WardId = viewModel.AddCompanyModel.Ward.Id;
            viewModel.AddCompanyModel.NganhNgheId = viewModel.AddCompanyModel.LoaiCongTy.Id;
            viewModel.AddCompanyModel.CreatedDate = DateTime.Now;

            //viewModel.AddCompanyModel.Logo = viewModel.AddCompanyModel.Logo.Replace($"{ApiConfig.CloudStorageApiCDN}/company/", "");

            if (UserLogged.IsLogged)
            {
                if (UserLogged.Id != null)
                {
                    viewModel.AddCompanyModel.CreatedById = Guid.Parse(UserLogged.Id);
                }
            }

            bool ImageUploaded = true;
            if (viewModel.Media.Any(x => x.Path != null))
            {
                ApiResponse uploadImageResponse = await UploadImage(form);
                if (!uploadImageResponse.IsSuccess)
                {
                    await DisplayAlert("", Language.hinh_anh_vuot_qua_dung_luong_vui_long_thu_lai, Language.dong);
                    ImageUploaded = false;
                    loadingPopup.IsVisible = false;
                }
            }
            else
            {
                ImageUploaded = true; // ko can upload
            }

            if (ImageUploaded)
            {
                ApiResponse apiResponse = null;
                if (this._id == Guid.Empty)
                {
                    Guid CompanyId = Guid.NewGuid();
                    viewModel.AddCompanyModel.Id = CompanyId;
                    apiResponse = await ApiHelper.Post(ApiRouter.COMPANY_ADD, viewModel.AddCompanyModel);
                    if (apiResponse.IsSuccess)
                    {
                        UserLogged.CompanyId = CompanyId.ToString();
                        UserLogged.RoleId = 0;

                        var appShell = (AppShell)Shell.Current;

                        appShell.AddMenu_QuanLyCongTy();
                        appShell.AddMenu_QuanLyMoiGioi(); // Để thêm nội bộ

                        try
                        {
                            var answer = await DisplayAlert(Language.thong_bao, Language.ban_co_muon_bo_sung_thong_tin_khong, Language.dong_y, Language.huy);
                            if (!answer)
                            {
                                await Shell.Current.Navigation.PopAsync();
                                MessagingCenter.Send<AddCompanyPage>(this, "OnSaveCompany");

                                await Shell.Current.GoToAsync("//" + AppShell.QUANLYCONGTY);
                                ToastMessageHelper.ShortMessage(Language.cong_ty_cua_ban_da_duoc_luu_thanh_cong);
                            }
                            else
                            {
                                stThongTinBoSung.IsVisible = true;
                                //ModalPopupHistory.IsVisible = true;
                                await modalAddHistory.Show();
                                _id = viewModel.AddCompanyModel.Id;
                                modalAddHistory.Title = Language.lich_su_phat_trien;
                                viewModel.TitleCompany = Language.thong_tin_cong_ty;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        loadingPopup.IsVisible = false;
                        await DisplayAlert("", Language.khong_the_luu_cong_ty_vui_long_thu_lai, Language.dong);
                    }
                }
                else
                {
                    apiResponse = await ApiHelper.Put(ApiRouter.COMPANY_UPDATE, viewModel.AddCompanyModel);
                    if (apiResponse.IsSuccess)
                    {
                        try
                        {
                            await Shell.Current.Navigation.PopAsync();
                            MessagingCenter.Send<AddCompanyPage>(this, "OnSaveCompany");
                            ToastMessageHelper.ShortMessage(Language.cap_nhat_thanh_cong);
                        }
                        catch { }
                    }
                    else
                    {
                        loadingPopup.IsVisible = false;
                        await DisplayAlert("", Language.khong_the_cap_nhat_cong_ty_vui_long_thu_lai, Language.dong);
                    }
                }


            }
            loadingPopup.IsVisible = false;
        }

        public async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=company", form);
            string body = await uploadResponse.Content.ReadAsStringAsync();
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(body);
            return uploadResonse;
        }

        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }
        public void DeleteContentSlogan_Clicked(object sender, EventArgs e)
        {
            edtSlogan.Text = null;
        }
        public void DeleteContentAboutCompany_Clicked(object sender, EventArgs e)
        {
            edtAboutCompany.Text = null;
        }

        public async void CloseModalHistory_Clicked(object sender, EventArgs e)
        {
            await modalAddHistory.Hide();
            viewModel.CancelPopUp();
        }
        public async void XemThemLichSuPhatTrien(object sender, EventArgs e)
        {
            viewModel.LichSuPhatTrienCongTyPage += 1;
            await viewModel.GetLichSuPhatTrienCongTyAsync(_id);
        }
        public async void XemThemThanhTuu(object sender, EventArgs e)
        {
            viewModel.ThanhTuuCongTyPage += 1;
            await viewModel.GetThanhTuuCongTyAsync(_id);
        }
        public async void OnBtnSaveHistory_Click(object sender, EventArgs e)
        {
            if (viewModel.Title == null) await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_tieu_de, Language.dong);
            else if (viewModel.SelectedYear == null) await DisplayAlert(Language.thong_bao, Language.vui_long_chon_thoi_gian, Language.dong);
            else if (string.IsNullOrWhiteSpace(viewModel.Description)) await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_noi_dung, Language.dong);
            else
            {
                if (modalAddHistory.Title == Language.lich_su_phat_trien)
                {
                    await viewModel.PostLichSuPhatTrienCongTyAsync();
                    await modalAddHistory.Hide();
                    viewModel.LichSuPhatTrienCongTyPage = 1;
                    viewModel.LichSuPhatTrienCongTyList.Clear();
                    await viewModel.GetLichSuPhatTrienCongTyAsync(_id);
                }
                else if (modalAddHistory.Title == Language.thanh_tuu_dat_duoc)
                {
                    await viewModel.PostThanhTuuCongTyAsync();
                    await modalAddHistory.Hide();
                    viewModel.ThanhTuuCongTyPage = 1;
                    viewModel.ThanhTuuCongTyList.Clear();
                    await viewModel.GetThanhTuuCongTyAsync(_id);
                }
            }
        }

        public async void DeleteHistoryItemClick(Object Sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.xac_nhan_xoa, Language.luu_y_ban_co_chac_chan_muon_xoa_lich_su_phat_trien_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            LichSuPhatTrienCongTy item = ((Sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as LichSuPhatTrienCongTy;
            await viewModel.DeleteLichSuPhatTrienCongTyAsync(item);
        }
        public async void DeleteSucceedItemClick(Object Sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.xac_nhan_xoa, Language.luu_y_ban_co_chac_chan_muon_xoa_thanh_tuu_dat_duoc_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;
            ThanhTuuCongTy item = ((Sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as ThanhTuuCongTy;
            await viewModel.DeleteThanhTuuCongTyAsync(item);
        }
        public void DeleteContentModalPopup_Clicked(object sender, EventArgs e)
        {
            edtContentModalPopup.Text = null;
        }
    }
}
