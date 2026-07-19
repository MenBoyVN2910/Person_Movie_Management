using System;
using System.IO;
using System.Linq;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;
using System.Windows.Forms;

namespace Person_Movie_Management.Services
{
    public class FolderWatcherService
    {
        private FileSystemWatcher _watcher;
        private MovieRepository _movieRepo;
        private int _userId;
        private Control _invokeTarget;

        public FolderWatcherService(int userId, Control invokeTarget)
        {
            _userId = userId;
            _movieRepo = new MovieRepository();
            _invokeTarget = invokeTarget;
            
            // Watch default Videos folder
            string watchPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            if (!Directory.Exists(watchPath)) return;

            _watcher = new FileSystemWatcher(watchPath);
            _watcher.IncludeSubdirectories = false; // Only top level for performance
            _watcher.Filter = "*.*";
            
            _watcher.Created += Watcher_Created;
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // Debounce or filter file extensions
            string ext = Path.GetExtension(e.FullPath).ToLower();
            if (ext == ".mp4" || ext == ".mkv" || ext == ".avi")
            {
                // Delay to ensure file is completely written (primitive way)
                System.Threading.Tasks.Task.Delay(1000).ContinueWith(t => 
                {
                    try
                    {
                        string fileName = Path.GetFileNameWithoutExtension(e.FullPath);
                        if (_movieRepo.GetByCode(_userId, fileName) == null)
                        {
                            var movie = new Movie
                            {
                                UserId = _userId,
                                MovieCode = fileName,
                                SourceType = 1, // Local
                                MediaUrl = e.FullPath,
                                Note = "Tự động thêm từ Watcher"
                            };
                            
                            _movieRepo.Insert(movie);

                            _invokeTarget.Invoke(new Action(() => 
                            {
                                DataCache.Invalidate();
                                // Notify user optionally via Toast, but let's keep it silent for now
                            }));
                        }
                    }
                    catch { }
                });
            }
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
            }
        }
    }
}
