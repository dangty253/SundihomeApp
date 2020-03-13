using Android.Content.PM;
using SundihomeApp.Droid.Services;
using SundihomeApp.IServices;
using Xamarin.Forms;
[assembly: Dependency(typeof(VersionAndBuild_Android))]
namespace SundihomeApp.Droid.Services
{
    public class VersionAndBuild_Android : IAppVersionAndBuild
    {
        PackageInfo _appInfo;
        public VersionAndBuild_Android()
        {
            var context = Android.App.Application.Context;
            _appInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
        }
        public string GetVersionNumber()
        {
            return _appInfo.VersionName;
        }
        public string GetBuildNumber()
        {
            return _appInfo.VersionCode.ToString();
        }
    }
}
