using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.Views.CompanyViews;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SundihomeApp.Views
{
    public partial class AddProjectPage : ContentPage
    {
        public AddProjectPageViewModel viewModel;
        public Guid _id;
        public ModalDiaryContentView modalDiaryContentview;
        public AddProjectPage(bool IsCompany = false)
        {
            Init();

            if (IsCompany)
            {
                viewModel.AddProjectModel.CompanyId = Guid.Parse(UserLogged.CompanyId);
            }

            InitAdd();
        }

        public AddProjectPage(Guid Id)
        {
            _id = Id;
            Init();
            InitUpdate();
        }

        public async void Init()
        {
            InitializeComponent(); 
            this.BindingContext = viewModel = new AddProjectPageViewModel();
            await CrossMedia.Current.Initialize();
            await viewModel.GetProjectDiary(_id);
            BindableLayout.SetItemsSource(ItemSourceDiary, viewModel.ProjectDiaryList);
            MessagingCenter.Subscribe<ModalDiaryContentView>(this, "OnClose", async (sender) =>
            {
                await ModalAddGhiNhan.Hide();
            });
            LookUpProjectType.OnSave += async (object sender, EventArgs e) => {
                Hide();
                Show();
                
            };
            LookUpProjectType.OnDelete += async (object sender, EventArgs e) => {
                Hide();
                viewModel.NumUtilitiChecked = 0;
            };
            LookUpUtilities.OnSave+= async (object sender, EventArgs e) => {
                viewModel.NumUtilitiChecked = viewModel.TienIchDuAnSelecteIds.Count();
            };
            LookUpUtilities.OnDelete += async (object sender, EventArgs e) => {
                viewModel.NumUtilitiChecked = 0;
            };
        }

        public async void InitAdd()
        {
            await viewModel.GetProvinceAsync();

            //set title
            viewModel.TitlePostProject = Language.dang_tin_du_an;

            viewModel.NumUtilitiChecked = 0;
            loadingPopup.IsVisible = false;
        }

        public async void InitUpdate()
        {
            AddToolBar();
            var apiResponse = await ApiHelper.Get<AddProjectModel>("api/project/" + this._id);

            stThongTinBoSung.IsVisible = true;

            if (apiResponse.IsSuccess == false) return;

            var model = apiResponse.Content as AddProjectModel;
            viewModel.AddProjectModel = model;

            DecimailEntDienTichSanTrungBinh_From.SetPrice(model.DienTichSanTrungBinh_From);
            DecimalEntDienTichSanTrungBinh_To.SetPrice(model.DienTichSanTrungBinh_To);
            DecimalDienTichCayXanhFrom.SetPrice(model.DienTichCayXanh_From);
            DecimalDienTichCayXanhTo.SetPrice(model.DienTichCayXanh_To);
            DecimalDienTichKhuDatFrom.SetPrice(model.DienTichKhuDat_From);
            DecimalDienTichKhuDatTo.SetPrice(model.DienTichKhuDat_To);
            DecimalDienTichXayDungFrom.SetPrice(model.DienTichXayDung_From);
            DecimalDienTichXayDungTo.SetPrice(model.DienTichXayDung_To);

            DecimalEntryMatDoXayDung.SetPrice(model.MatDoXayDungPercent);

            //set title
            viewModel.TitlePostProject = viewModel.AddProjectModel.Name;

            await Task.WhenAll(
                viewModel.GetProvinceAsync(),
                viewModel.GetDistrictAsync(),
                viewModel.GetWardAsync()
                );

            //gan lai tinh/thanh, quan/huyen, phuong/xa voi ID
            viewModel.AddProjectModel.Province = viewModel.ProvinceList.Single(x => x.Id == model.ProvinceId);
            viewModel.AddProjectModel.District = viewModel.DistrictList.Single(x => x.Id == model.DistrictId);
            viewModel.AddProjectModel.Ward = viewModel.WardList.Single(x => x.Id == model.WardId);



            //set loai du an cho control // Loai du an tren app = categoriBDS trong database
            if (!string.IsNullOrEmpty(model.CategoriBDS))
            {
                string[] arrCategoriBDS = model.CategoriBDS.Split(',');
                List<int> listLoaiDuAn = new List<int>();
                foreach (var item in arrCategoriBDS)
                {
                    listLoaiDuAn.Add(int.Parse(item));
                }
                viewModel.LoaiDuAnSelecteIds = listLoaiDuAn;
            }

            //set show cac stacklayout theo loai bat dong san (so tang, tang ham....)
            Show();

            //set trang thai cho control
            if (model.Status != null)
            {
                string item = viewModel.AddProjectModel.Status;
                var listStatus = ProjectStatusData.GetList();
                ProjectStatusModel modelStatus = listStatus.Where(x => x.Id == short.Parse(item)).Single();
                viewModel.AddProjectModel.TrangThai = modelStatus;
            }
            else viewModel.AddProjectModel.TrangThai = null;

            //set hinh anh cho ffimageloading
            if (string.IsNullOrEmpty(model.Images) == false)
            {
                string[] imageList = model.Images.Split(',');
                foreach (var image in imageList)
                {
                    viewModel.Media.Add(new MediaFile()
                    {
                        PreviewPath = ImageHelper.GetImageUrl("project", image),
                    });
                }
            }

            //set ckecked tien ich du an
            if (model.ImageUtilities != null)
            {
                string[] arrImageUtilities = viewModel.AddProjectModel.ImageUtilities.Split(',');
                List<int> list = new List<int>();
                foreach (var item in arrImageUtilities)
                {
                    list.Add(int.Parse(item));
                }
                viewModel.TienIchDuAnSelecteIds = list;
                viewModel.NumUtilitiChecked = viewModel.TienIchDuAnSelecteIds.Count();
            }


            loadingPopup.IsVisible = false;
        }

        public void Tapped_Checked(object sender, EventArgs e)
        {
            var grid = sender as Grid;
            var tap = grid.GestureRecognizers[0] as TapGestureRecognizer;
            Option loaiBatDongSanModel = (Option)tap.CommandParameter;
            if (loaiBatDongSanModel.IsSelected)
            {
                loaiBatDongSanModel.IsSelected = false;
            }
            else
            {
                loaiBatDongSanModel.IsSelected = true;
            }
        }
        

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            //chon tinh/thanh 
            if (viewModel.AddProjectModel.Province != null)
            {
                viewModel.AddProjectModel.ProvinceId = viewModel.AddProjectModel.Province.Id;
            }
            else
            {
                viewModel.AddProjectModel.ProvinceId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.AddProjectModel.District = null;
            viewModel.AddProjectModel.Ward = null;
            loadingPopup.IsVisible = false;
        }
        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            //chon quan/huyen
            if (viewModel.AddProjectModel.District != null)
            {
                viewModel.AddProjectModel.DistrictId = viewModel.AddProjectModel.District.Id;
            }
            else
            {
                viewModel.AddProjectModel.DistrictId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.AddProjectModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public void AddToolBar()
        {
            if (_id != Guid.Empty)
            {
                var deleteToolbar = new ToolbarItem()
                {
                    Text = Language.xoa,
                    IconImageSource = "ic_remove.png"
                };
                deleteToolbar.Clicked += Delete_Post;
                this.ToolbarItems.Add(deleteToolbar);
            }
        }

        public async void Save_Clicked(object sender1, EventArgs e1)
        {
            if (SwitchIsNegotiate.IsToggled)
            {
                SwitchIsPriceRange.IsToggled = false;
            }

            //kiem tra null Tieu de
            if (viewModel.AddProjectModel.Name == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_tieu_de_du_an, Language.dong);
                return;
            }

            //kiem tra null Loai bat dong san
            if (viewModel.LoaiDuAnSelecteIds ==null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_loai_bat_dong_san, Language.dong);
                return;
            }

            //kiem tra null Trang thai
            if (viewModel.AddProjectModel.TrangThai == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_trang_thai, Language.dong);
                return;
            }

            // kiem tra Null khoang gia
            if (SwitchIsPriceRange.IsToggled == true)
            {
                if (viewModel.AddProjectModel.PriceFrom == null || viewModel.AddProjectModel.PriceTo == null)
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_khoang_gia, Language.dong);
                    return;
                }
            }
            if (SwitchIsNegotiate.IsToggled == false && SwitchIsPriceRange.IsToggled == false)
            {
                if (viewModel.AddProjectModel.PriceFrom == null)
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_gia, Language.dong);
                    return;
                }
            }


            //kiem tra null dien tich san trung binh
            if (SwitchDienTichSanTrungBinh.IsToggled == true)
            {
                if (string.IsNullOrWhiteSpace(DecimailEntDienTichSanTrungBinh_From.Text) || string.IsNullOrWhiteSpace(DecimalEntDienTichSanTrungBinh_To.Text))
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_day_du_thong_tin_dien_tich_san, Language.dong);
                    return;
                }
            }

            //kiem tra Null dien tich cay xanh
            if (SwitchDienTichCayXanh.IsToggled == true)
            {
                if (string.IsNullOrWhiteSpace(DecimalDienTichCayXanhFrom.Text) || string.IsNullOrWhiteSpace(DecimalDienTichCayXanhTo.Text))
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_day_du_thong_tin_dien_tich_cay_xanh, Language.dong);
                    return;
                }
            }

            // kiem ta Null dien tich khu dat
            if (SwitchDienTichKhuDat.IsToggled == true)
            {
                if (string.IsNullOrWhiteSpace(DecimalDienTichKhuDatFrom.Text) || string.IsNullOrWhiteSpace(DecimalDienTichKhuDatTo.Text))
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_day_du_thong_tin_dien_tich_khu_dat, Language.dong);
                    return;
                }
            }

            //kiem tra null dien tich xay dung
            if (SwitchDienTichXayDung.IsToggled == true)
            {
                if (string.IsNullOrWhiteSpace(DecimalDienTichXayDungFrom.Text) || string.IsNullOrWhiteSpace(DecimalDienTichXayDungTo.Text))
                {
                    await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_day_du_thong_tin_dien_tich_xay_dung, Language.dong);
                    return;
                }
            }

            if (DecimalEntryMatDoXayDung.Price > 100)
            {
                await DisplayAlert(Language.thong_bao, Language.mat_do_xay_dung_khong_dung_vui_long_nhap_lai, Language.dong);
                return;
            }
            //set loai bat dong san
            if (viewModel.LoaiDuAnSelecteIds != null && viewModel.LoaiDuAnSelecteIds.Any())
            {
                viewModel.AddProjectModel.CategoriBDS = string.Join(",", viewModel.LoaiDuAnSelecteIds.ToArray());
            }
            else viewModel.AddProjectModel.CategoriBDS = null;

            //set trang thai
            if (viewModel.AddProjectModel.TrangThai != null)
            {
                viewModel.AddProjectModel.Status = viewModel.AddProjectModel.TrangThai.Id.ToString();
            }
            else viewModel.AddProjectModel.Status = null;

            //set dia chi va kiem tra null
            if (viewModel.AddProjectModel.Province != null)
            {
                viewModel.AddProjectModel.ProvinceId = viewModel.AddProjectModel.Province.Id;
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_tinh_thanh, Language.dong);
                return;
            }
            if (viewModel.AddProjectModel.District != null)
            {
                viewModel.AddProjectModel.DistrictId = viewModel.AddProjectModel.District.Id;
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_quan_huyen, Language.dong);
                return;
            }
            if (viewModel.AddProjectModel.Ward != null)
            {
                viewModel.AddProjectModel.WardId = viewModel.AddProjectModel.Ward.Id;
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_phuong_xa, Language.dong);
                return;
            }

            if (viewModel.AddProjectModel.Description == null)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_nhap_mo_ta, Language.dong);
                return;
            }

            if (viewModel.Media.Count == 0)
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_chon_hinh, Language.dong);
                return;
            }
            if (viewModel.Media.Count < 3)
            {
                await DisplayAlert(Language.thong_bao, Language.chon_it_nhat_3_hinh, Language.dong);
                return;
            }

            try
            {
                // lay dia chi	
                IEnumerable<Position> result = await new Geocoder().GetPositionsForAddressAsync(viewModel.AddProjectModel.Address);
                if (result.Any())
                {
                    viewModel.AddProjectModel.Lat = result.First().Latitude;
                    viewModel.AddProjectModel.Long = result.First().Longitude;
                }
                else
                {
                    await DisplayAlert("", Language.dia_chi_khong_thay_tren_ban_do_vui_long_nhap_lai, Language.dong);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("", Language.dia_chi_khong_thay_tren_ban_do_vui_long_nhap_lai, Language.dong);
                return;
            }

            viewModel.AddProjectModel.MatDoXayDungPercent = DecimalEntryMatDoXayDung.Price;
            viewModel.AddProjectModel.DienTichSanTrungBinh_From = DecimailEntDienTichSanTrungBinh_From.Price;
            viewModel.AddProjectModel.DienTichSanTrungBinh_To = DecimalEntDienTichSanTrungBinh_To.Price;
            viewModel.AddProjectModel.DienTichXayDung_From = DecimalDienTichXayDungFrom.Price;
            viewModel.AddProjectModel.DienTichXayDung_To = DecimalDienTichXayDungTo.Price;
            viewModel.AddProjectModel.DienTichKhuDat_From = DecimalDienTichKhuDatFrom.Price;
            viewModel.AddProjectModel.DienTichKhuDat_To = DecimalDienTichKhuDatTo.Price;
            viewModel.AddProjectModel.DienTichCayXanh_From = DecimalDienTichCayXanhFrom.Price;
            viewModel.AddProjectModel.DienTichCayXanh_To = DecimalDienTichCayXanhTo.Price;

            loadingPopup.IsVisible = true;
            

            if (viewModel.TienIchDuAnSelecteIds!=null && viewModel.TienIchDuAnSelecteIds.Any())
            {
                viewModel.AddProjectModel.ImageUtilities = string.Join(",", viewModel.TienIchDuAnSelecteIds.ToArray());
            }
            else
            {
                viewModel.AddProjectModel.ImageUtilities = null;
            }
            

            // set image va avatar
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
                    imageList[i] = item.PreviewPath.Replace(Configuration.ApiConfig.CloudStorageApiCDN + "/project/", "");
                }
            }
            if (imageList.Length > 8)
            {
                await DisplayAlert("", Language.vui_long_upload_toi_da_8_hinh_anh_bat_dong_san, Language.dong);
                loadingPopup.IsVisible = false;
                return;
            }

            viewModel.AddProjectModel.Avatar = imageList[0];
            viewModel.AddProjectModel.Images = string.Join(",", imageList);

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
            if (ImageUploaded) // neu khong can upload hoac up load thanh cong thi moi goi api.
            {
                ApiResponse apiResponse = null;
                if (this._id == Guid.Empty)
                {
                    viewModel.AddProjectModel.Id = Guid.NewGuid();
                    apiResponse = await ApiHelper.Post("api/project", viewModel.AddProjectModel, true);
                    if (apiResponse.IsSuccess)
                    {
                        MessagingCenter.Send<AddProjectPage>(this, "OnSaveProject");
                        var answer = await DisplayAlert(Language.thong_bao, Language.ban_co_muon_bo_sung_nhat_ky_du_an_khong, Language.dong_y, Language.huy);
                        if (answer)
                        {
                            _id = viewModel.AddProjectModel.Id;
                            stThongTinBoSung.IsVisible = true;
                            var view = new ModalDiaryContentView(null, _id);
                            ModalAddGhiNhan.Body = view;
                            await ModalAddGhiNhan.Show();
                            view.OnSaved += async (sender, e) =>
                            {
                                await viewModel.GetProjectDiary(_id);
                                await ModalAddGhiNhan.Hide();
                                ToastMessageHelper.ShortMessage(Language.them_ghi_nhan_thanh_cong);
                            };
                            view.OnCancel += async (sender, e) =>
                            {
                                await ModalAddGhiNhan.Hide();
                            };
                        }
                        else
                        {
                            await Shell.Current.Navigation.PopAsync();
                            ToastMessageHelper.ShortMessage(Language.luu_thanh_cong);
                        }
                    }
                }
                else
                {
                    apiResponse = await ApiHelper.Post("api/project/update", viewModel.AddProjectModel, true);
                    if (apiResponse.IsSuccess)
                    {
                        await Shell.Current.Navigation.PopAsync();
                        MessagingCenter.Send<AddProjectPage>(this, "OnSaveProject");
                        ToastMessageHelper.ShortMessage(Language.cap_nhat_thong_tin_thanh_cong);
                    }
                }
                if (!apiResponse.IsSuccess)
                {
                    loadingPopup.IsVisible = false;
                    await DisplayAlert("", Language.khong_the_luu_du_an_vui_long_thu_lai, Language.dong);
                }

            }
            loadingPopup.IsVisible = false;
        }

        void Show()
        {
            
            //kiem tra show theo loai bat dong san
            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x=>x==0)) //cao oc van phong
            {
                stTongSoVonDauTu.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoThangMay.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
                stSoLuongToaNha.IsVisible = true;
                stMatDoXayDung.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stTangHam.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
            }
            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 1)) //biet thu
            {
                stTongSoVonDauTu.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoThangMay.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
                stSoLuongToaNha.IsVisible = true;
                stMatDoXayDung.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stTangHam.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 2))// can ho cao cap
            {
                stDienTichCayXanh.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stSoTang.IsVisible = true;
                stSoThangMay.IsVisible = true;
                stTangHam.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 3)) // can ho chung cu
            {
                stSoTang.IsVisible = true;
                stSoThangMay.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stMatDoXayDung.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stDienTichSanTrungBinh.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 4)) //du lich nghi duong
            {
                stSoTang.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
                stTongSoVonDauTu.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stTangHam.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 5)) // dat phan lo
            {
                stDienTichKhuDat.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 6)) //do thi moi
            {
                stSoTang.IsVisible = true;
                stSoLuongToaNha.IsVisible = true;
                stTangHam.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 7)) // khu cong nghiep
            {
                stDienTichKhuDat.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 8)) // khu dan cu
            {
                stSoLuongSanPham.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 9)) //nha o xa hoi
            {
                stSoLuongSanPham.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
            }

            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 10)) //phuc hop
            {
                stSoLuongSanPham.IsVisible = true;
                stSoLuongToaNha.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
                stMatDoXayDung.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
            }
            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x=>x==11)) //trung tam thuong mai
            {
                stSoThangMay.IsVisible = true;
                stSoLuongToaNha.IsVisible = true;
                stSoTang.IsVisible = true;
                stTongSoVonDauTu.IsVisible = true;
                stTangHam.IsVisible = true;
            }
            if (viewModel.LoaiDuAnSelecteIds.ToArray().Any(x => x == 12)) //khac
            {
                stTongSoVonDauTu.IsVisible = true;
                stSoLuongSanPham.IsVisible = true;
                stSoLuongToaNha.IsVisible = true;
                stMatDoXayDung.IsVisible = true;
                stSoThangMay.IsVisible = true;
                stSoTang.IsVisible = true;
                stTangHam.IsVisible = true;
                stDienTichCayXanh.IsVisible = true;
                stDienTichKhuDat.IsVisible = true;
                stDienTichSanTrungBinh.IsVisible = true;
                stDienTichXayDung.IsVisible = true;
                stTongDienTichSan.IsVisible = true;
            }
        }
        void Hide()
        {
            stTongSoVonDauTu.IsVisible = false;
            stSoLuongSanPham.IsVisible = false;
            stSoLuongToaNha.IsVisible = false;
            stMatDoXayDung.IsVisible = false;
            stSoThangMay.IsVisible = false;
            stSoTang.IsVisible = false;
            stTangHam.IsVisible = false;
            stDienTichCayXanh.IsVisible = false;
            stDienTichKhuDat.IsVisible = false;
            stDienTichSanTrungBinh.IsVisible = false;
            stDienTichXayDung.IsVisible = false;
            stTongDienTichSan.IsVisible = false;
        }

        public async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=project", form);
            string body = await uploadResponse.Content.ReadAsStringAsync();
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(body);
            return uploadResonse;
        }

        private async void Delete_Post(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.thong_bao, Language.ban_co_chac_chan_muon_xoa_du_an_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;

            var deleteReponse = await ApiHelper.Delete("api/project/" + this.viewModel.AddProjectModel.Id);
            if (deleteReponse.IsSuccess)
            {
                await Navigation.PopAsync();
                MessagingCenter.Send<AddProjectPage, Guid>(this, "OnDeleteSuccess", this.viewModel.AddProjectModel.Id);
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.xoa_khong_thanh_cong, Language.dong);

            }
            loadingPopup.IsVisible = false;
        }

        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }
        public void ClearDescription_Clicked(object sender, EventArgs e)
        {
            viewModel.AddProjectModel.Description = null;
        }

        ////project diary
        public async void AddDiary_Tapped(object sender1, EventArgs e1)// tapped them ghi nhan
        {
            var view = new ModalDiaryContentView(null, _id);

            ModalAddGhiNhan.Body = view;
            await ModalAddGhiNhan.Show();

            view.OnSaved += async (sender, e) =>
            {
                await viewModel.GetProjectDiary(_id);
                await ModalAddGhiNhan.Hide();
                ToastMessageHelper.ShortMessage(Language.them_nhat_ky_thanh_cong);
            };
            view.OnCancel += async (sender, e) =>
            {
                await ModalAddGhiNhan.Hide();
            };
        }
        ////xoa nhat ky
        private async void DeleteDiary_Tapped(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.thong_bao, Language.ban_co_chac_chan_muon_xoa_nhat_ky_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;

            ProjectDiary item = ((sender as Label).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as ProjectDiary;

            var deleteReponse = await ApiHelper.Delete($"{ApiRouter.PROJECT_DIARY_DELETE_PROEJCTDIARY}/{item.Id}", true);
            if (deleteReponse.IsSuccess)
            {
                await viewModel.GetProjectDiary(_id);
                if (item.Image != null)
                {
                    await ApiHelper.Delete(ApiRouter.DELETE_IMAGE + "?bucketName=sundihome/project/diary&files=" + item.Image);
                }
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.xoa_khong_thanh_cong, Language.dong);
            }
            loadingPopup.IsVisible = false;
        }
        ////cap nhat nhat ky
        public async void UpdateDiary_Tapped(object sender2, EventArgs e2)
        {
            ProjectDiary item = ((sender2 as Telerik.XamarinForms.Primitives.RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as ProjectDiary;
            var apireponse = await ApiHelper.Get<ProjectDiary>($"{ApiRouter.PROJECT_DIARY_GET_ONE_PROJECTDIARY}/{item.Id}");
            if (!apireponse.IsSuccess) return;
            var model = apireponse.Content as ProjectDiary;
            var view = new ModalDiaryContentView(model.Id, model.ProjectId);
            ModalAddGhiNhan.Body = view;
            await ModalAddGhiNhan.Show();
            view.OnSaved += async (sender, e) =>
            {
                await viewModel.GetProjectDiary(_id);
                await ModalAddGhiNhan.Hide();
                ToastMessageHelper.ShortMessage(Language.cap_nhat_thong_tin_thanh_cong);
            };
            view.OnCancel += async (sender, e) =>
            {
                await ModalAddGhiNhan.Hide();
            };
        }
        public async void XemThemNhatKy(object sender, EventArgs e)
        {
            viewModel.Page += 1;
            await viewModel.GetProjectDiary(_id);
        }
    }
}
