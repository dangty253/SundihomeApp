using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Mongodb;
using SundihomeApi.Entities.Mongodb.Furniture;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.IServices;
using SundihomeApp.IServices.IFurniture;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Services;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels
{
    public class PostItemListPageViewModel : ListViewPageViewModel2<PostItem>
    {
        public ObservableCollection<Province> ProvinceList { get; set; } = new ObservableCollection<Province>();
        public ObservableCollection<District> DistrictList { get; set; } = new ObservableCollection<District>();
        public ObservableCollection<Ward> WardList { get; set; } = new ObservableCollection<Ward>();

        public bool ShowClearFilterButton => this.Province != null || this.District != null || this.Ward != null;

        private Province _province;
        public Province Province { get => _province; set { _province = value; OnPropertyChanged(nameof(Province)); OnPropertyChanged(nameof(ShowClearFilterButton)); } }

        private District _district;
        public District District { get => _district; set { _district = value; OnPropertyChanged(nameof(District)); OnPropertyChanged(nameof(ShowClearFilterButton)); } }

        private Ward _ward;
        public Ward Ward { get => _ward; set { _ward = value; OnPropertyChanged(nameof(Ward)); OnPropertyChanged(nameof(ShowClearFilterButton)); } }


        public string Keyword { get; set; }
        public int Type { get; set; } = -1;
        public PostItemListPageViewModel()
        {
            IUserService userService = DependencyService.Get<IUserService>();
            OnMapItem = new Command<PostItem>((post) =>
            {
                post.CreatedBy = userService.Find(post.CreatedById);
                switch (post.Type)
                {
                    case 0:
                        post.TypeTextFormat = Language.can_ban;
                        post.TypeColor = "#4F9A4D";
                        break;
                    case 1:
                        post.TypeTextFormat = Language.cho_thue;
                        post.TypeColor = "#418ACC";
                        break;
                    case 2:
                        post.TypeTextFormat = Language.can_mua;
                        post.TypeColor = "#5ac8ed";
                        break;
                    case 3:
                        post.TypeTextFormat = Language.can_thue;
                        post.TypeColor = "#4EA1BD";
                        break;
                    default:
                        post.TypeTextFormat = "";
                        break;
                }
                if (post.HasLoaiBatDongSan)
                {
                    LoaiBatDongSanModel loaiBatDongSan = null;
                    if (post.BDSTypeId.HasValue)
                    {
                        loaiBatDongSan = LoaiBatDongSanModel.GetById(post.BDSTypeId.Value);
                        if (loaiBatDongSan != null)
                        {
                            post.BDSType = loaiBatDongSan.Name;
                        }
                    }
                }
                if (post.UserFollows != null && post.UserFollows.Any(x => x == UserLogged.Id))
                {
                    post.IsFollow = true;
                }
            });
            PreLoadData = new Command(() =>
            {
                string url = $"api/postitems?page={Page}&type={Type}";
                if (!string.IsNullOrWhiteSpace(this.Keyword))
                {
                    url += $"&keyword={ this.Keyword}";
                }
                if (Province != null)
                {
                    url += $"&provinceId={Province.Id}";
                    if (District != null)
                    {
                        url += $"&districtId={District.Id}";
                        if (Ward != null)
                        {
                            url += $"&wardId={Ward.Id}";
                        }
                    }
                }
                ApiUrl = url;
            });
        }

        public async Task<bool> Follow(string PostId)
        {
            IPostItemService postItemService = DependencyService.Get<IPostItemService>();
            var isFollow = await postItemService.Follow(PostId, UserLogged.Id);
            var currentPost = this.Data.Where(x => x.Id == PostId).SingleOrDefault();
            if (currentPost != null)
            {
                currentPost.IsFollow = isFollow;
            }
            return isFollow;
        }

        public async Task LoadProvinceAsync()
        {
            ProvinceList.Clear();
            ApiResponse apiResponse = await ApiHelper.Get<List<Province>>("api/provinces", false, false);
            if (apiResponse.IsSuccess)
            {
                ProvinceList.Add(new Province() { Id = -1, Name = Language.tat_ca, Sort = -1 });
                List<Province> data = (List<Province>)apiResponse.Content;
                foreach (var item in data)
                {
                    ProvinceList.Add(item);
                }
            }
        }



        public async Task LoadDistrictAsync()
        {
            DistrictList.Clear();
            District = null;
            if (Province != null)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<District>>($"api/districts/{Province.Id}", false, false);
                if (apiResponse.IsSuccess)
                {
                    DistrictList.Add(new District() { Id = -1, Name = Language.tat_ca, Pre = null, ProvinceId = -1 });
                    List<District> data = (List<District>)apiResponse.Content;
                    foreach (var item in data)
                    {
                        DistrictList.Add(item);
                    }
                }
            }
        }

        public async Task LoadWardAsync()
        {
            WardList.Clear();
            Ward = null;
            if (District != null)
            {
                ApiResponse apiResponse = await ApiHelper.Get<List<Ward>>($"api/wards/{District.Id}", false, false);
                if (apiResponse.IsSuccess)
                {
                    WardList.Add(new Ward() { Id = -1, Name = Language.tat_ca });
                    List<Ward> data = (List<Ward>)apiResponse.Content;
                    foreach (var item in data)
                    {
                        WardList.Add(item);
                    }
                }
            }
        }

        public void Share(string Id)
        {
            string url = ApiConfig.WEB_IP + $"postitem/{Id}";
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }
    }
}
