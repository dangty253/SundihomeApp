using System;
namespace SundihomeApp.Models
{
    public class ZaloUser
    {
        public string birthday { get; set; }
        public string gender { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public ZaloPicture picture { get; set; }
    }

    public class PictureData
    {
        public string url { get; set; }
    }

    public class ZaloPicture
    {
        public PictureData data { get; set; }
    }
}
