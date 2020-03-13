using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.FurnitureViewModels
{
    public class PromotionViewModel : ListViewPageViewModel2<FurnitureProduct>
    {
        public ObservableCollection<FurnitureCategory> Categories { get; set; }
        public FilterFurnitureProductModel FilterModel { get; set; }

        private DateTime? _promotionFromDate;
        public DateTime? PromotionFromDate
        {
            get => _promotionFromDate;
            set
            {
                _promotionFromDate = value;
                OnPropertyChanged(nameof(PromotionFromDate));
            }
        }
        private DateTime? _promotionToDate;
        public DateTime? PromotionToDate
        {
            get => _promotionToDate;
            set
            {
                _promotionToDate = value;
                OnPropertyChanged(nameof(PromotionToDate));
            }
        }

        private decimal? _promotionPrice;
        public decimal? PromotionPrice
        {
            get => _promotionPrice;
            set
            {
                _promotionPrice = value;
                OnPropertyChanged(nameof(PromotionPrice));
            }
        }
        public DateTime? DateNow { get; set; }
        public PromotionViewModel()
        {

            FilterModel = new FilterFurnitureProductModel();
            FilterModel.ProductStatus = 0;
            FilterModel.IsPromotion = true;
            PreLoadData = new Command(() =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.FilterModel);
                ApiUrl = $"{Configuration.ApiRouter.FURNITUREPRODUCT_FILTER}?json={json}&page={this.Page}";
            });
            Categories = new ObservableCollection<FurnitureCategory>();
        }

        public async Task LoadCategories()
        {
            //get list category
            var response = await ApiHelper.Get<List<FurnitureCategory>>(ApiRouter.FURNITURECATEGOR_GET_CATEGORY);
            if (response.IsSuccess)
            {
                var data = response.Content as List<FurnitureCategory>;
                foreach (var item in data)
                {
                    item.Name = Language.ResourceManager.GetString(item.LanguageKey, Language.Culture);
                    Categories.Add(item);
                }
            }
        }
    }
}
