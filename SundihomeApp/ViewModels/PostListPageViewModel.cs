using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class PostListPageViewModel : ListViewPageViewModel2<Post>
    {
        public FilterModel filterModel { get; set; }
        
        public PostListPageViewModel(short postType)
        {
            filterModel = new FilterModel();
            filterModel.PostType = postType;
            PreLoadData = new Command(() =>
            {
                string json = JsonConvert.SerializeObject(filterModel);
                ApiUrl = $"api/post/filter?json={json}&page={Page}";
            });

            
        }
        
    }

}
