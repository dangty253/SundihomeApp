using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class ProductListPageViewModel : ListViewPageViewModel2<FurnitureProduct>
    {
        private Guid _parentCategoryId;
        private FilterFurnitureProductModel _filterModel;

        public FilterFurnitureProductModel FilterModel
        {
            get => _filterModel;
            set
            {
                _filterModel = value;
                OnPropertyChanged(nameof(FilterModel));
            }
        }

        public ObservableCollection<FurnitureCategory> Categories { get; set; }

        public ProductListPageViewModel(Guid parentCategoryId)
        {
            _parentCategoryId = parentCategoryId;
            Categories = new ObservableCollection<FurnitureCategory>();
            FilterModel = new FilterFurnitureProductModel();
            FilterModel.ParentCategoryId = parentCategoryId;

            PreLoadData = new Command(() =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.FilterModel);
                ApiUrl = $"{ApiRouter.FURNITUREPRODUCT_FILTER}?json={json}&page={this.Page}";
            });
        }

        public async Task GetCategories()
        {
            var response = await ApiHelper.Get<List<FurnitureCategory>>($"{ApiRouter.FURNITURECATEGORY_GET_BY_PARENT}/{_parentCategoryId}");
            if (response.IsSuccess)
            {
                var list = response.Content as List<FurnitureCategory>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        item.Name = Language.ResourceManager.GetString(item.LanguageKey, Language.Culture);
                        Categories.Add(item);
                    }
                }
            }
        }
    }
}
