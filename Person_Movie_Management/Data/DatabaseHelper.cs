using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Windows.Forms;

namespace Person_Movie_Management.Data
{
    public static class DatabaseHelper
    {
        public static string AppDataFolder => Path.Combine(Application.StartupPath, "App_Data");
        public static string DbPath => Path.Combine(AppDataFolder, "AppDatabase.db");
        public static string ConnectionString => $"Data Source={DbPath}";

        public static void Initialize()
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            
            // Users Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    DisplayName TEXT NOT NULL,
                    Email TEXT,
                    PasswordHash TEXT NOT NULL,
                    AvatarPath TEXT,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
                    IsActive INTEGER NOT NULL DEFAULT 1
                );
            ";
            command.ExecuteNonQuery();

            // Movies Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Movies (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    MovieCode TEXT NOT NULL,
                    SourceType INTEGER NOT NULL DEFAULT 0,
                    MediaUrl TEXT,
                    CoverImage TEXT,
                    Note TEXT,
                    Rating INTEGER DEFAULT 0,
                    IsFavorite INTEGER DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
                    UpdatedAt TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    UNIQUE(UserId, MovieCode)
                );
            ";
            command.ExecuteNonQuery();

            // MovieImages Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS MovieImages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    MovieId INTEGER NOT NULL,
                    ImagePath TEXT NOT NULL,
                    SortOrder INTEGER DEFAULT 0,
                    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE
                );
            ";
            command.ExecuteNonQuery();

            // AppSettings Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS AppSettings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    SettingKey TEXT NOT NULL,
                    SettingValue TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    UNIQUE(UserId, SettingKey)
                );
            ";
            command.ExecuteNonQuery();

            // Audios Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Audios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    AudioCode TEXT NOT NULL,
                    AudioData BLOB,
                    CoverImage TEXT,
                    Note TEXT,
                    Rating INTEGER DEFAULT 0,
                    IsFavorite INTEGER DEFAULT 0,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
                    UpdatedAt TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    UNIQUE(UserId, AudioCode)
                );
            ";
            command.ExecuteNonQuery();
        }
    }
}
