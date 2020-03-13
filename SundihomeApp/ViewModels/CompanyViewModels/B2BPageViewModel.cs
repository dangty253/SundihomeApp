using System;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.Configuration;
using SundihomeApp.IServices;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.CompanyViewModels
{
    public class B2BPageViewModel : ViewModels.ListViewPageViewModel2<B2BPostItem>
    {
        public int Type { get; set; } = -1;
        public string Keyword { get; set; }
        public B2BPageViewModel()
        {
            IUserService userService = DependencyService.Get<IUserService>();
            OnMapItem = new Command<B2BPostItem>((post) =>
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
                    ApiUrl = $"{ApiRouter.B2BPOSTITEM_LIST}?page={Page}&type={Type}&keyword={this.Keyword}";
                }
                else
                {
                    ApiUrl = $"{ApiRouter.B2BPOSTITEM_LIST}?page={Page}&type={Type}";
                }
            });
        }

        public async Task<bool> Follow(string PostId)
        {
            IB2BPostItemService postItemService = DependencyService.Get<IB2BPostItemService>();
            var isFollow = await postItemService.Follow(PostId, UserLogged.Id);
            var currentPost = this.Data.Where(x => x.Id == PostId).SingleOrDefault();
            if (currentPost != null)
            {
                currentPost.IsFollow = isFollow;
            }
            return isFollow;
        }

    }
}
