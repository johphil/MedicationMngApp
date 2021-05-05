using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedicationMngApp.Models
{
    public static class PersistentSettings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static int AccountID
        {
            get => AppSettings.GetValueOrDefault(nameof(AccountID), 0);
            set => AppSettings.AddOrUpdateValue(nameof(AccountID), value);
        }
        public static string UserName
        {
            get => AppSettings.GetValueOrDefault(nameof(UserName), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(UserName), value);
        }
        public static string PassWord
        {
            get => AppSettings.GetValueOrDefault(nameof(PassWord), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(PassWord), value);
        }
    }
}
