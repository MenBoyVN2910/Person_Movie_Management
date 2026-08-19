using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;
using System.Data;

namespace Person_Movie_Management.Repositories
{
    public class AudioRepository
    {
        private readonly string _connectionString;

        public AudioRepository()
        {
            _connectionString = DatabaseHelper.ConnectionString;
        }

        public List<Audio> GetAllByUser(int userId, bool includeAudioData = false)
        {
            var audios = new List<Audio>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string dataCol = includeAudioData ? ", AudioData" : "";
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, WatchProgress, LastWatched {dataCol}
                FROM Audios
                WHERE UserId = @UserId AND IsDeleted = 0
                ORDER BY CreatedAt DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                audios.Add(MapAudio(reader, includeAudioData));
            }

            return audios;
        }

        public Audio? GetById(int id, bool includeAudioData = false, bool includeDeleted = false)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string dataCol = includeAudioData ? ", AudioData" : "";
            using var command = connection.CreateCommand();
            string condition = includeDeleted ? "Id = @Id" : "Id = @Id AND IsDeleted = 0";
            command.CommandText = $@"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, WatchProgress, LastWatched {dataCol}
                FROM Audios
                WHERE {condition}
            ";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapAudio(reader, includeAudioData);
            }
            return null;
        }

        public Audio? GetByCode(int userId, string audioCode)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, WatchProgress, LastWatched
                FROM Audios 
                WHERE UserId = @UserId AND AudioCode = @AudioCode COLLATE NOCASE AND IsDeleted = 0;";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@AudioCode", audioCode);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapAudio(reader, false);
            }
            return null;
        }

        public bool Insert(Audio audio)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Audios (UserId, AudioCode, AudioData, CoverImage, Note, Rating, IsFavorite, CreatedAt)
                VALUES (@UserId, @AudioCode, @AudioData, @CoverImage, @Note, @Rating, @IsFavorite, @CreatedAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@UserId", audio.UserId);
            command.Parameters.AddWithValue("@AudioCode", audio.AudioCode);
            
            var audioDataParam = command.Parameters.Add("@AudioData", SqliteType.Blob);
            audioDataParam.Value = audio.AudioData ?? (object)DBNull.Value;
            
            command.Parameters.AddWithValue("@CoverImage", audio.CoverImage ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Note", audio.Note ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Rating", audio.Rating);
            command.Parameters.AddWithValue("@IsFavorite", audio.IsFavorite ? 1 : 0);
            command.Parameters.AddWithValue("@CreatedAt", (audio.CreatedAt == default ? DateTime.Now : audio.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));

            var newId = command.ExecuteScalar();
            if (newId != null)
            {
                audio.Id = Convert.ToInt32(newId);
                return true;
            }
            return false;
        }

        public bool Update(Audio audio)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            
            string updateDataSql = audio.AudioData != null ? ", AudioData = @AudioData" : "";

            command.CommandText = $@"
                UPDATE Audios SET 
                    AudioCode = @AudioCode,
                    CoverImage = @CoverImage,
                    Note = @Note,
                    Rating = @Rating,
                    IsFavorite = @IsFavorite,
                    UpdatedAt = @UpdatedAt
                    {updateDataSql}
                WHERE Id = @Id
            ";
            
            command.Parameters.AddWithValue("@Id", audio.Id);
            command.Parameters.AddWithValue("@AudioCode", audio.AudioCode);
            command.Parameters.AddWithValue("@CoverImage", audio.CoverImage ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Note", audio.Note ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Rating", audio.Rating);
            command.Parameters.AddWithValue("@IsFavorite", audio.IsFavorite ? 1 : 0);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            if (audio.AudioData != null)
            {
                var audioDataParam = command.Parameters.Add("@AudioData", SqliteType.Blob);
                audioDataParam.Value = audio.AudioData;
            }

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Audios SET IsDeleted = 1, DeletedAt = datetime('now','localtime'), AudioCode = AudioCode || '_$DEL$_' || Id WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public void DeleteAll(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = "UPDATE Audios SET IsDeleted = 1, DeletedAt = datetime('now','localtime'), AudioCode = AudioCode || '_$DEL$_' || Id WHERE UserId = @UserId AND IsDeleted = 0";
            command.Parameters.AddWithValue("@UserId", userId);
            
            command.ExecuteNonQuery();
        }

        public void HardDeleteAll(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM PlaylistItems WHERE ItemType = 2 AND ItemId IN (SELECT Id FROM Audios WHERE UserId = @UserId);
                DELETE FROM Audios WHERE UserId = @UserId;
            ";
            command.Parameters.AddWithValue("@UserId", userId);
            
            command.ExecuteNonQuery();
        }

        public bool HardDelete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                DELETE FROM PlaylistItems WHERE ItemType = 2 AND ItemId = @Id;
                DELETE FROM Audios WHERE Id = @Id;
            ";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateProgress(int audioId, int progress)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Audios
                SET WatchProgress = @Progress,
                    LastWatched = @LastWatched
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Progress", progress);
            command.Parameters.AddWithValue("@LastWatched", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@Id", audioId);

            return command.ExecuteNonQuery() > 0;
        }

        public bool ToggleFavorite(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Audios 
                SET IsFavorite = CASE WHEN IsFavorite = 1 THEN 0 ELSE 1 END 
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }
        
        public List<Audio> GetFavorites(int userId)
        {
            var audios = new List<Audio>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, WatchProgress, LastWatched
                FROM Audios
                WHERE UserId = @UserId AND IsFavorite = 1 AND IsDeleted = 0
                ORDER BY CreatedAt DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                audios.Add(MapAudio(reader, false));
            }
            return audios;
        }

        public async System.Threading.Tasks.Task<List<Audio>> GetFavoritesAsync(int userId)
        {
            var audios = new List<Audio>();
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, WatchProgress, LastWatched
                FROM Audios
                WHERE UserId = @UserId AND IsFavorite = 1 AND IsDeleted = 0
                ORDER BY CreatedAt DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                audios.Add(MapAudio(reader, false));
            }

            return audios;
        }

        public List<Audio> GetDeleted(int userId)
        {
            var audios = new List<Audio>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt, IsDeleted, DeletedAt, WatchProgress, LastWatched
                FROM Audios
                WHERE UserId = @UserId AND IsDeleted = 1
                ORDER BY DeletedAt DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                audios.Add(MapAudio(reader, false));
            }

            return audios;
        }

        private static Audio MapAudio(SqliteDataReader reader, bool includeAudioData = false)
        {
            var audio = new Audio
            {
                Id = Convert.ToInt32(reader["Id"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                AudioCode = reader["AudioCode"]?.ToString() ?? "",
                CoverImage = reader["CoverImage"] != DBNull.Value ? reader["CoverImage"].ToString() : null,
                Note = reader["Note"] != DBNull.Value ? reader["Note"].ToString() : null,
                Rating = reader["Rating"] != DBNull.Value ? Convert.ToInt32(reader["Rating"]) : 0,
                IsFavorite = reader["IsFavorite"] != DBNull.Value && Convert.ToInt32(reader["IsFavorite"]) == 1,
                CreatedAt = ParseDateTimeSafe(reader["CreatedAt"]),
                UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? ParseDateTimeNullable(reader["UpdatedAt"]) : null,
                IsDeleted = reader["IsDeleted"] != DBNull.Value && Convert.ToInt32(reader["IsDeleted"]) == 1,
                DeletedAt = reader["DeletedAt"] != DBNull.Value ? ParseDateTimeNullable(reader["DeletedAt"]) : null,
                WatchProgress = reader["WatchProgress"] != DBNull.Value ? Convert.ToInt32(reader["WatchProgress"]) : 0,
                LastWatched = reader["LastWatched"] != DBNull.Value ? ParseDateTimeNullable(reader["LastWatched"]) : null
            };

            if (includeAudioData && HasColumn(reader, "AudioData") && reader["AudioData"] != DBNull.Value)
            {
                audio.AudioData = (byte[])reader["AudioData"];
            }

            return audio;
        }

        private static DateTime ParseDateTimeSafe(object? val)
        {
            if (val == null || val == DBNull.Value) return DateTime.Now;
            if (val is DateTime dt) return dt;
            if (DateTime.TryParse(val.ToString(), out var parsed)) return parsed;
            return DateTime.Now;
        }

        private static DateTime? ParseDateTimeNullable(object? val)
        {
            if (val == null || val == DBNull.Value) return null;
            if (val is DateTime dt) return dt;
            if (DateTime.TryParse(val.ToString(), out var parsed)) return parsed;
            return null;
        }

        private static bool HasColumn(SqliteDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
