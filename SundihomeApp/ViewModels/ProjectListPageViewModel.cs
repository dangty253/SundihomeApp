using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.Models;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class ProjectListPageViewModel : ListViewPageViewModel2<Project>
    {
        public string Keyword { get; set; }
        public string TypeProject { get; set; }
        public ProjectListPageViewModel()
        {
            PreLoadData = new Command(() =>
            {
                if (string.IsNullOrEmpty(Keyword) && string.IsNullOrEmpty(TypeProject))
                {
                    ApiUrl = $"api/project?page={Page}";
                }
                else if(!string.IsNullOrEmpty(Keyword) && !string.IsNullOrEmpty(TypeProject))
                {
                    ApiUrl = $"api/project?page={Page}&keyword={Keyword}&typeproject={TypeProject}";
                }
                else if (string.IsNullOrEmpty(Keyword))
                {
                    ApiUrl = $"api/project?page={Page}&typeproject={TypeProject}";
                }
                else
                {
                    ApiUrl = $"api/project?page={Page}&keyword={Keyword}";
                }
            });
        }

        public ProjectListPageViewModel(bool isMyProjectList)
        {
            PreLoadData = new Command(() => ApiUrl = $"api/project/me?page={Page}");
        }
    }
}
