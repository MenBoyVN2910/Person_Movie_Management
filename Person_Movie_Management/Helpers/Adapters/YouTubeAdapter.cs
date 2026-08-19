using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers.Adapters
{
    /// <summary>
    /// Adapter cho YouTube — trích xuất videoId để lấy thumbnail chất lượng cao
    /// và embed URL trực tiếp không cần scrape HTML.
    /// </summary>
    public class YouTubeAdapter : ISiteAdapter
    {
        public string SiteName => "YouTube";
        public string SiteIcon => "▶️";

        // Regex khớp các dạng URL YouTube phổ biến
        private static readonly Regex _urlRegex = new(
            @"(?:youtube\.com/(?:watch\?.*v=|embed/|shorts/|live/)|youtu\.be/)([a-zA-Z0-9_-]{11})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool CanHandle(string url)
        {
            return _urlRegex.IsMatch(url);
        }

        public Task<SiteMetadata?> ExtractMetadataAsync(string url, string html)
        {
            var match = _urlRegex.Match(url);
            if (!match.Success) return Task.FromResult<SiteMetadata?>(null);

            string videoId = match.Groups[1].Value;

            var metadata = new SiteMetadata
            {
                SiteName = SiteName,
                SiteIcon = SiteIcon,
                EmbedUrl = $"https://www.youtube.com/embed/{videoId}",
                // YouTube thumbnail URLs — maxresdefault là chất lượng cao nhất
                CoverImageUrl = $"https://i.ytimg.com/vi/{videoId}/maxresdefault.jpg",
                ExtraImageUrls =
                {
                    $"https://i.ytimg.com/vi/{videoId}/hqdefault.jpg",
                    $"https://i.ytimg.com/vi/{videoId}/sddefault.jpg"
                }
            };

            // Cố gắng lấy title từ HTML nếu có (đã được scrape sẵn)
            if (!string.IsNullOrWhiteSpace(html))
            {
                // Tìm title trong og:title hoặc <title>
                var titleMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:title[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (titleMatch.Success)
                {
                    metadata.Title = System.Net.WebUtility.HtmlDecode(titleMatch.Groups[1].Value);
                }
                else
                {
                    var htmlTitleMatch = Regex.Match(html, @"<title[^>]*>([^<]+)</title>", RegexOptions.IgnoreCase);
                    if (htmlTitleMatch.Success)
                    {
                        string title = System.Net.WebUtility.HtmlDecode(htmlTitleMatch.Groups[1].Value);
                        // Loại bỏ " - YouTube" suffix
                        title = Regex.Replace(title, @"\s*-\s*YouTube\s*$", "", RegexOptions.IgnoreCase).Trim();
                        metadata.Title = title;
                    }
                }

                // Tìm description
                var descMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:description[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (descMatch.Success)
                {
                    metadata.Description = System.Net.WebUtility.HtmlDecode(descMatch.Groups[1].Value);
                }
            }

            return Task.FromResult<SiteMetadata?>(metadata);
        }
    }
}
