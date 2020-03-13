using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Plugin.Media;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class AddPostItemPage : ContentPage
    {
        private IPostItemService postItemService;
        private bool CheckLastPost = false; // kiem tra khi tu post qua.
        public AddPostItemPageViewModel viewModel;
        public List<LoaiBatDongSanModel> loaiBatDongSans;
        public List<string> ImageList;
        private Color SelectedBGColor = Color.DarkGreen;
        private Color SelectedTextColor = Color.White;
        private Color UnSelectedTextColor = Color.FromHex("#444444");
        private int[] NumberList = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public AddPostItemPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddPostItemPageViewModel();
            pickerUnitPrice.SelectedItem = this.viewModel.PriceOptions.First(); //set mac dinh vnd.
            ControlSegment.ItemsSource = new List<string> { Language.can_ban, Language.cho_thue, Language.can_mua, Language.can_thue };
            ControlSegment.SetActive(0);
            ImageList = new List<string>();
            Init();
        }

        public AddPostItemPage(SundihomeApi.Entities.Post post)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddPostItemPageViewModel();
            Init();

            CheckLastPost = true;
            ControlSegment.ItemsSource = new List<string> { Language.can_ban, Language.cho_thue, Language.can_mua, Language.can_thue };
            ControlSegment.SetActive(post.PostType);
            editor.Text = post.Description;

            EntryTitle.Text = post.Title;

            // loai bat dong san
            LookUpType.SelectedItem = loaiBatDongSans.Single(x => x.Id == post.LoaiBatDongSanId);

            // gia
            if (!post.IsNegotiate) // khong phai la thoa thuan
            {
                EntryPrice.Text = post.PriceFrom;

                var priceOption = this.viewModel.PriceOptions.SingleOrDefault(x => x.Id == post.PriceFromUnit);
                if (priceOption != null)
                {
                    pickerUnitPrice.SelectedItem = priceOption;
                }
                else
                {
                    pickerUnitPrice.SelectedItem = this.viewModel.PriceOptions.First();
                }
            }
            else
            {
                pickerUnitPrice.SelectedItem = this.viewModel.PriceOptions.First();
            }

            // dien tich.
            EntryArea.Text = post.AreaFormatText?.Replace("m2", "") ?? "";

            // phong ngu
            if (post.NumOfBedroom.HasValue)
            {
                SoPhongNgu_Tapped(StacklayoutSophongngu.Children[this.NumberList.ToList().IndexOf((int)post.NumOfBedroom)], EventArgs.Empty);
            }

            // phong ta
            if (post.NumOfBathroom.HasValue)
            {
                SoPhongNgu_Tapped(StacklayoutSoPhongtam.Children[this.NumberList.ToList().IndexOf((int)post.NumOfBathroom)], EventArgs.Empty);
            }

            if (!string.IsNullOrEmpty(post.Images))
            {
                ImageList = post.ImageList.ToList();

                foreach (var image in post.ImageList)
                {
                    viewModel.Media.Add(new MediaFile()
                    {
                        PreviewPath = ImageHelper.GetImageUrl("post", image),
                    });
                }
            }
            else
            {
                ImageList = new List<string>();
            }

            EntryPrice.Text = post.PriceFrom;

            //EntryAddress.Text = post.Address;

        }

        public async void Init()
        {
            await CrossMedia.Current.Initialize();
            LoadLoaiBatDongSan();
            LoadSoPhongNgu();
            LoadSoPhongTam();
            loadingPopup.IsVisible = false;
        }

        public void LoadLoaiBatDongSan()
        {
            loaiBatDongSans = LoaiBatDongSanModel.GetList(null);
            LookUpType.ItemsSource = loaiBatDongSans;
        }

        public void LoadSoPhongNgu()
        {
            foreach (var num in NumberList)
            {
                RadBorder radBorder = new RadBorder()
                {
                    HeightRequest = 38,
                    WidthRequest = 38,
                    BorderColor = Color.Gray,
                    BorderThickness = 1,
                    BackgroundColor = Color.White,
                    CornerRadius = 19,
                    Content = new Label()
                    {
                        TextColor = Color.FromHex("#444444"),
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = num.ToString()
                    }
                };
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.NumberOfTapsRequired = 1;
                tap.Tapped += SoPhongNgu_Tapped;
                radBorder.GestureRecognizers.Add(tap);
                StacklayoutSophongngu.Children.Add(radBorder);
            }
        }

        public void LoadSoPhongTam()
        {
            foreach (var num in NumberList)
            {
                RadBorder radBorder = new RadBorder()
                {
                    HeightRequest = 38,
                    WidthRequest = 38,
                    BorderColor = Color.Gray,
                    BorderThickness = 1,
                    BackgroundColor = Color.White,
                    CornerRadius = 19,
                    Content = new Label()
                    {
                        TextColor = Color.FromHex("#444444"),
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Text = num.ToString()
                    }
                };
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.NumberOfTapsRequired = 1;
                tap.Tapped += SoPhongNgu_Tapped;
                radBorder.GestureRecognizers.Add(tap);
                StacklayoutSoPhongtam.Children.Add(radBorder);
            }
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            // truong hop dang them tu bên post .
            if (CheckLastPost)
            {
                if (postItemService == null)
                {
                    postItemService = DependencyService.Get<IPostItemService>();
                }

                DateTime? lastTime = postItemService.GatLastPostTime(UserLogged.Id);
                if (lastTime.HasValue)
                {
                    var now = DateTime.Now;
                    TimeSpan timeSpan = now.Subtract(lastTime.Value);
                    if (timeSpan.TotalMinutes >= 120)
                    {

                    }
                    else
                    {
                        //Mỗi bài đăng từ tin đăng phải cách 2 tiếng! Vui lòng thử lại.
                        await DisplayAlert("", Language.moi_bai_dang_phai_tu_it_nhat_x_tieng, Language.dong);
                        return;
                    }
                }

            }
            if (string.IsNullOrWhiteSpace(EntryTitle.Text))
            {
                await DisplayAlert("", Language.vui_long_nhap_tieu_de, Language.dong);
                return;
            }

            if (string.IsNullOrWhiteSpace(editor.Text))
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


            PostItem item = new PostItem();
            item.Title = EntryTitle.Text.Trim();
            item.Type = ControlSegment.GetCurrentIndex();
            item.Description = editor.Text;
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

            // gia
            if (EntryPrice.Text.HasValue)
            {
                item.HasPrice = true;
                item.Price = DecimalHelper.ToCurrency(EntryPrice.Text.Value) + " " + (pickerUnitPrice.SelectedItem != null ? ((PriceOption)pickerUnitPrice.SelectedItem).Name.ToLower() : "");
            }
            // dien tich
            if (!string.IsNullOrWhiteSpace(EntryArea.Text))
            {
                item.HasArea = true;
                item.Area = EntryArea.Text + "m2";
            }

            //if (!string.IsNullOrWhiteSpace(EntryAddress.Text))
            //{
            //    item.HasAddress = true;
            //    item.Address = EntryAddress.Text;
            //}

            if (LookUpType.SelectedItem != null)
            {
                var loaiBatDongSan = LookUpType.SelectedItem as LoaiBatDongSanModel;
                item.HasLoaiBatDongSan = true;
                item.BDSTypeId = loaiBatDongSan.Id;
            }

            if (CheckSelected(StacklayoutSoPhongtam))
            {
                item.HasSoPhongTam = true;
                RadBorder radBorder = GetRadBorder(StacklayoutSoPhongtam);
                var label = radBorder.Content as Label;
                item.SoPhongTam = int.Parse(label.Text);
            }

            if (CheckSelected(StacklayoutSophongngu))
            {
                item.HasSoPhongNgu = true;
                RadBorder radBorder = GetRadBorder(StacklayoutSophongngu);
                var label = radBorder.Content as Label;
                item.SoPhongNgu = int.Parse(label.Text);
            }

            if (postItemService == null)
            {
                postItemService = DependencyService.Get<IPostItemService>();
            }

            postItemService.AddPostItem(item);
            MessagingCenter.Send<AddPostItemPage, PostItem>(this, "AddPostItemSuccess", item);
            loadingPopup.IsVisible = false;
            await Navigation.PopAsync();
            ToastMessageHelper.ShortMessage(Language.luu_thanh_cong);
        }

        private async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=post", form);
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
            return uploadResonse;
        }

        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }

        private void SoPhongNgu_Tapped(object sender, EventArgs e)
        {
            var radBorder = (sender as RadBorder);
            var label = radBorder.Content as Label;

            var wrap = radBorder.Parent as StackLayout;
            foreach (RadBorder item in wrap.Children.Where(x => x != radBorder))
            {
                item.BackgroundColor = Color.White;
                (item.Content as Label).TextColor = UnSelectedTextColor;
            }

            if (label.TextColor == UnSelectedTextColor) // chua chon
            {
                radBorder.BackgroundColor = SelectedBGColor;
                label.TextColor = SelectedTextColor;
            }
            else
            {
                radBorder.BackgroundColor = Color.White;
                label.TextColor = UnSelectedTextColor;
            }
        }

        public bool CheckSelected<T>(T stackLayout) where T : Layout<View>
        {
            return stackLayout.Children.Where(x => x.BackgroundColor == SelectedBGColor).Any();
        }

        public RadBorder GetRadBorder<T>(T layout) where T : Layout<View>
        {
            return (RadBorder)layout.Children.Where(x => x.BackgroundColor == SelectedBGColor).SingleOrDefault();
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
    }
}
