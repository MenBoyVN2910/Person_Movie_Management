using System;
using System.Collections.Generic;
using System.IO;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Services
{
    public class MovieService
    {
        private readonly MovieRepository _movieRepo;

        public MovieService()
        {
            _movieRepo = new MovieRepository();
        }

        public List<Movie> AutoScanLocalFolder(int userId, string folderPath)
        {
            var newMovies = new List<Movie>();
            if (!Directory.Exists(folderPath)) return newMovies;

            string[] allowedExtensions = { ".mp4", ".mkv", ".avi", ".wmv", ".mov" };
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string ext = Path.GetExtension(file).ToLower();
                if (Array.Exists(allowedExtensions, e => e == ext))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    
                    // Check if exists
                    var existing = _movieRepo.GetByCode(userId, fileName);
                    if (existing == null)
                    {
                        // Extract video thumbnail using Windows Shell
                        string? coverPath = null;
                        try
                        {
                            var img = VideoThumbnailHelper.ExtractThumbnail(file);
                            if (img != null)
                            {
                                FileHelper.EnsureDirectories();
                                string safeCode = FileHelper.SanitizeFileName(fileName);
                                string newFileName = $"{safeCode}_{Guid.NewGuid()}.jpg";
                                string appDataPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "App_Data", "CoverImages");
                                string destPath = Path.Combine(appDataPath, newFileName);
                                
                                img.Save(destPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                coverPath = $"App_Data\\CoverImages\\{newFileName}";
                                img.Dispose();
                            }
                        }
                        catch { /* Ignore thumbnail extraction failures */ }

                        var movie = new Movie
                        {
                            UserId = userId,
                            MovieCode = fileName,
                            SourceType = 1, // Local
                            MediaUrl = file,
                            CoverImage = coverPath,
                            Note = $"Tự động quét từ thư mục: {folderPath}"
                        };
                        _movieRepo.Insert(movie);
                        newMovies.Add(movie);
                    }
                }
            }

            return newMovies;
        }
    }
}
