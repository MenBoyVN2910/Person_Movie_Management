using Person_Movie_Management.Models;

namespace Person_Movie_Management.Helpers
{
    public static class SessionManager
    {
        public static User? CurrentUser { get; private set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void Login(User user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
            DataCache.Invalidate();
            ImageCache.Clear();
        }

        public static void UpdateCurrentUser(User user)
        {
            CurrentUser = user;
        }

        public static bool IsDropWidgetEnabled
        {
            get
            {
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "widget_pref.txt");
                if (System.IO.File.Exists(path))
                {
                    return System.IO.File.ReadAllText(path).Trim() == "1";
                }
                return true; // Default is true
            }
            set
            {
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "widget_pref.txt");
                System.IO.File.WriteAllText(path, value ? "1" : "0");
            }
        }

        public static bool IsFolderWatcherEnabled
        {
            get
            {
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "folder_watcher_pref.txt");
                if (System.IO.File.Exists(path))
                {
                    return System.IO.File.ReadAllText(path).Trim() == "1";
                }
                return true; // Default is true
            }
            set
            {
                string path = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "folder_watcher_pref.txt");
                System.IO.File.WriteAllText(path, value ? "1" : "0");
            }
        }
    }
}
