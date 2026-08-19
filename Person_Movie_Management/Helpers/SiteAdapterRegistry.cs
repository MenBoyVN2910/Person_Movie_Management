using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers
{
    /// <summary>
    /// Kết quả metadata trích xuất từ một trang web.
    /// </summary>
    public class SiteMetadata
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? EmbedUrl { get; set; }
        public List<string> ExtraImageUrls { get; set; } = new();
        public string? Duration { get; set; }
        public string SiteName { get; set; } = "Web";
        public string SiteIcon { get; set; } = "🌐";
    }

    /// <summary>
    /// Interface cho mỗi Site Adapter — nhận dạng URL và trích xuất metadata.
    /// </summary>
    public interface ISiteAdapter
    {
        string SiteName { get; }
        string SiteIcon { get; }
        bool CanHandle(string url);
        Task<SiteMetadata?> ExtractMetadataAsync(string url, string html);
    }

    /// <summary>
    /// Registry quản lý tất cả Site Adapter. Tự động chọn adapter phù hợp dựa trên URL.
    /// </summary>
    public static class SiteAdapterRegistry
    {
        private static readonly List<ISiteAdapter> _adapters = new();

        static SiteAdapterRegistry()
        {
            // Đăng ký adapter theo thứ tự ưu tiên (đặc thù trước, generic sau)
            _adapters.Add(new Adapters.YouTubeAdapter());
            _adapters.Add(new Adapters.DailymotionAdapter());
            _adapters.Add(new Adapters.VimeoAdapter());
            _adapters.Add(new Adapters.BilibiliAdapter());
            _adapters.Add(new Adapters.TwitchAdapter());
            _adapters.Add(new Adapters.GenericOEmbedAdapter());
        }

        /// <summary>
        /// Tìm adapter phù hợp cho URL. Trả về null nếu không có adapter nào match.
        /// </summary>
        public static ISiteAdapter? FindAdapter(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            return _adapters.FirstOrDefault(a => a.CanHandle(url));
        }

        /// <summary>
        /// Nhận dạng tên trang web từ URL (dùng cho hiển thị badge).
        /// Trả về (SiteName, SiteIcon).
        /// </summary>
        public static (string Name, string Icon) IdentifySite(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return ("Web", "🌐");

            var adapter = _adapters.FirstOrDefault(a => a.CanHandle(url));
            if (adapter != null && adapter is not Adapters.GenericOEmbedAdapter)
            {
                return (adapter.SiteName, adapter.SiteIcon);
            }

            // Fallback: trích xuất domain name
            try
            {
                var uri = new Uri(url);
                string host = uri.Host.Replace("www.", "");
                // Rút gọn domain cho gọn
                string name = host.Split('.')[0];
                if (name.Length > 0)
                    name = char.ToUpper(name[0]) + name.Substring(1);
                return (name, "🌐");
            }
            catch
            {
                return ("Web", "🌐");
            }
        }
    }
}
