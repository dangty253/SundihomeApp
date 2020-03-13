using System;
using SundihomeApi.Entities;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class SearchPageResultViewModel : ListViewPageViewModel2<Post>
    {
        public FilterModel FilterModel { get; set; }
        public SearchPageResultViewModel(FilterModel filterModel = null)
        {
            if (filterModel == null)
            {
                FilterModel = new FilterModel();
            }
            else
            {
                FilterModel = filterModel;
            }

            PreLoadData = new Command(() =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(this.FilterModel);
                ApiUrl = $"api/post/filter?json={json}&page={Page}";
            });
        }
    }
}
