using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class PostItemListPageViewModel : ListViewPageViewModel2<PostItem>
    {
        public string Keyword { get; set; }
        public int Type { get; set; } = -1;
        public PostItemListPageViewModel()
        {
            IUserService userService = DependencyService.Get<IUserService>();
            OnMapItem = new Command<PostItem>((post) =>
            {
                post.CreatedBy = userService.Find(post.CreatedById);
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
                if (post.UserFollows != null && post.UserFollows.Any(x => x == UserLogged.Id))
                {
                    post.IsFollow = true;
                }
            });
            PreLoadData = new Command(() =>
            {
                if (string.IsNullOrWhiteSpace(this.Keyword))
                {
                    ApiUrl = $"api/postitems?page={Page}&type={Type}";
                }
                else
                {
                    ApiUrl = $"api/postitems?page={Page}&type={Type}&keyword={this.Keyword}";
                }
            });
        }

        public async Task<bool> Follow(string PostId)
        {
            IPostItemService postItemService = DependencyService.Get<IPostItemService>();
            var isFollow = await postItemService.Follow(PostId, UserLogged.Id);
            var currentPost = this.Data.Where(x => x.Id == PostId).SingleOrDefault();
            if (currentPost != null)
            {
                currentPost.IsFollow = isFollow;
            }
            return isFollow;
        }

        public void Share(string Id)
        {
            string url = ApiConfig.WEB_IP + $"postitem/{Id}";
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
