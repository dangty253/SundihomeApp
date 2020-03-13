using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Share;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.Furniture
{
    public class FurniturePostItemDetailPageViewModel : BaseViewModel
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

        private FurniturePostItem _postItem;
        public FurniturePostItem PostItem
        {
            get => _postItem;
            set { _postItem = value; OnPropertyChanged(nameof(PostItem)); }
        }

        public ICommand LoadMoreCommentCommand { get; private set; }

        // danh sach comment
        public ObservableCollection<FurniturePostItemComment> Comments { get; set; }

        // list post trong comment.
        public ObservableCollection<FurniturePostItemProductComment> CommentPosts { get; set; }


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

        public IFurniturePostItemService _postItemService { get; private set; }
        public IUserService _userService { get; private set; }

        public Command OnShareDataCommand { get; set; }

        public FurniturePostItemDetailPageViewModel()
        {
            _postItemService = DependencyService.Get<IFurniturePostItemService>();
            _userService = DependencyService.Get<IUserService>();
            Comments = new ObservableCollection<FurniturePostItemComment>();
            CommentPosts = new ObservableCollection<FurniturePostItemProductComment>();
            LoadMoreCommentCommand = new Command(async () =>
            {
                this.Page += 1;
                await this.GetComments();
            });
            OnShareDataCommand = new Command(() => Share());
        }

        public FurniturePostItemDetailPageViewModel(Guid postItemId)
        {
            _postItemService = DependencyService.Get<IFurniturePostItemService>();
            _userService = DependencyService.Get<IUserService>();
            Comments = new ObservableCollection<FurniturePostItemComment>();
            CommentPosts = new ObservableCollection<FurniturePostItemProductComment>();
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
            string typeFormatText = post.TypeTextFormat;


            this.PostItem = post;
            OnPropertyChanged(nameof(this.PostItem.TypeColor));
        }

        // kich hoat an hien button clear
        public void FireOnChangeClearCommentButton() => OnPropertyChanged(nameof(ShowClearCommentButton));

        public void InsertComment(FurniturePostItemComment postItemComment)
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
            string url = ApiConfig.WEB_IP + $"furniture/postitem/{PostItem.Id}";
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
