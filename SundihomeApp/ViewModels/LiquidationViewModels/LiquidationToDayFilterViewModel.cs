using System;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.LiquidationViewModels
{
    public class LiquidationToDayFilterViewModel : ListViewPageViewModel2<LiquidationToDay>
    {
        public LiquidationToDayFilterModel FilterModel { get; set; }
        public LiquidationToDayFilterViewModel(LiquidationToDayFilterModel filterModel = null)
        {
            if (filterModel == null)
            {
                FilterModel = new LiquidationToDayFilterModel();
            }
            else
            {
                FilterModel = filterModel;
            }
            PreLoadData = new Command(() =>
            {
                string json = JsonConvert.SerializeObject(FilterModel);
                ApiUrl = $"{Configuration.ApiRouter.LIQUIDATIONTODAY_FILTER}?json={json}&page={this.Page}";
            });
        }
    }
}
