using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class FilterFurnitureProductViewModel : ListViewPageViewModel2<FurnitureProduct>
    {
        public ObservableCollection<FurnitureCategory> Categories { get; set; }
        public FilterFurnitureProductModel FilterModel { get; set; }
        public FilterFurnitureProductViewModel(FilterFurnitureProductModel filterModel = null)
        {
            if (filterModel != null)
            {
                FilterModel = filterModel;
            }
            else
            {
                FilterModel = new FilterFurnitureProductModel();
                FilterModel.IsPromotion = false;
                FilterModel.ProductStatus = 0;
            }
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
