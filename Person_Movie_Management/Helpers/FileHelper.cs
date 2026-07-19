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

        public static string CopyCoverImage(string sourcePath, string movieCode)
        {
            EnsureDirectories();
            string ext = Path.GetExtension(sourcePath);
            string newFileName = $"{movieCode}_{Guid.NewGuid()}{ext}";
            string destPath = Path.Combine(CoverImagesPath, newFileName);
            
            File.Copy(sourcePath, destPath, true);
            
            // Return relative path
            return $"App_Data\\CoverImages\\{newFileName}";
        }

        public static string CopyDetailImage(string sourcePath, string movieCode)
        {
            EnsureDirectories();
            string ext = Path.GetExtension(sourcePath);
            string newFileName = $"{movieCode}_{Guid.NewGuid()}{ext}";
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
                    using (var img = System.Drawing.Image.FromStream(stream))
                    {
                        return new System.Drawing.Bitmap(img);
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
