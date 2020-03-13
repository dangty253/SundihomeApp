using System;
using Xamarin.Forms.Maps;

namespace SundihomeApp.Controls
{
    public class CustomPin : Pin
    {
        public string Url { get; set; }
        public string PriceText { get; set; }
        public byte[] PinBytes { get; set; }
        public Guid PostId { get; set; }
        public CustomPin()
        {
        }
    }
}
