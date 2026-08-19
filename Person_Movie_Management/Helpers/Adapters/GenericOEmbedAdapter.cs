using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers.Adapters
{
    /// <summary>
    /// Adapter fallback — thử oEmbed discovery trước, rồi fallback về generic og:tag extraction.
    /// Adapter này luôn trả về CanHandle = true, nhưng được đặt CUỐI CÙNG trong registry.
    /// </summary>
    public class GenericOEmbedAdapter : ISiteAdapter
    {
        public string SiteName => "Web";
        public string SiteIcon => "🌐";

        public bool CanHandle(string url)
        {
            // Fallback: luôn nhận, nhưng được đặt cuối cùng trong danh sách adapter
            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<SiteMetadata?> ExtractMetadataAsync(string url, string html)
        {
            var metadata = new SiteMetadata
            {
                SiteName = SiteName,
                SiteIcon = SiteIcon
            };

            // Bước 1: Thử tìm oEmbed endpoint trong HTML
            if (!string.IsNullOrWhiteSpace(html))
            {
                string? oembedUrl = DiscoverOEmbedUrl(html);
                if (!string.IsNullOrEmpty(oembedUrl))
                {
                    try
                    {
                        await FetchOEmbedAsync(oembedUrl, metadata);
                    }
                    catch { /* Ignore, fallback to og tags */ }
                }

                // Bước 2: Trích xuất từ og tags (bổ sung cho oEmbed hoặc thay thế nếu oEmbed thất bại)
                if (string.IsNullOrEmpty(metadata.Title))
                {
                    var titleMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:title[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                    if (titleMatch.Success)
                        metadata.Title = System.Net.WebUtility.HtmlDecode(titleMatch.Groups[1].Value);
                }

                if (string.IsNullOrEmpty(metadata.CoverImageUrl))
                {
                    var imgMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:image[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                    if (imgMatch.Success)
                        metadata.CoverImageUrl = imgMatch.Groups[1].Value;
                }

                if (string.IsNullOrEmpty(metadata.Description))
                {
                    var descMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:description[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                    if (descMatch.Success)
                        metadata.Description = System.Net.WebUtility.HtmlDecode(descMatch.Groups[1].Value);
                }

                // Tìm og:site_name cho hiển thị
                var siteNameMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:site_name[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (siteNameMatch.Success)
                    metadata.SiteName = System.Net.WebUtility.HtmlDecode(siteNameMatch.Groups[1].Value);

                // Tìm og:video (embed URL)
                var videoMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:video(?::url)?[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (videoMatch.Success)
                    metadata.EmbedUrl = videoMatch.Groups[1].Value;
            }

            return metadata;
        }

        /// <summary>
        /// Tìm oEmbed endpoint trong HTML qua link[type="application/json+oembed"]
        /// </summary>
        private string? DiscoverOEmbedUrl(string html)
        {
            var match = Regex.Match(html,
                @"<link[^>]+type=[""']application/json\+oembed[""'][^>]+href=[""']([^""']+)[""']",
                RegexOptions.IgnoreCase);
            if (match.Success) return System.Net.WebUtility.HtmlDecode(match.Groups[1].Value);

            // Thử kiểu ngược (href trước type)
            match = Regex.Match(html,
                @"<link[^>]+href=[""']([^""']+)[""'][^>]+type=[""']application/json\+oembed[""']",
                RegexOptions.IgnoreCase);
            if (match.Success) return System.Net.WebUtility.HtmlDecode(match.Groups[1].Value);

            return null;
        }

        private async Task FetchOEmbedAsync(string oembedUrl, SiteMetadata metadata)
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
            string json = await client.GetStringAsync(oembedUrl);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("title", out var titleProp) && titleProp.ValueKind == JsonValueKind.String)
                metadata.Title = titleProp.GetString();
            if (root.TryGetProperty("thumbnail_url", out var thumbProp) && thumbProp.ValueKind == JsonValueKind.String)
                metadata.CoverImageUrl = thumbProp.GetString();
            if (root.TryGetProperty("provider_name", out var provProp) && provProp.ValueKind == JsonValueKind.String)
                metadata.SiteName = provProp.GetString() ?? "Web";
        }
    }
}
