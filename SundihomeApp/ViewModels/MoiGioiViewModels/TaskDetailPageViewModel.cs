using System;
using System.Threading.Tasks;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class TaskDetailPageViewModel: BaseViewModel
    {
        private CongViec _congViec;
        public CongViec CongViec
        {
            get => _congViec;
            set
            {
                _congViec = value;
                OnPropertyChanged(nameof(CongViec));
            }
        }

        public TaskDetailPageViewModel()
        {
            CongViec = new CongViec();
        }

        public async Task GetCongViec(Guid taskId)
        {
            ApiResponse response = await ApiHelper.Get<CongViec>($"{ApiRouter.TASK_CRUD}/{taskId}", true);
            if (response.IsSuccess)
            {
                CongViec = response.Content as CongViec;
            }
            else
            {
                await Shell.Current.DisplayAlert("", response.Message, Language.dong);
            }
        }

    }
}
