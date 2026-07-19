using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace Person_Movie_Management.Helpers
{
    /// <summary>
    /// LRU Image Cache với Strong Reference.
    /// - Giữ ảnh chắc trong RAM (không bị GC xóa như WeakReference).
    /// - Tự động xóa ảnh ít dùng nhất khi vượt MaxItems.
    /// - Resize ảnh thành thumbnail khi cache → giảm ~90% bộ nhớ.
    /// - Thread-safe cho async loading.
    /// </summary>
    public static class ImageCache
    {
        private const int MaxItems = 300;
        
        // Thumbnail size khớp với kích thước PictureBox hiển thị
        private const int ThumbMaxWidth = 400;
        private const int ThumbMaxHeight = 220;

        private static readonly object _lock = new object();
        private static readonly Dictionary<string, LinkedListNode<CacheEntry>> _cache = new();
        private static readonly LinkedList<CacheEntry> _lruList = new();

        private class CacheEntry
        {
            public string Key;
            public Image Image;
        }

        /// <summary>
        /// Lấy ảnh từ cache (sync). Nếu chưa có, load từ đĩa + resize + cache.
        /// </summary>
        public static Image? Get(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            lock (_lock)
            {
                if (_cache.TryGetValue(path, out var node))
                {
                    // Di chuyển lên đầu LRU (mới dùng nhất)
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                    return node.Value.Image;
                }
            }

            // Cache miss → load từ đĩa + resize thành thumbnail
            var loadedImg = FileHelper.LoadImageSafe(path, ThumbMaxWidth, ThumbMaxHeight);
            if (loadedImg != null)
            {
                Put(path, loadedImg);
            }

            return loadedImg;
        }

        /// <summary>
        /// Lấy ảnh từ cache (async). Chạy I/O trên background thread.
        /// </summary>
        public static Task<Image?> GetAsync(string path)
        {
            if (string.IsNullOrEmpty(path)) return Task.FromResult<Image?>(null);

            // Kiểm tra cache trước (nhanh, không cần background thread)
            lock (_lock)
            {
                if (_cache.TryGetValue(path, out var node))
                {
                    _lruList.Remove(node);
                    _lruList.AddFirst(node);
                    return Task.FromResult<Image?>(node.Value.Image);
                }
            }

            // Cache miss → load trên background thread
            return Task.Run(() => Get(path));
        }

        private static void Put(string key, Image img)
        {
            lock (_lock)
            {
                // Nếu đã có, xóa entry cũ
                if (_cache.TryGetValue(key, out var existingNode))
                {
                    _lruList.Remove(existingNode);
                    _cache.Remove(key);
                }

                // Evict nếu vượt quá giới hạn
                while (_cache.Count >= MaxItems && _lruList.Count > 0)
                {
                    var oldest = _lruList.Last;
                    if (oldest != null)
                    {
                        _lruList.RemoveLast();
                        _cache.Remove(oldest.Value.Key);
                        // Không dispose ảnh vì có thể đang được hiển thị
                    }
                }

                // Thêm entry mới vào đầu LRU
                var entry = new CacheEntry { Key = key, Image = img };
                var newNode = _lruList.AddFirst(entry);
                _cache[key] = newNode;
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _cache.Clear();
                _lruList.Clear();
            }
        }

        /// <summary>
        /// Kiểm tra ảnh đã có trong cache chưa (không load).
        /// </summary>
        public static bool Contains(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            lock (_lock)
            {
                return _cache.ContainsKey(path);
            }
        }
    }
}
