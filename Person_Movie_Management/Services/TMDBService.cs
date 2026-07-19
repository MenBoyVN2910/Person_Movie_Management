using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Services
{
    public class TMDBMovie
    {
        public string Title { get; set; }
        public string Overview { get; set; }
        public string PosterUrl { get; set; }
        public double Rating { get; set; }
        public List<string> Genres { get; set; }
        public string ReleaseDate { get; set; }
    }

    public class TMDBService
    {
        private readonly HttpClient _httpClient;
        
        // This is a free public API key for development/demo purposes provided by TMDB.
        // It's widely used in tutorials. If it fails, user can replace it in appsettings.json.
        private const string DefaultApiKey = "3e28406f5280b39c63b400936e788e0f";

        public TMDBService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        }

        private string GetApiKey()
        {
            string key = AppSettings.Current.TMDBApiKey;
            if (string.IsNullOrWhiteSpace(key))
            {
                return DefaultApiKey;
            }
            return key;
        }

        public async Task<List<TMDBMovie>> SearchMoviesAsync(string query)
        {
            var results = new List<TMDBMovie>();
            if (string.IsNullOrWhiteSpace(query)) return results;

            string apiKey = GetApiKey();
            string url = $"search/movie?api_key={apiKey}&query={Uri.EscapeDataString(query)}&language=vi-VN";

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    
                    var resultsArray = doc.RootElement.GetProperty("results");
                    foreach (var item in resultsArray.EnumerateArray())
                    {
                        var movie = new TMDBMovie();
                        movie.Title = item.GetProperty("title").GetString() ?? "";
                        movie.Overview = item.GetProperty("overview").GetString() ?? "";
                        
                        if (item.TryGetProperty("poster_path", out var posterProp) && posterProp.ValueKind != JsonValueKind.Null)
                        {
                            movie.PosterUrl = $"https://image.tmdb.org/t/p/w500{posterProp.GetString()}";
                        }
                        
                        if (item.TryGetProperty("vote_average", out var voteProp))
                        {
                            // Convert TMDB 10-point scale to 5-star scale
                            double vote = voteProp.GetDouble();
                            movie.Rating = Math.Round(vote / 2.0, 1);
                        }
                        
                        if (item.TryGetProperty("release_date", out var releaseProp))
                        {
                            movie.ReleaseDate = releaseProp.GetString();
                        }
                        
                        movie.Genres = new List<string>();
                        if (item.TryGetProperty("genre_ids", out var genreIdsProp))
                        {
                            foreach (var idElem in genreIdsProp.EnumerateArray())
                            {
                                int id = idElem.GetInt32();
                                string genreName = GetGenreName(id);
                                if (!string.IsNullOrEmpty(genreName))
                                {
                                    movie.Genres.Add(genreName);
                                }
                            }
                        }
                        
                        results.Add(movie);
                    }
                }
            }
            catch (Exception)
            {
                // Handle or log exception
            }
            return results;
        }

        private string GetGenreName(int id)
        {
            // Simple hardcoded map for TMDB movie genres (vi-VN)
            var genres = new Dictionary<int, string>
            {
                { 28, "Hành động" },
                { 12, "Phiêu lưu" },
                { 16, "Hoạt hình" },
                { 35, "Hài hước" },
                { 80, "Tội phạm" },
                { 99, "Tài liệu" },
                { 18, "Chính kịch" },
                { 10751, "Gia đình" },
                { 14, "Viễn tưởng" },
                { 36, "Lịch sử" },
                { 27, "Kinh dị" },
                { 10402, "Nhạc" },
                { 9648, "Bí ẩn" },
                { 10749, "Lãng mạn" },
                { 878, "Khoa học viễn tưởng" },
                { 10770, "Chương trình truyền hình" },
                { 53, "Giật gân" },
                { 10752, "Chiến tranh" },
                { 37, "Miền Tây" }
            };

            if (genres.TryGetValue(id, out string name))
                return name;
            
            return "";
        }
    }
}
