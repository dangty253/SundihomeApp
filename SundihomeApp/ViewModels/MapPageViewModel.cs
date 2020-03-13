using System;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class MapPageViewModel : ListViewPageViewModel2<Post>
    {
        private bool _showImage;
        public bool ShowImage
        {
            get => _showImage;
            set
            {
                _showImage = value;
                OnPropertyChanged(nameof(ShowImage));
            }
        }
        public MapPageViewModel(FilterModel filterModel)
        {
            ShowImage = true;
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(filterModel);
            PreLoadData = new Command(() => ApiUrl = $"api/post/filter?json={json}&page={Page}");
        }
    }
}
