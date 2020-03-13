using System;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SundihomeApp.Resources;
using Xamarin.Forms;
namespace SundihomeApp.Helpers
{
    public class PermissionHelper
    {
        public static async Task<PermissionStatus> CheckPermissions(Permission permission, string title, string Description)
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            bool request = false;
            if (permissionStatus == PermissionStatus.Denied)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var positive = "Cài đặt";
                    var negative = "Để sau";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, Description, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }

                    return permissionStatus;
                }

                request = true;

            }

            if (request || permissionStatus != PermissionStatus.Granted)
            {
                var newStatus = await CrossPermissions.Current.RequestPermissionsAsync(permission);

                if (!newStatus.ContainsKey(permission))
                {
                    return permissionStatus;
                }

                permissionStatus = newStatus[permission];

                if (newStatus[permission] != PermissionStatus.Granted)
                {
                    permissionStatus = newStatus[permission];
                    //var question = $"To use the plugin the {permission} permission is required.";
                    var positive = "Cài đăt";
                    var negative = "Để sau";
                    var task = Application.Current?.MainPage?.DisplayAlert(title, Description, positive, negative);
                    if (task == null)
                        return permissionStatus;

                    var result = await task;
                    if (result)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                    }
                    return permissionStatus;
                }
            }

            return permissionStatus;
        }

        public static async Task<PermissionStatus> CheckPermission(Permission permission, string title, string des)
        {
            PermissionStatus cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

            if (cameraStatus != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await Shell.Current.DisplayAlert(title, des, Language.dong_y);
                }
                if (Device.RuntimePlatform == Device.iOS)
                {
                    cameraStatus = await CheckPermissions(Permission.Camera, title, des);
                }
                else
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    cameraStatus = results[Permission.Camera];
                }
            }
            return (await CrossPermissions.Current.RequestPermissionsAsync(permission))[permission];
        }
    }
}
