﻿using System;

using Xamarin.Forms;

namespace SundihomeApp.Models
{
    public enum MediaFileType
    {
        Image,
        Video
    }

    public class MediaFile
    {
        public string PreviewPath { get; set; }
        public string Path { get; set; }
        public MediaFileType Type { get; set; }
    }
}

