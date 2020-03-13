using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        public ObservableCollection<Post> NewestBuyOrRentList { get; set; }  // ban
        public ObservableCollection<ContactNeed> ContactNeeds { get; set; }
        public Company Company { get; set; }
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    await Task.WhenAll(this.LoadNewestBuyOrRentList(), LoadContactNeeds());
                    IsRefreshing = false;
                });
            }
        }
        public HomePageViewModel()
        {
            NewestBuyOrRentList = new ObservableCollection<Post>();
            ContactNeeds = new ObservableCollection<ContactNeed>();
        }

        public async Task LoadNewestBuyOrRentList()
        {
            NewestBuyOrRentList.Clear();
            var response = await ApiHelper.Get<List<Post>>(ApiRouter.COMPANY_GETNEWPOST + "/" + UserLogged.CompanyId, true);
            if (response.IsSuccess)
            {
                var data = new ObservableCollection<Post>(response.Content as List<Post>);
                for (int i = 0; i < data.Count; i++)
                {
                    NewestBuyOrRentList.Add(data[i]);
                }
            }
        }

        public async Task LoadContactNeeds()
        {
            ContactNeeds.Clear();
            var response = await ApiHelper.Get<List<ContactNeed>>(ApiRouter.COMPANY_GETNEWCONCTACTNEEDS + "/" + UserLogged.CompanyId, true);
            if (response.IsSuccess)
            {
                var data = new ObservableCollection<ContactNeed>(response.Content as List<ContactNeed>);
                for (int i = 0; i < data.Count; i++)
                {
                    ContactNeeds.Add(data[i]);
                }
            }
        }

        public async Task GetCompany()
        {
            var response = await ApiHelper.Get<Company>(ApiRouter.COMANY_GETBYID + "/" + UserLogged.CompanyId);
            if (response.IsSuccess)
            {
                Company = response.Content as Company;
            }
        }
    }
}
