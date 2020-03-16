using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApp.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Extended;

namespace SundihomeApp.ViewModels
{
    public class ListViewPageViewModel2<TEntity> : BaseViewModel where TEntity : class
    {
        public InfiniteScrollCollection<TEntity> Data { get; set; }
        public string ApiUrl { get; set; }
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

        public bool OutOfData { get; set; } = false;

        private bool _isLoadingMore = false;
        public bool IsLoadingMore
        {
            get { return _isLoadingMore; }
            set
            {
                _isLoadingMore = value;
                OnPropertyChanged(nameof(IsLoadingMore));
            }
        }

        private int _page;
        public int Page
        {
            get => _page;
            set
            {
                if (_page != value)
                {
                    _page = value;
                    OnPropertyChanged(nameof(Page));
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmptyList
        {
            get => Data.Count == 0;
        }

        public ICommand PreLoadData { get; set; }

        public Command<TEntity> OnMapItem { get; set; }

        public ICommand RefreshCommand
        {
            get => new Command(async () => await LoadOnRefreshCommandAsync());
        }

        public ListViewPageViewModel2()
        {
            Data = new InfiniteScrollCollection<TEntity>
            {
                OnLoadMore = async () =>
                {
                    _page += 1;
                    return await this.LoadItems();
                },
                OnCanLoadMore = () => OutOfData == false
            };
            _page = 1;
        }

        public async Task LoadData()
        {
            var items = await LoadItems();
            this.Data.AddRange(items);
        }

        public virtual async Task<List<TEntity>> LoadItems()
        {
            PreLoadData.Execute(null);
            var items = new List<TEntity>();

            var result = await ApiHelper.Get<List<TEntity>>(ApiUrl, true);
            if (result.IsSuccess)
            {
                var list = (List<TEntity>)result.Content;
                var count = list.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {

                        var item = list[i];
                        if (OnMapItem != null)
                        {
                            OnMapItem.Execute(item);
                        }
                        items.Add(item);
                    }
                }
                else
                {
                    OutOfData = true;
                }
            }
            else
            {
                OutOfData = false;
                Data.Clear();
                _page = 1;
            }
            OnPropertyChanged(nameof(IsEmptyList));
            return items;
        }

        public async Task LoadOnRefreshCommandAsync()
        {
            IsRefreshing = true;
            _page = 1;
            Data.Clear();
            OutOfData = false;
            await LoadData();
            IsRefreshing = false;
        }
    }
}
