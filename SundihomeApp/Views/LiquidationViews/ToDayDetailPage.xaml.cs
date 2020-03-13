using System;
using System.Collections.Generic;
using System.Linq;
using Stormlion.PhotoBrowser;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices.ILiquidation;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class ToDayDetailPage : ContentPage
    {
        private Guid _id;
        public ToDayDetailPageViewModel viewModel;
        private PhotoBrowser photoBrowser = null;
        public Color BorderColorActive = Color.FromHex("#026294");
        public Color BorderColorInActive = Color.Transparent;

        public ToDayDetailPage(Guid id)
        {
            InitializeComponent();
            _id = id;
            BindingContext = viewModel = new ToDayDetailPageViewModel(_id);
            Init();

            MessagingCenter.Subscribe<AddToDayPage, Guid>(this, "OnSaveItem", (sender, arg) =>
            {
                _id = arg;
                Init();
            });
        }

        public async void Init()
        {
            bool result = await viewModel.GetLiquidationToDay(_id);
            if (result == false)
            {
                await DisplayAlert("", Language.khong_tim_thay_bai_dang, Language.dong);
                await Navigation.PopAsync();
                return;
            }
            if (viewModel.ImageList.Length > 1)
            {
                ImageListScroll.IsVisible = true;
                SetCategoryActiveStyle(ImageList.Children[0] as RadBorder);
            }

            if (viewModel.ButtonCommandList.Count < 2)
            {
                SetFloatingButtonGroup();
            }

            viewModel.CategoryName = DependencyService.Get<ILiquidationCategoryService>().GetById(viewModel.LiquidationToDay.CategoryId).Name;

            loadingPopup.IsVisible = false;
        }

        private void OnClickedShowGallery(object sender, EventArgs e)
        {
            if (photoBrowser == null)
            {
                photoBrowser = new PhotoBrowser();
                photoBrowser.EnableGrid = true;
                List<Photo> Photos = new List<Photo>();

                for (int i = 0; i < viewModel.ImageList.Length; i++)
                {
                    Photo p = new Photo();
                    p.Title = "";
                    p.URL = viewModel.ImageList[i];
                    Photos.Add(p);
                }

                photoBrowser.Photos = Photos;
            }

            string url = ((sender as Image).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;
            int index = viewModel.ImageList.ToList().IndexOf(url);

            photoBrowser.StartIndex = index;
            photoBrowser.Show();
        }

        public void OnImageTapped(object sender, EventArgs e)
        {
            var imageChoosed = sender as RadBorder;

            //set all image style to inactive
            SetCategoryInActiveStyle(ImageList);

            //set active style to image choosed
            SetCategoryActiveStyle(imageChoosed);

            //get image choosed
            var tap = imageChoosed.GestureRecognizers[0] as TapGestureRecognizer;
            var imageSource = tap.CommandParameter as string;


            for (int i = 0; i < viewModel.ImageList.Length; i++)
            {
                if (viewModel.ImageList[i] == imageSource)
                {
                    viewModel.Position = i;
                    break;
                }
            }
        }

        private void SetFloatingButtonGroup()
        {
            bool IsOwner = UserLogged.IsLogged && Guid.Parse(UserLogged.Id) == viewModel.LiquidationToDay.CreatedById; // kiem tra nguoi dang nhap co phai la nguoi tao ra post nay khong.

            if (IsOwner)
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chinh_sua, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProduct));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xoa, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2ed", null, Delete_Clicked));
            }
            else
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.thong_tin_ca_nhan, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf129", null, ViewProfile_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, Call_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Chat", FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, Chat_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessage_Clicked));
                btnOrder.IsVisible = true;
            }
        }

        public async void OnEditProduct(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddToDayPage(_id));
        }

        public async void ViewProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserProfilePage(viewModel.LiquidationToDay.CreatedById));
        }

        public async void Call_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            if (UserLogged.Id == viewModel.LiquidationToDay.CreatedById.ToString())
            {
                return;
            }
            try
            {
                PhoneDialer.Open(viewModel.LiquidationToDay.CreatedBy.Phone);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        public async void Chat_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new ChatPage(viewModel.LiquidationToDay.CreatedById.ToString()));
        }

        public async void SendMessage_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            try
            {
                await Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.LiquidationToDay.CreatedBy.Phone));
            }
            catch
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        public async void Delete_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert(Language.xac_nhan_xoa, Language.ban_co_chac_chan_muon_xoa_tin_dang_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;

            loadingPopup.IsVisible = true;

            ApiResponse response = await ApiHelper.Delete(ApiRouter.LIQUIDATIONTODAY_DELETE + "/" + viewModel.LiquidationToDay.Id);
            if (response.IsSuccess)
            {
                loadingPopup.IsVisible = false;
                await Shell.Current.Navigation.PopAsync();
                MessagingCenter.Send<ToDayDetailPage, Guid>(this, "OnDeleted", viewModel.LiquidationToDay.Id);
                ToastMessageHelper.ShortMessage(Language.xoa_thanh_cong);
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.loi_khong_the_xoa_san_pham);
                loadingPopup.IsVisible = false;
            }
        }

        public async void OnOrder_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserLogged.Id))
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            //goto order page
            await DisplayAlert("", Language.chuc_nang_dang_hoan_thien_vui_long_quay_lai_sau, Language.dong);
        }

        public async void OnReUpload_Clicked(object sender, EventArgs e)
        {
            //reupload liquidation
            await DisplayAlert("", Language.dang_lai_tin, Language.dong);
        }

        public void SetCategoryActiveStyle(RadBorder item)
        {
            //set style
            item.BorderColor = BorderColorActive;

            //set scroll position
            var itemX = item.X;
            var itemWidth = item.Width;
            var center = Application.Current.MainPage.Width / 2;
            double x = 0;
            if (itemX > center)
            {
                x = itemX - center + itemWidth / 2 + 10;
            }
            ImageListScroll.ScrollToAsync(x, ImageListScroll.ScrollY, true);

        }

        public void SetCategoryInActiveStyle(Layout layout)
        {
            foreach (RadBorder item in layout.Children)
            {
                item.BorderColor = BorderColorInActive;
            }
        }
    }
}
