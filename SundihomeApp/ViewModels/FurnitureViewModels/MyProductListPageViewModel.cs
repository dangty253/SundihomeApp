using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class MyProductListPageViewModel : ListViewPageViewModel2<FurnitureProduct>
    {
        public FilterFurnitureProductModel FilterModel { get; set; }
        public MyProductListPageViewModel()
        {
            FilterModel = new FilterFurnitureProductModel();
            FilterModel.CreatedById = Guid.Parse(UserLogged.Id);

            PreLoadData = new Command(() =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.FilterModel);
                ApiUrl = $"{ApiRouter.FURNITUREPRODUCT_FILTER}?json={json}&page={Page}";
            });
        }
    }
}
