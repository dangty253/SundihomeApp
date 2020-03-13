using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Text;
using Android.Util;
using Android.Views;
using Java.IO;
using Java.Net;
using SundihomeApp.Controls;
using SundihomeApp.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace SundihomeApp.Droid.Renderers
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<CustomPin> customPins;
        int index = 0;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                //NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            //NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }
        public void tst()
        {
        }
        protected override MarkerOptions CreateMarker(Pin pin)
        {

            var marker = new MarkerOptions();
            var latLong = new LatLng(pin.Position.Latitude, pin.Position.Longitude);

            //var customPin = this.customPins.Where(x => x.Position == new Position(latLong.Latitude, latLong.Longitude)).SingleOrDefault();
            var customPin = this.customPins[index];
            index++;

            marker.SetPosition(latLong);
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromBitmap(GetBitmap(customPin.PriceText)));
            return marker;
        }

        public Bitmap GetBitmap(string text)
        {


            Paint paint = new Paint(PaintFlags.AntiAlias);
            paint.TextSize = 15 * Context.Resources.DisplayMetrics.Density;
            paint.Color = Android.Graphics.Color.Black;
            paint.TextAlign = Paint.Align.Left;
            paint.StrokeWidth = 3;
            Rect bounds = new Rect();
            paint.GetTextBounds(text, 0, text.Length, bounds);

            float baseline = -paint.Ascent(); // ascent() is negative
            int width = (int)(paint.MeasureText(text) + 0.5f) + 30; // round
            int height = (int)(baseline + paint.Descent() + 0.5f) + 15;
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

            Canvas canvas = new Canvas(bitmap);
            canvas.DrawColor(Android.Graphics.Color.White);
            canvas.DrawText(text, 15, baseline + 7, paint);

            #region border
            Paint strokePaint = new Paint();
            strokePaint.SetStyle(Paint.Style.Stroke);
            strokePaint.Color = Android.Graphics.Color.Gray;
            strokePaint.StrokeWidth = 4;
            RectF r = new RectF(0, 0, width, height);
            canvas.DrawRect(r, strokePaint);
            #endregion

            return doHighlightImage(bitmap);
        }

        public static Bitmap doHighlightImage(Bitmap src)
        {
            Bitmap bmOut = Bitmap.CreateBitmap(src.Width + 96, src.Width + 96, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bmOut);
            canvas.DrawColor(Android.Graphics.Color.Red, PorterDuff.Mode.Clear);
            Paint ptBlur = new Paint();
            ptBlur.SetMaskFilter(new BlurMaskFilter(15, BlurMaskFilter.Blur.Normal));
            int[] offsetXY = new int[2];
            Bitmap bmAlpha = src.ExtractAlpha(ptBlur, offsetXY);
            Paint ptAlphaColor = new Paint();
            ptAlphaColor.Color = Android.Graphics.Color.Black;
            canvas.DrawBitmap(bmAlpha, offsetXY[0], offsetXY[1], ptAlphaColor);
            bmAlpha.Recycle();
            canvas.DrawBitmap(src, 0, 0, null);
            return bmOut;
        }
        public Android.Views.View GetInfoContents(Marker marker)
        {
            //var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            //if (inflater != null)
            //{
            //    Android.Views.View view;

            //    var customPin = GetCustomPin(marker);
            //    if (customPin == null)
            //    {
            //        throw new Exception("Custom pin not found");
            //    }

            //    if (customPin.MarkerId.ToString() == "Xamarin")
            //    {
            //        view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
            //    }
            //    else
            //    {
            //        view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
            //    }

            //    var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
            //    var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

            //    if (infoTitle != null)
            //    {
            //        infoTitle.Text = marker.Title;
            //    }
            //    if (infoSubtitle != null)
            //    {
            //        infoSubtitle.Text = marker.Snippet;
            //    }

            //    return view;
            //}
            //return null;
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }
    }
}
