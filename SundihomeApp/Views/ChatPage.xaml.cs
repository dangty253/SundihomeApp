using System;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApp.Controls;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using SundihomeApp.ViewModels.Furniture;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class ChatPage : ContentPage
    {
        private string _userId; // nguoi dang chat.
        private readonly ChatPageViewModel viewModel;

        public ContentView FilterPostPage;
        public ContentView FilterLiquidationPage;
        public ContentView FilterFurntiureProductPage;

        public ChatPage(string userId)
        {
            InitializeComponent();
            _userId = userId;
            this.BindingContext = viewModel = new ChatPageViewModel(userId);
            labelTitle.Text = viewModel.user.FullName;
            imageAvatar.Source = viewModel.user.AvatarFullUrl;
            this.Initialize();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.ConnectToHub();
        }

        public async void Initialize()
        {
            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.DisconnecHub();
        }

        public void ScrollTap(object sender, System.EventArgs e)
        {
            lock (new object())
            {
                if (BindingContext != null)
                {
                    var vm = BindingContext as ChatPageViewModel;

                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    while (vm.DelayedMessages.Count > 0)
                    //    {
                    //        vm.Data.Insert(0, vm.DelayedMessages.Dequeue());
                    //    }
                    //    vm.ShowScrollTap = false;
                    //    vm.LastMessageVisible = true;
                    //    vm.PendingMessageCount = 0;
                    //    ChatList?.ScrollToFirst();
                    //});
                }
            }
        }

        public void OnListTapped(object sender, ItemTappedEventArgs e)
        {
            //   chatInput.UnFocusEntry();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            viewModel.MessageAppearingCommand.Execute(e.Item);
        }

        private void OnItem_Tapped(object sender, ItemTappedEventArgs e)
        {
            chatListView.SelectedItem = null;
            chatInput.UnFocusEntry();
        }

        private async void PickPost_Clicked(object sender, EventArgs e)
        {
            ModalPicker.Title = Language.chon_bat_dong_san;
            loadingPopup.IsVisible = true;
            if (FilterPostPage == null)
            {
                FilterPostPage = new ContentView();
                SearchPageResultViewModel filterViewModel = new SearchPageResultViewModel();
                filterViewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
                FilterPostPage.BindingContext = filterViewModel;
                BsdListView bsdListView = new BsdListView()
                {
                    ItemTemplate = new DataTemplate(typeof(Cells.PostViewCell))
                };
                bsdListView.ItemTapped += async (object listview, ItemTappedEventArgs listviewEventArgs) =>
                {
                    var item = listviewEventArgs.Item as SundihomeApi.Entities.Post;
                    var post = new SundihomeApi.Entities.Mongodb.Post()
                    {
                        Title = item.Title,
                        Avatar = item.Avatar,
                        PostId = item.Id.ToString(),
                        PriceText = item.PriceFormatText,
                        Address = item.Address
                    };
                    viewModel.SendPostMessage(post);
                    await ModalPicker.Hide();
                };

                await filterViewModel.LoadData();

                FilterPostPage.Content = bsdListView;
            }

            ContentViewLookUp.Content = FilterPostPage;
            await ModalPicker.Show();
            loadingPopup.IsVisible = false;
        }

        private async void PickFurnitureProduct_Clicked(object sender, EventArgs e)
        {
            ModalPicker.Title = Language.chon_tu_san_pham_noi_that;
            loadingPopup.IsVisible = true;
            if (FilterFurntiureProductPage == null)
            {
                FilterFurntiureProductPage = new ContentView();

                FilterFurnitureProductViewModel filterViewModel = new FilterFurnitureProductViewModel();
                filterViewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
                FilterFurntiureProductPage.BindingContext = filterViewModel;
                BsdListView bsdListView = new BsdListView()
                {
                    ItemTemplate = new DataTemplate(typeof(Views.Cells.FurnitureCells.ProductViewCell))
                };
                bsdListView.ItemTapped += async (object listview, ItemTappedEventArgs listviewEventArgs) =>
                {
                    var item = listviewEventArgs.Item as FurnitureProduct;
                    var product = new FurnitureProductChatMessage
                    {
                        Title = item.Name,
                        Avatar = item.AvatarUrl,
                        ProductId = item.Id.ToString(),
                        PriceText = item.Price.HasValue ? string.Format("{0:0,0 đ}", item.Price.Value) : "",
                        Address = item.Address
                    };
                    viewModel.SendFurnitureProductMessage(product);
                    await ModalPicker.Hide();
                };

                await filterViewModel.LoadData();

                FilterFurntiureProductPage.Content = bsdListView;
            }

            ContentViewLookUp.Content = FilterFurntiureProductPage;
            await ModalPicker.Show();
            loadingPopup.IsVisible = false;
        }

        private async void PickerLiquidationPost_Clicked(object sender, EventArgs e)
        {
            ModalPicker.Title = Language.chon_tu_san_pham_thanh_ly;
            loadingPopup.IsVisible = true;
            if (FilterLiquidationPage == null)
            {
                FilterLiquidationPage = new ContentView();

                LiquidationFilterViewModel filterViewModel = new LiquidationFilterViewModel();
                filterViewModel.FilterModel.CreatedById = Guid.Parse(UserLogged.Id);
                filterViewModel.FilterModel.Status = 0;
                FilterLiquidationPage.BindingContext = filterViewModel;
                BsdListView bsdListView = new BsdListView()
                {
                    ItemTemplate = new DataTemplate(typeof(Views.Cells.LiquidationCells.LiquidationViewCell))
                };
                bsdListView.ItemTapped += async (object listview, ItemTappedEventArgs listviewEventArgs) =>
                {
                    var item = listviewEventArgs.Item as Liquidation;
                    var post = new LiquidationCommentPost
                    {
                        Title = item.Name,
                        Avatar = item.Avatar,
                        PostId = item.Id.ToString(),
                        PriceText = item.Price > 0 ? string.Format("{0:0,0 đ}", item.Price) : "",
                        Address = item.Address
                    };
                    viewModel.SendLiquidationtMessage(post);
                    await ModalPicker.Hide();
                };

                await filterViewModel.LoadData();

                FilterLiquidationPage.Content = bsdListView;
            }

            ContentViewLookUp.Content = FilterLiquidationPage;
            await ModalPicker.Show();
            loadingPopup.IsVisible = false;
        }

        private async void ViewUserProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserProfilePage(Guid.Parse(this._userId)));
        }
    }
}
