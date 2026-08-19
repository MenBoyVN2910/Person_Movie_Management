using System;
using System.IO;
using System.Text.Json;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Models
{
    public class AppSettings
    {
        public UIHelper.ThemeMode Theme { get; set; } = UIHelper.ThemeMode.Dark;
        public string TMDBApiKey { get; set; } = ""; // Get free key at themoviedb.org

        private static readonly string SettingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        private static AppSettings _current = new();

        public static AppSettings Current
        {
            get
            {
                return _current;
            }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string json = File.ReadAllText(SettingsFile);
                    _current = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }
            
            if (_current == null) _current = new AppSettings();
            
            _current.Theme = UIHelper.ThemeMode.Dark; // Force dark theme since light theme is removed
            UIHelper.ApplyTheme(_current.Theme);
        }

        public static void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(_current, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }
    }
}
