using System;
using System.IO;
using System.Windows.Forms;

namespace Person_Movie_Management.Helpers
{
    public static class FileHelper
    {
        private static string CoverImagesPath => Path.Combine(Application.StartupPath, "App_Data", "CoverImages");
        private static string DetailImagesPath => Path.Combine(Application.StartupPath, "App_Data", "DetailImages");

        public static void EnsureDirectories()
        {
            if (!Directory.Exists(CoverImagesPath)) Directory.CreateDirectory(CoverImagesPath);
            if (!Directory.Exists(DetailImagesPath)) Directory.CreateDirectory(DetailImagesPath);
        }

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return "file";
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string sanitized = fileName;
            foreach (char c in invalidChars)
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }
            
            sanitized = sanitized.Trim();
            if (sanitized.Length > 50)
            {
                sanitized = sanitized.Substring(0, 50).Trim();
            }
            if (string.IsNullOrEmpty(sanitized)) return "file";
            
            return sanitized;
        }

        public static string CopyCoverImage(string sourcePath, string movieCode)
        {
            EnsureDirectories();
            string ext = Path.GetExtension(sourcePath);
            string safeCode = SanitizeFileName(movieCode);
            string newFileName = $"{safeCode}_{Guid.NewGuid()}{ext}";
            string destPath = Path.Combine(CoverImagesPath, newFileName);
            
            File.Copy(sourcePath, destPath, true);
            
            // Return relative path
            return $"App_Data\\CoverImages\\{newFileName}";
        }

        public static string CopyDetailImage(string sourcePath, string movieCode)
        {
            EnsureDirectories();
            string ext = Path.GetExtension(sourcePath);
            string safeCode = SanitizeFileName(movieCode);
            string newFileName = $"{safeCode}_{Guid.NewGuid()}{ext}";
            string destPath = Path.Combine(DetailImagesPath, newFileName);
            
            File.Copy(sourcePath, destPath, true);
            
            // Return relative path
            return $"App_Data\\DetailImages\\{newFileName}";
        }

        public static string GetFullPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return string.Empty;
            return Path.Combine(Application.StartupPath, relativePath);
        }

        public static System.Drawing.Image? LoadImageSafe(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    try
                    {
                        using (var img = System.Drawing.Image.FromStream(stream))
                        {
                            return new System.Drawing.Bitmap(img);
                        }
                    }
                    catch
                    {
                        // Fallback to Magick.NET for WEBP and other formats
                        try
                        {
                            stream.Position = 0;
                            using var magickImage = new ImageMagick.MagickImage(stream);
                            using var outMs = new MemoryStream();
                            magickImage.Format = ImageMagick.MagickFormat.Jpeg;
                            magickImage.Write(outMs);
                            outMs.Position = 0;
                            using var img = System.Drawing.Image.FromStream(outMs);
                            return new System.Drawing.Bitmap(img);
                        }
                        catch { return null; }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Load ảnh và resize thành thumbnail nếu maxWidth/maxHeight > 0.
        /// Giảm ~90% bộ nhớ: ảnh 1920x1080 (~6MB) → 400x225 (~250KB).
        /// Dùng HighQualityBicubic để giữ chất lượng hình ảnh.
        /// </summary>
        public static System.Drawing.Image? LoadImageSafe(string path, int maxWidth, int maxHeight)
        {
            if (maxWidth <= 0 && maxHeight <= 0) return LoadImageSafe(path);
            if (!File.Exists(path)) return null;
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    System.Drawing.Image? original = null;
                    try
                    {
                        original = System.Drawing.Image.FromStream(stream);
                    }
                    catch
                    {
                        // Fallback to Magick.NET
                        try
                        {
                            stream.Position = 0;
                            using var magickImage = new ImageMagick.MagickImage(stream);
                            using var outMs = new MemoryStream();
                            magickImage.Format = ImageMagick.MagickFormat.Jpeg;
                            magickImage.Write(outMs);
                            outMs.Position = 0;
                            original = System.Drawing.Image.FromStream(outMs);
                        }
                        catch { return null; }
                    }

                    if (original == null) return null;

                    using (original)
                    {
                        // Tính tỷ lệ scale giữ nguyên aspect ratio
                        float ratioW = maxWidth > 0 ? (float)maxWidth / original.Width : float.MaxValue;
                        float ratioH = maxHeight > 0 ? (float)maxHeight / original.Height : float.MaxValue;
                        float ratio = Math.Min(ratioW, ratioH);

                        // Nếu ảnh đã nhỏ hơn target, không cần resize
                        if (ratio >= 1.0f)
                        {
                            return new System.Drawing.Bitmap(original);
                        }

                        int newWidth = (int)(original.Width * ratio);
                        int newHeight = (int)(original.Height * ratio);

                        var thumb = new System.Drawing.Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                        using (var g = System.Drawing.Graphics.FromImage(thumb))
                        {
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                            g.DrawImage(original, 0, 0, newWidth, newHeight);
                        }
                        return thumb;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
