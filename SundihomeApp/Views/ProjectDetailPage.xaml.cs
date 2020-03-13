using FFImageLoading.Forms;
using Stormlion.PhotoBrowser;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SundihomeApp.Views
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPageViewModel viewModel;
        public Guid _id;
        public string Api_Link = ApiConfig.CloudStorageApiCDN + "/project/";
        public string Api_LinkProjectDiary = ApiConfig.CloudStorageApiCDN + "/project/diary/";
        public List<Photo> GetPhotos = new List<Photo>();
        public ObservableCollection<Photo> GetPhotosDiary = new ObservableCollection<Photo>();
        private PhotoBrowser PhotoBrowser;
        public ProjectDetailPage(Guid Id)
        {
            _id = Id;
            InitializeComponent();
            this.BindingContext = viewModel = new ProjectDetailPageViewModel();
            Init();
            Default();
            PhotoBrowser = new PhotoBrowser()
            {
                EnableGrid = true
            };
        }

        private async void Init()
        {
            await viewModel.LoadProject(_id);
            await viewModel.LoadPostProjectID(_id);
            await viewModel.GetListProjectDiary(_id);

            if (viewModel.DuAn.ImageUtilities != null)
            {
                string[] utilitiesArr = viewModel.DuAn.ImageUtilities.Split(',');
                Option[] utilitiesImg = new Option[utilitiesArr.Length];
                List<Option> tienichduan = BDSUtilities.GetListUtilities();
                for (int i = 0; i < utilitiesArr.Length; i++)
                {
                    var item = utilitiesArr[i];
                    Option model = tienichduan.Where(x => x.Id == short.Parse(item)).SingleOrDefault();
                    utilitiesImg[i] = model;
                }
                flowListView.FlowItemsSource = utilitiesImg;
                stTienIchDuAn.IsVisible = true;
            }
            if (viewModel.DuAn.ImageList != null)
            {
                string[] ListImages = viewModel.DuAn.ImageList;
                for (int i = 0; i < ListImages.Length; i++)
                {
                    Photo photo = new Photo();
                    photo.URL = Api_Link + ListImages[i];
                    GetPhotos.Add(photo);
                }
                lbNumImages.Text = ListImages.Count().ToString();
                carouseView.ItemsSource = GetPhotos;
            }

            if (viewModel.DuAn.CategoriBDS != null)
            {
                string[] categoriArr = viewModel.DuAn.CategoriBDS.Split(',');
                string[] categoryNameArr = new string[categoriArr.Length];

                //projectType tren app la categoriBDS tren database
                List<ProjectTypeModel> loaiDuAn = ProjectTypeData.GetListProjectType();
                for (int i = 0; i < categoriArr.Length; i++)
                {
                    var item = categoriArr[i];
                    ProjectTypeModel model = loaiDuAn.Where(x => x.Id == short.Parse(item)).SingleOrDefault();
                    categoryNameArr[i] = model.Name;
                }
                lbCategoriBDS.Text = string.Join(". ", categoryNameArr);

                //lay "Status"
                if (viewModel.DuAn.Status.HasValue)
                {
                    int idStatus = viewModel.DuAn.Status.Value;
                    List<ProjectStatusModel> listStatus = ProjectStatusData.GetList();
                    ProjectStatusModel modelStatus = listStatus.Single(x => x.Id == idStatus);
                    lbStatus.Text = modelStatus.Name;
                    if (viewModel.DuAn.Status.Value == 0)
                    {
                        stThoiGianMoBan.IsVisible = true;
                        stThoiGianBanGiao.IsVisible = true;
                    }

                    if (viewModel.DuAn.Status.Value == 1)
                    {
                        stThoiGianBanGiao.IsVisible = true;
                    }
                }

            }

            await ChekcIsFollowPost();
            loadingPopup.IsVisible = false;

            stNamKhoiCong.IsVisible = true;
            stNamHoanThanh.IsVisible = true;

            // load data "Tong quan du an"
            lbDienTichCayXanh.Text = viewModel.DuAn.DienTichCayXanh;
            lbDienTichKhuDat.Text = viewModel.DuAn.DienTichKhuDat;
            lbDienTichSanTrungBinh.Text = viewModel.DuAn.DienTichSanTrungBinh;
            lbDienTichXayDung.Text = viewModel.DuAn.DienTichXayDung;
            lbNamKhoiCong.Text = viewModel.DuAn.NamKhoiCong.ToString();
            lbNamHoanThanh.Text = viewModel.DuAn.NamHoanThanh.ToString();
            lbMatDoXayDung.Text = viewModel.DuAn.MatDoXayDungPercent.ToString();
            lbSoLuongSanPham.Text = viewModel.DuAn.SoLuongSanPham;
            lbSoTang.Text = viewModel.DuAn.SoTang.ToString();
            lbSoLuongToaNha.Text = viewModel.DuAn.SoLuongToaNha.ToString();
            lbSoThangMay.Text = viewModel.DuAn.SoThangMay.ToString();
            lbTongDienTichSan.Text = viewModel.DuAn.TongDienTichSan;
            lbTongSoVongDauTu.Text = viewModel.DuAn.TongVonDauTu;
            lbTangHam.Text = viewModel.DuAn.TangHam.ToString();
            lbThoiGianMoBan.Text = viewModel.DuAn.ThoiGianMoBan.ToString();
            lbThoiGianBanGiao.Text = viewModel.DuAn.ThoiGianBanGiao.ToString();
            lblChuDauTu.Text = viewModel.DuAn.ChuDauTu;
            lblDonViThietKeThiCong.Text = viewModel.DuAn.DonViThietKeThiCong;
            lblPhanKhuDuAn.Text = viewModel.DuAn.PhanKhuDuAn;

            var gridChildren = grContent.Children.Where(x => x.IsVisible == true).ToList();
            for (int i = 0; i < gridChildren.Count(); i++)
            {
                grContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(9, GridUnitType.Auto) });
                Grid.SetRow(gridChildren[i], i);
            }

        }
        protected void OnClickedShowGallery(object sender, EventArgs e)
        {
            var img = sender as Image;
            string url = (img.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;
            var image = GetPhotos.Where(x => x.URL == url).SingleOrDefault();
            var index = GetPhotos.IndexOf(image);
            //new PhotoBrowser
            //{
            //    Photos = GetPhotos,
            //    EnableGrid = true,
            //    StartIndex = index,
            //}.Show();
            PhotoBrowser.Photos = GetPhotos;
            PhotoBrowser.StartIndex = index;
            PhotoBrowser.Show();

        }
        void Default()
        {
            HideShow_TQ.IsVisible = false;
            HideShow_Utility.IsVisible = false;
            Contact.IsVisible = false;
            HideShow_TDA.IsVisible = false;
            TabMore1.IsVisible = true;
            TabMore2.IsVisible = false;
            UnderlineBtnBuysell.BackgroundColor = Color.FromHex("#026294");
            UnderlineBtnRent.BackgroundColor = Color.FromHex("#fff");
            btnBuySell.TextColor = Color.FromHex("#000");
            btnRent.TextColor = Color.FromHex("#8e8e8e");
            lbAbout.IsVisible = true;
            showmore_Description.RelRotateTo(-180);
        }
        private async Task HideButton()
        {
            if (UserLogged.IsLogged == false) return;
        }
        public void onClick_BtnBuy_Sell(object sender, EventArgs e)
        {
            TabMore1.IsVisible = true;
            TabMore2.IsVisible = false;
            UnderlineBtnBuysell.BackgroundColor = Color.FromHex("#026294");
            UnderlineBtnRent.BackgroundColor = Color.FromHex("#fff");
            btnRent.TextColor = Color.FromHex("#8e8e8e");
            btnBuySell.TextColor = Color.FromHex("#000");
        }
        public void onClick_BtnRent(object sender, EventArgs e)
        {
            TabMore1.IsVisible = false;
            TabMore2.IsVisible = true;
            UnderlineBtnBuysell.BackgroundColor = Color.FromHex("#fff");
            UnderlineBtnRent.BackgroundColor = Color.FromHex("#026294");
            btnBuySell.TextColor = Color.FromHex("#8e8e8e");
            btnRent.TextColor = Color.FromHex("#000");
        }
        public void onClick_LogoChuDautu(object sender, EventArgs e)
        {
            DisplayAlert("thong bao", "logo chu dau tu", "ok");
        }
        public async void onClick_GoToPost(object sender, EventArgs e)
        {
            TapGestureRecognizer click;
            if (sender is Grid)
            {
                var grid = sender as Grid;
                click = grid.GestureRecognizers[0] as TapGestureRecognizer;
            }
            else if (sender is StackLayout)
            {
                var stack = sender as StackLayout;
                click = stack.GestureRecognizers[0] as TapGestureRecognizer;
            }
            else
            {
                click = null;
                return;
            }
            Guid id = (Guid)click.CommandParameter;
            await Shell.Current.Navigation.PushAsync(new PostDetailPage(id));
        }

        public async void onClick_MoreProducts(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PushAsync(new SearchResultPage(new FilterModel()
            {
                ProjectId = _id
            }));
        }
        public void onClick_Call(object sender, EventArgs e)
        {
            DisplayAlert("Homedy", "Goi", "OK");
        }
        public void onClick_Message(object sender, EventArgs e)
        {
            DisplayAlert("Homedy", "Message", "OK");
        }
        public async void onClick_HideShow_Description(object sender, EventArgs e)
        {
            if (lbAbout.IsVisible == true)
            {
                lbAbout.IsVisible = false;
                await showmore_Description.RelRotateTo(180);
            }
            else
            {
                lbAbout.IsVisible = true;
                await showmore_Description.RelRotateTo(-180);
            }
        }
        public async void onClick_HideShow_TQ(object sender, EventArgs e)
        {
            if (HideShow_TQ.IsVisible == false)
            {

                HideShow_TQ.IsVisible = true;
                await showmore_TQ.RelRotateTo(180);
            }
            else
            {
                HideShow_TQ.IsVisible = false;
                await showmore_TQ.RelRotateTo(-180);
            }

        }
        public async void onClick_HideShow_NhatKyDuAn(object sender, EventArgs e)
        {

            if (stListNhatKyDuAn.IsVisible == false)
            {
                stListNhatKyDuAn.IsVisible = true;
                await showmore_NhatKyDuAn.RelRotateTo(180);
            }
            else
            {
                stListNhatKyDuAn.IsVisible = false;
                await showmore_NhatKyDuAn.RelRotateTo(-180);
            }
        }
        public async void onClick_HideShow_TDA(object sender, EventArgs e)
        {
            if (HideShow_TDA.IsVisible == false)
            {
                HideShow_TDA.IsVisible = true;
                await showmore_TDA.RelRotateTo(180);
            }
            else
            {
                HideShow_TDA.IsVisible = false;
                await showmore_TDA.RelRotateTo(-180);
            }
        }
        public async void onClick_HideShow_Utility(object sender, EventArgs e)
        {
            if (HideShow_Utility.IsVisible == false)
            {
                HideShow_Utility.IsVisible = true;
                await showmore_Utility.RelRotateTo(180);
            }
            else
            {
                HideShow_Utility.IsVisible = false;
                await showmore_Utility.RelRotateTo(-180);
            }
        }
        public async void onClick_HideShow_Contact(object sender, EventArgs e)
        {
            if (Contact.IsVisible == false)
            {
                Contact.IsVisible = true;
                await showmore_Contact.RelRotateTo(180);
            }
            else
            {
                Contact.IsVisible = false;
                await showmore_Contact.RelRotateTo(-180);
            }
        }
        public async Task ChekcIsFollowPost()
        {
            if (UserLogged.IsLogged)
            {
                var response = await ApiHelper.Get<object>("api/post/IsFollowPost/" + viewModel.DuAn.Id, true);
                if (response.IsSuccess)
                {
                    bool follow = (bool)response.Content;
                    if (follow)
                    {
                        BtnFollowPost.TextColor = Color.Red;
                    }
                    else
                    {
                        BtnFollowPost.TextColor = Color.Gray;
                    }
                }
            }
        }

        public async void FollowPost_Clicked(object sender, EventArgs e)
        {
            if (UserLogged.IsLogged == false)
            {
                await DisplayAlert("", Language.vui_long_dang_nha_dang_ky_de_thuc_hien_chuc_nang_nay, Language.dong);
                return;
            }
            if (BtnFollowPost.TextColor != Color.Red)
            {
                var response = await ApiHelper.Post("api/post/AddToFavoritePosts/" + this.viewModel.DuAn.Id, null, true);
                if (response.IsSuccess)
                {
                    BtnFollowPost.TextColor = Color.Red;
                }
            }
            else
            {
                var response = await ApiHelper.Post("api/post/RemoveFromFavoritePosts/" + this.viewModel.DuAn.Id, null, true);
                if (response.IsSuccess)
                {
                    BtnFollowPost.TextColor = Color.Gray;
                }
            }
        }

        private async void ViewProfile_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserProfilePage(viewModel.DuAn.CreatedBy.Id));
        }

        private async void GoToDetailDiary_Tapped(object sender, EventArgs e)
        {
            StackLayout stack = sender as StackLayout;
            TapGestureRecognizer item = stack.GestureRecognizers[0] as TapGestureRecognizer;
            Guid id = (Guid)item.CommandParameter;
            await viewModel.GetProjectDiary(id);
            lblTitle.Text = viewModel.ProjectDiary.Title;
            lblDate.Text = viewModel.ProjectDiary.Date.ToShortDateString();
            lblDescription.Text = viewModel.ProjectDiary.Description;

            GetPhotosDiary.Clear();
            if (!string.IsNullOrWhiteSpace(viewModel.ProjectDiary.Image))
            {
                string[] listImgDiary = viewModel.ProjectDiary.Image.Split(',');
                for (int i = 0; i < listImgDiary.Length; i++)
                {
                    Photo photo = new Photo();
                    photo.URL = Api_LinkProjectDiary + listImgDiary[i];
                    GetPhotosDiary.Add(photo);
                }
            }
            BindableLayout.SetItemsSource(stMediaDiary, GetPhotosDiary);

            await ModalDetailProjectDiary.Show();
        }
        public void ShowImageDiary_Tapped(object sender, EventArgs e)
        {

            var img = sender as CachedImage;
            string url = (img.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as string;

            List<Photo> PhotoDiary = new List<Photo>();
            foreach (var item in GetPhotosDiary)
            {
                PhotoDiary.Add(item);
            }
            var image = PhotoDiary.Where(x => x.URL == url).SingleOrDefault();
            var index = PhotoDiary.IndexOf(image);

            PhotoBrowser.Photos = PhotoDiary;
            PhotoBrowser.StartIndex = index;
            PhotoBrowser.Show();
        }
        public async void XemThemNhatKy(object sender, EventArgs e)
        {
            viewModel.Page += 1;
            await viewModel.GetProjectDiary(_id);
        }
    }


}