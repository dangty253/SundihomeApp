using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.LiquidationViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public ObservableCollection<SlideItem> SlideList { get; set; }
        public ObservableCollection<Liquidation> Type0List { get; set; }
        public ObservableCollection<LiquidationToDay> LiquidationToDayList { get; set; }

        private int _currentSlideImageIndex;
        public int CurrentSlideImageIndex
        {
            get => _currentSlideImageIndex;
            set
            {
                _currentSlideImageIndex = value;
                OnPropertyChanged(nameof(CurrentSlideImageIndex));
            }
        }

        private int _imageSlideCount;
        public int ImageSlideCount
        {
            get => _imageSlideCount;
            set
            {
                _imageSlideCount = value;
                OnPropertyChanged(nameof(ImageSlideCount));
            }
        }

        private Advertise _advertise;
        public Advertise Advertise
        {
            get => _advertise;
            set
            {
                _advertise = value;
                OnPropertyChanged(nameof(Advertise));
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    this.CurrentSlideImageIndex = 1;
                    await Task.WhenAll(this.LoadLiquidationList(), this.LoadLiquidationToDayList(), this.LoadSlideList(), this.LoadAdvertise());
                    IsRefreshing = false;
                });
            }
        }

        public HomePageViewModel()
        {
            Type0List = new ObservableCollection<Liquidation>();
            LiquidationToDayList = new ObservableCollection<LiquidationToDay>();
            SlideList = new ObservableCollection<SlideItem>();
            CurrentSlideImageIndex = 1;
        }

        public async Task LoadLiquidationList()
        {
            Type0List.Clear();
            ApiResponse response = await ApiHelper.Get<List<Liquidation>>($"{ApiRouter.LIQUIDATION_GETBYTYPE}?limit=5");
            if (response.IsSuccess)
            {
                List<Liquidation> data = response.Content as List<Liquidation>;
                foreach (var item in data)
                {
                    Type0List.Add(item);
                }
            }
        }

        public async Task LoadLiquidationToDayList()
        {
            LiquidationToDayList.Clear();
            ApiResponse response = await ApiHelper.Get<List<LiquidationToDay>>($"{ApiRouter.LIQUIDATIONTODAY_GETTODAY}");
            if (response.IsSuccess)
            {
                List<LiquidationToDay> data = response.Content as List<LiquidationToDay>;
                foreach (var item in data)
                {
                    LiquidationToDayList.Add(item);
                }
            }
        }

        public async Task LoadSlideList()
        {
            SlideList.Clear();
            ApiResponse response = await ApiHelper.Get<List<SlideItem>>($"{ApiRouter.LIQUIDATION_SLIDEITEM}");
            if (response.IsSuccess)
            {
                List<SlideItem> data = response.Content as List<SlideItem>;
                ImageSlideCount = data.Count;
                foreach (var item in data)
                {
                    SlideList.Add(item);
                }
            }
        }

        public async Task LoadAdvertise()
        {
            Advertise = null;
            ApiResponse response = await ApiHelper.Get<Advertise>($"{ApiRouter.LIQUIDATION_ADVERTISE}");
            if (response.IsSuccess)
            {
                this.Advertise = response.Content as Advertise;
            }
        }
    }

}
