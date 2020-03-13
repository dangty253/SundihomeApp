using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Stormlion.PhotoBrowser;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class ProductDetailPage : ContentPage
    {
        private Guid _furnitureProductId;
        private bool _isMyProduct;
        public ProductDetailPageViewModel viewModel;

        public Color BorderColorActive = Color.FromHex("#026294");
        public Color BorderColorInActive = Color.Transparent;

        private PhotoBrowser photoBrowser = null;
        public ProductDetailPage(Guid furnitureProductId)
        {
            InitializeComponent();
            _furnitureProductId = furnitureProductId;
            BindingContext = viewModel = new ProductDetailPageViewModel(_furnitureProductId);
            MessagingCenter.Subscribe<AddProductPage, FurnitureProduct>(this, "UpdateProduct", async (sender, product) =>
            {
                viewModel.FurnitureProduct = product;
                Init();
            });
            Init();
        }


        private void SetFloatingButtonGroup()
        {
            bool IsOwner = UserLogged.IsLogged && Guid.Parse(UserLogged.Id) == viewModel.FurnitureProduct.CreatedById; // kiem tra nguoi dang nhap co phai la nguoi tao ra post nay khong.
            if (IsOwner)
            {
                viewModel.ButtonCommandList.Insert(0, viewModel.FurnitureProduct.ProductStatus == 0 ? new FloatButtonItem(Language.ngung_ban, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProductStatus) : new FloatButtonItem(Language.dang_ban_status, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProductStatus));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.chinh_sua, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProduct));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.xoa, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2ed", null, OnDeleteProduct));
            }
            else
            {
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.thong_tin_ca_nhan, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf129", null, ViewProfile_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.goi_dien, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf2a0", null, Call_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem("Chat", FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, Chat_Clicked));
                viewModel.ButtonCommandList.Add(new FloatButtonItem(Language.nhan_tin, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf4ad", null, SendMessage_Clicked));
            }
        }

        private async void OnEditProductStatus(object sender, EventArgs e)
        {
            if (UserLogged.Id == viewModel.FurnitureProduct.CreatedById.ToString())
            {

                var response = await ApiHelper.Put($"{ApiRouter.FURNITUREPRODUCT_UPDATE_PRODUCT_STATUS}/{viewModel.FurnitureProduct.Id}?status={(viewModel.FurnitureProduct.ProductStatus == 0 ? 1 : 0)}", viewModel.FurnitureProduct, true);
                if (response.IsSuccess)
                {
                    viewModel.FurnitureProduct.ProductStatus = viewModel.FurnitureProduct.ProductStatus == 0 ? 1 : 0;
                    viewModel.ButtonCommandList.RemoveAt(0);
                    viewModel.ButtonCommandList.Insert(0, viewModel.FurnitureProduct.ProductStatus == 0 ? new FloatButtonItem(Language.ngung_ban, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProductStatus) : new FloatButtonItem(Language.dang_ban_status, FontAwesomeHelper.GetFont("FontAwesomeRegular"), "\uf044", null, OnEditProductStatus));
                    viewModel.fireonchange();
                    //MessagingCenter.Send<ProductDetailPageViewModel, Guid>(this, "DeleteProduct", productId);
                    MessagingCenter.Send<ProductDetailPage, Guid>(this, "UpdateProductStatus", viewModel.FurnitureProduct.Id);
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_thanh_cong);
                }
                else
                {
                    ToastMessageHelper.ShortMessage(Language.cap_nhat_that_bai);
                }
            }
        }

        public async void OnEditProduct(object sender, EventArgs e)
        {
            if (UserLogged.Id == viewModel.FurnitureProduct.CreatedById.ToString())
            {
                await Navigation.PushAsync(new AddProductPage(viewModel.FurnitureProduct.Id, true) { Title = Language.cap_nhat_san_pham });
            }
        }

        public async void OnDeleteProduct(object sender, EventArgs e)
        {
            if (UserLogged.Id == viewModel.FurnitureProduct.CreatedById.ToString())
            {
                viewModel.DeleteProduct(viewModel.FurnitureProduct.Id);
            }
        }

        public async void Init()
        {
            // kiem tra co lay duoc san pham khong. 
            bool result = await viewModel.GetProduct();
            if (result == false)
            {
                await DisplayAlert("", Language.khong_tim_thay_san_pham, Language.dong);
                await Navigation.PopAsync();
                return;
            }

            viewModel.GetDetail();
            Title = viewModel.FurnitureProduct.Name;

            if (viewModel.FurnitureProduct.IsPromotion == true)
            {
                LabelPriceStrike.IsVisible = true;
                LabelPromotionPrice.IsVisible = true;
                LabelPrice.IsVisible = false;
            }
            else
            {
                LabelPriceStrike.IsVisible = false;
                LabelPromotionPrice.IsVisible = false;
                LabelPrice.IsVisible = true;
            }

            ImageListStackLayout.IsVisible = true;
            if (viewModel.FurnitureProduct.ImageList.Length > 1)
            {
                ImageListScroll.IsVisible = true;
                SetCategoryActiveStyle(ImageList.Children[0] as RadBorder);
            }

            if (viewModel.ButtonCommandList.Count < 2)
            {
                SetFloatingButtonGroup();
            }

            loadingPopup.IsVisible = false;
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


            for (int i = 0; i < viewModel.FurnitureProduct.ImageList.Length; i++)
            {
                if (viewModel.FurnitureProduct.ImageList[i] == imageSource)
                {
                    viewModel.Position = i;
                    //ImageListCarouselView.SelectedIndex = i;
                    break;
                }
            }
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
        private void OnClickedShowGallery(object sender, EventArgs e)
        {
            if (photoBrowser == null)
            {
                photoBrowser = new PhotoBrowser();
                photoBrowser.EnableGrid = true;
                List<Photo> Photos = new List<Photo>();

                for (int i = 0; i < viewModel.FurnitureProduct.ImageList.Length; i++)
                {
                    Photo p = new Photo();
                    p.Title = "";
                    p.URL = viewModel.FurnitureProduct.ImageList[i];
                    Photos.Add(p);
                }

                photoBrowser.Photos = Photos;
            }

            string url = ((sender as Image).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;
            int index = viewModel.FurnitureProduct.ImageList.ToList().IndexOf(url);

            photoBrowser.StartIndex = index;
            photoBrowser.Show();
        }
        //public void OnImageListCarouselViewPositionSelected(object sender, PositionSelectedEventArgs e)
        //{
        //    if (viewModel.FurnitureProduct.ImageList != null)
        //    {
        //        SetCategoryInActiveStyle(ImageList);
        //        var imageChoosed = ImageList.Children[e.NewValue] as RadBorder;
        //        SetCategoryActiveStyle(imageChoosed);
        //    }
        //}


        private async void Call_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            if (UserLogged.Id == viewModel.FurnitureProduct.CreatedById.ToString())
            {
                return;
            }
            try
            {
                PhoneDialer.Open(viewModel.FurnitureProduct.CreatedBy.Phone);
            }
            catch (Exception ex)
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            try
            {
                await Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.FurnitureProduct.CreatedBy.Phone));
            }
            catch
            {
                await DisplayAlert(Language.loi, Language.khong_the_thuc_hien_chuc_nang_nay, Language.dong);
            }
        }

        private async void Chat_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new ChatPage(viewModel.FurnitureProduct.CreatedById.ToString()));
        }

        private async void ViewProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserProfilePage(viewModel.FurnitureProduct.CreatedById));
        }

    }
}
