using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class TaskListPageViewModel: ListViewPageViewModel2<CongViec>
    {
        private int _type;
        public int Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public TaskListPageViewModel(int type)
        {
            Type = type;
            PreLoadData = new Command(() =>
            { 
                ApiUrl = $"{ApiRouter.TASK_GETMYTASKS}?type={Type}&page={Page}";
            });
        }
    }
}
