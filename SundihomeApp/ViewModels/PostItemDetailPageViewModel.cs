using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Share;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class PostItemDetailPageViewModel : BaseViewModel
    {
        public int Page { get; set; } = 1;
        private bool _showLoadmoreCommentButton;
        public bool ShowLoadmoreCommentButton
        {
            get => _showLoadmoreCommentButton;
            set
            {
                _showLoadmoreCommentButton = value;
                OnPropertyChanged(nameof(ShowLoadmoreCommentButton));
            }
        }

        private PostItem _postItem;
        public PostItem PostItem
        {
            get => _postItem;
            set { _postItem = value; OnPropertyChanged(nameof(PostItem)); }
        }

        public ICommand LoadMoreCommentCommand { get; private set; }

        // danh sach comment
        public ObservableCollection<PostItemComment> Comments { get; set; }

        // list post trong comment.
        public ObservableCollection<Post> CommentPosts { get; set; }


        // comment text
        private string _commentText;
        public string CommentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                OnPropertyChanged(nameof(CommentText));
            }
        }

        // hien thi khi co comment tex, chonh inh, hoac chon bat dong san.
        public bool ShowClearCommentButton => this.CommentPosts.Count > 0;

        public IPostItemService _postItemService { get; private set; }
        public IUserService _userService { get; private set; }

        public Command OnShareDataCommand { get; set; }

        public PostItemDetailPageViewModel()
        {
            _postItemService = DependencyService.Get<IPostItemService>();
            _userService = DependencyService.Get<IUserService>();
            Comments = new ObservableCollection<PostItemComment>();
            CommentPosts = new ObservableCollection<Post>();
            LoadMoreCommentCommand = new Command(async () =>
            {
                this.Page += 1;
                await this.GetComments();
            });
            OnShareDataCommand = new Command(() => Share());
        }

        public async Task GetComments()
        {
            ShowLoadmoreCommentButton = false;
            var data = _postItemService.GetComment(PostItem.Id, Page);
            int count = data.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var item = data[i];
                    item.CreatedBy = _userService.Find(item.CreatedById);
                    Comments.Add(item);
                }
                if (count > 9)
                {
                    ShowLoadmoreCommentButton = true;
                }
            }
        }

        public async void LoadPostById(string PostId)
        {
            var post = await _postItemService.GetById(PostId);
            if (post.UserFollows != null && post.UserFollows.Any(x => x == UserLogged.Id))
            {
                post.IsFollow = true;
            }

            switch (post.Type)
            {
                case 0:
                    post.TypeTextFormat = Language.can_ban;
                    post.TypeColor = "#4F9A4D";
                    break;
                case 1:
                    post.TypeTextFormat = Language.cho_thue;
                    post.TypeColor = "#418ACC";
                    break;
                case 2:
                    post.TypeTextFormat = Language.can_mua;
                    post.TypeColor = "#5ac8ed";
                    break;
                case 3:
                    post.TypeTextFormat = Language.can_thue;
                    post.TypeColor = "#4EA1BD";
                    break;
                default:
                    post.TypeTextFormat = "";
                    break;
            }


            if (post.HasLoaiBatDongSan)
            {
                LoaiBatDongSanModel loaiBatDongSan = null;
                if (post.BDSTypeId.HasValue)
                {
                    loaiBatDongSan = LoaiBatDongSanModel.GetById(post.BDSTypeId.Value);
                    if (loaiBatDongSan != null)
                    {
                        post.BDSType = loaiBatDongSan.Name;
                    }
                }
            }
            this.PostItem = post;
        }

        // kich hoat an hien button clear
        public void FireOnChangeClearCommentButton() => OnPropertyChanged(nameof(ShowClearCommentButton));

        public void InsertComment(PostItemComment postItemComment)
        {
            _postItemService.InsertComment(postItemComment);
        }

        public async Task<bool> Follow(string PostId)
        {
            var isFollow = await _postItemService.Follow(PostId, UserLogged.Id);
            _postItem.IsFollow = isFollow;
            return isFollow;
        }

        public void Share()
        {
            string url = ApiConfig.WEB_IP + $"postitem/{PostItem.Id}";
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
