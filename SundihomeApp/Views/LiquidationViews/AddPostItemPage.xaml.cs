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
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices.ILiquidation;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class AddPostItemPage : ContentPage
    {
        private AddPostItemPageViewModel viewModel;
        private Color SelectedBGColor = Color.DarkGreen;
        private Color SelectedTextColor = Color.White;
        private Color UnSelectedTextColor = Color.FromHex("#444444");
        public AddPostItemPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddPostItemPageViewModel();
            ControlSegment.ItemsSource = new List<string> { Language.can_thanh_ly, Language.can_mua };
            ControlSegment.SetActive(0);
            Init();
        }

        private async void Init()
        {
            await CrossMedia.Current.Initialize();
            LoadCategories();
            loadingPopup.IsVisible = false;
        }

        public void LoadCategories()
        {
            List<LiquidationCategory> furnitureCategories = DependencyService.Get<ILiquidationCategoryService>().GetLiquidations();
            for (int i = 0; i < furnitureCategories.Count; i++)
            {
                Label lbl = new Label()
                {
                    Text = furnitureCategories[i].Name,
                    FontSize = 15,
                    TextColor = Color.FromHex("#444444")
                };

                var item = new RadBorder();
                item.Padding = new Thickness(10, 5);
                item.BorderColor = Color.Gray;
                item.BorderThickness = 1;
                item.BorderColor = Color.Gray;
                item.CornerRadius = 15;
                item.BackgroundColor = Color.White;

                var tap = new TapGestureRecognizer();
                tap.NumberOfTapsRequired = 1;
                tap.CommandParameter = furnitureCategories[i];
                tap.Tapped += async (o, e) =>
                {
                    foreach (RadBorder radBorder in Flexlayout_ParentCategory.Children.Where(x => x != item))
                    {
                        radBorder.BackgroundColor = Color.White;
                        (radBorder.Content as Label).TextColor = UnSelectedTextColor;
                    }

                    if (item.BackgroundColor == Color.White) // dang inactive
                    {
                        item.BackgroundColor = SelectedBGColor;
                        lbl.TextColor = SelectedTextColor;

                        //n eu co active thi load child theo parrent.

                        loadingPopup.IsVisible = true;
                        var selectedCateogry = ((o as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as LiquidationCategory;


                        viewModel.Category = null;
                        viewModel.Category = selectedCateogry;
                        loadingPopup.IsVisible = false;
                    }
                    else // dang active thi set unactive
                    {
                        // reset lai.
                        viewModel.Category = null;

                        item.BackgroundColor = Color.White;
                        lbl.TextColor = UnSelectedTextColor;
                    }
                };
                item.GestureRecognizers.Add(tap);
                item.Content = lbl;

                Flexlayout_ParentCategory.Children.Add(item);
            }
        }


        public void Remove_Image(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.Media.Remove(file);
        }

        private async Task<ApiResponse> UploadImage(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=liquidation/post", form);
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
            return uploadResonse;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EntryTitle.Text))
            {
                await DisplayAlert("", Language.vui_long_nhap_tieu_de_bai_dang, Language.dong);
                return;
            }
            if (string.IsNullOrEmpty(editor.Text))
            {
                await DisplayAlert("", Language.vui_long_nhap_mo_ta_bai_dang, Language.dong);
                return;
            }

            loadingPopup.IsVisible = true;
            MultipartFormDataContent form = new MultipartFormDataContent();
            string[] imageList = null;

            // kiem tra co hinh thi upload.
            if (viewModel.Media.Count > 0)
            {
                if (viewModel.Media.Count > 9)
                {
                    loadingPopup.IsVisible = false;
                    await DisplayAlert("", Language.vui_long_chon_toi_da_9_anh, Language.dong);
                    return;
                }

                imageList = new string[viewModel.Media.Count];
                for (int i = 0; i < viewModel.Media.Count; i++)
                {
                    var media = viewModel.Media[i];
                    // chua upload. upload roi link = null 
                    if (string.IsNullOrEmpty(media.Path) == false) // co link la co chon tu dien thoai.
                    {
                        imageList[i] = $"{Guid.NewGuid().ToString()}.jpg";
                        var stream = new System.IO.MemoryStream(File.ReadAllBytes(media.Path));
                        var content = new StreamContent(stream);
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "files" + i,
                            FileName = imageList[i]
                        };
                        form.Add(content);
                    }
                }

                var uploadResponse = await UploadImage(form);
                if (uploadResponse.IsSuccess == false)
                {
                    loadingPopup.IsVisible = false;
                    await DisplayAlert("", Language.loi_upload_hinh_anh_vui_long_thu_lai, Language.dong);
                    return;
                }
            }

            LiquidationPostItem item = new LiquidationPostItem();
            item.Title = EntryTitle.Text;
            item.Type = ControlSegment.GetCurrentIndex();
            item.Description = editor.Text;
            item.Images = imageList;
            item.CreatedById = UserLogged.Id;
            item.CreatedBy = new PostItemUser()
            {
                UserId = UserLogged.Id,
                FullName = UserLogged.FullName,
                Avatar = UserLogged.AvatarUrl.Replace(ApiConfig.IP2, "")
            };
            item.CreatedDate = DateTime.Now;
            item.HasImage = imageList != null && imageList.Length > 0;

            if (EntryPrice.Text.HasValue)
            {
                item.HasPrice = true;
                item.Price = DecimalHelper.ToCurrency(EntryPrice.Text.Value) + " đ";
            }

            if (!string.IsNullOrWhiteSpace(EntryAddress.Text))
            {
                item.HasAddress = true;
                item.Address = EntryAddress.Text;
            }

            if (viewModel.Category != null)
            {
                item.HasCategory = true;
                item.CategoryName = viewModel.Category.Name;
            }

            ILiquidationPostItemService postItemService = DependencyService.Get<ILiquidationPostItemService>();
            postItemService.AddPostItem(item);
            MessagingCenter.Send<AddPostItemPage, LiquidationPostItem>(this, "AddPostItemSuccess", item);
            loadingPopup.IsVisible = false;
            await Navigation.PopAsync();
        }

        private void CBMyAddress_Checked(object sender, EventArgs e)
        {
            if (CheckBoxMyAddress.IsChecked.HasValue && CheckBoxMyAddress.IsChecked.Value)
            {
                CheckBoxMyAddress.IsChecked = false;
            }
            else
            {
                CheckBoxMyAddress.IsChecked = true;
                this.EntryAddress.Text = UserLogged.Address;
            }
        }
    }
}
