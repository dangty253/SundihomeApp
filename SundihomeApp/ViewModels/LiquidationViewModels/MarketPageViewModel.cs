using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Share;
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.IServices.ILiquidation;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.LiquidationViewModels
{
    public class MarketPageViewModel : ListViewPageViewModel2<LiquidationPostItem>
    {
        public int Type { get; set; } = -1;
        public string Keyword { get; set; }
        public MarketPageViewModel()
        {
            IUserService userService = DependencyService.Get<IUserService>();
            OnMapItem = new Command<LiquidationPostItem>((post) =>
            {
                post.CreatedBy = userService.Find(post.CreatedById);
                if (post.UserFollows != null && post.UserFollows.Any(x => x == UserLogged.Id))
                {
                    post.IsFollow = true;
                }
            });
            PreLoadData = new Command(() =>
            {
                if (!string.IsNullOrEmpty(this.Keyword))
                {
                    ApiUrl = $"{ApiRouter.LIQUIDATIONPOSTITEM_GETALL}?page={Page}&type={Type}&keyword={this.Keyword}";
                }
                else
                {
                    ApiUrl = $"{ApiRouter.LIQUIDATIONPOSTITEM_GETALL}?page={Page}&type={Type}";
                }
            });
        }

        public async Task<bool> Follow(string PostId)
        {
            ILiquidationPostItemService postItemService = DependencyService.Get<ILiquidationPostItemService>();
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
            //string url = ApiConfig.WEB_IP + $"furniture/postitem/{Id}";
            //CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
