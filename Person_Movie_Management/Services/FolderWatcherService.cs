using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Person_Movie_Management.Forms;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;
using Person_Movie_Management.Repositories;

namespace Person_Movie_Management.Services
{
    public class FolderWatcherService
    {
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers = new(StringComparer.OrdinalIgnoreCase);
        private readonly MovieRepository _movieRepo;
        private readonly int _userId;
        private readonly Control _invokeTarget;
        private readonly ConcurrentDictionary<string, DateTime> _recentlyProcessed = new(StringComparer.OrdinalIgnoreCase);

        private static readonly string[] AllowedExtensions = new[]
        {
            ".mp4", ".mkv", ".avi", ".wmv", ".mov", ".flv", ".webm", ".m4v"
        };

        public FolderWatcherService(int userId, Control invokeTarget)
        {
            _userId = userId;
            _movieRepo = new MovieRepository();
            _invokeTarget = invokeTarget;

            InitializeWatchers();
        }

        private void InitializeWatchers()
        {
            try
            {
                // 1. Always include Windows Videos folder
                string defaultVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                if (!string.IsNullOrWhiteSpace(defaultVideosPath))
                {
                    if (!Directory.Exists(defaultVideosPath))
                    {
                        Directory.CreateDirectory(defaultVideosPath);
                    }
                    WatchFolder(defaultVideosPath);
                }

                // 2. Load custom watched folders from persisted settings
                var customFolders = LoadPersistedWatchFolders();
                foreach (var folder in customFolders)
                {
                    WatchFolder(folder);
                }

                // 3. Scan existing local movies in DB and watch their parent directories
                var existingLocalMovies = _movieRepo.GetAllByUser(_userId, sourceType: 1);
                foreach (var movie in existingLocalMovies)
                {
                    if (!string.IsNullOrWhiteSpace(movie.MediaUrl))
                    {
                        try
                        {
                            string? dir = Path.GetDirectoryName(movie.MediaUrl);
                            if (!string.IsNullOrWhiteSpace(dir) && Directory.Exists(dir))
                            {
                                WatchFolder(dir);
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { /* Ignore initial watcher setup errors */ }

            if (SessionManager.IsFolderWatcherEnabled)
            {
                Task.Run(() =>
                {
                    try
                    {
                        var movieSvc = new MovieService();
                        string defaultVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                        if (!string.IsNullOrWhiteSpace(defaultVideosPath) && Directory.Exists(defaultVideosPath))
                        {
                            var added = movieSvc.AutoScanLocalFolder(_userId, defaultVideosPath);
                            if (added.Count > 0)
                            {
                                NotifyUI(() => DataCache.Invalidate());
                            }
                        }
                    }
                    catch { }
                });
            }
        }

        public void AddWatchPath(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath)) return;

            string normalizedPath = Path.GetFullPath(folderPath).TrimEnd('\\', '/');
            WatchFolder(normalizedPath);
            PersistWatchFolder(normalizedPath);
        }

        private void WatchFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath)) return;

            string normalizedPath = Path.GetFullPath(folderPath).TrimEnd('\\', '/');

            // If already being watched directly, skip
            if (_watchers.ContainsKey(normalizedPath)) return;

            try
            {
                var watcher = new FileSystemWatcher(normalizedPath)
                {
                    IncludeSubdirectories = true,
                    Filter = "*.*",
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
                    EnableRaisingEvents = SessionManager.IsFolderWatcherEnabled
                };

                watcher.Created += (s, e) => ProcessFileEvent(e.FullPath);
                watcher.Renamed += (s, e) => ProcessRenamedEvent(e.OldFullPath, e.FullPath);
                watcher.Deleted += (s, e) => ProcessDeletedEvent(e.FullPath);
                watcher.Changed += (s, e) => ProcessFileEvent(e.FullPath);

                _watchers[normalizedPath] = watcher;
            }
            catch { /* Ignore folder access permission errors */ }
        }

        private List<string> LoadPersistedWatchFolders()
        {
            var list = new List<string>();
            try
            {
                string filePath = Path.Combine(Application.StartupPath, $"watched_folders_{_userId}.txt");
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        string trimmed = line.Trim();
                        if (!string.IsNullOrWhiteSpace(trimmed) && Directory.Exists(trimmed))
                        {
                            list.Add(trimmed);
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        private void PersistWatchFolder(string folderPath)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, $"watched_folders_{_userId}.txt");
                var current = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (File.Exists(filePath))
                {
                    foreach (var l in File.ReadAllLines(filePath))
                    {
                        if (!string.IsNullOrWhiteSpace(l)) current.Add(l.Trim());
                    }
                }
                current.Add(folderPath);
                File.WriteAllLines(filePath, current);
            }
            catch { }
        }

