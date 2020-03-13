using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class CategoryPageViewModel : BaseViewModel
    {
        public async Task<List<FurnitureCategory>> GetCategories()
        {
            var response = await ApiHelper.Get<List<FurnitureCategory>>(ApiRouter.FURNITURECATEGORY_GET_ONLY_PARENT);
            if (response.IsSuccess)
            {
                var list = response.Content as List<FurnitureCategory>;
                return list;
            }
            return null;
        }
    }
}
