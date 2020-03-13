using System;
using System.Collections.Generic;
using SundihomeApi.Entities;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class UserFollowPage : ContentPage
    {
        public UserFollowPageViewModel viewModel;
        bool _isFollowing;
        List<Guid> _listId;
        public UserFollowPage(bool isFollowing, List<Guid> listID)
        {
            InitializeComponent();
            _isFollowing = isFollowing;
            _listId = listID;
            if (_isFollowing)
            {
                //goto ds dang theo doi
                BindingContext = viewModel = new UserFollowPageViewModel(_isFollowing, _listId);
            }
            else
            {
                //goto ds ng theo doi
                BindingContext = viewModel = new UserFollowPageViewModel(_listId);
            }
        }

        void OnCustomerTapped(object sender, ItemTappedEventArgs e)
        {
            var user = e.Item as User;
            Navigation.PushAsync(new UserProfilePage(user.Id));
        }
    }
}
