using System;
using CoreGraphics;
using SundihomeApp.iOS.Renderers;
using SundihomeApp.ViewModels;
using SundihomeApp.Views;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PostPage), typeof(PostPageRenderer))]
namespace SundihomeApp.iOS.Renderers
{
    public class PostPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //SetCustomBackButton();

        }

        public override void ViewDidAppear(bool animated)
        {
            // khong cho phep swipe de pop ve trang truoc
            base.ViewDidAppear(animated);
            var navctrl = this.ViewController.NavigationController;
            navctrl.InteractivePopGestureRecognizer.Enabled = false;
        }


        //private void SetCustomBackButton()
        //{
        //    var page = Element as PostPage;
        //    // NOTE: this doesn't look exactly right, you need to create an image to replicate the back arrow properly
        //    UIBarButtonItem btn = new UIBarButtonItem("Huỷ", UIBarButtonItemStyle.Plain, async (sender, args) =>
        //    {
        //        var navPage = Shell.Current.Navigation;
        //        var vm = page.BindingContext as PostPageViewModel;

        //        if (vm != null)
        //        {
        //            bool answer = await vm.OnBackButtonPressed();

        //            if (!answer)
        //                await navPage.PopAsync();
        //        }
        //        else
        //        {
        //            await navPage.PopAsync();
        //        }
        //    });
        //    this.NavigationController.TopViewController.NavigationItem.SetLeftBarButtonItem(btn, true);
        //}
    }
}
