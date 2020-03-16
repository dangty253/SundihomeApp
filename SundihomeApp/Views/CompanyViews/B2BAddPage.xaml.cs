using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class B2BAddPage : ContentPage
    {
        private IB2BPostItemService postItemService = DependencyService.Get<IB2BPostItemService>();
        public B2BAddPageViewModel viewModel;
        public List<string> ImageList;
        public B2BAddPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new B2BAddPageViewModel();
            ControlSegment.ItemsSource = new List<string> { Language.hop_tac, Language.cung_cap };
            ControlSegment.SetActive(0);
            ImageList = new List<string>();
            Init();
        }

        private async void Init()
        {
            await CrossMedia.Current.Initialize();
            await viewModel.GetProvinceAsync();
            loadingPopup.IsVisible = false;
        }

        public void OnProvice_Change(object sender, LookUpChangeEvent e)
        {
            viewModel.District = null;
        }

        public void OnDistrict_Change(object sender, LookUpChangeEvent e)
        {
            viewModel.Ward = null;
        }

        private void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }

        private async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=b2bpostitem", form);
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
            return uploadResonse;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryTitle.Text))
            {
                await DisplayAlert("", Language.vui_long_nhap_tieu_de, Language.dong);
                return;
            }
            else if (string.IsNullOrWhiteSpace(editor.Text))
            {
                await DisplayAlert("", Language.vui_long_nhap_noi_dung, Language.dong);
                return;
            }


            loadingPopup.IsVisible = true;
            // kiem tra co hinh thi upload.
            if (viewModel.Media.Count > 0)
            {
                if (viewModel.Media.Count > 9)
                {
                    loadingPopup.IsVisible = false;
                    await DisplayAlert("", Language.vui_long_chon_toi_da_9_anh, Language.dong);
                    return;
                }

                List<MediaFile> listUploadMedia = viewModel.Media.Where(x => x.Path != null).ToList();
                if (listUploadMedia.Any())
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    for (int i = 0; i < listUploadMedia.Count; i++)
                    {
                        string imageName = $"postitem_{Guid.NewGuid().ToString()}.jpg";
                        ImageList.Add(imageName);

                        var media = listUploadMedia[i];
                        var stream = new MemoryStream(File.ReadAllBytes(media.Path));
                        var content = new StreamContent(stream);
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "files" + i,
                            FileName = imageName
                        };
                        form.Add(content);
                    }

                    var uploadResonse = await UploadImage(form);
                    if (!uploadResonse.IsSuccess)
                    {
                        loadingPopup.IsVisible = false;
                        await DisplayAlert("", uploadResonse.Message, Language.dong);
                        return;
                    }
                }
            }

            #region address

            List<string> addressList = new List<string>();
            if (!string.IsNullOrWhiteSpace(EntryStreet.Text))
            {
                addressList.Add(EntryStreet.Text.Trim());
            }

            if (viewModel.Ward != null)
            {
                addressList.Add(viewModel.Ward.Name);
            }

            if (viewModel.District != null)
            {
                addressList.Add(viewModel.District.Name);
            }

            if (viewModel.Province != null)
            {
                addressList.Add(viewModel.Province.Name);
            }
            string Address = string.Join(", ", addressList.ToArray());
            #endregion



            B2BPostItem item = new B2BPostItem();
            //item.Type = SegmentType.SelectedIndex;
            item.Type = ControlSegment.GetCurrentIndex();
            item.Title = EntryTitle.Text.Trim();
            item.Description = editor.Text.Trim();
            item.Images = ImageList.Any() ? ImageList.ToArray() : null;
            item.CreatedById = UserLogged.Id;
            item.CreatedBy = new PostItemUser()
            {
                UserId = UserLogged.Id,
                FullName = UserLogged.FullName,
                Avatar = UserLogged.AvatarUrl.Replace(ApiConfig.IP2, "")
            };
            item.CreatedDate = DateTime.Now;
            item.HasImage = ImageList != null && ImageList.Count() > 0;

            if (viewModel.Province != null)
            {
                item.ProvinceId = viewModel.Province.Id;
                if (viewModel.District != null)
                {
                    item.DistrictId = viewModel.District.Id;
                    if (viewModel.Ward != null)
                    {
                        item.WardId = viewModel.Ward.Id;
                    }
                }
            }

            item.Address = Address;
            item.HasAddress = !string.IsNullOrWhiteSpace(Address);

            #region price
            if (SwitchIsNegotiate.IsToggled)
            {
                item.HasPrice = true;
                item.PriceText = Language.thoa_thuan;
            }
            else
            {
                List<string> PriceList = new List<string>();
                if (SwitchIsPriceRange.IsToggled)
                {
                    if (EntryPriceFrom.Text.HasValue && EntryPriceFrom.Text.Value > 0)
                    {
                        PriceList.Add(string.Format("{0:0,0 đ}", EntryPriceFrom.Text));
                    }

                    if (EntryPriceTo.Text.HasValue && EntryPriceTo.Text.Value > 0)
                    {
                        PriceList.Add(string.Format("{0:0,0 đ}", EntryPriceTo.Text));
                    }
                }
                else
                {
                    if (EntryPrice.Text.HasValue && EntryPrice.Text.Value > 0)
                    {
                        PriceList.Add(string.Format("{0:0,0 đ}", EntryPrice.Text));
                    }
                }

                if (PriceList.Any())
                {
                    item.HasPrice = true;
                    item.PriceText = string.Join(" - ", PriceList.ToArray());
                }
            }
            #endregion

            postItemService.AddPostItem(item);
            MessagingCenter.Send<B2BAddPage, B2BPostItem>(this, "AddPostItemSuccess", item);
            loadingPopup.IsVisible = false;
            await Navigation.PopAsync();
            ToastMessageHelper.ShortMessage(Language.dang_thanh_cong);
        }
    }
}
