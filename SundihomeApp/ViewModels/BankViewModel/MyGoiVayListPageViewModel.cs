using System;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.BankViewModel
{
    public class MyGoiVayListPageViewModel : ListViewPageViewModel2<GoiVay>
    {
        public GoiVayFilterModel _filterModel;
        public MyGoiVayListPageViewModel(GoiVayFilterModel FilterModel = null)
        {
            if (FilterModel == null)
            {
                _filterModel = new GoiVayFilterModel();
            }

            PreLoadData = new Command(() =>
            {
                var json = JsonConvert.SerializeObject(this._filterModel);
                ApiUrl = $"api/bank/goivay/filter?json={json}&page={this.Page}";
            });
        }
    }
}