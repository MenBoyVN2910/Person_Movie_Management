using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers.Adapters
{
    /// <summary>
    /// Adapter cho Twitch — hỗ trợ VODs và Clips.
    /// </summary>
    public class TwitchAdapter : ISiteAdapter
    {
        public string SiteName => "Twitch";
        public string SiteIcon => "🎮";

        private static readonly Regex _vodRegex = new(
            @"twitch\.tv/videos/(\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex _clipRegex = new(
            @"(?:clips\.twitch\.tv/|twitch\.tv/\w+/clip/)([a-zA-Z0-9_-]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool CanHandle(string url)
        {
            return url.Contains("twitch.tv/", StringComparison.OrdinalIgnoreCase);
        }

        public Task<SiteMetadata?> ExtractMetadataAsync(string url, string html)
        {
            var metadata = new SiteMetadata
            {
                SiteName = SiteName,
                SiteIcon = SiteIcon
            };

            // Xác định loại: VOD hay Clip
            var vodMatch = _vodRegex.Match(url);
            if (vodMatch.Success)
            {
                string vodId = vodMatch.Groups[1].Value;
                metadata.EmbedUrl = $"https://player.twitch.tv/?video={vodId}&parent=localhost";
            }
            else
            {
                var clipMatch = _clipRegex.Match(url);
                if (clipMatch.Success)
                {
                    string clipSlug = clipMatch.Groups[1].Value;
                    metadata.EmbedUrl = $"https://clips.twitch.tv/embed?clip={clipSlug}&parent=localhost";
                }
            }

            // Trích xuất từ HTML
            if (!string.IsNullOrWhiteSpace(html))
            {
                var titleMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:title[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (titleMatch.Success)
                    metadata.Title = System.Net.WebUtility.HtmlDecode(titleMatch.Groups[1].Value);

                var imgMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:image[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (imgMatch.Success)
                    metadata.CoverImageUrl = imgMatch.Groups[1].Value;

                var descMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:description[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (descMatch.Success)
                    metadata.Description = System.Net.WebUtility.HtmlDecode(descMatch.Groups[1].Value);
            }

            return Task.FromResult<SiteMetadata?>(metadata);
        }
    }
}
