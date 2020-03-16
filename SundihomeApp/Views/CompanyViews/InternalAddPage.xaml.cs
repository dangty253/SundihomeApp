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
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews
{
    public partial class InternalAddPage : ContentPage
    {
        private IInternalPostItemService postItemService = DependencyService.Get<IInternalPostItemService>();
        public InternalAddPageViewModel viewModel;
        private ListViewPageViewModel2<SundihomeApi.Entities.Post> searchPageResultViewModel;
        public List<string> ImageList = new List<string>();
        public InternalAddPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new InternalAddPageViewModel();
            Init();
        }

        private async void Init()
        {
            await CrossMedia.Current.Initialize();
            loadingPopup.IsVisible = false;
        }

        private void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }

        private async void PickerPost_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;

            if (searchPageResultViewModel == null) // chua bat popup lan nao.
            {
                this.ListView0.ItemTapped += ListView0_ItemTapped; ;
                searchPageResultViewModel = new ListViewPageViewModel2<SundihomeApi.Entities.Post>();
                searchPageResultViewModel.PreLoadData = new Command(() =>
                {
                    searchPageResultViewModel.ApiUrl = ApiRouter.COMPANY_GETPOSTLIST + $"?CompanyId={UserLogged.CompanyId}&Keyword={viewModel.PostKeyword}&page={searchPageResultViewModel.Page}&Status=2";
                });
                this.ListView0.BindingContext = searchPageResultViewModel;
            }
            else
            {
                viewModel.PostKeyword = null;
                ModalPopupSearchBar.Text = null;
            }
            await searchPageResultViewModel.LoadOnRefreshCommandAsync();
            loadingPopup.IsVisible = false;
            await BottomModal.Show();
        }

        private async void ListView0_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as SundihomeApi.Entities.Post;
            var post = new SundihomeApi.Entities.Mongodb.Post()
            {
                PostId = item.Id.ToString(),
                Title = item.Title,
                PriceText = item.PriceFormatText,
                Avatar = item.Avatar,
                Address = item.Address
            };
            viewModel.InternalPost = post;
            await BottomModal.Hide();
        }

        private void RemovePost_Clicked(object sender, EventArgs e)
        {
            viewModel.InternalPost = null;
        }

        public void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // huy search va dieu kien saerch hien tai khac "" hoac emtpy thi moi chay lai. 
            if (string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(viewModel.PostKeyword))
            {
                Search_Clicked(sender, EventArgs.Empty);
            }
        }

        private void Search_Clicked(object sender, EventArgs e)
        {
            viewModel.PostKeyword = ModalPopupSearchBar.Text ?? "";
            searchPageResultViewModel.RefreshCommand.Execute(null);
        }


        private async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=internalpostitem", form);
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
            return uploadResonse;
        }

        public async void Province_Change(object sender, LookUpChangeEvent e)
        {
            loadingPopup.IsVisible = true;
            await viewModel.GetDistrictAsync();
            viewModel.WardList.Clear();
            viewModel.District = null;
            viewModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        public async void District_Change(object sender, LookUpChangeEvent e)
        {
            loadingPopup.IsVisible = true;
            await viewModel.GetWardAsync();
            viewModel.Ward = null;
            loadingPopup.IsVisible = false;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            try
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

                InternalPostItem item = new InternalPostItem();
                item.CompanyId = UserLogged.CompanyId.ToLower();
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

                if (viewModel.InternalPost != null)
                {
                    item.HasPost = true;
                    item.Post = viewModel.InternalPost;
                }

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

                item.Address = viewModel.Address;

                postItemService.AddPostItem(item);

                MessagingCenter.Send<InternalAddPage, InternalPostItem>(this, "AddPostItemSuccess", item);
                loadingPopup.IsVisible = false;
                await Navigation.PopAsync();
                ToastMessageHelper.ShortMessage(Language.dang_thanh_cong);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
