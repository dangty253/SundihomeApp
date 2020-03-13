using System;
using System.Collections.Generic;
using System.Linq;
using SundihomeApi.Entities;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using SundihomeApp.ViewModels.CompanyViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views.CompanyViews.QuanLyCongTyViews
{
    public partial class FilterCompanyPostListView : ContentView
    {
        public FilterCompanyPostListViewModel viewModel;
        public FilterCompanyPostListView(bool? IsCommitment, int? Status)
        {
            InitializeComponent();

            this.BindingContext = viewModel = new FilterCompanyPostListViewModel();
            viewModel.CompanyId = Guid.Parse(UserLogged.CompanyId);
            viewModel.IsCommitment = IsCommitment;
            viewModel.Status = Status;

            Init();
        }
        public async void Init()
        {
            DataListView.ItemTapped += DataListView_ItemTapped;

            MessagingCenter.Subscribe<PostPage>(this, "OnSavePost", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });

            MessagingCenter.Subscribe<FilterCompanyPostListView>(this, "OnUpdateStatus", async (sender) =>
            {
                this.viewModel.RefreshCommand.Execute(null);
            });

            MessagingCenter.Subscribe<PostPage, Guid>(this, "OnDeleteSuccess", async (arg1, postId) =>
            {
                var post = viewModel.Data.SingleOrDefault(x => x.Id == postId);
                if (post != null)
                    viewModel.Data.Remove(post);
            });

            await viewModel.LoadData();
            loadingPopup.IsVisible = false;
        }

        private async void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var post = e.Item as Post;
            if (UserLogged.RoleId != 0)
            {
                await Shell.Current.Navigation.PushAsync(new PostDetailPage(post.Id));
                return;
            }

            IDictionary<int, string> options = new Dictionary<int, string>()
            {
                {1,Language.xem_thong_tin_chi_tiet },
            };

            if (UserLogged.RoleId == 0 && viewModel.Status != 0)
            {
                options[2] = Language.xoa;
            }

            if (viewModel.Status.HasValue && viewModel.Status == 0)
            {
                options[3] = Language.duyet;
                options[4] = Language.khong_duyet;
            }
            //if (viewModel.Status.HasValue && viewModel.Status == 1)
            //{
            //    options[5] = Language.dua_vao_gio_chung;
            //}

            string action = await Shell.Current.DisplayActionSheet("", Language.huy, null, options.Values.ToArray());
            if (action == options[1])
            {
                await Shell.Current.Navigation.PushAsync(new PostDetailPage(post.Id));
            }
            else if (options.ContainsKey(2) && action == options[2])
            {
                var response = await ApiHelper.Delete(ApiRouter.COMPANY_REMOVOUT_POST + "/" + post.Id);
                if (response.IsSuccess)
                {
                    MessagingCenter.Send<FilterCompanyPostListView>(this, "OnUpdateStatus");
                    ToastMessageHelper.ShortMessage(Language.xoa_thanh_cong);
                }
                else
                {
                    await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                }
            }
            else if (options.ContainsKey(3) && action == options[3])
            {
                var response = await ApiHelper.Put(ApiRouter.COMPANY_APPROVE_POST + "/" + post.Id, null, false);
                if (response.IsSuccess)
                {
                    MessagingCenter.Send<FilterCompanyPostListView>(this, "OnUpdateStatus");
                    ToastMessageHelper.ShortMessage(Language.duyet_thanh_cong);
                }
                else
                {
                    await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                }
            }
            else if (options.ContainsKey(4) && action == options[4])
            {
                var response = await ApiHelper.Put(ApiRouter.COMPANY_REJECT_POST + "/" + post.Id, null, false);
                if (response.IsSuccess)
                {
                    MessagingCenter.Send<FilterCompanyPostListView>(this, "OnUpdateStatus");
                    ToastMessageHelper.ShortMessage(Language.thanh_cong);
                }
                else
                {
                    await Shell.Current.DisplayAlert("", Language.loi_he_thong_vui_long_thu_lai, Language.dong);
                }
            }
            //else if (options.ContainsKey(5) && action == options[5])
            //{
            //    var response = await ApiHelper.Put(ApiRouter.COMPANY_ADDTO_GIOCHUNG + "/" + post.Id, null, false);
            //    if (response.IsSuccess)
            //    {
            //        MessagingCenter.Send<FilterCompanyPostListView>(this, "OnUpdateStatus");
            //        ToastMessageHelper.ShortMessage(Language.dua_vao_gio_chung_thanh_cong);
            //    }
            //    else
            //    {
            //        await Shell.Current.DisplayAlert("", response.Message, Language.dong);
            //    }
            //}
        }
        public void Search_Pressed(object sender, EventArgs e)
        {
            viewModel.Keyword = searchBar.Text;
            viewModel.RefreshCommand.Execute(null);
        }
        public void Search_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBar.Text) && !string.IsNullOrWhiteSpace(viewModel.Keyword))
            {
                viewModel.Keyword = null;
                viewModel.RefreshCommand.Execute(null);
            }
        }
    }
}
