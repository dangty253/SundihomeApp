using System;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class InviteUserListPageViewModel : ListViewPageViewModel2<InviteUser>
    {
        public InviteUserListPageViewModel()
        {
            PreLoadData = new Command(() => ApiUrl = $"api/company/GetInviteUserList/{UserLogged.CompanyId}?page={Page}");
        }
    }
}