        public void Start()
        {
            foreach (var watcher in _watchers.Values)
            {
                try
                {
                    watcher.EnableRaisingEvents = true;
                }
                catch { }
            }

            Task.Run(() =>
            {
                try
                {
                    var movieSvc = new MovieService();
                    string defaultVideosPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                    if (!string.IsNullOrWhiteSpace(defaultVideosPath) && Directory.Exists(defaultVideosPath))
                    {
                        var added = movieSvc.AutoScanLocalFolder(_userId, defaultVideosPath);
                        if (added.Count > 0)
                        {
                            NotifyUI(() => DataCache.Invalidate());
                        }
                    }
                }
                catch { }
            });
        }

        public void Stop()
        {
            foreach (var watcher in _watchers.Values)
            {
                try
                {
                    watcher.EnableRaisingEvents = false;
                }
                catch { }
            }
        }

        private void ProcessFileEvent(string? fullPath)
        {
            if (!SessionManager.IsFolderWatcherEnabled) return;
            if (string.IsNullOrWhiteSpace(fullPath)) return;

            string ext = Path.GetExtension(fullPath).ToLower();
            if (!AllowedExtensions.Contains(ext)) return;

            if (IsRecentlyProcessed(fullPath)) return;

            Task.Run(async () =>
            {
                await HandleNewVideoFileAsync(fullPath);
            });
        }

        private void ProcessRenamedEvent(string? oldFullPath, string? newFullPath)
        {
            if (!SessionManager.IsFolderWatcherEnabled) return;
            if (string.IsNullOrWhiteSpace(newFullPath) || string.IsNullOrWhiteSpace(oldFullPath)) return;

            string newExt = Path.GetExtension(newFullPath).ToLower();
            string oldExt = Path.GetExtension(oldFullPath).ToLower();

            bool isNewExtAllowed = AllowedExtensions.Contains(newExt);
            bool isOldExtAllowed = AllowedExtensions.Contains(oldExt);

            // Case 1: Renamed from non-video (.crdownload / .tmp) to video (.mp4) -> Treat as newly created video
            if (isNewExtAllowed && !isOldExtAllowed)
            {
                ProcessFileEvent(newFullPath);
                return;
            }

            // Case 2: Renamed from video (.mp4) to non-video (.tmp / .bak) -> Treat as deleted video
            if (!isNewExtAllowed && isOldExtAllowed)
            {
                ProcessDeletedEvent(oldFullPath);
                return;
            }

            // Case 3: Renamed from video (.mp4) to another video name (.mp4) -> Rename existing movie
            if (isNewExtAllowed && isOldExtAllowed)
            {
                if (IsRecentlyProcessed(newFullPath)) return;

                Task.Run(async () =>
                {
                    await HandleRenamedVideoFileAsync(oldFullPath, newFullPath);
                });
            }
        }

        private void ProcessDeletedEvent(string? fullPath)
        {
            if (!SessionManager.IsFolderWatcherEnabled) return;
            if (string.IsNullOrWhiteSpace(fullPath)) return;

            string ext = Path.GetExtension(fullPath).ToLower();
            if (!AllowedExtensions.Contains(ext)) return;

            Task.Run(() =>
            {
                try
                {
                    var existing = _movieRepo.GetByMediaUrl(_userId, fullPath);
                    if (existing == null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(fullPath);
                        existing = _movieRepo.GetByCode(_userId, fileName);
                    }

                    if (existing != null)
                    {
                        _movieRepo.Delete(existing.Id);
                        NotifyUI(() =>
                        {
                            DataCache.Invalidate();
                        });
                    }
                }
                catch { }
            });
        }

        private bool IsRecentlyProcessed(string path)
        {
            if (_recentlyProcessed.TryGetValue(path, out var lastTime))
            {
                if ((DateTime.Now - lastTime).TotalSeconds < 4)
                {
                    return true;
                }
            }
            _recentlyProcessed[path] = DateTime.Now;

            // Prune old debounce entries
            if (_recentlyProcessed.Count > 100)
            {
                var threshold = DateTime.Now.AddSeconds(-30);
                foreach (var kvp in _recentlyProcessed)
                {
                    if (kvp.Value < threshold) _recentlyProcessed.TryRemove(kvp.Key, out _);
                }
            }
            return false;
        }

