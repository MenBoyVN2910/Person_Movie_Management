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
        public static string ConnectionString => $"Data Source={DbPath};Foreign Keys=True;";

        public static void Initialize()
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            // Enable WAL mode and memory cache for highest concurrency and performance
            using var pragmaCmd = connection.CreateCommand();
            pragmaCmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL; PRAGMA cache_size=-16000; PRAGMA temp_store=MEMORY;";
            pragmaCmd.ExecuteNonQuery();

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

            // Tags Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tags (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    TagName TEXT NOT NULL,
                    ColorHex TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    UNIQUE(UserId, TagName)
                );
            ";
            command.ExecuteNonQuery();

            // MovieTags Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS MovieTags (
                    MovieId INTEGER NOT NULL,
                    TagId INTEGER NOT NULL,
                    PRIMARY KEY (MovieId, TagId),
                    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE,
                    FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
                );
            ";
            command.ExecuteNonQuery();

            // Actors Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Actors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    AvatarPath TEXT,
                    DateOfBirth TEXT,
                    Nationality TEXT,
                    Bio TEXT,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
                    UpdatedAt TEXT,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
            ";
            command.ExecuteNonQuery();

            // ActorImages Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS ActorImages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ActorId INTEGER NOT NULL,
                    ImagePath TEXT NOT NULL,
                    SortOrder INTEGER DEFAULT 0,
                    FOREIGN KEY (ActorId) REFERENCES Actors(Id) ON DELETE CASCADE
                );
            ";
            command.ExecuteNonQuery();

            // MovieActors Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS MovieActors (
                    MovieId INTEGER NOT NULL,
                    ActorId INTEGER NOT NULL,
                    Role TEXT,
                    PRIMARY KEY (MovieId, ActorId),
                    FOREIGN KEY (MovieId) REFERENCES Movies(Id) ON DELETE CASCADE,
                    FOREIGN KEY (ActorId) REFERENCES Actors(Id) ON DELETE CASCADE
                );
            ";
            command.ExecuteNonQuery();

            // AudioTags Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS AudioTags (
                    AudioId INTEGER NOT NULL,
                    TagId INTEGER NOT NULL,
                    PRIMARY KEY (AudioId, TagId),
                    FOREIGN KEY (AudioId) REFERENCES Audios(Id) ON DELETE CASCADE,
                    FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
                );
            ";
            command.ExecuteNonQuery();

            // Playlists Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Playlists (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    Description TEXT,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
            ";
            command.ExecuteNonQuery();

            // PlaylistItems Table (ItemType: 1=Movie, 2=Audio)
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS PlaylistItems (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlaylistId INTEGER NOT NULL,
                    ItemId INTEGER NOT NULL,
                    ItemType INTEGER NOT NULL, 
                    SortOrder INTEGER DEFAULT 0,
                    FOREIGN KEY (PlaylistId) REFERENCES Playlists(Id) ON DELETE CASCADE
                );
            ";
            command.ExecuteNonQuery();

            // Nationalities Table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Nationalities (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now','localtime')),
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    UNIQUE(UserId, Name)
                );
            ";
            command.ExecuteNonQuery();

            // Migrations for Soft Delete
            AddColumnIfNotExists(connection, "Movies", "IsDeleted", "INTEGER DEFAULT 0");
            AddColumnIfNotExists(connection, "Audios", "IsDeleted", "INTEGER DEFAULT 0");
            AddColumnIfNotExists(connection, "Movies", "DeletedAt", "TEXT");
            AddColumnIfNotExists(connection, "Audios", "DeletedAt", "TEXT");

            // Migrations for Playlist Enhancement
            AddColumnIfNotExists(connection, "Playlists", "CoverImage", "TEXT");
            AddColumnIfNotExists(connection, "Playlists", "IsPrivate", "INTEGER DEFAULT 0");

            // Migrations for Feature 3: Watch History & Progress
            AddColumnIfNotExists(connection, "Movies", "WatchProgress", "INTEGER DEFAULT 0");
            AddColumnIfNotExists(connection, "Movies", "LastWatched", "TEXT");
            AddColumnIfNotExists(connection, "Audios", "WatchProgress", "INTEGER DEFAULT 0");
            AddColumnIfNotExists(connection, "Audios", "LastWatched", "TEXT");

            // Add Indexes for performance optimization
            command.CommandText = @"
                CREATE INDEX IF NOT EXISTS idx_movies_user_deleted ON Movies(UserId, IsDeleted);
                CREATE INDEX IF NOT EXISTS idx_movies_user_source ON Movies(UserId, SourceType, IsDeleted);
                CREATE INDEX IF NOT EXISTS idx_movies_user_favorite ON Movies(UserId, IsFavorite, IsDeleted);
                CREATE INDEX IF NOT EXISTS idx_audios_user_deleted ON Audios(UserId, IsDeleted);
                CREATE INDEX IF NOT EXISTS idx_audios_user_favorite ON Audios(UserId, IsFavorite, IsDeleted);
                CREATE INDEX IF NOT EXISTS idx_movietags_movie ON MovieTags(MovieId);
                CREATE INDEX IF NOT EXISTS idx_movietags_tag ON MovieTags(TagId);
                CREATE INDEX IF NOT EXISTS idx_audiotags_audio ON AudioTags(AudioId);
                CREATE INDEX IF NOT EXISTS idx_playlistitems_playlist ON PlaylistItems(PlaylistId);
                CREATE INDEX IF NOT EXISTS idx_actors_user ON Actors(UserId);
                CREATE INDEX IF NOT EXISTS idx_actorimages_actor ON ActorImages(ActorId);
                CREATE INDEX IF NOT EXISTS idx_movieactors_actor ON MovieActors(ActorId);
                CREATE INDEX IF NOT EXISTS idx_playlists_user ON Playlists(UserId);
            ";
            command.ExecuteNonQuery();

            // Auto-heal any local movie records that were mistakenly marked as SourceType = 0
            try
            {
                using var fixSourceCmd = connection.CreateCommand();
                fixSourceCmd.CommandText = @"
                    UPDATE Movies 
                    SET SourceType = 1 
                    WHERE SourceType = 0 
                      AND MediaUrl IS NOT NULL 
                      AND (MediaUrl LIKE '%\%' OR MediaUrl LIKE '%:\%' OR (MediaUrl NOT LIKE 'http://%' AND MediaUrl NOT LIKE 'https://%'));
                ";
                fixSourceCmd.ExecuteNonQuery();
            }
            catch { }
        }

        private static void AddColumnIfNotExists(SqliteConnection connection, string tableName, string columnName, string columnDefinition)
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"SELECT {columnName} FROM {tableName} LIMIT 1";
                command.ExecuteScalar(); // Will throw if column doesn't exist
            }
            catch (SqliteException)
            {
                // Column doesn't exist, add it
                using var alterCmd = connection.CreateCommand();
                alterCmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition}";
                alterCmd.ExecuteNonQuery();
            }
        }
    }
}
