using System;
using Android.Content;
using SundihomeApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Shell), typeof(ShellRendererCustomDispose))]
namespace SundihomeApp.Droid.Renderers
{
    public class ShellRendererCustomDispose : ShellRenderer
    {
        bool _disposed;

        public ShellRendererCustomDispose(Context context)
            : base(context)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Element.PropertyChanged -= OnElementPropertyChanged;
                Element.SizeChanged -= (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), this, "OnElementSizeChanged"); // OnElementSizeChanged is private, so use reflection
            }

            _disposed = true;
        }
    }
}
