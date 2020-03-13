using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.LiquidationViewModels
{
    public class LiquidationDetailPageViewModel: BaseViewModel
    {
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } = new ObservableCollection<FloatButtonItem>();

        private Liquidation _liquidation;
        public Liquidation Liquidation
        {
            get => _liquidation;
            set
            {
                _liquidation = value;
                OnPropertyChanged(nameof(Liquidation));
            }
        }

        private string[] _imageList;
        public string[] ImageList
        {
            get => _imageList;
            set
            {
                _imageList = value;
                OnPropertyChanged(nameof(ImageList));
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

        private string _categoryName;
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged(nameof(CategoryName));
            }
        }

        public Command OnShareDataCommand { get; set; }

        public LiquidationDetailPageViewModel(Guid id)
        {
            Liquidation = null;
            Position = 0;
            string url = ApiConfig.WEB_IP + $"liquidation/{id}";
            OnShareDataCommand = new Command(() => Share(url));
            ButtonCommandList.Add(new FloatButtonItem(Language.chia_se, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf1e0", OnShareDataCommand, null));
        }

        public async Task<bool> GetLiquidation(Guid liquidationId)
        {
            ApiResponse response = await ApiHelper.Get<Liquidation>($"{ApiRouter.LIQUIDATION_GETBYID}/{liquidationId}");
            if (response.IsSuccess)
            {
                if (response.Content != null)
                {
                    Liquidation = response.Content as Liquidation;
                    ImageList = GetImageList(Liquidation.Images);
                    return true;
                }
            }
            return false;
        }

        public string[] GetImageList(string str)
        {
            string[] newList = null;
            if (!string.IsNullOrEmpty(str))
            {
                char[] spearator = {','}; 
                string[] list = Liquidation.Images.Split(spearator);
                newList = new string[list.Length];
                for (int i = 0; i < list.Length; i++)
                {
                    newList[i] = ImageHelper.GetImageUrl("liquidation", list[i]);
                }
            }
            return newList;
        }

        public void Share(string url)
        {
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
