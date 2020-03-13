using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class ListViewPageViewModel<TEntity> : BaseViewModel where TEntity : class
    {
        public ObservableCollection<TEntity> Data { get; set; }
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


        public Command ItemAppearingCommand
        {
            get => new Command<TEntity>(async (item) =>
            {
                if (Data.Count == 0) return;
                if (item == Data[Data.Count - 1])
                {
                    await LoadMoreData();
                }
            });
        }

        public ListViewPageViewModel()
        {
            Data = new ObservableCollection<TEntity>();
            _page = 1;
        }

        public virtual async Task LoadData()
        {
            PreLoadData.Execute(null);
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
                        Data.Add(item);
                    }
                }
                else
                {
                    OutOfData = true;
                }
            }
            else
            {
                Data.Clear();
                _page = 1;
            }
            OnPropertyChanged(nameof(IsEmptyList));
        }

        public virtual async Task LoadMoreData()
        {
            if (OutOfData == false)
            {
                //IsLoadingMore = true;
                _page += 1;
                OutOfData = false;
                await LoadData();
                //IsLoadingMore = false;
            }
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
