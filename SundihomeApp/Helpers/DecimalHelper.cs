using System;
using System.Globalization;

namespace SundihomeApp.Helpers
{
    public class DecimalHelper
    {

        public static string DecimalToText(decimal? dec, int NumberDecimalDigits = 2)
        {
            if (dec.HasValue)
            {
                NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;
                nfi.NumberDecimalDigits = NumberDecimalDigits;
                string text = dec.Value.ToString("N", nfi);

                // 3 truong hop sau so 0.
                if (text.EndsWith(",0", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Replace(",0", "");
                }
                else if (text.EndsWith(",00", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Replace(",00", "");
                }
                else if (text.EndsWith(",000", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Replace(",000", "");
                }
                else if (text.EndsWith(",0000", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Replace(",0000", "");
                }

                if (text.Contains(",") && text.EndsWith("0000", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Substring(0, text.Length - 4);
                }

                if (text.Contains(",") && text.EndsWith("000", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Substring(0, text.Length - 3);
                }

                if (text.Contains(",") && text.EndsWith("00", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Substring(0, text.Length - 2);
                }

                if (text.Contains(",") && text.EndsWith("0", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Substring(0, text.Length - 1);
                }
                return text;
            }
            else
            {
                return "";
            }
        }
        public static decimal? TextToDecimal(string text, int NumberDecimalDigits = 2)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;
            nfi.NumberDecimalDigits = NumberDecimalDigits;
            return decimal.Parse(text, nfi);
        }

        public static string ToCurrency(decimal dec)
        {
            dec = Math.Round(dec, 0, MidpointRounding.AwayFromZero);
            NumberFormatInfo nfi = new CultureInfo("vi-VN", false).NumberFormat;
            nfi.NumberDecimalDigits = 0;
            string text = dec.ToString("N", nfi);
            return text;
        }
    }
}
