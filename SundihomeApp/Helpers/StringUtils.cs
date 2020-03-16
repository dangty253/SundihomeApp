using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SundihomeApi.Entities;
using SundihomeApp.Models;
using SundihomeApp.Resources;

namespace SundihomeApp.Helpers
{
    public static class StringUtils
    {
        private static HttpClient _client = BsdHttpClient.Instance();
        private static Random _random = new Random();

        //random OTP
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        //send OTP
        public static async Task SendOTP(string phone, string content)
        {
            var response = await ApiHelper.Get<SendSmsResponse>($"api/auth/sendotp?phone={phone.Replace("+", "%2B")}&content={content}");
            if (response.IsSuccess)
            {
                var otpResponse = (SendSmsResponse)response.Content;
                if (otpResponse.CodeResult == 99)
                {
                    throw new Exception(Language.sdt_khong_hop_le);
                }
                if (otpResponse.CodeResult != 100)
                {
                    throw new Exception(Language.loi_xac_thuc);
                }
            }
        }

        public static string PlaceholderLink(string avatarText)
        {
            return "https://ui-avatars.com/api/?background=0D8ABC&rounded=true&color=fff&size=128&name=" + avatarText;
        }

    }
}
