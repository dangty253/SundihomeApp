using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApi.Entities.Response;
using SundihomeApp.Configuration;
using SundihomeApp.Controls;
using SundihomeApp.Helpers;
using SundihomeApp.Models;
using SundihomeApp.Resources;
using SundihomeApp.ViewModels;
using Xamarin.Forms;

namespace SundihomeApp.Views
{
    public partial class ModalDiaryContentView : ContentView
    {
        public event EventHandler OnCancel;
        public event EventHandler OnSaved;


        public ContentViewModalDiaryViewModel viewModel;
        public string OldImages;
        public Guid? _diaryId;
        public Guid _projectId;
        public ModalDiaryContentView(Guid? diaryId, Guid projectId)
        {
            InitializeComponent();
            _diaryId = diaryId;
            _projectId = projectId;
            Init();
        }
        public void Init()
        {
            this.BindingContext = viewModel = new ContentViewModalDiaryViewModel();
            if (_diaryId.HasValue)
            {
                UpdateDiary();
            }
            loadingPopup.IsVisible = false;
        }
        //project diary
        public async void UpdateDiary()
        {
            var apireponse = await ApiHelper.Get<ProjectDiary>($"{ApiRouter.PROJECT_DIARY_GET_ONE_PROJECTDIARY}/{_diaryId}");
            if (!apireponse.IsSuccess) return;
            var model = apireponse.Content as ProjectDiary;
            viewModel.ProjectDiary = model;
            OldImages = model.Image;
            //set hinh anh cho ffimageloading
            if (string.IsNullOrEmpty(model.Image) == false)
            {
                string[] imageList = model.Image.Split(',');
                foreach (var image in imageList)
                {
                    viewModel.MediaDiary.Add(new MediaFile()
                    {
                        PreviewPath = ImageHelper.GetImageUrl("project/diary", image),
                    });
                }
            }
        }
        // luu nhat ky
        public async void AddProjectDiary_Clicked(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(entTitleProjectDiary.Text))
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_nhap_tieu_de_nhat_ky, Language.dong);
                return;
            }
            if (string.IsNullOrWhiteSpace(edtModalDescriptionDiary.Text))
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_nhap_noi_dung_nhat_ky, Language.dong);
                return;
            }
            loadingPopup.IsVisible = true;
            // set image va avatar
            MultipartFormDataContent form = new MultipartFormDataContent();
            string[] imageList = new string[viewModel.MediaDiary.Count];
            if (imageList.Length > 8)
            {
                await Shell.Current.DisplayAlert(Language.thong_bao, Language.vui_long_upload_toi_da_8_hinh_anh_bat_dong_san, Language.dong);
                loadingPopup.IsVisible = false;
                return;
            }
            if (imageList.Count() != 0)
            {

                for (int i = 0; i < viewModel.MediaDiary.Count; i++)
                {
                    var item = viewModel.MediaDiary[i];
                    // chua upload. upload roi link = null 
                    if (string.IsNullOrEmpty(item.Path) == false) // co link la co chon tu dien thoai.
                    {
                        imageList[i] = $"{Guid.NewGuid().ToString()}.jpg";
                        var stream = new MemoryStream(File.ReadAllBytes(item.Path));
                        var content = new StreamContent(stream);
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "files" + i,
                            FileName = imageList[i]
                        };
                        form.Add(content);
                    }
                    else
                    {
                        imageList[i] = item.PreviewPath.Replace(Configuration.ApiConfig.CloudStorageApiCDN + "/project/diary/", "");
                    }
                }
                viewModel.ProjectDiary.Image = string.Join(",", imageList);
            }
            else
            {
                viewModel.ProjectDiary.Image = null;
            }


            bool ImageUploaded = true;

            if (viewModel.MediaDiary.Any(x => x.Path != null))
            {
                ApiResponse uploadImageResponse = await UploadImageDiary(form);
                if (!uploadImageResponse.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("", Language.hinh_anh_vuot_qua_dung_luong_vui_long_thu_lai, Language.dong);
                    ImageUploaded = false;
                }
            }
            else
            {
                ImageUploaded = true; // ko can upload
            }
            if (ImageUploaded)
            {
                if (viewModel.ProjectDiary.Id == Guid.Empty)
                {
                    viewModel.ProjectDiary.Id = Guid.NewGuid();
                    viewModel.ProjectDiary.ProjectId = _projectId;
                    ApiResponse apiResponse = await ApiHelper.Post(ApiRouter.PROJECT_DIARY_ADD_PROJECTDIARY, viewModel.ProjectDiary, true);
                    if (apiResponse.IsSuccess)
                    {
                        OnSaved?.Invoke(this, EventArgs.Empty);
                        loadingPopup.IsVisible = false;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert(Language.thong_bao, apiResponse.Message, Language.dong);
                    }
                }
                else
                {
                    ApiResponse apiResponse = await ApiHelper.Put($"{ApiRouter.PROJECT_DIARY_UPDATE}", viewModel.ProjectDiary, true);
                    if (apiResponse.IsSuccess)
                    {
                        OnSaved?.Invoke(this, EventArgs.Empty);
                        if (!string.IsNullOrWhiteSpace(OldImages)&& OldImages != viewModel.ProjectDiary.Image && viewModel.ProjectDiary.Image != null)
                        {
                            string[] arrOldImages = OldImages.Split(',');
                            List<string> ImagesToDelete = new List<string>();
                            for (int i = 0; i < arrOldImages.Length; i++)
                            {
                                if (!imageList.Any(x => x == arrOldImages[i]))
                                {
                                    ImagesToDelete.Add(arrOldImages[i]);
                                }
                            }

                            await ApiHelper.Delete(ApiRouter.DELETE_IMAGE + "?bucketName=sundihome/project/diary&files=" + string.Join(",", ImagesToDelete.ToArray()));
                        }
                        loadingPopup.IsVisible = false;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert(Language.thong_bao, apiResponse.Message, Language.dong);
                    }
                }
            }
            loadingPopup.IsVisible = false;
        }
        public async Task<ApiResponse> UploadImageDiary(MultipartFormDataContent form)
        {
            var client = BsdHttpClient.Instance();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage uploadResponse = await client.PostAsync(ApiConfig.CloudStorageApi + "/api/files/upload?folder=project/diary", form);
            string body = await uploadResponse.Content.ReadAsStringAsync();
            ApiResponse uploadResonse = JsonConvert.DeserializeObject<ApiResponse>(body);
            return uploadResonse;
        }
        
        //button xoa hinh nhat ky
        public void Remove_ImageDiary(object sender, EventArgs e)
        {
            var file = (sender as Button).CommandParameter as MediaFile;
            viewModel.MediaDiary.Remove(file);
        }

        public void DeleteContentModalPopup_Clicked(object sender, EventArgs e)
        {
            edtModalDescriptionDiary.Text = null;
        }
        public void CloseModalDiary_Clicked(object sender, EventArgs e)
        {
            OnCancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
