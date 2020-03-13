using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace SundihomeApp.ViewModels
{
    public class ProjectDetailPageViewModel : BaseViewModel
    {
        private Project _duAn;
        public Project DuAn
        {
            get => _duAn;
            set
            {
                _duAn = value;
                OnPropertyChanged(nameof(DuAn));
            }
        }
        private ProjectDiary _projectDiary;
        public ProjectDiary ProjectDiary
        {
            get => _projectDiary;
            set
            {
                _projectDiary = value;
                OnPropertyChanged(nameof(ProjectDiary));
            }
        }
        private int _page = 1;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }
        private bool _showMoreDiary;
        public bool ShowMoreDiary
        {
            get => _showMoreDiary;
            set
            {
                _showMoreDiary = value;
                OnPropertyChanged(nameof(ShowMoreDiary));
            }
        }

        public ObservableCollection<Post> BDSThuocDuAn_MuaBan { get; set; } = new ObservableCollection<Post>();
        public ObservableCollection<Post> BDSThuocDuAn_ChoThue { get; set; } = new ObservableCollection<Post>();
        public ObservableCollection<ProjectDiary> ListProjectDiary { get; set; } = new ObservableCollection<ProjectDiary>();

        public async Task LoadProject(Guid Id)
        {

            ApiResponse response = await ApiHelper.Get<Project>("api/project/" + Id);
            if (response.IsSuccess)
            {
                this.DuAn = response.Content as Project;
            }
        }
        public async Task LoadPostProjectID(Guid Id)
        {
            ApiResponse response = await ApiHelper.Get<List<Post>>("api/project/GetNewPost/" + Id);
            if (response.IsSuccess)
            {
                List<Post> data = (List<Post>)response.Content;
                foreach (var item in data)
                {
                    if (item.PostType == 0)
                    {
                        BDSThuocDuAn_MuaBan.Add(item);
                    }
                    if (item.PostType == 1)
                    {
                        BDSThuocDuAn_ChoThue.Add(item);
                    }
                }
                               
                
            }
        }
        public async Task GetListProjectDiary(Guid Id)
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<ProjectDiary>>($"{Configuration.ApiRouter.PROJECT_DIARY_GET_LIST_PROJECTDIARY_BYPROJECTID}/{Id}?page={Page}",true);
            if (apiResponse.IsSuccess)
            {
                List<ProjectDiary> data = (List<ProjectDiary>)apiResponse.Content;
                if (data.Count == 10) ShowMoreDiary = true;
                else ShowMoreDiary = false;
                foreach (var item in data)
                {
                    ListProjectDiary.Add(item);
                }
            }
        }
        public async Task GetProjectDiary(Guid Id)
        {
            ApiResponse apiResponse = await ApiHelper.Get<ProjectDiary>($"{Configuration.ApiRouter.PROJECT_DIARY_GET_ONE_PROJECTDIARY}/{Id}");
            if (apiResponse.IsSuccess)
            {
                this.ProjectDiary = apiResponse.Content as ProjectDiary;
            }
        }
    }
}
