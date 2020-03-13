using System;
namespace SundihomeApp.Models
{
    public class ChangePasswordModel
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string Confirm { get; set; }
    }
}
