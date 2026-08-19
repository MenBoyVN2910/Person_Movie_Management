using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Services
{
    /// <summary>
    /// Dịch vụ Sao lưu & Khôi phục cho toàn bộ hệ thống (tất cả users, tất cả dữ liệu).
    /// Backup paths được lưu ở cấp hệ thống, không phụ thuộc userId.
    /// </summary>
    public class BackupService
    {
        private List<string> _backupPaths = new();
        private bool _isBackingUp = false;
        private readonly object _lockObj = new();
        private const string CONFIG_FILE = "backup_paths.json";

        // Progress callback: (current, total, message)
        public event Action<int, int, string>? ProgressChanged;

        public BackupService()
        {
        }

        /// <summary>Khởi động dịch vụ backup. Nạp danh sách đường dẫn.</summary>
        public void Start()
        {
            LoadBackupPaths();
        }

        /// <summary>Dừng dịch vụ backup.</summary>
        public void Stop()
        {
        }

        // ─── Quản lý đường dẫn ────────────────────────────────────────────────

        public List<string> GetBackupPaths()
        {
            lock (_lockObj)
            {
                return new List<string>(_backupPaths);
            }
        }

        public void AddBackupPath(string path)
        {
            lock (_lockObj)
            {
                if (!_backupPaths.Contains(path))
                {
                    _backupPaths.Add(path);
                    SaveBackupPaths();
                }
            }
        }

        public void RemoveBackupPath(string path)
        {
            lock (_lockObj)
            {
                if (_backupPaths.Remove(path))
                {
                    SaveBackupPaths();
                }
            }
        }

        private void LoadBackupPaths()
        {
            string configPath = GetConfigFilePath();
            if (!File.Exists(configPath))
            {
                _backupPaths = new List<string>();
                return;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                var paths = JsonSerializer.Deserialize<List<string>>(json);
                lock (_lockObj)
                {
                    _backupPaths = paths ?? new List<string>();
                }
            }
            catch
            {
                lock (_lockObj)
                {
                    _backupPaths = new List<string>();
                }
            }
        }

        private void SaveBackupPaths()
        {
            try
            {
                string json = JsonSerializer.Serialize(_backupPaths,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(GetConfigFilePath(), json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error saving backup paths: " + ex.Message);
            }
        }

        private static string GetConfigFilePath()
        {
            return Path.Combine(DatabaseHelper.AppDataFolder, CONFIG_FILE);
        }

        // ─── Backup ───────────────────────────────────────────────────────────

        /// <summary>
        /// Thực hiện backup đồng bộ (dùng khi tắt ứng dụng / đóng form).
        /// </summary>
        public void PerformBackupSync()
        {
            lock (_lockObj)
            {
                if (_isBackingUp) return;
                _isBackingUp = true;
            }

            List<string> paths;
            lock (_lockObj)
            {
                paths = new List<string>(_backupPaths);
            }

            if (paths.Count == 0)
            {
                lock (_lockObj) { _isBackingUp = false; }
                return;
            }

            try
            {
                string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                foreach (var folder in paths)
                {
                    if (!Directory.Exists(folder)) continue;
                    try
                    {
                        BackupToFolder(folder, timestamp);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Sync backup failed for {folder}: {ex.Message}");
                    }
                }
            }
            finally
            {
                lock (_lockObj) { _isBackingUp = false; }
            }
        }

        /// <summary>
        /// Thực hiện backup bất đồng bộ đến tất cả đường dẫn đã cấu hình.
        /// Chỉ giữ lại DUY NHẤT 1 file backup mới nhất trong mỗi thư mục + 1 file README.md.
        /// </summary>
        public async Task PerformBackupAsync()
        {
            lock (_lockObj)
            {
                if (_isBackingUp) return;
                _isBackingUp = true;
            }

            List<string> paths;
            lock (_lockObj)
            {
                paths = new List<string>(_backupPaths);
            }

            if (paths.Count == 0)
            {
                lock (_lockObj) { _isBackingUp = false; }
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    int done = 0;

                    foreach (var folder in paths)
                    {
                        done++;
                        if (!Directory.Exists(folder)) continue;

                        try
                        {
                            ReportProgress(done, paths.Count, $"Đang sao lưu đến: {folder}");
                            BackupToFolder(folder, timestamp);
                            ReportProgress(done, paths.Count, $"✅ Hoàn thành: {folder}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"Backup failed for {folder}: {ex.Message}");
                            ReportProgress(done, paths.Count, $"❌ Lỗi tại: {folder}");
                        }
                    }
                });
            }
            finally
            {
                lock (_lockObj) { _isBackingUp = false; }
            }
        }

        private void BackupToFolder(string folder, string timestamp)
        {
            // ── 1. Dọn dẹp tất cả các file backup cũ trong thư mục ──
            try
            {
                var oldDbFiles = Directory.GetFiles(folder, "MovieVault_Backup*.db");
                foreach (var f in oldDbFiles) { try { File.Delete(f); } catch { } }
                var oldZipFiles = Directory.GetFiles(folder, "MovieVault_Backup*.zip");
                foreach (var f in oldZipFiles) { try { File.Delete(f); } catch { } }
                var legacyFiles = Directory.GetFiles(folder, "AppDatabase_Backup*.db");
                foreach (var f in legacyFiles) { try { File.Delete(f); } catch { } }
            }
            catch { }

            // ── 2. Tạo thư mục tạm để chuẩn bị nội dung ZIP ──
            string tempDir = Path.Combine(Path.GetTempPath(), "MovieVault_Backup_" + Guid.NewGuid().ToString("N"));
            try
            {
                Directory.CreateDirectory(tempDir);

                // ── 3. Tạo snapshot cơ sở dữ liệu SQLite bằng VACUUM INTO ──
                string tempDbFile = Path.Combine(tempDir, "MovieVault_Backup.db");
                using (var conn = new SqliteConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = $"VACUUM INTO '{tempDbFile.Replace("'", "''")}'";
                    cmd.ExecuteNonQuery();
                }

                // ── 4. Copy toàn bộ thư mục media vào thư mục tạm ──
                string appRoot = System.Windows.Forms.Application.StartupPath;

                // App_Data\CoverImages
                string coverSrc = Path.Combine(appRoot, "App_Data", "CoverImages");
                if (Directory.Exists(coverSrc))
                    CopyDirectoryRecursive(coverSrc, Path.Combine(tempDir, "App_Data", "CoverImages"));

                // App_Data\DetailImages
                string detailSrc = Path.Combine(appRoot, "App_Data", "DetailImages");
                if (Directory.Exists(detailSrc))
                    CopyDirectoryRecursive(detailSrc, Path.Combine(tempDir, "App_Data", "DetailImages"));

                // Data\Avatars
                string avatarSrc = Path.Combine(appRoot, "Data", "Avatars");
                if (Directory.Exists(avatarSrc))
                    CopyDirectoryRecursive(avatarSrc, Path.Combine(tempDir, "Data", "Avatars"));

                // backup_paths.json
                string configSrc = GetConfigFilePath();
                if (File.Exists(configSrc))
                    File.Copy(configSrc, Path.Combine(tempDir, CONFIG_FILE), overwrite: true);

                // ── 5. Đóng gói thành file ZIP ──
                string backupZipFile = Path.Combine(folder, "MovieVault_Backup.zip");
                if (File.Exists(backupZipFile))
                    try { File.Delete(backupZipFile); } catch { }

                ZipFile.CreateFromDirectory(tempDir, backupZipFile, CompressionLevel.Fastest, includeBaseDirectory: false);

                // ── 6. Tạo file README.md ──
                string readmeFile = Path.Combine(folder, "README.md");
                File.WriteAllText(readmeFile, BuildReadmeContent(timestamp, "MovieVault_Backup.zip"), Encoding.UTF8);
            }
            finally
            {
                // ── 7. Dọn thư mục tạm ──
                try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, recursive: true); } catch { }
            }
        }

        /// <summary>
        /// Copy toàn bộ nội dung thư mục nguồn sang thư mục đích (đệ quy).
        /// </summary>
        private static void CopyDirectoryRecursive(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                try { File.Copy(file, destFile, overwrite: true); } catch { }
            }
            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetDirectoryName(subDir) != null ? new DirectoryInfo(subDir).Name : Path.GetFileName(subDir));
                CopyDirectoryRecursive(subDir, destSubDir);
            }
        }

        private static string BuildReadmeContent(string timestamp, string fileName)
        {
            return $@"# 🎬 Movie Vault - Bản Sao Lưu Hệ Thống (System Backup)

> **💡 HƯỚNG DẪN MỞ BẰNG TRÌNH DUYỆT BÊN NGOÀI:**
> - Bạn có thể xem tài liệu này trên bất kỳ trình duyệt web nào (**Google Chrome, Microsoft Edge, Mozilla Firefox, Cốc Cốc, Safari...**).
> - **Cách mở:** Nhấp chuột phải vào file `README.md` này ➔ Chọn **Open with (Mở bằng)** ➔ Chọn trình duyệt web của bạn, hoặc kéo thả file này trực tiếp vào cửa sổ trình duyệt đang mở.

---

## 📌 1. Thông Tin Bản Sao Lưu

| Thuộc tính | Chi tiết |
|---|---|
| **Thời gian sao lưu** | `{timestamp}` |
| **File sao lưu** | `{fileName}` |
| **Phiên bản hệ thống** | `Movie Vault Ver 3.0` |
| **Phạm vi dữ liệu** | **Toàn bộ hệ thống** (Tất cả tài khoản, dữ liệu, hình ảnh & cấu hình) |

---

## 📦 2. Toàn Bộ Dữ Liệu Được Sao Lưu Bao Gồm

File `{fileName}` chứa **toàn bộ** dữ liệu hệ thống:
- ✅ **Cơ sở dữ liệu:** Toàn bộ database (tài khoản, phim, audio, diễn viên, playlist, tags, quốc tịch, yêu thích, thùng rác...).
- ✅ **Ảnh bìa (Cover Images):** Tất cả hình ảnh đại diện của phim, audio, diễn viên.
- ✅ **Ảnh phụ / Gallery:** Toàn bộ hình ảnh chi tiết, ảnh gallery của phim và diễn viên.
- ✅ **Avatar người dùng:** Ảnh đại diện của tất cả tài khoản.
- ✅ **Ghi chú & Đánh giá:** Toàn bộ ghi chú cá nhân và điểm đánh giá.
- ✅ **Cấu hình đường dẫn backup:** Danh sách thư mục sao lưu.

---

## 🔄 3. Hướng Dẫn Khôi Phục Dữ Liệu

### Cách 1: Khôi phục tự động qua Ứng Dụng Movie Vault (Khuyến nghị)
1. Mở ứng dụng **Movie Vault**.
2. Trên thanh menu bên trái (Sidebar), nhấn vào mục **💾 Sao Lưu**.
3. Nhấn vào nút **🔄 Khôi phục từ Backup**.
4. Chọn file `{fileName}` (file `.zip` hoặc `.db` cũ đều được hỗ trợ).
5. Nhấn **Yes** để xác nhận ➔ Hệ thống sẽ tự động khôi phục toàn bộ dữ liệu, hình ảnh và khởi động lại.

### Cách 2: Xem trực tiếp cơ sở dữ liệu (chỉ xem, không khôi phục ảnh)
1. Giải nén file `{fileName}` → lấy file `MovieVault_Backup.db` bên trong.
2. Mở bằng **DB Browser for SQLite** tại: [https://sqlitebrowser.org](https://sqlitebrowser.org).

---

## ⚠️ Lưu Ý Quan Trọng
- Thư mục này chỉ lưu giữ duy nhất **bản sao lưu mới nhất** để tối ưu hóa dung lượng ổ đĩa.
- Vui lòng **không tự ý đổi tên** hoặc sửa đổi nội dung file `{fileName}` nếu bạn có ý định dùng nó để khôi phục trong tương lai.

---
*Tạo tự động bởi Hệ Thống Movie Vault Backup Engine - {DateTime.Now:yyyy}*
";
        }

        private void ReportProgress(int current, int total, string message)
        {
            ProgressChanged?.Invoke(current, total, message);
        }

        // ─── Restore ──────────────────────────────────────────────────────────

        /// <summary>
        /// Thông tin của một backup file để hiển thị trước khi restore.
        /// </summary>
        public class BackupInfo
        {
            public string FolderPath { get; set; } = "";
            public string DbFilePath { get; set; } = "";
            /// <summary>Đường dẫn file gốc mà user chọn (.zip hoặc .db)</summary>
            public string OriginalFilePath { get; set; } = "";
            public string BackupTime { get; set; } = "Không xác định";
            public bool IsValid { get; set; }
            public bool IsZipFormat { get; set; }
            public string ErrorMessage { get; set; } = "";
        }

        /// <summary>
        /// Đọc và kiểm tra tính hợp lệ của một file backup .db.
        /// </summary>
        public static BackupInfo ReadBackupFileInfo(string filePath)
        {
            var info = new BackupInfo
            {
                DbFilePath = filePath,
                OriginalFilePath = filePath
            };

            if (!File.Exists(filePath))
            {
                info.ErrorMessage = "File backup không tồn tại.";
                return info;
            }

            info.FolderPath = Path.GetDirectoryName(filePath) ?? "";
            string ext = Path.GetExtension(filePath).ToLowerInvariant();

            if (ext == ".zip")
            {
                // ── Đọc thông tin từ file ZIP ──
                info.IsZipFormat = true;
                try
                {
                    using var archive = ZipFile.OpenRead(filePath);
                    var dbEntry = archive.GetEntry("MovieVault_Backup.db");
                    if (dbEntry == null)
                    {
                        info.ErrorMessage = "File ZIP không chứa cơ sở dữ liệu MovieVault_Backup.db.";
                        return info;
                    }

                    // Giải nén DB tạm vào thư mục temp để kiểm tra tính hợp lệ
                    string tempDb = Path.Combine(Path.GetTempPath(), "mv_check_" + Guid.NewGuid().ToString("N") + ".db");
                    try
                    {
                        dbEntry.ExtractToFile(tempDb, overwrite: true);
                        using var conn = new SqliteConnection($"Data Source={tempDb};Mode=ReadOnly");
                        conn.Open();
                        using var cmd = conn.CreateCommand();
                        cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Users';";
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count == 0)
                        {
                            info.ErrorMessage = "File ZIP chứa database không hợp lệ (không tìm thấy bảng Users).";
                            return info;
                        }
                    }
                    finally
                    {
                        try { if (File.Exists(tempDb)) File.Delete(tempDb); } catch { }
                    }

                    info.BackupTime = File.GetLastWriteTime(filePath).ToString("dd/MM/yyyy HH:mm:ss");
                    info.IsValid = true;
                }
                catch (InvalidDataException)
                {
                    info.ErrorMessage = "File này không phải là file ZIP hợp lệ.";
                }
                catch (Exception ex)
                {
                    info.ErrorMessage = $"Lỗi đọc file ZIP: {ex.Message}";
                }
            }
            else
            {
                // ── Đọc thông tin từ file .db truyền thống (backward compatible) ──
                try
                {
                    using var conn = new SqliteConnection($"Data Source={filePath};Mode=ReadOnly");
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Users';";
                    var count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count == 0)
                    {
                        info.ErrorMessage = "File này không phải là cơ sở dữ liệu Movie Vault hợp lệ.";
                        return info;
                    }

                    info.BackupTime = File.GetLastWriteTime(filePath).ToString("dd/MM/yyyy HH:mm:ss");
                    info.IsValid = true;
                }
                catch (Exception ex)
                {
                    info.ErrorMessage = $"Lỗi đọc file database: {ex.Message}";
                }
            }

            return info;
        }

        /// <summary>
        /// Đọc thông tin backup từ một thư mục (chứa .db + README.md).
        /// </summary>
        public static BackupInfo ReadBackupFolderInfo(string folderPath)
        {
            var info = new BackupInfo { FolderPath = folderPath };

            if (!Directory.Exists(folderPath))
            {
                info.ErrorMessage = "Thư mục không tồn tại.";
                return info;
            }

            // Ưu tiên tìm file .zip trước
            string targetZip = Path.Combine(folderPath, "MovieVault_Backup.zip");
            if (File.Exists(targetZip))
                return ReadBackupFileInfo(targetZip);

            // Fallback: tìm file .db
            string targetDb = Path.Combine(folderPath, "MovieVault_Backup.db");
            if (!File.Exists(targetDb))
            {
                var zipFiles = Directory.GetFiles(folderPath, "*.zip");
                if (zipFiles.Length > 0)
                {
                    Array.Sort(zipFiles);
                    return ReadBackupFileInfo(zipFiles[zipFiles.Length - 1]);
                }

                var dbFiles = Directory.GetFiles(folderPath, "MovieVault_Backup*.db");
                if (dbFiles.Length == 0)
                    dbFiles = Directory.GetFiles(folderPath, "*.db");

                if (dbFiles.Length == 0)
                {
                    info.ErrorMessage = "Không tìm thấy file backup (.zip hoặc .db) trong thư mục này.";
                    return info;
                }

                Array.Sort(dbFiles);
                targetDb = dbFiles[dbFiles.Length - 1];
            }

            return ReadBackupFileInfo(targetDb);
        }

        /// <summary>
        /// Khôi phục toàn bộ dữ liệu hệ thống từ file backup .db cụ thể.
        /// </summary>
        public async Task<(bool success, string message)> RestoreFromFileAsync(string filePath)
        {
            var info = ReadBackupFileInfo(filePath);
            if (!info.IsValid)
                return (false, info.ErrorMessage);

            try
            {
                await Task.Run(() =>
                {
                    string currentDb = DatabaseHelper.DbPath;
                    string appRoot = System.Windows.Forms.Application.StartupPath;

                    // Đóng tất cả connection SQLite đang mở trong app
                    SqliteConnection.ClearAllPools();
                    Thread.Sleep(500);

                    // Tạo bản sao lưu dự phòng của DB hiện tại
                    string fallbackPath = currentDb + ".fallback";
                    try
                    {
                        if (File.Exists(currentDb))
                            File.Copy(currentDb, fallbackPath, overwrite: true);
                    }
                    catch { }

                    try
                    {
                        if (info.IsZipFormat)
                        {
                            // ── Khôi phục từ file ZIP ──
                            string tempExtract = Path.Combine(Path.GetTempPath(), "MovieVault_Restore_" + Guid.NewGuid().ToString("N"));
                            try
                            {
                                ZipFile.ExtractToDirectory(filePath, tempExtract, overwriteFiles: true);

                                // 1. Khôi phục Database
                                string extractedDb = Path.Combine(tempExtract, "MovieVault_Backup.db");
                                if (File.Exists(extractedDb))
                                    File.Copy(extractedDb, currentDb, overwrite: true);

                                // 2. Khôi phục App_Data\CoverImages
                                string extractedCovers = Path.Combine(tempExtract, "App_Data", "CoverImages");
                                if (Directory.Exists(extractedCovers))
                                {
                                    string destCovers = Path.Combine(appRoot, "App_Data", "CoverImages");
                                    Directory.CreateDirectory(destCovers);
                                    CopyDirectoryRecursive(extractedCovers, destCovers);
                                }

                                // 3. Khôi phục App_Data\DetailImages
                                string extractedDetails = Path.Combine(tempExtract, "App_Data", "DetailImages");
                                if (Directory.Exists(extractedDetails))
                                {
                                    string destDetails = Path.Combine(appRoot, "App_Data", "DetailImages");
                                    Directory.CreateDirectory(destDetails);
                                    CopyDirectoryRecursive(extractedDetails, destDetails);
                                }

                                // 4. Khôi phục Data\Avatars
                                string extractedAvatars = Path.Combine(tempExtract, "Data", "Avatars");
                                if (Directory.Exists(extractedAvatars))
                                {
                                    string destAvatars = Path.Combine(appRoot, "Data", "Avatars");
                                    Directory.CreateDirectory(destAvatars);
                                    CopyDirectoryRecursive(extractedAvatars, destAvatars);
                                }

                                // 5. Khôi phục backup_paths.json
                                string extractedConfig = Path.Combine(tempExtract, CONFIG_FILE);
                                if (File.Exists(extractedConfig))
                                    File.Copy(extractedConfig, GetConfigFilePath(), overwrite: true);
                            }
                            finally
                            {
                                try { if (Directory.Exists(tempExtract)) Directory.Delete(tempExtract, recursive: true); } catch { }
                            }
                        }
                        else
                        {
                            // ── Khôi phục từ file .db truyền thống (backward compatible) ──
                            File.Copy(filePath, currentDb, overwrite: true);
                        }

                        // Kiểm tra tính toàn vẹn của DB vừa khôi phục
                        using (var testConn = new SqliteConnection($"Data Source={currentDb};Mode=ReadOnly"))
                        {
                            testConn.Open();
                            using var cmd = testConn.CreateCommand();
                            cmd.CommandText = "SELECT COUNT(*) FROM Users";
                            cmd.ExecuteScalar();
                        }

                        // Xóa file fallback khi đã thành công
                        if (File.Exists(fallbackPath))
                            File.Delete(fallbackPath);
                    }
                    catch (Exception)
                    {
                        // Rollback nếu có lỗi trong quá trình ghi
                        if (File.Exists(fallbackPath))
                            File.Copy(fallbackPath, currentDb, overwrite: true);
                        throw;
                    }
                });

                return (true, "Khôi phục thành công!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Restore failed: {ex.Message}");
                return (false, $"Khôi phục thất bại: {ex.Message}");
            }
        }

        /// <summary>
        /// Khôi phục toàn bộ dữ liệu từ thư mục backup (tự động tìm file .db hợp lệ trong thư mục).
        /// </summary>
        public async Task<(bool success, string message)> RestoreAsync(string backupFolderPath)
        {
            var info = ReadBackupFolderInfo(backupFolderPath);
            if (!info.IsValid)
                return (false, info.ErrorMessage);

            return await RestoreFromFileAsync(info.DbFilePath);
        }
    }
}
