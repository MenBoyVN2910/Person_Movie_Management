using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Person_Movie_Management.Helpers
{
    /// <summary>
    /// Extracts video thumbnails using NReco.VideoConverter (FFMpeg wrapper).
    /// </summary>
    public static class VideoThumbnailHelper
    {
        /// <summary>
        /// Extract thumbnail as Image object. Returns null on failure.
        /// </summary>
        public static Image? ExtractThumbnail(string videoPath)
        {
            if (!File.Exists(videoPath)) return null;

            try
            {
                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                using var ms = new MemoryStream();
                // Extract frame at 5 seconds
                ffMpeg.GetVideoThumbnail(videoPath, ms, 5f);
                ms.Position = 0;
                
                using var bmp = Image.FromStream(ms);
                return new Bitmap(bmp);
            }
            catch
            {
                return null;
            }
        }
    }
}
