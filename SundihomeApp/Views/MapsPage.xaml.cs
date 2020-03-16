using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Plugin.FirebasePushNotification;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace SundihomeApp.Views
{
    public partial class MapsPage : ContentPage
    {
        public CustomMap map;
        public MapPageViewModel viewModel;
        public MapsPage(FilterModel filterModel)
        {
            InitializeComponent();
            this.BindingContext = viewModel = new MapPageViewModel(filterModel);
            Init();

        }

        public async void Init()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                var status = await PermissionHelper.CheckPermission(Plugin.Permissions.Abstractions.Permission.Location, "Truy cập vị trí", "Sundihome cần quyền truy cập ví trị, để lấy vị trí và tìm bất động sản xung quanh cho bạn.");
                if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    await Navigation.PopAsync();
                    return;
                }
            }

            await viewModel.LoadData();

            var first = viewModel.Data.Where(x => x.Lat.HasValue && x.Long.HasValue).FirstOrDefault();
            if (first == null)
            {
                await DisplayAlert("", "Không tìm thấy bất động sản nào.", Language.dong);
                await Navigation.PopAsync();
                return;
            }

            MapSpan mapSpan = MapSpan.FromCenterAndRadius(
                    new Position(first.Lat.Value, first.Long.Value), Distance.FromMiles(3));

            map = new CustomMap()
            {
                IsShowingUser = true,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                MoveToLastRegionOnLayoutChange = true,
                CustomPins = new List<CustomPin>(),
            };

            var webClient = new WebClient();
            foreach (var item in viewModel.Data.Where(x => x.Lat.HasValue && x.PriceFormatText != null))
            {
                var pin = new CustomPin
                {
                    Url = $"{Configuration.ApiConfig.IP}api/post/priceimage?text={item.PriceFormatText}",
                    Type = PinType.Place,
                    Position = new Position(item.Lat.Value, item.Long.Value),
                    Label = item.Title,
                    Address = item.Address,
                    PriceText = item.PriceFromText ?? item.PriceFormatText,
                    PostId = item.Id
                };

                pin.PinBytes = webClient.DownloadData(pin.Url);
                pin.Clicked += async (object o, EventArgs e) =>
                {
                    Pin pinClicked = (Pin)o;
                    await Navigation.PushAsync(new PostDetailPage(item.Id));
                };

                map.Pins.Add(pin);
                map.CustomPins.Add(pin);
            }
            webClient.Dispose();
            map.MoveToRegion(mapSpan);
            grid.Children.Insert(0, map);



            loadingPopup.IsVisible = false;
        }

        public async void GoToDetail_Clicked(object sender, EventArgs e)
        {
            var id = Guid.Parse(((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter.ToString());
            await Navigation.PushAsync(new PostDetailPage(id));
        }
        public void ViewDetail_Clicked(object sender, EventArgs e)
        {
            var tap = (sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer;
            Post post = tap.CommandParameter as Post;
            if (post.Lat.HasValue == false) return;

            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Position(post.Lat.Value, post.Long.Value), Distance.FromMiles(3)));
        }
    }
}
