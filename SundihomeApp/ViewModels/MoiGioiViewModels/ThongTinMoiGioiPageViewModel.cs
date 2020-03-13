using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Share;
using SundihomeApi.Entities;
using SundihomeApi.Entities.MoiGioiEntities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.Settings;
using Xamarin.Forms;

namespace SundihomeApp.ViewModels.MoiGioiViewModels
{
    public class ThongTinMoiGioiPageViewModel : ListViewPageViewModel2<Post>
    {
        public ObservableCollection<FloatButtonItem> ButtonCommandList { get; set; } = new ObservableCollection<FloatButtonItem>();

        private MoiGioi _moiGioi;
        public MoiGioi MoiGioi
        {
            get => _moiGioi;
            set
            {
                _moiGioi = value;
                OnPropertyChanged(nameof(MoiGioi));
            }
        }

        private bool _isMoiGioi;
        public bool IsMoiGioi
        {
            get => _isMoiGioi;
            set
            {
                _isMoiGioi = value;
                OnPropertyChanged(nameof(IsMoiGioi));
            }
        }

        private string _genderFormatString;
        public string GenderFormatString
        {
            get => _genderFormatString;
            set
            {
                _genderFormatString = value;
                OnPropertyChanged(nameof(GenderFormatString));
            }
        }

        private string _typeFormatString;
        public string TypeFormatString
        {
            get => _typeFormatString;
            set
            {
                _typeFormatString = value;
                OnPropertyChanged(nameof(TypeFormatString));
            }
        }

        public Command OnShareDataCommand { get; set; }

        private int _followerCount;
        public int FollowerCount
        {
            get => _followerCount;
            set
            {
                _followerCount = value;
                OnPropertyChanged(nameof(FollowerCount));
            }
        }

        private bool _isFollow;
        public bool IsFollow
        {
            get => _isFollow;
            set
            {
                _isFollow = value;
                OnPropertyChanged(nameof(IsFollow));
            }
        }

        private bool _isUnFollow;
        public bool IsUnFollow
        {
            get => _isUnFollow;
            set
            {
                _isUnFollow = value;
                OnPropertyChanged(nameof(IsUnFollow));
            }
        }

        public ThongTinMoiGioiPageViewModel(Guid moiGioiId)
        {
            MoiGioi = new MoiGioi();

            PreLoadData = new Command(() =>
            {
                FilterModel filterModel = new FilterModel();
                filterModel.CreatedById = moiGioiId;
                string json = JsonConvert.SerializeObject(filterModel);
                ApiUrl = $"api/post/filter?json={json}&page={Page}";
            });

            string url = ApiConfig.WEB_IP + $"moigioi/information/{moiGioiId}";
            OnShareDataCommand = new Command(() => Share(url));
            ButtonCommandList.Add(new FloatButtonItem(Language.chia_se, FontAwesomeHelper.GetFont("FontAwesomeSolid"), "\uf1e0", OnShareDataCommand, null));
        }

        public async Task LoadThongTinMoiGioi(Guid moiGioiId)
        {
            ApiResponse response = await ApiHelper.Get<MoiGioi>($"{ApiRouter.MOIGIOI_GETBYID}/{moiGioiId}");
            if (response.IsSuccess)
            {
                MoiGioi = response.Content as MoiGioi;
                GetGenderFormatString(MoiGioi.User.Sex);
                GetTypeFormatString(MoiGioi.Type);
            }
        }

        public void GetGenderFormatString(int? gender)
        {
            if (gender.HasValue)
            {
                switch (gender)
                {
                    case 0:
                        GenderFormatString = Language.nam;
                        break;
                    case 1:
                        GenderFormatString = Language.nu;
                        break;
                    case 2:
                        GenderFormatString = Language.khac;
                        break;
                    default:
                        GenderFormatString = null;
                        break;
                }
            }
        }

        public void GetTypeFormatString(int? type)
        {
            if (type.HasValue)
            {
                Option option = LoaiMoiGioiData.GetById(type.Value);
                if (option != null) TypeFormatString = option.Name;
            }
            else
            {
                TypeFormatString = "";
            }
        }

        public void Share(string url)
        {
            CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage { Url = url });
        }

        //lay ds nguoi theo doi user
        public async Task GetFollowers(Guid userId)
        {
            var response = await ApiHelper.Get<List<Guid>>($"api/following/follower/{userId}");
            bool isFollow = false;
            if (response.IsSuccess)
            {
                if (response.Content != null)
                {
                    List<Guid> listId = response.Content as List<Guid>;
                    FollowerCount = listId.Count;
                    if (UserLogged.IsLogged && listId.Contains(Guid.Parse(UserLogged.Id)))
                    {
                        isFollow = true;
                    }
                }
                else
                    FollowerCount = 0;
            }
            IsFollow = isFollow;
            IsUnFollow = !IsFollow;
        }

        public async void Follow(Guid userId)
        {
            if (!UserLogged.IsLogged)
            {
                await Shell.Current.DisplayAlert("", Language.vui_long_dang_nhap, Language.dong);
                ((AppShell)Shell.Current).SetLoginPageActive();
                return;
            }

            ApiResponse response = await ApiHelper.Put($"api/following/follow/{userId}", null, true);
            if (response.IsSuccess)
            {
                await GetFollowers(userId);
                MessagingCenter.Send<ThongTinMoiGioiPageViewModel, Guid>(this, "UpdateFollowing", userId);
            }
        }
    }
}
