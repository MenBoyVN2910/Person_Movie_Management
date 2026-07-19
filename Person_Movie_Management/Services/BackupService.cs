using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Helpers;

namespace Person_Movie_Management.Services
{
    public class BackupService
    {
        private int _currentUserId;
        private List<string> _backupPaths = new List<string>();
        private System.Windows.Forms.Timer _debounceTimer;
        private bool _isBackingUp = false;

        public BackupService()
        {
            _debounceTimer = new System.Windows.Forms.Timer();
            _debounceTimer.Interval = 3000; // 3 seconds
            _debounceTimer.Tick += DebounceTimer_Tick;
        }

        public void Start(int userId)
        {
            _currentUserId = userId;
            LoadBackupPaths();
            
            // Subscribe to data changes
            DataCache.DataInvalidated -= DataCache_DataInvalidated;
            DataCache.DataInvalidated += DataCache_DataInvalidated;
        }

        public void Stop()
        {
            DataCache.DataInvalidated -= DataCache_DataInvalidated;
            _debounceTimer?.Stop();
        }

        public List<string> GetBackupPaths()
        {
            return new List<string>(_backupPaths);
        }

        public void AddBackupPath(string path)
        {
            if (!_backupPaths.Contains(path))
            {
                _backupPaths.Add(path);
                SaveBackupPaths();
                TriggerBackup(); // Initial backup when new path added
            }
        }

        public void RemoveBackupPath(string path)
        {
            if (_backupPaths.Contains(path))
            {
                _backupPaths.Remove(path);
                SaveBackupPaths();
            }
        }

        private void LoadBackupPaths()
        {
            string configPath = GetConfigFilePath();
            if (File.Exists(configPath))
            {
                try
                {
                    string json = File.ReadAllText(configPath);
                    var paths = JsonSerializer.Deserialize<List<string>>(json);
                    if (paths != null)
                    {
                        _backupPaths = paths;
                    }
                }
                catch
                {
                    _backupPaths = new List<string>();
                }
            }
            else
            {
                _backupPaths = new List<string>();
            }
        }

        private void SaveBackupPaths()
        {
            try
            {
                string configPath = GetConfigFilePath();
                string json = JsonSerializer.Serialize(_backupPaths);
                File.WriteAllText(configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error saving backup paths: " + ex.Message);
            }
        }

        private string GetConfigFilePath()
        {
            return Path.Combine(DatabaseHelper.AppDataFolder, $"backup_paths_{_currentUserId}.json");
        }

        private void DataCache_DataInvalidated()
        {
            TriggerBackup();
        }

        public void TriggerBackup()
        {
            if (_backupPaths.Count == 0) return;

            // Use WinForms Timer for debouncing to ensure it runs on UI thread or manage thread safety
            if (_debounceTimer.Enabled)
            {
                _debounceTimer.Stop();
            }
            _debounceTimer.Start();
        }

        private async void DebounceTimer_Tick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            await PerformBackupAsync();
        }

        public async Task PerformBackupAsync()
        {
            if (_isBackingUp || _backupPaths.Count == 0) return;
            
            _isBackingUp = true;
            try
            {
                await Task.Run(() =>
                {
                    foreach (var folder in _backupPaths)
                    {
                        if (Directory.Exists(folder))
                        {
                            try
                            {
                                string backupFile = Path.Combine(folder, "AppDatabase_Backup.db");
                                string metaFile = Path.Combine(folder, "AppDatabase_Backup.meta.json");

                                // If file exists, SQLite VACUUM INTO will fail. We need to delete it first.
                                if (File.Exists(backupFile))
                                {
                                    File.Delete(backupFile);
                                }

                                using (var connection = new SqliteConnection(DatabaseHelper.ConnectionString))
                                {
                                    connection.Open();
                                    using var cmd = connection.CreateCommand();
                                    // VACUUM INTO creates a perfect snapshot of the database
                                    cmd.CommandText = $"VACUUM INTO '{backupFile.Replace("'", "''")}'";
                                    cmd.ExecuteNonQuery();
                                }

                                // Write metadata
                                var meta = new
                                {
                                    BackupTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                    UserId = _currentUserId,
                                    Version = "1.0"
                                };
                                File.WriteAllText(metaFile, JsonSerializer.Serialize(meta, new JsonSerializerOptions { WriteIndented = true }));
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to backup to {folder}: {ex.Message}");
                            }
                        }
                    }
                });
            }
            finally
            {
                _isBackingUp = false;
            }
        }

        public async Task<bool> RestoreAsync(string backupFilePath)
        {
            if (!File.Exists(backupFilePath)) return false;

            try
            {
                await Task.Run(() =>
                {
                    // Copy the backup file to replace the current DB
                    string currentDb = DatabaseHelper.DbPath;
                    
                    // We can't overwrite it while it's in use, but SQLite in WAL mode might lock it.
                    // Actually, if we're in the app, connections might be open.
                    // SqliteConnection.ClearAllPools() can help.
                    SqliteConnection.ClearAllPools();
                    
                    // Small delay to let pools clear
                    Thread.Sleep(500);

                    // Backup the current one just in case
                    string fallback = currentDb + ".fallback";
                    File.Copy(currentDb, fallback, true);

                    try
                    {
                        File.Copy(backupFilePath, currentDb, true);
                        
                        // Delete fallback on success
                        if (File.Exists(fallback)) File.Delete(fallback);
                    }
                    catch
                    {
                        // Restore fallback on failure
                        if (File.Exists(fallback))
                        {
                            File.Copy(fallback, currentDb, true);
                        }
                        throw;
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Restore failed: {ex.Message}");
                return false;
            }
        }
    }
}
