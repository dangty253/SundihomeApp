using System;
namespace SundihomeApp.Models
{
    public class SendOTPResponse
    {
        public string CodeResult { get; set; }
        public int CountRegenerate { get; set; }
        public string ErrorMessage { get; set; }
    }
}
