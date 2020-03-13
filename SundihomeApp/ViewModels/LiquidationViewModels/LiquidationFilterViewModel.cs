using System;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.LiquidationViewModels
{
    public class LiquidationFilterViewModel : ListViewPageViewModel2<Liquidation>
    {
        public LiquidationFilterModel FilterModel { get; set; }
        public LiquidationFilterViewModel(LiquidationFilterModel filterModel = null)
        {
            if (filterModel == null)
            {
                FilterModel = new LiquidationFilterModel();
            }
            else
            {
                FilterModel = filterModel;
            }
            PreLoadData = new Command(() =>
            {
                string json = JsonConvert.SerializeObject(FilterModel);
                ApiUrl = $"{Configuration.ApiRouter.LIQUIDATION_FILTER}?json={json}&page={this.Page}";
            });
        }
    }
}
