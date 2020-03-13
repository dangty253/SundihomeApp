using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using SundihomeApp.Controls;
using SundihomeApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BlankEditor), typeof(BlankEditorRenderer))]
namespace SundihomeApp.Droid.Renderers
{
	public class BlankEditorRenderer: EditorRenderer
	{
		public BlankEditorRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;
                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
                shape.Paint.SetStyle(Paint.Style.Stroke);
                nativeEditText.Background = shape;
                nativeEditText.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}
