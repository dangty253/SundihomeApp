using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Views.Furniture;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class HomePageViewModel : BaseViewModel
    {
        public ObservableCollection<SlideItem> SlideList { get; set; }
        public ObservableCollection<FurnitureProduct> Products { get; set; }
        public ObservableCollection<FurnitureProduct> PromotionProducts { get; set; }
        public List<FurnitureCategory> FurnitureParentCategoryList { get; set; }


        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    this.CurrentSlideImageIndex = 1;
                    await Task.WhenAll(this.LoadProducts(), this.LoadSlideList(), this.LoadAdvertise());
                    IsRefreshing = false;
                });
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

        public HomePageViewModel()
        {
            FurnitureParentCategoryList = new List<FurnitureCategory>();
            SlideList = new ObservableCollection<SlideItem>();
            Products = new ObservableCollection<FurnitureProduct>();
            PromotionProducts = new ObservableCollection<FurnitureProduct>();
            CurrentSlideImageIndex = 1;
        }

        public async Task LoadCategories()
        {
            //get list category
            var response = await ApiHelper.Get<List<FurnitureCategory>>(ApiRouter.FURNITURECATEGOR_GET_CATEGORY);
            if (response.IsSuccess)
            {
                FurnitureParentCategoryList = response.Content as List<FurnitureCategory>;
                FurnitureParentCategoryList.ForEach(x => x.Name = Language.ResourceManager.GetString(x.LanguageKey, Language.Culture));
            }
        }

        public async Task LoadPromotionProducts()
        {
            this.PromotionProducts.Clear();
            ApiResponse response = await ApiHelper.Get<List<FurnitureProduct>>($"{ApiRouter.FURNITUREPRODUCT_GETNEWPROMOTIONS}?take=5");
            if (response.IsSuccess)
            {
                List<FurnitureProduct> data = response.Content as List<FurnitureProduct>;
                foreach (var item in data)
                {
                    PromotionProducts.Add(item);
                }
            }
        }

        public async Task LoadProducts()
        {
            this.Products.Clear();
            ApiResponse response = await ApiHelper.Get<List<FurnitureProduct>>($"{ApiRouter.FURNITUREPRODUCT_GETNEW}?take=5");
            if (response.IsSuccess)
            {
                List<FurnitureProduct> data = response.Content as List<FurnitureProduct>;
                foreach (var item in data)
                {
                    Products.Add(item);
                }
            }
        }

        public async Task LoadSlideList()
        {
            SlideList.Clear();
            ApiResponse response = await ApiHelper.Get<List<SlideItem>>($"{ApiRouter.FURNITUREPRODUCT_SLIDEITEME}");
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
            ApiResponse response = await ApiHelper.Get<Advertise>($"{ApiRouter.FURNITUREPRODUCT_ADVERTISE}");
            if (response.IsSuccess)
            {
                this.Advertise = response.Content as Advertise;
            }
        }
    }
}
