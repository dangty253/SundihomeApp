using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class CompanyListPageViewModel : ListViewPageViewModel2<Company>
    {
        public FilterCompanyModel filterCompanyModel { get; set; } = new FilterCompanyModel();

        public CompanyListPageViewModel()
        {
            PreLoadData = new Command(() =>
            {
                string json = JsonConvert.SerializeObject(this.filterCompanyModel);
                ApiUrl = $"api/company/filter?json={json}&page={Page}";
            });
        }
    }
}
