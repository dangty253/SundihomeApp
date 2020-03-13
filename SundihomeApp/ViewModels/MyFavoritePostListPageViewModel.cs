using System;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApp.Helpers;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class MyFavoritePostListPageViewModel : ListViewPageViewModel2<Post>
    {
        public MyFavoritePostListPageViewModel()
        {
            PreLoadData = new Command(() =>
            {
                ApiUrl = $"api/post/MyFavoritePosts?page={Page}";
            });
        }
    }
}

