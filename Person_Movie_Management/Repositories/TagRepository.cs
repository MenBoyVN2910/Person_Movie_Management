using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class TagRepository
    {
        private readonly string _connectionString;

        public TagRepository()
        {
            _connectionString = DatabaseHelper.ConnectionString;
        }

        public List<Tag> GetAllByUser(int userId)
        {
            var tags = new List<Tag>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, TagName, ColorHex FROM Tags WHERE UserId = @UserId ORDER BY TagName ASC";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(new Tag
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TagName = reader.GetString(2),
                    ColorHex = reader.IsDBNull(3) ? "#8b5cf6" : reader.GetString(3)
                });
            }

            return tags;
        }

        public Tag? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, TagName, ColorHex FROM Tags WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Tag
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TagName = reader.GetString(2),
                    ColorHex = reader.IsDBNull(3) ? "#8b5cf6" : reader.GetString(3)
                };
            }
            return null;
        }

        public Tag? GetByName(int userId, string tagName, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return null;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, TagName, ColorHex FROM Tags WHERE UserId = @UserId AND TRIM(TagName) = @TagName COLLATE NOCASE AND Id != @ExcludeId LIMIT 1";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@TagName", tagName.Trim());
            command.Parameters.AddWithValue("@ExcludeId", excludeId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Tag
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TagName = reader.GetString(2),
                    ColorHex = reader.IsDBNull(3) ? "#8b5cf6" : reader.GetString(3)
                };
            }
            return null;
        }

        public bool Exists(int userId, string tagName, int excludeId = 0)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return false;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM Tags WHERE UserId = @UserId AND TRIM(TagName) = @TagName COLLATE NOCASE AND Id != @ExcludeId LIMIT 1";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@TagName", tagName.Trim());
            command.Parameters.AddWithValue("@ExcludeId", excludeId);

            var result = command.ExecuteScalar();
            return result != null;
        }

        public int Insert(Tag tag)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Tags (UserId, TagName, ColorHex)
                    VALUES (@UserId, @TagName, @ColorHex);
                    SELECT last_insert_rowid();
                ";
                command.Parameters.AddWithValue("@UserId", tag.UserId);
                command.Parameters.AddWithValue("@TagName", tag.TagName.Trim());
                command.Parameters.AddWithValue("@ColorHex", tag.ColorHex ?? (object)DBNull.Value);

                var newId = command.ExecuteScalar();
                return newId != null ? Convert.ToInt32(newId) : 0;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                // Unique constraint failed: (UserId, TagName) already exists
                return -1;
            }
        }

        public bool Update(Tag tag)
        {
            try
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE Tags SET TagName = @TagName, ColorHex = @ColorHex WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", tag.Id);
                command.Parameters.AddWithValue("@TagName", tag.TagName.Trim());
                command.Parameters.AddWithValue("@ColorHex", tag.ColorHex ?? (object)DBNull.Value);

                return command.ExecuteNonQuery() > 0;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                // Unique constraint failed
                return false;
            }
        }

        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Tags WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public Dictionary<string, int> GetTagDistribution(int userId)
        {
            var dist = new Dictionary<string, int>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT t.TagName, COUNT(mt.MovieId)
                FROM Tags t
                JOIN MovieTags mt ON t.Id = mt.TagId
                WHERE t.UserId = @UserId
                GROUP BY t.TagName
                ORDER BY COUNT(mt.MovieId) DESC
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                dist[reader.GetString(0)] = reader.GetInt32(1);
            }

            return dist;
        }

        // Movie Tags Link
        public List<Tag> GetTagsForMovie(int movieId)
        {
            var tags = new List<Tag>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT t.Id, t.UserId, t.TagName, t.ColorHex
                FROM Tags t
                INNER JOIN MovieTags mt ON t.Id = mt.TagId
                WHERE mt.MovieId = @MovieId
            ";
            command.Parameters.AddWithValue("@MovieId", movieId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(new Tag
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TagName = reader.GetString(2),
                    ColorHex = reader.IsDBNull(3) ? "#8b5cf6" : reader.GetString(3)
                });
            }
            return tags;
        }

        public Dictionary<int, List<Tag>> GetTagsForMovies(List<int> movieIds)
        {
            var result = new Dictionary<int, List<Tag>>();
            if (movieIds.Count == 0) return result;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string ids = string.Join(",", movieIds);
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT mt.MovieId, t.Id, t.UserId, t.TagName, t.ColorHex
                FROM Tags t
                INNER JOIN MovieTags mt ON t.Id = mt.TagId
                WHERE mt.MovieId IN ({ids})
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int movieId = reader.GetInt32(0);
                if (!result.ContainsKey(movieId)) result[movieId] = new List<Tag>();
                
                result[movieId].Add(new Tag
                {
                    Id = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    TagName = reader.GetString(3),
                    ColorHex = reader.IsDBNull(4) ? "#8b5cf6" : reader.GetString(4)
                });
            }

            return result;
        }

        public async System.Threading.Tasks.Task<Dictionary<int, List<Tag>>> GetTagsForMoviesAsync(List<int> movieIds)
        {
            var result = new Dictionary<int, List<Tag>>();
            if (movieIds.Count == 0) return result;

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string ids = string.Join(",", movieIds);
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT mt.MovieId, t.Id, t.UserId, t.TagName, t.ColorHex
                FROM Tags t
                INNER JOIN MovieTags mt ON t.Id = mt.TagId
                WHERE mt.MovieId IN ({ids})
            ";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int movieId = reader.GetInt32(0);
                if (!result.ContainsKey(movieId)) result[movieId] = new List<Tag>();
                
                result[movieId].Add(new Tag
                {
                    Id = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    TagName = reader.GetString(3),
                    ColorHex = reader.IsDBNull(4) ? "#8b5cf6" : reader.GetString(4)
                });
            }

            return result;
        }

        public bool SetMovieTags(int movieId, List<int> tagIds)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var cmdDelete = connection.CreateCommand();
                cmdDelete.Transaction = transaction;
                cmdDelete.CommandText = "DELETE FROM MovieTags WHERE MovieId = @MovieId";
                cmdDelete.Parameters.AddWithValue("@MovieId", movieId);
                cmdDelete.ExecuteNonQuery();

                using var cmdInsert = connection.CreateCommand();
                cmdInsert.Transaction = transaction;
                cmdInsert.CommandText = "INSERT INTO MovieTags (MovieId, TagId) VALUES (@MovieId, @TagId)";
                var movieIdParam = cmdInsert.Parameters.Add("@MovieId", SqliteType.Integer);
                var tagIdParam = cmdInsert.Parameters.Add("@TagId", SqliteType.Integer);
                movieIdParam.Value = movieId;

                foreach (var tagId in tagIds)
                {
                    tagIdParam.Value = tagId;
                    cmdInsert.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        // Audio Tags Link
        public List<Tag> GetTagsForAudio(int audioId)
        {
            var tags = new List<Tag>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT t.Id, t.UserId, t.TagName, t.ColorHex
                FROM Tags t
                INNER JOIN AudioTags mt ON t.Id = mt.TagId
                WHERE mt.AudioId = @AudioId
            ";
            command.Parameters.AddWithValue("@AudioId", audioId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tags.Add(new Tag
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    TagName = reader.GetString(2),
                    ColorHex = reader.IsDBNull(3) ? "#8b5cf6" : reader.GetString(3)
                });
            }

            return tags;
        }

        public Dictionary<int, List<Tag>> GetTagsForAudios(List<int> audioIds)
        {
            var result = new Dictionary<int, List<Tag>>();
            if (audioIds.Count == 0) return result;

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string ids = string.Join(",", audioIds);
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT mt.AudioId, t.Id, t.UserId, t.TagName, t.ColorHex
                FROM Tags t
                INNER JOIN AudioTags mt ON t.Id = mt.TagId
                WHERE mt.AudioId IN ({ids})
            ";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int audioId = reader.GetInt32(0);
                if (!result.ContainsKey(audioId)) result[audioId] = new List<Tag>();
                
                result[audioId].Add(new Tag
                {
                    Id = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    TagName = reader.GetString(3),
                    ColorHex = reader.IsDBNull(4) ? "#8b5cf6" : reader.GetString(4)
                });
            }

            return result;
        }

        public async System.Threading.Tasks.Task<Dictionary<int, List<Tag>>> GetTagsForAudiosAsync(List<int> audioIds)
        {
            var result = new Dictionary<int, List<Tag>>();
            if (audioIds.Count == 0) return result;

            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            string ids = string.Join(",", audioIds);
            using var command = connection.CreateCommand();
            command.CommandText = $@"
                SELECT mt.AudioId, t.Id, t.UserId, t.TagName, t.ColorHex
                FROM Tags t
                INNER JOIN AudioTags mt ON t.Id = mt.TagId
                WHERE mt.AudioId IN ({ids})
            ";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                int audioId = reader.GetInt32(0);
                if (!result.ContainsKey(audioId)) result[audioId] = new List<Tag>();
                
                result[audioId].Add(new Tag
                {
                    Id = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    TagName = reader.GetString(3),
                    ColorHex = reader.IsDBNull(4) ? "#8b5cf6" : reader.GetString(4)
                });
            }

            return result;
        }

        public bool SetAudioTags(int audioId, List<int> tagIds)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var cmdDelete = connection.CreateCommand();
                cmdDelete.Transaction = transaction;
                cmdDelete.CommandText = "DELETE FROM AudioTags WHERE AudioId = @AudioId";
                cmdDelete.Parameters.AddWithValue("@AudioId", audioId);
                cmdDelete.ExecuteNonQuery();

                using var cmdInsert = connection.CreateCommand();
                cmdInsert.Transaction = transaction;
                cmdInsert.CommandText = "INSERT INTO AudioTags (AudioId, TagId) VALUES (@AudioId, @TagId)";
                var audioIdParam = cmdInsert.Parameters.Add("@AudioId", SqliteType.Integer);
                var tagIdParam = cmdInsert.Parameters.Add("@TagId", SqliteType.Integer);
                audioIdParam.Value = audioId;

                foreach (var tagId in tagIds)
                {
                    tagIdParam.Value = tagId;
                    cmdInsert.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
