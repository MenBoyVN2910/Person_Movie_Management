using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers.Adapters
{
    /// <summary>
    /// Adapter cho Dailymotion — dùng oEmbed API để lấy metadata chính xác.
    /// </summary>
    public class DailymotionAdapter : ISiteAdapter
    {
        public string SiteName => "Dailymotion";
        public string SiteIcon => "📺";

        private static readonly Regex _urlRegex = new(
            @"dailymotion\.com/video/([a-zA-Z0-9]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool CanHandle(string url)
        {
            return url.Contains("dailymotion.com/video/", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<SiteMetadata?> ExtractMetadataAsync(string url, string html)
        {
            var match = _urlRegex.Match(url);
            if (!match.Success) return null;

            string videoId = match.Groups[1].Value;

            var metadata = new SiteMetadata
            {
                SiteName = SiteName,
                SiteIcon = SiteIcon,
                EmbedUrl = $"https://www.dailymotion.com/embed/video/{videoId}"
            };

            // Gọi oEmbed API
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                string oembedUrl = $"https://www.dailymotion.com/services/oembed?url={Uri.EscapeDataString(url)}&format=json";
                string json = await client.GetStringAsync(oembedUrl);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("title", out var titleProp))
                    metadata.Title = titleProp.GetString();
                if (root.TryGetProperty("thumbnail_url", out var thumbProp))
                    metadata.CoverImageUrl = thumbProp.GetString();
                if (root.TryGetProperty("description", out var descProp))
                    metadata.Description = descProp.GetString();
            }
            catch
            {
                // Fallback: trích xuất từ HTML nếu oEmbed thất bại
                if (!string.IsNullOrWhiteSpace(html))
                {
                    ExtractFromHtml(html, metadata);
                }
            }

            return metadata;
        }

        private void ExtractFromHtml(string html, SiteMetadata metadata)
        {
            var titleMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:title[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            if (titleMatch.Success)
                metadata.Title = System.Net.WebUtility.HtmlDecode(titleMatch.Groups[1].Value);

            var imgMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:image[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
            if (imgMatch.Success)
                metadata.CoverImageUrl = imgMatch.Groups[1].Value;
        }
    }
}
