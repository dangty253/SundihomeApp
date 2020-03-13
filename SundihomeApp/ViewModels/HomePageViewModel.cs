using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private Project _project;
        public Project Project
        {
            get => _project;
            set
            {
                _project = value;
                OnPropertyChanged(nameof(Project));
            }
        }
        public ObservableCollection<Post> NewestBuyOrRentList { get; set; }  // ban
        public ObservableCollection<Post> NewestNeedToBuyOrRentList { get; set; } // cho thue
        public ObservableCollection<FurnitureProduct> FurnitureProducts { get; set; }
        public ObservableCollection<Liquidation> Liquidations { get; set; }

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
                    await Task.WhenAll(this.LoadNewestBuyOrRentList(), this.LoadNewestNeedtoBuyOrRentList(), this.LoadProjectList(), this.LoadLiquidationList());
                    IsRefreshing = false;
                });
            }
        }
        public HomePageViewModel()
        {
            NewestBuyOrRentList = new ObservableCollection<Post>();
            NewestNeedToBuyOrRentList = new ObservableCollection<Post>();
            FurnitureProducts = new ObservableCollection<FurnitureProduct>();
            Liquidations = new ObservableCollection<Liquidation>();
        }

        public async Task LoadProjectList()
        {
            var response = await ApiHelper.Get<Project>("api/project/random");
            if (response.IsSuccess)
            {
                this.Project = response.Content as Project;
            }
            else
            {
                this.Project = null;
            }
        }

        public async Task LoadNewestBuyOrRentList()
        {
            NewestBuyOrRentList.Clear();
            var response = await ApiHelper.Get<List<Post>>("api/post/NewesTbuyOrRentList");
            if (response.IsSuccess)
            {
                var data = new ObservableCollection<Post>(response.Content as List<Post>);

                for (int i = 0; i < data.Count; i++)
                {
                    NewestBuyOrRentList.Add(data[i]);
                }
            }
        }
        public async Task LoadNewestNeedtoBuyOrRentList()
        {
            NewestNeedToBuyOrRentList.Clear();
            var response = await ApiHelper.Get<List<Post>>("api/post/NewestNeedToBuyOrRentList");
            if (response.IsSuccess)
            {
                var data = new ObservableCollection<Post>(response.Content as List<Post>);
                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    NewestNeedToBuyOrRentList.Add(item);
                }
            }
        }

        public async Task LoadNewFurnitureProducts()
        {
            FurnitureProducts.Clear();
            var response = await ApiHelper.Get<List<FurnitureProduct>>(ApiRouter.FURNITUREPRODUCT_GETNEW);
            if (response.IsSuccess)
            {
                var data = new ObservableCollection<FurnitureProduct>(response.Content as List<FurnitureProduct>);
                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    FurnitureProducts.Add(item);
                }
            }
        }

        public async Task LoadLiquidationList()
        {
            Liquidations.Clear();
            var response = await ApiHelper.Get<List<Liquidation>>($"{ApiRouter.LIQUIDATION_GETBYTYPE}");
            if (response.IsSuccess)
            {
                List<Liquidation> data = response.Content as List<Liquidation>;
                foreach (var item in data)
                {
                    Liquidations.Add(item);
                }
            }
        }
    }
}
