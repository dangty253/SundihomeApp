using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Settings;
using Xamarin.Forms;
namespace SundihomeApp.ViewModels.FurnitureViewModels
{
    public class MyPromotionListPageViewModel : ListViewPageViewModel2<FurnitureProduct>
    {
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
        public MyPromotionListPageViewModel()
        {
            FilterModel = new FilterFurnitureProductModel();
            FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
            FilterModel.IsPromotion = true;

            PreLoadData = new Command(() =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.FilterModel);
                ApiUrl = $"{ApiRouter.FURNITUREPRODUCT_FILTER}?json={json}&page={Page}";
            });
        }
    }
}
