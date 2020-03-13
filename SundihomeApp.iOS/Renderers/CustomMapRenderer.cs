using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MapKit;
using SundihomeApp.Controls;
using SundihomeApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace SundihomeApp.iOS.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        UIView customPinView;
        List<CustomPin> customPins;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                nativeMap.GetViewForAnnotation = null;
                nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                customPins = formsMap.CustomPins;

                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }
        public UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
                return UIImage.LoadFromData(data);
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {

            MKAnnotationView annotationView = null;

            if (annotation is MKUserLocation)
                return null;

            var customPin = GetCustomPin(annotation as MKPointAnnotation);
            if (customPin == null)
            {
                return null;
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.MarkerId.ToString());
            if (annotationView == null)
            {

                annotationView = new CustomMKAnnotationView(annotation, customPin.MarkerId.ToString());

                var data = NSData.FromArray(customPin.PinBytes);
                annotationView.Image = UIImage.LoadFromData(data);
                annotationView.CalloutOffset = new CGPoint(0, 0);
                annotationView.LeftCalloutAccessoryView = new UIImageView();
                annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                ((CustomMKAnnotationView)annotationView).MarkerId = customPin.MarkerId.ToString();
                //((CustomMKAnnotationView)annotationView).Url = customPin.Url;
            }
            annotationView.CanShowCallout = true;

            return annotationView;
        }

        async void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var annotation = e.View.Annotation;
            var customPin = GetCustomPin(annotation as MKPointAnnotation);

            await Xamarin.Forms.Shell.Current.Navigation.PushAsync(new SundihomeApp.Views.PostDetailPage(customPin.PostId));
            //var customView = e.View as CustomPin;
            //var customPin = GetCustomPin(annotation as MKPointAnnotation);
            //if (!string.IsNullOrWhiteSpace(customView.Url))
            //{
            //    UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
            //}
        }

        async void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            //var annotation = e.View.Annotation;
            //var customPin = GetCustomPin(annotation as MKPointAnnotation);

            //await Xamarin.Forms.Shell.Current.Navigation.PushAsync(new SundihomeApp.Views.PostDetailPage(customPin.PostId));
            //var customView = e.View as CustomMKAnnotationView;
            //customPinView = new UIView();

            //if (customView.MarkerId == "Xamarin")
            //{
            //    customPinView.Frame = new CGRect(0, 0, 200, 84);
            //    var image = new UIImageView(new CGRect(0, 0, 200, 84));
            //    image.Image = UIImage.FromFile("xamarin.png");
            //    customPinView.AddSubview(image);
            //    customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
            //    e.View.AddSubview(customPinView);
            //}
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            //if (!e.View.Selected)
            //{
            //    customPinView.RemoveFromSuperview();
            //    customPinView.Dispose();
            //    customPinView = null;
            //}
        }

        CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            try
            {
                var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
                foreach (var pin in customPins)
                {
                    if (pin.Position == position)
                    {
                        return pin;
                    }
                }
            }
            catch { }
            return null;
        }
    }

    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string MarkerId { get; set; }

        public string Url { get; set; }

        public CustomMKAnnotationView(IMKAnnotation annotation, string id)
            : base(annotation, id)
        {
        }
    }
}
