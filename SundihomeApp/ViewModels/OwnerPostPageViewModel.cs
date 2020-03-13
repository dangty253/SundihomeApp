using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class OwnerPostPageViewModel : BaseViewModel
    {
        private Post _owner;
        public Post Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                OnPropertyChanged(nameof(Owner));
            }
        }
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } 
        public OwnerPostPageViewModel()
        {
            ButtonCommandList = new ObservableCollection<FloatButtonItem>();
        }
        public async Task GetOwnerAsync(Guid Id)
        {
            ApiResponse apiResponse = await ApiHelper.Get<Post>(ApiRouter.POST_GETBYID + "/" + Id);
            if (apiResponse.IsSuccess)
            {
                this.Owner = apiResponse.Content as Post;
            }
        }
    }
}

