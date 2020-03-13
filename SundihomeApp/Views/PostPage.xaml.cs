using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SundihomeApp.Views
{
    public partial class PostPage : ContentPage
    {
        public PostPageViewModel viewModel;
        private Guid _id;
        public PostPage(int postType = 0, Guid? CompanyId = null)
        {
            InitializeComponent();
            Init();
            InitAdd();
            if (CompanyId.HasValue)
            {
                viewModel.PostModel.CompanyId = CompanyId.Value;
                viewModel.PostModel.CompanyStatus = 1;
            }
            SegmentedLoaiHinh.SelectedIndex = postType;
        }
        public PostPage(Guid Id)
        {
            InitializeComponent();
            Init();
            _id = Id;
            InitUpdate();
        }
        public async void Init()
        {
            this.BindingContext = viewModel = new PostPageViewModel();

            //set mac dinh ngay cam ket la ngay hien tai
            radDateTimePicker_From.DefaultDisplayDate = DateTime.Now;
            radDateTimePicker_To.DefaultDisplayDate = DateTime.Now;

            SegmentedLoaiHinh.ItemsSource = new string[] { Language.can_ban, Language.cho_thue, Language.can_mua, Language.can_thue };
            await CrossMedia.Current.Initialize();
            await viewModel.GetProjects();
        }

        public async void InitAdd()
        {
            await viewModel.GetProvinceAsync();
            loadingPopup.IsVisible = false;
        }

        private async void InitUpdate()
        {

            var apiResponse = await ApiHelper.Get<PostModel>($"{ApiRouter.POST_GETBYID}/{this._id}");
            if (apiResponse.IsSuccess == false) return;

            var model = apiResponse.Content as PostModel;
            model.AreaFromText = DecimalHelper.DecimalToText(model.AreaFrom, 2);
            model.AreaToText = DecimalHelper.DecimalToText(model.AreaTo, 2);
            model.ChieuSauFormatText = DecimalHelper.DecimalToText(model.ChieuSau, 2);
            model.DuongVaoFormatText = DecimalHelper.DecimalToText(model.DuongVao, 2);
            model.MatTienFormatText = DecimalHelper.DecimalToText(model.MatTien, 2);

            EntryAreaFrom.SetPrice(model.AreaFrom);
            EntryAreaTo.SetPrice(model.AreaTo);
            EntryChieuSau.SetPrice(model.ChieuSau);
            EntryMatTien.SetPrice(model.MatTien);
            EntryDuongVao.SetPrice(model.DuongVao);

            viewModel.PostModel = model;

            await Task.WhenAll(
                viewModel.LoadLoaiBatDongSan(viewModel.PostModel.PostType),
                viewModel.GetProvinceAsync(),
                viewModel.GetDistrictAsync(),
                viewModel.GetWardAsync()
            );

            viewModel.PostModel.LoaiBatDongSan = viewModel.LoaiBatDongSanList.SingleOrDefault(x => x.Id == model.LoaiBatDongSanId);
            viewModel.PostModel.TinhTrangPhapLy = viewModel.TinhTrangPhaplyList.SingleOrDefault(x => x.Id == model.TinhTrangPhapLyId);

            LoaiBatDongSan_Change(null, null);

            // set dia chi

            viewModel.PostModel.Province = viewModel.ProvinceList.Single(x => x.Id == model.ProvinceId);
            viewModel.PostModel.District = viewModel.DistrictList.Single(x => x.Id == model.DistrictId);
            viewModel.PostModel.Ward = viewModel.WardList.Single(x => x.Id == model.WardId);

            if (model.ProjectId.HasValue)
            {
                viewModel.PostModel.Project = viewModel.ProjectList.SingleOrDefault(x => x.Id == model.ProjectId);
            }

            // set so phong ngu cho control
            if (model.NumOfBedRoom.HasValue)
            {
                var SoPhongNguList = SegmentSoPhongNgu.ItemsSource.Cast<short>().ToList();
                SegmentSoPhongNgu.SelectedIndex = SoPhongNguList.IndexOf(viewModel.PostModel.NumOfBedRoom.Value);
            }

            // set so phong tam cho control
            if (model.NumOfBathRoom.HasValue)
            {
                var SoPhongTamList = SegmentSoPhongTam.ItemsSource.Cast<short>().ToList();
                SegmentSoPhongTam.SelectedIndex = SoPhongTamList.IndexOf(viewModel.PostModel.NumOfBathRoom.Value);
            }

            // set huong
            if (model.HuongId.HasValue)
            {
                viewModel.PostModel.Huong = viewModel.HuongList.SingleOrDefault(x => x.Id == model.HuongId);
            }

            // noi that tien ich
            if (!string.IsNullOrEmpty(model.Utilities))
            {
                viewModel.SelectedUtitlitesId = Array.ConvertAll(model.Utilities.Split(','), s => int.Parse(s)).ToList();
            }

            // ban/cho thue moi load hinh.
            if (viewModel.PostModel.PostType == 0 || viewModel.PostModel.PostType == 1)
            {
                // load hinh anh.
                if (string.IsNullOrEmpty(model.Images) == false)
                {
                    string[] imageList = model.Images.Split(',');
                    foreach (var image in imageList)
                    {
                        viewModel.Media.Add(new MediaFile()
                        {
                            PreviewPath = ImageHelper.GetImageUrl("post", image),
                        });
                    }
                }
            }

            loadingPopup.IsVisible = false;
        }

        // thay doi loai hinh
        public async void LoaiHinh_Change(object sender, ValueChangedEventArgs e)
        {
            var index = SegmentedLoaiHinh.SelectedIndex;
            viewModel.PostModel.PostType = (short)index;
            await viewModel.LoadLoaiBatDongSan(index);

            // kiem tra loai bat dong san hien tai co dung ko. ko co thi set null.
            if (viewModel.PostModel.LoaiBatDongSan != null)
            {
                if (!viewModel.LoaiBatDongSanList.Any(x => x.Id == viewModel.PostModel.LoaiBatDongSan.Id))
                {
                    viewModel.PostModel.LoaiBatDongSan = null;
                    viewModel.PostModel.LoaiBatDongSanId = null;
                }
            }
        }

        // loai bat dong san change
        public void LoaiBatDongSan_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.PostModel.LoaiBatDongSan == null) return;
            var id = viewModel.PostModel.LoaiBatDongSan.Id;
            viewModel.PostModel.LoaiBatDongSanId = id;
            switch (id)
            {
                case 0: // Căn hộ
                    {
                        viewModel.ShowTang = true;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = true;
                        viewModel.ShowBedroom = true;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 1: //Căn hộ dịch vụ
                    {
                        viewModel.ShowTang = true;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = true;
                        viewModel.ShowBedroom = true;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 2: //Nhà riêng
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = true;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = true;
                        viewModel.ShowBedroom = true;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 3: //Nhà mặt phố, shophouse
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = true;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = true;
                        viewModel.ShowBedroom = true;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 4: //Nhà biệt thự, liền kề
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = true;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = true;
                        viewModel.ShowBedroom = true;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 5: //Nhà trọ, phòng trọ
                    {
                        viewModel.ShowTang = true;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = false;
                        viewModel.ShowBedroom = false;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 6: //Văn phòng
                    {
                        viewModel.ShowTang = true;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = false;
                        viewModel.ShowBedroom = false;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 7: //Cửa hàng, mặt bằng bán lẻ
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = true;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = true;
                        viewModel.ShowBedroom = true;
                        viewModel.Showutilities = true;
                        break;
                    }
                case 8: //Đất, nhà xưởng, kho bãi
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = true;
                        viewModel.ShowBathroom = false;
                        viewModel.ShowBedroom = false;
                        viewModel.Showutilities = false;
                        break;
                    }
                case 9: //Đất
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = true;
                        viewModel.ShowBathroom = false;
                        viewModel.ShowBedroom = false;
                        viewModel.Showutilities = false;
                        break;
                    }
                case 10: //Bất động sản khác
                    {
                        viewModel.ShowTang = false;
                        viewModel.ShowSoTang = false;
                        viewModel.ShowChieuSau = false;
                        viewModel.ShowBathroom = false;
                        viewModel.ShowBedroom = false;
                        viewModel.Showutilities = true;
                        break;
                    }

                default:
                    break;
            }
        }

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.PostModel.Province != null)
            {
                viewModel.PostModel.ProvinceId = viewModel.PostModel.Province.Id;
            }
            else
            {
                viewModel.PostModel.ProvinceId = null;
            }
            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.PostModel.District = null;
            viewModel.PostModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            if (viewModel.PostModel.District != null)
            {
                viewModel.PostModel.DistrictId = viewModel.PostModel.District.Id;
            }
            else
            {
                viewModel.PostModel.DistrictId = null;
            }

            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.PostModel.Ward = null;

            loadingPopup.IsVisible = false;
        }

        public async void Next_Clicked(object sender, EventArgs e)
        {
            if (viewModel.CurrentStep == 1)
            {
                if (viewModel.PostModel.LoaiBatDongSan == null)
                {
                    await DisplayAlert("", Language.vui_long_chon_loai_bat_dong_san, Language.dong);
                    return;
                }
                if (viewModel.PostModel.Province == null)
                {
                    await DisplayAlert("", Language.vui_long_chon_tinh_thanh, Language.dong);
                    return;
                }
                if (viewModel.PostModel.District == null)
                {
                    await DisplayAlert("", Language.vui_long_chon_quan_huyen, Language.dong);
                    return;
                }
                if (viewModel.PostModel.Ward == null)
                {
                    await DisplayAlert("", Language.vui_long_chon_phuong_xa, Language.dong);
                    return;
                }

                Step1.IsVisible = false;
                Step2.IsVisible = true;
                BackBtn.IsVisible = true;

                viewModel.CurrentStep = 2;

                // can mua/can thue.
                if (viewModel.PostModel.PostType == 2 || viewModel.PostModel.PostType == 3)
                {
                    StackImages.IsVisible = false;
                }
            }
            else if (viewModel.CurrentStep == 2)
            {
                viewModel.PostModel.AreaFrom = EntryAreaFrom.Price;
                viewModel.PostModel.AreaTo = EntryAreaTo.Price;

                if (viewModel.PostModel.AreaFrom == null)
                {
                    await DisplayAlert("", Language.vui_long_nhap_dien_tich, Language.dong);
                    return;
                }
                if (viewModel.PostModel.IsAreaRange && viewModel.PostModel.AreaTo == null)
                {
                    await DisplayAlert("", Language.vui_long_nhap_dien_tich, Language.dong);
                    return;
                }

                if (viewModel.PostModel.IsNegotiate == false)
                {
                    //viewModel.PostModel.PriceFrom = priceFrom;
                    if (viewModel.PostModel.PriceFrom == null)
                    {
                        await DisplayAlert("", Language.vui_long_nhap_gia, Language.dong);
                        return;
                    }
                    if (viewModel.PostModel.IsPriceRange && viewModel.PostModel.PriceTo == null)
                    {
                        await DisplayAlert("", Language.vui_long_nhap_gia, Language.dong);
                        return;
                    }
                }

                Step2.IsVisible = false;
                Step3.IsVisible = true;

                viewModel.CurrentStep = 3;
            }
            else if (viewModel.CurrentStep == 3)
            {
                if (string.IsNullOrWhiteSpace(viewModel.PostModel.Title))
                {
                    await DisplayAlert("", Language.vui_long_nhap_tieu_de, Language.dong);
                    return;
                }
                if (string.IsNullOrWhiteSpace(viewModel.PostModel.Description))
                {
                    await DisplayAlert("", Language.vui_long_nhap_mo_ta, Language.dong);
                    return;
                }

                if (viewModel.PostModel.PostType == 0 || viewModel.PostModel.PostType == 1)
                {
                    if (viewModel.Media.Count < 3)
                    {
                        await DisplayAlert("", Language.ban_phai_cung_cap_toi_thieu_3_hinh_anh_ve_bat_dong_san, Language.dong);
                        return;
                    }
                }
                else
                {
                    viewModel.Media.Clear();
                }

                Step3.IsVisible = false;
                Step4.IsVisible = true;

                viewModel.CurrentStep = 4;
            }
            else if (viewModel.CurrentStep == 4)
            {
                loadingPopup.IsVisible = true;
                Save();
            }
        }

        public void Back_Clicked(object sender, EventArgs e)
        {
            if (viewModel.CurrentStep == 2)
            {
                Step2.IsVisible = false;
                Step1.IsVisible = true;
                BackBtn.IsVisible = false;

                viewModel.CurrentStep = 1;
            }
            else if (viewModel.CurrentStep == 3)
            {
                Step3.IsVisible = false;
                Step2.IsVisible = true;

                viewModel.CurrentStep = 2;
            }
            else if (viewModel.CurrentStep == 4)
            {
                Step4.IsVisible = false;
                Step3.IsVisible = true;

                viewModel.CurrentStep = 3;
            }
        }

        public async void Save()
        {
            try
            {
                if (viewModel.PostModel.TinhTrangPhapLy != null)
                    viewModel.PostModel.TinhTrangPhapLyId = viewModel.PostModel.TinhTrangPhapLy.Id;
                else viewModel.PostModel.TinhTrangPhapLyId = null;

                viewModel.PostModel.WardId = viewModel.PostModel.Ward.Id;

                if (viewModel.PostModel.Project != null)
                    viewModel.PostModel.ProjectId = viewModel.PostModel.Project.Id;
                else viewModel.PostModel.ProjectId = null;

                if (viewModel.ShowBathroom && SegmentSoPhongTam.SelectedIndex != -1)
                    viewModel.PostModel.NumOfBathRoom = viewModel.PhongTamList[SegmentSoPhongTam.SelectedIndex];
                else viewModel.PostModel.NumOfBathRoom = null;

                if (viewModel.ShowBedroom && SegmentSoPhongNgu.SelectedIndex != -1)
                    viewModel.PostModel.NumOfBedRoom = viewModel.PhongNguList[SegmentSoPhongNgu.SelectedIndex];
                else viewModel.PostModel.NumOfBedRoom = null;

                if (viewModel.PostModel.Huong != null)
                    viewModel.PostModel.HuongId = viewModel.PostModel.Huong.Id;
                else viewModel.PostModel.HuongId = null;

                if (viewModel.SelectedUtitlitesId != null && viewModel.SelectedUtitlitesId.Any())
                {
                    viewModel.PostModel.Utilities = string.Join(",", viewModel.SelectedUtitlitesId.ToArray());
                }
                else
                {
                    viewModel.PostModel.Utilities = null;
                }

                viewModel.PostModel.ChieuSau = EntryChieuSau.Price;
                viewModel.PostModel.DuongVao = EntryDuongVao.Price;
                viewModel.PostModel.MatTien = EntryMatTien.Price;

                if (SwitchCamket.IsToggled)
                {
                    if (viewModel.PostModel.CommitmentDateFrom == null || viewModel.PostModel.CommitmentDateTo == null)
                    {
                        await DisplayAlert("", Language.vui_long_chon_ngay_cam_ket, Language.dong);
                        loadingPopup.IsVisible = false;
                        return;
                    }
                    if (viewModel.PostModel.CommitmentDateFrom > viewModel.PostModel.CommitmentDateTo)
                    {
                        await Shell.Current.DisplayAlert("", Language.ngay_cam_ket_khong_dung_vui_long_thu_lai, Language.dong);
                        loadingPopup.IsVisible = false;
                        return;
                    }
                }


                try
                {
                    // lay dia chi
                    IEnumerable<Position> result = await new Geocoder().GetPositionsForAddressAsync(viewModel.PostModel.Address);
                    if (result.Any())
                    {
                        viewModel.PostModel.Lat = result.First().Latitude;
                        viewModel.PostModel.Long = result.First().Longitude;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("", Language.dia_chi_khong_tim_thay_tren_ban_do, Language.dong);
                }

                MultipartFormDataContent form = new MultipartFormDataContent();
                // can ban/cho thue
                if (viewModel.PostModel.PostType == 0 || viewModel.PostModel.PostType == 1)
                {
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
                            imageList[i] = item.PreviewPath.Replace(ApiConfig.CloudStorageApiCDN + "/post/", "");
                        }
                    }


                    //if (imageList.Length > 8)
                    //{
                    //    await DisplayAlert("", "Vui lòng upload tối đa 8 hình ảnh bất động sản.", Language.dong);
                    //    loadingPopup.IsVisible = false;
                    //    return;
                    //}

                    viewModel.PostModel.Avatar = imageList[0];
                    viewModel.PostModel.Images = string.Join(",", imageList);
                }
                else
                {
                    viewModel.PostModel.Avatar = null;
                    viewModel.PostModel.Images = null;
                }


                // kiem tra xem co vuot qua hinh khong.


                // bat dau up hinh.

                bool ImageUploaded = true;
                // can ban/cho thue. co thay doi hinh anh va can ban/cho thue moi cho up hinh.
                if (viewModel.Media.Any(x => x.Path != null) && viewModel.PostModel.PostType == 0 || viewModel.PostModel.PostType == 1)
                {
                    ApiResponse uploadImageResponse = await UploadImage(form);
                    if (!uploadImageResponse.IsSuccess)
                    {
                        await DisplayAlert("", Language.hinh_anh_vuot_qua_dung_luong, Language.dong);
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
                        viewModel.PostModel.Id = Guid.NewGuid();
                        apiResponse = await ApiHelper.Post(ApiRouter.POST_CREATE, viewModel.PostModel, true);
                    }
                    else
                    {
                        apiResponse = await ApiHelper.Post(ApiRouter.POST_UPDATE, viewModel.PostModel, true);
                    }


                    if (apiResponse.IsSuccess)
                    {
                        if (_id != Guid.Empty)
                        {
                            Page prevPage = Shell.Current.Navigation.NavigationStack[Shell.Current.Navigation.NavigationStack.Count - 2];

                            if (prevPage.GetType() == typeof(PostDetailPage))
                            {
                                var detailPage = (PostDetailPage)prevPage;

                                await Shell.Current.Navigation.PopAsync(false);
                                await Shell.Current.Navigation.PopAsync(false);
                                await Shell.Current.Navigation.PushAsync(new PostDetailPage(viewModel.PostModel.Id), false);
                            }
                        }

                        MessagingCenter.Send<PostPage>(this, "OnSavePost");
                        if (_id == Guid.Empty)
                        {
                            await Shell.Current.Navigation.PopAsync(false);
                            await Shell.Current.Navigation.PushAsync(new PostDetailPage(viewModel.PostModel.Id), false);
                            await CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = $"{ApiConfig.WEB_IP}post/{viewModel.PostModel.Id}" });
                        }
                        ToastMessageHelper.ShortMessage(Language.luu_thanh_cong);
                    }
                    else
                    {
                        loadingPopup.IsVisible = false;
                        await DisplayAlert("", Language.khong_the_luu_bai_dang, Language.dong);
                    }
                }
            }
            catch (Exception ex)
            {
                string mes = ex.Message;
            }

        }

        public async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=post", form);

            string body = await uploadResponse.Content.ReadAsStringAsync();
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(body);
            return uploadResonse;
        }

        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }

        public void ClearDescription_Clicked(object sender, EventArgs e)
        {
            viewModel.PostModel.Description = null;
        }

        public async void PriceFromPick_Clicked(object sender, EventArgs e)
        {
            SelectPricePage selectPricePage = new SelectPricePage(viewModel.PostModel.PriceFrom, viewModel.PostModel.PriceFromUnit);
            selectPricePage.SetSaveEvent(async (object sSender, EventArgs sE) =>
            {
                viewModel.PostModel.PriceFromUnit = (short)selectPricePage.SelectedOption.Id;
                viewModel.PostModel.PriceFromText = DecimalHelper.DecimalToText(selectPricePage.Price) + " " + selectPricePage.SelectedOption.Name.ToLower();
                viewModel.PostModel.PriceFrom = DecimalHelper.TextToDecimal(selectPricePage.Text);
                viewModel.PostModel.PriceFromQuyDoi = selectPricePage.Price * selectPricePage.SelectedOption.QuyDoi;
                await Navigation.PopAsync();
            });
            await Navigation.PushAsync(selectPricePage);
        }
        public async void PriceToPick_Clicked(object sender, EventArgs e)
        {
            SelectPricePage selectPricePage = new SelectPricePage(viewModel.PostModel.PriceTo, viewModel.PostModel.PriceToUnit);
            selectPricePage.SetSaveEvent(async (object sSender, EventArgs sE) =>
            {
                viewModel.PostModel.PriceToUnit = (short)selectPricePage.SelectedOption.Id;
                viewModel.PostModel.PriceToText = DecimalHelper.DecimalToText(selectPricePage.Price) + " " + selectPricePage.SelectedOption.Name.ToLower();
                viewModel.PostModel.PriceTo = selectPricePage.Price;
                viewModel.PostModel.PriceToQuyDoi = selectPricePage.Price * selectPricePage.SelectedOption.QuyDoi;
                await Navigation.PopAsync();
            });
            await Navigation.PushAsync(selectPricePage);
        }

        private void SwichCamKet_Togged(object sender, EventArgs e)
        {
            if (!SwitchCamket.IsToggled) // khong cam ket
            {
                viewModel.PostModel.CommitmentDateFrom = viewModel.PostModel.CommitmentDateTo = null;
            }
        }
    }
}
