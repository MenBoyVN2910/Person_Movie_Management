using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers.Adapters
{
    /// <summary>
    /// Adapter cho Bilibili — trích xuất BV ID để lấy thumbnail và embed URL.
    /// </summary>
    public class BilibiliAdapter : ISiteAdapter
    {
        public string SiteName => "Bilibili";
        public string SiteIcon => "📱";

        private static readonly Regex _urlRegex = new(
            @"bilibili\.com/video/(BV[a-zA-Z0-9]+|av\d+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool CanHandle(string url)
        {
            return url.Contains("bilibili.com/video/", StringComparison.OrdinalIgnoreCase);
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
                EmbedUrl = $"https://player.bilibili.com/player.html?bvid={videoId}&high_quality=1"
            };

            // Trích xuất metadata từ HTML (Bilibili không có public oEmbed)
            if (!string.IsNullOrWhiteSpace(html))
            {
                // og:title
                var titleMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:title[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (titleMatch.Success)
                {
                    string title = System.Net.WebUtility.HtmlDecode(titleMatch.Groups[1].Value);
                    title = Regex.Replace(title, @"_哔哩哔哩.*$", "", RegexOptions.IgnoreCase).Trim();
                    metadata.Title = title;
                }

                // og:image
                var imgMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:image[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (imgMatch.Success)
                    metadata.CoverImageUrl = imgMatch.Groups[1].Value;

                // og:description
                var descMatch = Regex.Match(html, @"<meta[^>]+property=[""']og:description[""'][^>]+content=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (descMatch.Success)
                    metadata.Description = System.Net.WebUtility.HtmlDecode(descMatch.Groups[1].Value);
            }

            return Task.FromResult<SiteMetadata?>(metadata);
        }
    }
}
