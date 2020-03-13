using System;
using System.Threading.Tasks;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Telerik.XamarinForms.Primitives;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class UserProfilePage : ContentPage
    {
        public UserProfilePageViewModel viewModel;
        private Guid _id;

        public UserProfilePage()
        {
            InitializeComponent();
        }

        public UserProfilePage(Guid userId)
        {
            InitializeComponent();
            _id = userId;
            BindingContext = viewModel = new UserProfilePageViewModel();
            Init();
        }

        public async void Init()
        {
            await Task.WhenAll(viewModel.GetUser(_id),
                viewModel.GetPosts(_id),
                viewModel.GetPostTotal(_id),
                viewModel.GetFollowers(_id));
            viewModel.IsLoading = false;
        }

        void OnFollowTapped(object sender, EventArgs e)
        {
            viewModel.Follow(_id);
        }

        void OnUnFollowTapped(object sender, EventArgs e)
        {
            viewModel.Follow(_id);
        }

        async void OnEmailTapped(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                return;
            }
            try
            {
                await Email.ComposeAsync(string.Empty, string.Empty, viewModel.User.Email);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi", ex.Message, Language.dong);
            }
        }

        async void OnPhoneTapped(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                return;
            }
            string action = await DisplayActionSheet(Language.tuy_chon, Language.huy, null, Language.goi_dien, Language.nhan_tin);
            if (action == Language.goi_dien)
            {
                try
                {
                    PhoneDialer.Open(viewModel.User.Phone);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("", ex.Message, Language.dong);
                }
            }
            if (action == Language.nhan_tin)
            {
                try
                {
                    await Sms.ComposeAsync(new SmsMessage(string.Empty, viewModel.User.Phone));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("", ex.Message, Language.dong);
                }
            }
        }

        void OnCompanyTapped(object sender, EventArgs e)
        {
            if (viewModel.User.CompanyId.HasValue)
            {
                Navigation.PushAsync(new Views.CompanyViews.CompanyProfileDetailPage(viewModel.User.CompanyId.Value));
            }
        }

        async void OnLoadMorePost(object sender, EventArgs e)
        {
            viewModel.IsLoading = true;
            viewModel.Page++;
            await viewModel.GetPosts(_id);
            viewModel.IsLoading = false;
        }

        async void OnChat_Tapped(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new ChatPage(viewModel.User.Id.ToString()));
        }

        void OnPostTapped(object sender, EventArgs e)
        {
            var radBorder = sender as RadBorder;
            var tap = radBorder.GestureRecognizers[0] as TapGestureRecognizer;
            var post = tap.CommandParameter as SundihomeApi.Entities.Post;
            Navigation.PushAsync(new PostDetailPage(post.Id));
        }
    }
}
