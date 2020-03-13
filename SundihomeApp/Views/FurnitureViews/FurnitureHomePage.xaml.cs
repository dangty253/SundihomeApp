using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Furniture;
using SundihomeApp.Configuration;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.Furniture;
using Telerik.XamarinForms.Primitives;
using Xamarin.Forms;

namespace SundihomeApp.Views.Furniture
{
    public partial class FurnitureHomePage : ContentPage
    {
        public HomePageViewModel viewModel;
        public FurnitureHomePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new HomePageViewModel();
            Init();

            MessagingCenter.Subscribe<AddProductPage, bool>(this, "AddProduct", async (sender, isPromotion) =>
            {
                if (isPromotion)
                {
                    await viewModel.LoadPromotionProducts();
                }
                else
                {
                    await viewModel.LoadProducts();
                }
            });


            MessagingCenter.Subscribe<AddProductPage, FurnitureProduct>(this, "UpdateProduct", async (sender, product) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModel.Products.Any(x => x.Id == product.Id))
                {
                    await viewModel.LoadProducts();
                }
                if (viewModel.PromotionProducts.Any(x => x.Id == product.Id))
                {
                    await viewModel.LoadPromotionProducts();
                }
                loadingPopup.IsVisible = false;
            });

            MessagingCenter.Subscribe<ProductDetailPageViewModel, Guid>(this, "DeleteProduct", async (sender, productId) =>
            {
                loadingPopup.IsVisible = true;
                if (viewModel.Products.Any(x => x.Id == productId))
                {
                    await viewModel.LoadProducts();
                }
                if (viewModel.PromotionProducts.Any(x => x.Id == productId))
                {
                    await viewModel.LoadPromotionProducts();
                }
                loadingPopup.IsVisible = false;
            });
            MessagingCenter.Subscribe<PromotionPage>(this, "AddProduct", async (sender) => {
                loadingPopup.IsVisible = true;
                await viewModel.LoadPromotionProducts();
                loadingPopup.IsVisible = false;
            });
        }


        public async void Init()
        {
            await Task.WhenAll(viewModel.LoadProducts(), viewModel.LoadPromotionProducts(), viewModel.LoadCategories(), viewModel.LoadSlideList(), viewModel.LoadAdvertise());
            SetUpSlideImages();
            LoadCategories();
            loadingPopup.IsVisible = false;
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

        public void SetUpSlideImages()
        {
            // set slide image
            carouseView.ItemsSource = viewModel.SlideList;
            SetSlideTimer();
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
            await Navigation.PushAsync(new ProductDetailPage(item.Id));
        }

        private async void OnCategoryTapped(object sender, EventArgs e)
        {
            var id = (Guid)((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter;
            var category = this.viewModel.FurnitureParentCategoryList.Single(x => x.Id == id);
            await Navigation.PushAsync(new ProductListPage(id)
            {
                Title = category.Name
            });
        }

        private async void LoadCategories()
        {
            for (int i = 0; i < 6; i++)
            {
                var item = this.viewModel.FurnitureParentCategoryList[i];
                StackLayout stackLayout = GetCategoryLayout(item.Icon, item.Name);
                Grid.SetColumn(stackLayout, i < 3 ? i : i - 3);
                Grid.SetRow(stackLayout, i < 3 ? 0 : 1);

                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.CommandParameter = item.Id;
                tap.Tapped += OnCategoryTapped;
                stackLayout.GestureRecognizers.Add(tap);
                GridHomeCategories.Children.Add(stackLayout);
            }
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

        public async void OnProduct_Tapped(object sender, EventArgs e)
        {
            var item = sender as Grid;
            var tap = item.GestureRecognizers[0] as TapGestureRecognizer;
            var selectedProduct = tap.CommandParameter as FurnitureProduct;
            await Navigation.PushAsync(new ProductDetailPage(selectedProduct.Id));
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
        private async void ViewAll_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//noithat/productlist");
            await Shell.Current.Navigation.PopToRootAsync();
        }
        private async void ViewAllPromotion_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//noithat/promotionlist");
            await Shell.Current.Navigation.PopToRootAsync();
        }
        public async void AddProduct_Clicked(object sender, EventArgs e)
        {
            if (!UserLogged.IsLogged)
            {
                await DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }
            await Navigation.PushAsync(new AddProductPage() { Title = Language.them_san_pham });
        }
    }
}
