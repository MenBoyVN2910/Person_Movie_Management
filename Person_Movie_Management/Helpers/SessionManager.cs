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
        }

        public static void UpdateCurrentUser(User user)
        {
            CurrentUser = user;
        }
    }
}
