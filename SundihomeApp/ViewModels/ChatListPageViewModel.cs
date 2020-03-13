using System;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApp.IServices;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class ChatListPageViewModel : ListViewPageViewModel2<ChatListItem>
    {
        public ChatListPageViewModel()
        {
            IUserService userService = DependencyService.Get<IUserService>();
            OnMapItem = new Command<ChatListItem>((post) =>
            {
                post.Partner = userService.Find(post.PartnerId.ToLower());
                if (post.Partner == null)
                {

                }
            });
            PreLoadData = new Command(() => ApiUrl = $"api/message/chatlist?page={this.Page}");
        }
    }
}
