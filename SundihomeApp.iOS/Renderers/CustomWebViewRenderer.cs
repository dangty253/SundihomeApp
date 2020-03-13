//using System;
//using Foundation;
//using SundihomeApp.Controls;
//using SundihomeApp.iOS.Renderers;
//using WebKit;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebViewRenderer))]
//namespace SundihomeApp.iOS.Renderers
//{
//    public class CustomWebViewRenderer : WkWebViewRenderer, IWKNavigationDelegate
//    {
//        protected override void OnElementChanged(VisualElementChangedEventArgs e)
//        {
//            base.OnElementChanged(e);

//            NavigationDelegate = this;
//        }

//        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
//        public void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
//        {
//            if (navigationAction.TargetFrame == null)
//            {
//                webView.LoadRequest(navigationAction.Request);
//            }

//            decisionHandler.Invoke(WKNavigationActionPolicy.Allow);
//        }
//    }
//}
