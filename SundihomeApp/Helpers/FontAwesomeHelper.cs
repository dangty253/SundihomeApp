using System;
using Xamarin.Forms;

namespace SundihomeApp.Helpers
{
    public class FontAwesomeHelper
    {

        public static string GetFont(string fontName)
        {
            string result = string.Empty;
            switch (fontName)
            {
                case "FontAwesomeSolid":
                    result = Device.RuntimePlatform == Device.iOS ? "FontAwesome5Free-Solid" : "FontAwesome5Solid.otf#Regular";
                    break;
                case "FontAwesomeRegular":
                    result = Device.RuntimePlatform == Device.iOS ? "FontAwesome5Free-Regular" : "FontAwesome5Regular.otf#Regular";
                    break;
                case "FontAwesomeBrands":
                    result = Device.RuntimePlatform == Device.iOS ? "FontAwesome5Brands-Regular" : "FontAwesome5BrandsRegular.otf#Regular";
                    break;
            }
            return result;
        }
    }
}
