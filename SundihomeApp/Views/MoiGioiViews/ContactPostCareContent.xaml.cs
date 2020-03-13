using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SundihomeApp.Views.MoiGioiViews
{
    public partial class ContactPostCareContent : ContentView
    {
        private Guid _contactId;
        private bool _isCompany;
        public string Keyword { get; set; }
        private ListViewPageViewModel2<SundihomeApi.Entities.Post> viewModel;
        private ListViewPageViewModel2<SundihomeApi.Entities.Post> searchPageResultViewModel;
        public ContactPostCareContent(Guid contactId, bool isCompany)
        {
            InitializeComponent();
            _contactId = contactId;
            _isCompany = isCompany;
            this.BindingContext = viewModel = new ListViewPageViewModel2<SundihomeApi.Entities.Post>();
            radSegmentedControl.ItemsSource = new string[] { Language.cua_toi, Language.cong_ty };
            if (_isCompany) {
                radSegmentedControl.IsVisible = false;
                radSegmentedControl.SelectedIndex = 1;
            }
            if (string.IsNullOrEmpty(UserLogged.CompanyId))
            {
                radSegmentedControl.IsVisible = false;
            }

            viewModel.PreLoadData = new Command(() =>
            {
                viewModel.ApiUrl = $"{ApiRouter.CONTACT_GET_POSTCARES}/{contactId}?page={viewModel.Page}";
            });
            Init();
        }

        public async void Init()
        {
            await viewModel.LoadData();
            DataListView.ItemTapped += DataListView_ItemTapped;

            loadingPopup.IsVisible = false;
        }
        private async void PickerPost_Clicked(object sender, EventArgs e)
        {
            loadingPopup.IsVisible = true;

            if (searchPageResultViewModel == null) // chua bat popup lan nao.
            {
                this.ListView0.ItemTapped += ListView0_ItemTapped;
                searchPageResultViewModel = new ListViewPageViewModel2<SundihomeApi.Entities.Post>();



                this.ListView0.BindingContext = searchPageResultViewModel;
            }
            else
            {
                Keyword = null;
                ModalPopupSearchBar.Text = null;
            }

            loadingPopup.IsVisible = false;
            LoaiHinh_Change(null, EventArgs.Empty);
            await ModalPickPost.Show();

        }



        private async void LoaiHinh_Change(object sender, EventArgs e)
        {
            if (searchPageResultViewModel == null) return;
            var index = radSegmentedControl.SelectedIndex;
            if (index == 0)
            {
                searchPageResultViewModel.PreLoadData = new Command(() =>
                {
                    searchPageResultViewModel.ApiUrl = ApiRouter.EMPLOYEE_GETMYPOSTLIST + $"?Keyword={Keyword}&page={searchPageResultViewModel.Page}";
                });
            }
            else
            {
                if (searchPageResultViewModel == null) // chua bat popup lan nao.
                {
                    this.ListView0.ItemTapped += ListView0_ItemTapped;
                    searchPageResultViewModel = new ListViewPageViewModel2<SundihomeApi.Entities.Post>();
                    this.ListView0.BindingContext = searchPageResultViewModel;
                }
                searchPageResultViewModel.PreLoadData = new Command(() =>
                {
                    if (_isCompany)
                    {
                        searchPageResultViewModel.ApiUrl = ApiRouter.COMPANY_GETPOSTLIST + $"?CompanyId={UserLogged.CompanyId}&Keyword={Keyword}&page={searchPageResultViewModel.Page}&Status=-1";
                    }
                    else
                        searchPageResultViewModel.ApiUrl = ApiRouter.COMPANY_GETPOSTLIST + $"?CompanyId={UserLogged.CompanyId}&Keyword={Keyword}&page={searchPageResultViewModel.Page}&Status=2";
                });
            }
            await searchPageResultViewModel.LoadOnRefreshCommandAsync();
        }

        public async void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as SundihomeApi.Entities.Post;
            var action = await Shell.Current.DisplayActionSheet(Language.tuy_chon, Language.huy, null, Language.xem_thong_tin_chi_tiet, Language.xoa);
            if (action == Language.xem_thong_tin_chi_tiet)
            {
                await Shell.Current.Navigation.PushAsync(new PostDetailPage(post.Id));
            }
            else if (action == Language.xoa)
            {
                var createResponse = await ApiHelper.Delete(ApiRouter.CONTACT_DELETE_POSTCARES + $"/{this._contactId.ToString()}/{post.Id.ToString()}", true);
                if (createResponse.IsSuccess)
                {
                    viewModel.Data.Remove(post);
                }
                else await Shell.Current.DisplayAlert(Language.thong_bao, Language.xoa_bds_quan_tam_that_bai, Language.dong);
            }
        }
        private async void ListView0_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            ContactPostCare care = new ContactPostCare()
            {
                ContactId = _contactId,
                PostId = post.Id
            };

            var createResponse = await ApiHelper.Post(ApiRouter.CONTACT_POST_POSTCARES, care, true);

            if (createResponse.IsSuccess)
            {
                // goi api tren day

                await viewModel.LoadOnRefreshCommandAsync();
                await ModalPickPost.Hide();
            }
            else
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, createResponse.Message, Language.dong);
            }
        }


        public void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // huy search va dieu kien saerch hien tai khac "" hoac emtpy thi moi chay lai. 
            if (string.IsNullOrWhiteSpace(e.NewTextValue) && !string.IsNullOrWhiteSpace(Keyword))
            {
                Search_Clicked(sender, EventArgs.Empty);
            }
        }

        private void Search_Clicked(object sender, EventArgs e)
        {
            Keyword = ModalPopupSearchBar.Text ?? "";
            searchPageResultViewModel.RefreshCommand.Execute(null);
        }
        

        
    }
}
