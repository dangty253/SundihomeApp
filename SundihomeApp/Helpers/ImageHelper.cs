using System;
using Xamarin.Forms;

namespace SundihomeApp.Helpers
{
    public class ImageHelper
    {
        public static string GetImageUrl(string bucketName, string imageUrl)
        {
            //if (Device.RuntimePlatform == Device.Android)
            //{
            //    //return "http://6501660-s3user.cdn.cloudstorage.com.vn/sundihome/" + bucketName + "/" + imageUrl;
            //}
            return Configuration.ApiConfig.CloudStorageApiCDN + "/" + bucketName + "/" + imageUrl;
        }
    }
}
