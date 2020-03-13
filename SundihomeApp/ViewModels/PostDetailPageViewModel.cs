using Plugin.FacebookClient;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class PostDetailPageViewModel : ViewModels.BaseViewModel
    {

        public const int BUTTON_SHARE_INDEX = 1;
        public const int BUTTON_CALL_INDEX = 2;
        public const int BUTTON_SMS_INDEX = 3;
        public const int BUTTON_CHAT_INDEX = 4;
        public const int BUTTON_SCHEDULE_INDEX = 5;

        private string _id;
        public Post _getPost;
        public Post GetPost
        {
            get => _getPost;
            set
            {
                _getPost = value;
                OnPropertyChanged(nameof(GetPost));
            }
        }

        public int PostCommentPage { get; set; } = 1;
        public ObservableCollection<Post> PostComments { get; set; }
        private bool _showLoadmorePostCommentButton;
        public bool ShowLoadmorePostCommentButton
        {
            get => _showLoadmorePostCommentButton;
            set
            {
                _showLoadmorePostCommentButton = value;
                OnPropertyChanged(nameof(ShowLoadmorePostCommentButton));
            }
        }


        public List<Post> _bdsKhac;
        public List<Post> BDSKhac
        {
            get => _bdsKhac;
            set
            {
                _bdsKhac = value;
                OnPropertyChanged(nameof(BDSKhac));
            }
        }

        public Command OnShareDataCommand { get; set; }

        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; }

        public PostDetailPageViewModel(Guid id)
        {
            _id = id.ToString();
            OnShareDataCommand = new Command(() => Share($"{ApiConfig.WEB_IP}post/{_id}"));
            ButtonCommandList = new ObservableCollection<FloatButtonItem>();
            ButtonCommandList.Add(new FloatButtonItem(Language.chia_se, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf1e0", OnShareDataCommand, null)); //0
            PostComments = new ObservableCollection<Post>();
        }

        public async Task LoadBDSKhac(Guid Id, Guid IdProject)
        {
            ApiResponse response = await ApiHelper.Get<List<Post>>($"{ApiRouter.POST_GETBYPROJECT}/{Id}/{IdProject}");
            if (response.IsSuccess)
            {
                this.BDSKhac = response.Content as List<Post>;
            }
        }

        public async Task<bool> LoadPost(Guid Id)
        {
            ApiResponse response = await ApiHelper.Get<Post>(ApiRouter.POST_GETBYID + "/" + Id);
            if (response.IsSuccess)
            {
                this.GetPost = response.Content as Post;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task LoadPostComment()
        {
            ShowLoadmorePostCommentButton = false;
            var response = await ApiHelper.Get<List<PostComment>>(ApiRouter.POST_COMMENT + "/" + _id + "?page=" + this.PostCommentPage, false);
            if (response.IsSuccess)
            {
                var data = response.Content as List<PostComment>;
                int count = data.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var item = data[i];
                        item.ChildPost.CreatedDate = item.CreatedDate;
                        PostComments.Add(item.ChildPost);
                    }
                    ShowLoadmorePostCommentButton = true;
                }
            }
        }

        void Share(string url)
        {
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
