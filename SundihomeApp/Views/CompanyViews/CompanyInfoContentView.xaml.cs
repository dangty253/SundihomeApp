using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ImageCircle.Forms.Plugin.Abstractions;
using SundihomeApi.Entities;
using SundihomeApi.Entities.CompanyEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Settings;
using Xamarin.Forms;
using CompanyEntity = SundihomeApi.Entities.Company;
namespace SundihomeApp.Views.CompanyViews
{
    public partial class CompanyInfoContentView : ContentView
    {
        public ObservableCollection<ThanhTuuCongTy> thanhTuuCongTyList = new ObservableCollection<ThanhTuuCongTy>();
        public ObservableCollection<LichSuPhatTrienCongTy> lichSuPhatTrienCongTyList = new ObservableCollection<LichSuPhatTrienCongTy>();
        public ObservableCollection<User> ListDoiNguNhanVien = new ObservableCollection<User>();

        private CompanyEntity _company;
        public CompanyEntity Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged(nameof(Company));
            }
        }
        private int _pageLichSu;
        public int PageLichSu
        {
            get => _pageLichSu;
            set
            {
                _pageLichSu = value;
                OnPropertyChanged(nameof(PageLichSu));
            }
        }
        private int _pageThanhTuu;
        public int PageThanhTuu
        {
            get => _pageThanhTuu;
            set
            {
                _pageThanhTuu = value;
                OnPropertyChanged(nameof(PageThanhTuu));
            }
        }

        private bool _showMoreLichSu;
        public bool ShowMoreLichSu
        {
            get => _showMoreLichSu;
            set
            {
                _showMoreLichSu = value;
                OnPropertyChanged(nameof(ShowMoreLichSu));
            }
        }

        private bool _showMoreThanhTuu;
        public bool ShowMoreThanhTuu
        {
            get => _showMoreThanhTuu;
            set
            {
                _showMoreThanhTuu = value;
                OnPropertyChanged(nameof(ShowMoreThanhTuu));
            }
        }

        public CompanyInfoContentView(CompanyEntity Company)
        {
            InitializeComponent();
            this.BindingContext = this;
            this.Company = Company;
            PageLichSu = 1;
            PageThanhTuu = 1;
            Init();
        }


        public void Init()
        {
            LoadHistoryList();
            LoadThanhTuuList();
            LoadListDoiNguNhanVien();

            ModalPopupDetail.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);
            ModalPopupDetailThanhTuu.BackgroundColor = Color.FromRgba(0, 0, 0, 0.3);

            BindableLayout.SetItemsSource(stHistory, lichSuPhatTrienCongTyList);
            BindableLayout.SetItemsSource(stThanhTuu, thanhTuuCongTyList);
            BindableLayout.SetItemsSource(stListDoiNguNhanVien, ListDoiNguNhanVien);


            //get NganhNghe
            int loaiCongTy = Company.NganhNgheId;
            List<LoaiCongTyModel> listLoaiCongty = LoaiCongTyData.GetListNganhNghe();
            LoaiCongTyModel loaiCongTyModel = listLoaiCongty.Single(x => x.Id == loaiCongTy);
            spLoaiCongTy.Text = loaiCongTyModel.Name;

            var gridChildren = grContent.Children.Where(x => x.IsVisible == true).ToList();
            for (int i = 0; i < gridChildren.Count(); i++)
            {
                grContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(9, GridUnitType.Auto) });

                Grid.SetRow(gridChildren[i], i);

            }
        }

        public async void LoadHistoryList()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<LichSuPhatTrienCongTy>>($"{ApiRouter.COMPANY_GET_LICHSUPHATTRIENCONGTY}/{Company.Id}?page={PageLichSu}", true);
            List<LichSuPhatTrienCongTy> data = (List<LichSuPhatTrienCongTy>)apiResponse.Content;
            if (data.Count == 5) ShowMoreLichSu = true;
            else ShowMoreLichSu = false;

            foreach (var item in data)
            {
                lichSuPhatTrienCongTyList.Add(item);
            }
        }

        public async void LoadThanhTuuList()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<ThanhTuuCongTy>>($"{ApiRouter.COMPANY_GET_THANHTUUCONGTY}/{Company.Id}?page={PageThanhTuu}", true);
            List<ThanhTuuCongTy> data = (List<ThanhTuuCongTy>)apiResponse.Content;
            if (data.Count == 5) ShowMoreThanhTuu = true;
            else ShowMoreThanhTuu = false;
            foreach (var item in data)
            {
                thanhTuuCongTyList.Add(item);
            }

        }
        public async void LoadListDoiNguNhanVien()
        {
            ApiResponse apiResponse = await ApiHelper.Get<List<User>>($"{ApiRouter.COMPANY_GET_NHANVIENUUTUCONGTY}/{Company.Id}");
            List<User> data = (List<User>)apiResponse.Content;
            foreach (var item in data)
            {
                ListDoiNguNhanVien.Add(item);
            }
        }

        public void GoToUserProfile_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PushAsync(new UserProfilePage(this.Company.CreatedBy.Id));
        }
        private void ViewHistoryDetail_Tapped(object sender, EventArgs e)
        {
            var history = ((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as LichSuPhatTrienCongTy;
            lblLichSuDetailTitle.Text = history.Title;
            lblLichSuDetailContent.Text = history.Description;
            ModalPopupDetail.IsVisible = true;
        }
        private async void Employee_Tapped(object sender, EventArgs e)
        {
            User user = ((sender as StackLayout).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as User;
            if (user.Type == 1)
            {
                await Shell.Current.Navigation.PushAsync(new MoiGioiViews.ThongTinMoiGioiPage(user.Id, false));
            }
            else
            {
                await Shell.Current.Navigation.PushAsync(new UserProfilePage(user.Id));
            }
        }
        private void ViewThanhTuuDetail_Tapped(object sender, EventArgs e)
        {
            var thanhTuu = ((sender as Grid).GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as ThanhTuuCongTy;
            lblThanhTuuDetailTitle.Text = thanhTuu.Title;
            lblThanhTuuDetailContent.Text = thanhTuu.Description;
            ModalPopupDetailThanhTuu.IsVisible = true;
        }
        public void ShowMoreLichSu_Clicked(object sender, EventArgs e)
        {
            PageLichSu += 1;
            LoadHistoryList();
        }
        public void ShowMoreThanhTuu_Clicked(object sender, EventArgs e)
        {
            PageThanhTuu += 1;
            LoadThanhTuuList();
        }
        //public void RegisterEmployee_Clicked(object sender, EventArgs e)
        //{
        //    Shell.Current.DisplayAlert("", "dang ky nhan vien", "ok");
        //}
        public void CloseModal_LichSu_Clicked(object sender, EventArgs e)
        {
            ModalPopupDetail.IsVisible = false;
        }
        public void CloseModal_ThanhTuu_Clicked(object sender, EventArgs e)
        {
            ModalPopupDetailThanhTuu.IsVisible = false;
        }
    }
}
