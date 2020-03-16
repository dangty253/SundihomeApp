using System;
using System.Text.RegularExpressions;

namespace SundihomeApp.Helpers
{
    public static class Validations
    {

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            else if (password.Length < 6)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return false;
            }

            return Regex.Match(phone, @"^([-0-9]*)$").Success;
        }
    }
}
