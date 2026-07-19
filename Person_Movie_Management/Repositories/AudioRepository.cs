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
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt {dataCol}
                FROM Audios
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var audio = new Audio
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AudioCode = reader.GetString(2),
                    CoverImage = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Note = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Rating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    IsFavorite = reader.IsDBNull(6) ? false : reader.GetInt32(6) == 1,
                    CreatedAt = reader.GetDateTime(7),
                    UpdatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };

                if (includeAudioData && !reader.IsDBNull(9))
                {
                    audio.AudioData = (byte[])reader.GetValue(9);
                }

                audios.Add(audio);
            }

            return audios;
        }

        public Audio? GetById(int id, bool includeAudioData = false)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string dataCol = includeAudioData ? ", AudioData" : "";
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt {dataCol}
                FROM Audios
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var audio = new Audio
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AudioCode = reader.GetString(2),
                    CoverImage = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Note = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Rating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    IsFavorite = reader.IsDBNull(6) ? false : reader.GetInt32(6) == 1,
                    CreatedAt = reader.GetDateTime(7),
                    UpdatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };

                if (includeAudioData && !reader.IsDBNull(9))
                {
                    audio.AudioData = (byte[])reader.GetValue(9);
                }

                return audio;
            }
            return null;
        }

        public Audio? GetByCode(int userId, string audioCode)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt
                FROM Audios
                WHERE UserId = @UserId AND AudioCode = @AudioCode
            ";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@AudioCode", audioCode);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Audio
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AudioCode = reader.GetString(2),
                    CoverImage = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Note = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Rating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    IsFavorite = reader.IsDBNull(6) ? false : reader.GetInt32(6) == 1,
                    CreatedAt = reader.GetDateTime(7),
                    UpdatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                };
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
            command.Parameters.AddWithValue("@CreatedAt", audio.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

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
            
            // If AudioData is null, don't update it to avoid overwriting existing data with null
            // unless we explicitly want to remove it, but usually we just load list (AudioData=null) and update properties.
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
            command.CommandText = "DELETE FROM Audios WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

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
                SELECT Id, UserId, AudioCode, CoverImage, Note, Rating, IsFavorite, CreatedAt, UpdatedAt
                FROM Audios
                WHERE UserId = @UserId AND IsFavorite = 1
                ORDER BY CreatedAt DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                audios.Add(new Audio
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    AudioCode = reader.GetString(2),
                    CoverImage = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Note = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Rating = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    IsFavorite = true,
                    CreatedAt = reader.GetDateTime(7),
                    UpdatedAt = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                });
            }

            return audios;
        }
    }
}
