using System;
using SundihomeApp.Controls;
using SundihomeApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BlankEntry), typeof(BlankEntryRenderer))]
namespace SundihomeApp.iOS.Renderers
{
    public class BlankEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // do whatever you want to the UITextField here!
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}
