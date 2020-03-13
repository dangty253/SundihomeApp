using System;
using System.ComponentModel;
using System.Linq;
using Foundation;
using SundihomeApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(SearchBar), typeof(ExtendedSearchBarRenderer))]
namespace SundihomeApp.iOS.Renderers
{
    public class ExtendedSearchBarRenderer
        : SearchBarRenderer
    {
        //protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> args)
        //{
        //    base.OnElementChanged(args);
        //    UISearchBar bar = (UISearchBar)this.Control;
        //    bar.TintColor = UIColor.Black;
        //}

        //// hide cancel button
        //protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);

        //    //if (e.PropertyName == "Text")
        //    //{
        //    //    Control.ShowsCancelButton = false;
        //    //}
        //}
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            //UISearchBar bar = (UISearchBar)this.Control;
            //bar.TintColor = UIColor.Black;

            //!!! Works, but only for the first search bar (we have 3) **
            UITextField.AppearanceWhenContainedIn(typeof(UISearchBar)).BackgroundColor =
            UIColor.White;

            //Match text field within SearchBar to its background color
            //using (var searchKey = new NSString("_searchField"))
            //{
            //    if (e.NewElement == null) return;

            //    //!!! Throws an iOS error on iOS 13 ***
            //    //var textField = (UITextField)Control.ValueForKey(searchKey);
            //    //textField.BackgroundColor = e.NewElement.BackgroundColor.ToUIColor();
            //    //textField.TintColor = UIColor.White;
            //}
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // Hide Cancel Button
            if (e.PropertyName == "Text") Control.ShowsCancelButton = false;
        }
    }
}
