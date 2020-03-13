using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace SundihomeApp.Settings
{
    public class LanguageSettings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string Language
        {
            get => AppSettings.GetValueOrDefault(nameof(Language), "vi");
            set
            {
                AppSettings.AddOrUpdateValue(nameof(Language), value);
            }
        }
    }
}
