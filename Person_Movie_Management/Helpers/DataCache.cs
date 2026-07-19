using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Person_Movie_Management.Models;
using Person_Movie_Management.Services;

namespace Person_Movie_Management.Helpers
{
    /// <summary>
    /// In-Memory Cache giúp truy xuất dữ liệu trên RAM với độ trễ micro-giây, bỏ qua Disk I/O.
    /// </summary>
    public static class DataCache
    {
        private static List<Movie> _cachedMovies = null;
        private static List<Audio> _cachedAudios = null;
        
        public static event Action DataInvalidated;

        public static async Task<List<Movie>> GetMoviesAsync(int userId, bool forceRefresh = false)
        {
            if (_cachedMovies == null || forceRefresh)
            {
                _cachedMovies = await AppServices.MovieRepo.GetAllByUserAsync(userId);
            }
            return _cachedMovies;
        }

        public static async Task<List<Movie>> GetFavoriteMoviesAsync(int userId, bool forceRefresh = false)
        {
            var movies = await GetMoviesAsync(userId, forceRefresh);
            return movies.Where(m => m.IsFavorite).ToList();
        }

        public static async Task<List<Audio>> GetFavoriteAudiosAsync(int userId, bool forceRefresh = false)
        {
            if (_cachedAudios == null || forceRefresh)
            {
                _cachedAudios = await AppServices.AudioRepo.GetFavoritesAsync(userId);
            }
            return _cachedAudios; 
        }
        
        public static void Invalidate()
        {
            _cachedMovies = null;
            _cachedAudios = null;
            DataInvalidated?.Invoke();
        }
    }
}
