using System;

using Xamarin.Forms;

namespace SundihomeApp.Helpers
{
    public class AvatarHelper
    {
        public static string GetUserAvatar(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            if (url.StartsWith("avatar/", StringComparison.OrdinalIgnoreCase))
                return Configuration.ApiConfig.CloudStorageApiCDN + "/" + url;
            return url;

        }

        public static string GetPostAvatar(string avatar)
        {
            return Configuration.ApiConfig.CloudStorageApiCDN + "/post/" + avatar;
        }
        public static string GetProjectAvatar(string avatar)
        {
            return Configuration.ApiConfig.CloudStorageApiCDN + "/project/" + avatar;
        }
    }
}

