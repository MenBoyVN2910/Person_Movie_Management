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
                        var movie = new Movie
                        {
                            UserId = userId,
                            MovieCode = fileName,
                            SourceType = 1, // Local
                            MediaUrl = file, // Using full path temporarily, could be relative to root
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
