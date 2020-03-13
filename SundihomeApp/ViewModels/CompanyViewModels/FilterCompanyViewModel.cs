using System;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class FilterCompanyViewModel : ListViewPageViewModel2<Company>
    {
        public FilterCompanyModel FilterCompanyModel { get; set; }
        public FilterCompanyViewModel(FilterCompanyModel filterCompanyModel)
        {
            FilterCompanyModel = filterCompanyModel;
            PreLoadData = new Command(() =>
            {
                string json = JsonConvert.SerializeObject(this.FilterCompanyModel);
                ApiUrl = $"api/company/filter?json={json}&page={Page}";
            });
        }
    }
}
