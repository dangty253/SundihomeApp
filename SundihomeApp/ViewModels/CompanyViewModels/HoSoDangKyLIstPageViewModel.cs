using System;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class HoSoDangKyLIstPageViewModel : ListViewPageViewModel2<SundihomeApi.Entities.CompanyEntities.HoSoDangKyNhanVien>
    {
        public HoSoDangKyLIstPageViewModel(Guid CompanyId)
        {
            PreLoadData = new Command(() =>
            {
                ApiUrl = $"api/employee/hosodangky/{CompanyId}?page={this.Page}";
            });
        }
    }
}
