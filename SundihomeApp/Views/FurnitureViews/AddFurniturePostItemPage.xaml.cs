using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Media;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class AddFurniturePostItemPage : ContentPage
    {
        private AddFurniturePostItemPageViewModel viewModel;
        private Color SelectedBGColor = Color.DarkGreen;
        private Color SelectedTextColor = Color.White;
        private Color UnSelectedTextColor = Color.FromHex("#444444");
        public AddFurniturePostItemPage()
        {
            InitializeComponent();
            this.BindingContext = viewModel = new AddFurniturePostItemPageViewModel();
            ControlSegment.ItemsSource = new List<string> { Language.can_ban, Language.can_mua };
            ControlSegment.SetActive(0);
            Init();
        }

        private async void Init()
        {
            await CrossMedia.Current.Initialize();
            await LoadParentCategory();
            loadingPopup.IsVisible = false;
        }

        public async Task LoadParentCategory()
        {
            List<FurnitureCategory> furnitureCategories = await viewModel.GetParentCategories();
            for (int i = 0; i < furnitureCategories.Count; i++)
            {
                Label lbl = new Label()
                {
                    Text = Language.ResourceManager.GetString(furnitureCategories[i].LanguageKey,Language.Culture),
                    FontSize = 15,
                    TextColor = Color.FromHex("#444444")
                };

                var item = new RadBorder();
                item.Padding = new Thickness(10, 5);
                item.Margin = new Thickness(0, 0, 5, 5);
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
                    Flexlayout_ChildCategory.Children.Clear();
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
                        var selectedCateogry = ((o as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as FurnitureCategory;


                        viewModel.ChildCategory = null;
                        viewModel.ParentCategory = selectedCateogry;

                        await LoadChildCategory();
                        loadingPopup.IsVisible = false;
                    }
                    else // dang active thi set unactive
                    {
                        // reset lai.
                        viewModel.ParentCategory = null;
                        viewModel.ChildCategory = null;

                        item.BackgroundColor = Color.White;
                        lbl.TextColor = UnSelectedTextColor;
                    }
                };
                item.GestureRecognizers.Add(tap);
                item.Content = lbl;

                Flexlayout_ParentCategory.Children.Add(item);
            }
        }


        public async Task LoadChildCategory()
        {
            List<FurnitureCategory> furnitureCategories = await viewModel.GetChildCategories();
            if (furnitureCategories == null)
            {
                await DisplayAlert("", Language.loi_load_danh_muc, Language.dong);
                return;
            }
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
                item.Margin = new Thickness(0, 0, 5, 5);
                item.BorderColor = Color.Gray;
                item.BorderThickness = 1;
                item.BorderColor = Color.Gray;
                item.CornerRadius = 15;
                item.BackgroundColor = Color.White;

                var tap = new TapGestureRecognizer();
                tap.NumberOfTapsRequired = 1;
                tap.CommandParameter = furnitureCategories[i];
                tap.Tapped += (o, e) =>
                {
                    foreach (RadBorder radBorder in Flexlayout_ChildCategory.Children.Where(x => x != item))
                    {
                        radBorder.BackgroundColor = Color.White;
                        (radBorder.Content as Label).TextColor = UnSelectedTextColor;
                    }

                    if (item.BackgroundColor == Color.White)
                    {
                        item.BackgroundColor = SelectedBGColor;
                        lbl.TextColor = SelectedTextColor;


                        //
                        var selectedCateogry = ((o as RadBorder).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as FurnitureCategory;
                        viewModel.ChildCategory = selectedCateogry;
                    }
                    else
                    {
                        item.BackgroundColor = Color.White;
                        lbl.TextColor = UnSelectedTextColor;
                    }
                };
                item.GestureRecognizers.Add(tap);
                item.Content = lbl;

                Flexlayout_ChildCategory.Children.Add(item);
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
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=furniture/post", form);
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(await uploadResponse.Content.ReadAsStringAsync());
            return uploadResonse;
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
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

            FurniturePostItem item = new FurniturePostItem();
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

            if (viewModel.ParentCategory != null)
            {
                item.HasParentCategory = true;
                item.ParentCategoryName = viewModel.ParentCategory.Name;
            }

            if (viewModel.ChildCategory != null)
            {
                item.HasChildCategory = true;
                item.ChildCategoryName = viewModel.ChildCategory.Name;
            }

            IFurniturePostItemService postItemService = DependencyService.Get<IFurniturePostItemService>();
            postItemService.AddPostItem(item);
            MessagingCenter.Send<AddFurniturePostItemPage, FurniturePostItem>(this, "AddPostItemSuccess", item);
            loadingPopup.IsVisible = false;
            await Navigation.PopAsync();
        }
    }
}
