using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb.Liquidation;
using SundihomeApp.Configuration;
using SundihomeApp.IServices.ILiquidation;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.LiquidationViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.LiquidationViews
{
    public partial class HomePage : ContentPage
    {
        private HomePageViewModel viewModel;
        private readonly ILiquidationCategoryService _liquidationCategoryService;
        private List<LiquidationCategory> liquidationCategories;
        public HomePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new HomePageViewModel();
            ModalOverlay.BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);
            _liquidationCategoryService = DependencyService.Get<ILiquidationCategoryService>();
            MessagingCenter.Subscribe<AddLiquidationPage>(this, "OnSaveItem", async (sender) => await viewModel.LoadLiquidationList());
            MessagingCenter.Subscribe<LiquidationDetailPage, Guid>(this, "OnDeleted", async (sender, liquidationId) => {
                if (viewModel.Type0List.Any(x=>x.Id== liquidationId))
                {
                    await viewModel.LoadLiquidationList();
                }
            });


            MessagingCenter.Subscribe<AddToDayPage>(this, "OnSaveItem", async (sender) => await viewModel.LoadLiquidationToDayList());
            MessagingCenter.Subscribe<PickerLiquidationPage>(this, "OnSaveItem", async (sender) => await viewModel.LoadLiquidationToDayList());
            MessagingCenter.Subscribe<ToDayDetailPage, Guid>(this, "OnDeleted", async (sender, todayId) =>
            {
                if (viewModel.LiquidationToDayList.Any(x => x.Id == todayId))
                {
                    await viewModel.LoadLiquidationToDayList();
                }
            });
            Init();
        }


        public async void Init()
        {
            await Task.WhenAll(viewModel.LoadLiquidationList(), viewModel.LoadLiquidationToDayList(), viewModel.LoadSlideList(), viewModel.LoadAdvertise());
            SetUpSlideImages();
            LoadCategories();
            loadingPopup.IsVisible = false;
        }

        public void SetUpSlideImages()
        {
            // set slide image
            carouseView.ItemsSource = viewModel.SlideList;
            SetSlideTimer();
        }

        public void SetSlideTimer()
        {
            // chay slide.
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                var currentIndex = carouseView.SelectedIndex;

                if (currentIndex < (viewModel.ImageSlideCount - 1))
                {
                    carouseView.SelectedIndex += 1;
                    viewModel.CurrentSlideImageIndex = carouseView.SelectedIndex + 1;
                }
                else
                {
                    carouseView.SelectedIndex = 0;
                    viewModel.CurrentSlideImageIndex = 1;
                }
                return true; // True = Repeat again, False = Stop the timer
            });
        }

        private void CarouseView_ItemSwiped(PanCardView.CardsView view, PanCardView.EventArgs.ItemSwipedEventArgs args)
        {
            int currentIndex = 0;
            if (args.Direction == PanCardView.Enums.ItemSwipeDirection.Right)
            {
                currentIndex = carouseView.SelectedIndex - 1;
            }
            else if (args.Direction == PanCardView.Enums.ItemSwipeDirection.Left)
            {
                currentIndex = carouseView.SelectedIndex + 1;
            }

            int next_index = (currentIndex + 1);
            if (next_index > viewModel.ImageSlideCount)
            {
                next_index = 1;
            }
            else if (next_index < 1)
            {
                next_index = viewModel.ImageSlideCount;
            }
            viewModel.CurrentSlideImageIndex = next_index;
        }

        public async void CarouselView_GotoDetail_Tapped(object sender, EventArgs e)
        {
            Grid grCarouselView = sender as Grid;
            TapGestureRecognizer tap = grCarouselView.GestureRecognizers[0] as TapGestureRecognizer;
            SlideItem item = tap.CommandParameter as SlideItem;
            if (item.Type == 0)
            {
                await Navigation.PushAsync(new ToDayDetailPage(item.Id));
            }
            else if (item.Type == 1)
            {
                await Navigation.PushAsync(new LiquidationDetailPage(item.Id));
            }

        }

        public async void GotoDetailPost_Tapped(object sender, EventArgs e)
        {
            Grid grAds = sender as Grid;
            var tap = grAds.GestureRecognizers[0] as TapGestureRecognizer;
            Advertise item = tap.CommandParameter as Advertise;
            if (item.Type == 0)
            {
                await Shell.Current.Navigation.PushAsync(new PostDetailPage(item.Id));
            }
        }

        private void LoadCategories()
        {
            liquidationCategories = _liquidationCategoryService.GetLiquidations();
            for (int i = 0; i < 7; i++)
            {
                var item = liquidationCategories[i];
                StackLayout stackLayout = GetCategoryLayout(item.Icon, item.Name);
                Grid.SetColumn(stackLayout, i < 4 ? i : i - 4);
                Grid.SetRow(stackLayout, i < 4 ? 0 : 1);

                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.CommandParameter = item.Id;
                tap.Tapped += OnCategoryTapped;
                stackLayout.GestureRecognizers.Add(tap);
                GridHomeCategories.Children.Add(stackLayout);
            }
            AddMoreCaregoriesButton();
        }

        private void AddMoreCaregoriesButton()
        {
            StackLayout stackLayout = GetCategoryLayout("ic_liquidation_more.png", Language.xem_them);
            Grid.SetColumn(stackLayout, 3);
            Grid.SetRow(stackLayout, 1);
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += ShowMoreCategories_Clicked;
            stackLayout.GestureRecognizers.Add(tap);
            GridHomeCategories.Children.Add(stackLayout);
        }

        private StackLayout GetCategoryLayout(string icon, string name)
        {
            StackLayout stackLayout = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Center
            };

            Image image = new Image();
            image.Source = icon;
            image.Style = (Style)Resources["ImageStyle"];
            stackLayout.Children.Add(image);

            Label label = new Label();
            label.Text = name;
            label.Style = (Style)Resources["TitleLabelStyle"];
            stackLayout.Children.Add(label);
            return stackLayout;
        }

        private async void OnCategoryTapped(object sender, EventArgs e)
        {
            var id = (int)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            var category = this.liquidationCategories.Where(x => x.Id == id).SingleOrDefault().Name;
            LiquidationFilterModel liquidationFilterModel = new LiquidationFilterModel();
            liquidationFilterModel.CategoryId = id;
            liquidationFilterModel.Status = 0;
            await Navigation.PushAsync(new ListPage(liquidationFilterModel));
        }

        private void ShowMoreCategories_Clicked(object sender, EventArgs e)
        {
            if (GridMoreCategories.Children.Count == 0) // chua load more.
            {
                int count = liquidationCategories.Count;
                decimal row_count = count / 4;
                if (row_count * 4 < count)
                {
                    row_count += 1;
                }

                for (int i = 0; i < row_count; i++)
                {
                    GridMoreCategories.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(0, GridUnitType.Auto)
                    });
                }

                for (int i = 0; i < count; i++)
                {
                    var item = liquidationCategories[i];
                    StackLayout stackLayout = GetCategoryLayout(item.Icon, item.Name);
                    (stackLayout.Children[1] as Label).MaxLines = 2;
                    if (i < 4)
                    {
                        Grid.SetColumn(stackLayout, i);
                        Grid.SetRow(stackLayout, 0);
                    }
                    else if (i < 8)
                    {
                        Grid.SetColumn(stackLayout, i - 4);
                        Grid.SetRow(stackLayout, 1);
                    }
                    else if (i < 12)
                    {
                        Grid.SetColumn(stackLayout, i - 8);
                        Grid.SetRow(stackLayout, 2);
                    }
                    else if (i < 16)
                    {
                        Grid.SetColumn(stackLayout, i - 12);
                        Grid.SetRow(stackLayout, 3);
                    }
                    else if (i < 20)
                    {
                        Grid.SetColumn(stackLayout, i - 16);
                        Grid.SetRow(stackLayout, 4);
                    }


                    TapGestureRecognizer tap = new TapGestureRecognizer();
                    tap.CommandParameter = item.Id;
                    tap.Tapped += OnCategoryTapped;
                    stackLayout.GestureRecognizers.Add(tap);

                    GridMoreCategories.Children.Add(stackLayout);
                }
            }
            ModalViewMoreCategories.IsVisible = true;
        }

        public async void AddLiquidation_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged)
            {
                await Shell.Current.Navigation.PushAsync(new AddLiquidationPage());
            }
            else
            {
                await DisplayAlert(Language.thong_bao, Language.vui_long_dang_nhap_dang_ky_de_duoc_dang_san_pham, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
            }
        }

        public async void ToDayList_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//ToDayListPage");
            await Shell.Current.Navigation.PopToRootAsync();
        }

        public async void Type0List_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LiquidationListPage");
            await Shell.Current.Navigation.PopToRootAsync();
        }

        public async void OnToDayDetail_Tapped(object sender, EventArgs e)
        {
            Grid stack = sender as Grid;
            TapGestureRecognizer tap = stack.GestureRecognizers[0] as TapGestureRecognizer;
            LiquidationToDay item = tap.CommandParameter as LiquidationToDay;
            await Navigation.PushAsync(new ToDayDetailPage(item.Id));
        }

        public async void OnLiquidationDetail_Tapped(object sender, EventArgs e)
        {
            Grid stack = sender as Grid;
            TapGestureRecognizer tap = stack.GestureRecognizers[0] as TapGestureRecognizer;
            Liquidation item = tap.CommandParameter as Liquidation;
            await Navigation.PushAsync(new LiquidationDetailPage(item.Id));
        }

        private void CloseModalCategories_Clicked(object sender, EventArgs e)
        {
            ModalViewMoreCategories.IsVisible = false;
        }
    }
}