        private async Task HandleRenamedVideoFileAsync(string oldFilePath, string newFilePath)
        {
            try
            {
                bool isReady = await WaitForFileReadyAsync(newFilePath, maxRetries: 25, delayMs: 600);
                if (!isReady || !File.Exists(newFilePath)) return;

                string oldName = Path.GetFileNameWithoutExtension(oldFilePath);
                string newName = Path.GetFileNameWithoutExtension(newFilePath);
                if (string.IsNullOrWhiteSpace(newName)) return;

                // Find old movie in database
                var oldMovie = _movieRepo.GetByMediaUrl(_userId, oldFilePath);
                if (oldMovie == null)
                {
                    oldMovie = _movieRepo.GetByCode(_userId, oldName);
                }

                if (oldMovie != null)
                {
                    // Generate unique new movie code
                    string finalCode = newName;
                    var existingByCode = _movieRepo.GetByCode(_userId, finalCode);
                    int duplicateIndex = 1;
                    while (existingByCode != null && existingByCode.Id != oldMovie.Id)
                    {
                        finalCode = $"{newName} ({duplicateIndex})";
                        existingByCode = _movieRepo.GetByCode(_userId, finalCode);
                        duplicateIndex++;
                    }

                    // Update existing movie record with new name and new path (effectively renaming the card & removing the old name)
                    oldMovie.MovieCode = finalCode;
                    oldMovie.MediaUrl = newFilePath;
                    oldMovie.Note = $"✦ Tự động đồng bộ từ thư mục: {Path.GetDirectoryName(newFilePath)} ({DateTime.Now:dd/MM/yyyy HH:mm})";

                    _movieRepo.Update(oldMovie);

                    // Load thumbnail if available for Toast
                    System.Drawing.Image? thumbImg = null;
                    try
                    {
                        if (!string.IsNullOrEmpty(oldMovie.CoverImage))
                        {
                            string fullCover = FileHelper.GetFullPath(oldMovie.CoverImage);
                            if (File.Exists(fullCover))
                            {
                                thumbImg = FileHelper.LoadImageSafe(fullCover);
                            }
                        }
                    }
                    catch { }

                    NotifyUI(() =>
                    {
                        DataCache.Invalidate();
                        FrmToastNotification.ShowNotification("🎬 Đã Đổi Tên Phim", $"{oldName} ➔ {finalCode}", newFilePath, thumbImg);
                    });
                }
                else
                {
                    // If old movie was not found in DB, import as new
                    await HandleNewVideoFileAsync(newFilePath);
                }
            }
            catch { }
        }

        private async Task HandleNewVideoFileAsync(string filePath)
        {
            try
            {
                // Wait for file copy/download to complete (not locked by other processes)
                bool isReady = await WaitForFileReadyAsync(filePath, maxRetries: 25, delayMs: 600);
                if (!isReady || !File.Exists(filePath)) return;

                // Check if movie already exists by MediaUrl
                var existingByUrl = _movieRepo.GetByMediaUrl(_userId, filePath);
                if (existingByUrl != null) return;

                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (string.IsNullOrWhiteSpace(fileName)) return;

                // Unique movie code generation
                string finalCode = fileName;
                var existingByCode = _movieRepo.GetByCode(_userId, finalCode);
                int duplicateIndex = 1;
                while (existingByCode != null)
                {
                    finalCode = $"{fileName} ({duplicateIndex})";
                    existingByCode = _movieRepo.GetByCode(_userId, finalCode);
                    duplicateIndex++;
                }

                // Extract video thumbnail
                string? coverPath = null;
                System.Drawing.Image? thumbImg = null;
                try
                {
                    thumbImg = VideoThumbnailHelper.ExtractThumbnail(filePath);
                    if (thumbImg != null)
                    {
                        FileHelper.EnsureDirectories();
                        string safeCode = FileHelper.SanitizeFileName(fileName);
                        string newFileName = $"{safeCode}_{Guid.NewGuid()}.jpg";
                        string appDataPath = Path.Combine(Application.StartupPath, "App_Data", "CoverImages");
                        if (!Directory.Exists(appDataPath)) Directory.CreateDirectory(appDataPath);

                        string destPath = Path.Combine(appDataPath, newFileName);
                        thumbImg.Save(destPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        coverPath = $"App_Data\\CoverImages\\{newFileName}";
                    }
                }
                catch { /* Ignore thumbnail extraction errors */ }

                // Insert into Database
                var movie = new Movie
                {
                    UserId = _userId,
                    MovieCode = finalCode,
                    SourceType = 1, // Local
                    MediaUrl = filePath,
                    CoverImage = coverPath,
                    Note = $"✦ Tự động theo dõi từ thư mục: {Path.GetDirectoryName(filePath)} ({DateTime.Now:dd/MM/yyyy HH:mm})"
                };

                _movieRepo.Insert(movie);

                // Notify UI and user
                NotifyUI(() =>
                {
                    DataCache.Invalidate();
                    FrmToastNotification.ShowNotification("🎬 Phát Hiện Phim Mới", finalCode, filePath, thumbImg);
                });
            }
            catch { /* Catch all unexpected background errors */ }
        }

        private void NotifyUI(Action action)
        {
            try
            {
                if (_invokeTarget != null && !_invokeTarget.IsDisposed && _invokeTarget.IsHandleCreated)
                {
                    _invokeTarget.BeginInvoke(action);
                }
            }
            catch { }
        }

        private async Task<bool> WaitForFileReadyAsync(string filePath, int maxRetries = 25, int delayMs = 600)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (!File.Exists(filePath)) return false;

                    using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    if (stream.Length > 0)
                    {
                        return true;
                    }
                }
                catch (IOException)
                {
                    // File is still locked (copying / downloading)
                }
                catch (Exception)
                {
                    return false;
                }

                await Task.Delay(delayMs);
            }
            return false;
        }

        public void StopAndDispose()
        {
            foreach (var watcher in _watchers.Values)
            {
                try
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
                catch { }
            }
            _watchers.Clear();
        }
    }
}
