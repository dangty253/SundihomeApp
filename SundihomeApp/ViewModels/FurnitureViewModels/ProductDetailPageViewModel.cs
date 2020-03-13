using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Models.Furniture;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class ProductDetailPageViewModel : BaseViewModel
    {
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } = new ObservableCollection<FloatButtonItem>();

        private FurnitureProduct _furnitureProduct;
        public FurnitureProduct FurnitureProduct
        {
            get => _furnitureProduct;
            set
            {
                _furnitureProduct = value;
                OnPropertyChanged(nameof(FurnitureProduct));
            }
        }

         public void fireonchange()
        {
            OnPropertyChanged(nameof(FurnitureProduct));
        }

        private Guid _furnitureProductId;

        private string _statusString;
        public string StatusString
        {
            get => _statusString;
            set
            {
                _statusString = value;
                OnPropertyChanged(nameof(StatusString));
            }
        }

        private int _position;
        public int Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public Command OnShareDataCommand { get; set; }

        public ProductDetailPageViewModel(Guid furnitureProductId)
        {
            _furnitureProductId = furnitureProductId;
            Position = 0;
            string url = ApiConfig.WEB_IP + $"product/{_furnitureProductId}";
            OnShareDataCommand = new Command(() => Share(url));
            ButtonCommandList.Add(new FloatButtonItem(Language.chia_se, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf1e0", OnShareDataCommand, null));
        }

        public async Task<bool> GetProduct()
        {
            var response = await ApiHelper.Get<FurnitureProduct>($"{ApiRouter.FURNITUREPRODUCT_GET_BY_ID}{_furnitureProductId}");
            if (response.IsSuccess)
            {
                var product = response.Content as FurnitureProduct;
                product.ParentCategory.Name = Language.ResourceManager.GetString(product.ParentCategory.LanguageKey, Language.Culture);
                product.Category.Name = Language.ResourceManager.GetString(product.Category.LanguageKey, Language.Culture);
                FurnitureProduct = product;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GetDetail()
        {
            if (FurnitureProduct.Status.HasValue)
            {
                if (FurnitureProduct.Status.Value)
                    StatusString = Language.moi;
                else
                    StatusString = Language.da_qua_su_dung;
            }
        }

        public async void DeleteProduct(Guid productId)
        {
            var answer = await Shell.Current.DisplayAlert(Language.xac_nhan_xoa, Language.ban_co_chac_chan_muon_xoa_san_pham_nay_khong, Language.xoa, Language.huy);
            if (!answer) return;

            var response = await ApiHelper.Delete($"{ApiRouter.FURNITUREPRODUCT_REMOVE}/{productId}");
            if (response.IsSuccess)
            {
                await Shell.Current.Navigation.PopAsync();
                MessagingCenter.Send<ProductDetailPageViewModel, Guid>(this, "DeleteProduct", productId);
                ToastMessageHelper.ShortMessage(Language.xoa_thanh_cong);
            }
            else
            {
                ToastMessageHelper.ShortMessage(Language.xoa_that_bai);
            }
        }

        public void Share(string url)
        {
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
