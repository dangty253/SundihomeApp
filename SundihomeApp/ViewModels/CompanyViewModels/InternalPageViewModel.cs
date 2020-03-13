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
    public class InternalPageViewModel : ListViewPageViewModel2<InternalPostItem>
    {
        public string Keyword { get; set; }
        public InternalPageViewModel()
        {
            IUserService userService = DependencyService.Get<IUserService>();
            OnMapItem = new Command<InternalPostItem>((post) =>
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
                    ApiUrl = $"{ApiRouter.INTERNAL_POSTITEMLIST}?page={Page}&CompanyId={UserLogged.CompanyId}&keyword={this.Keyword}";
                }
                else
                {
                    ApiUrl = $"{ApiRouter.INTERNAL_POSTITEMLIST}?page={Page}&CompanyId={UserLogged.CompanyId}";
                }
            });
        }

        public async Task<bool> Follow(string PostId)
        {
            IInternalPostItemService postItemService = DependencyService.Get<IInternalPostItemService>();
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
