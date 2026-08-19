using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Helpers;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class ActorRepository
    {
        private static readonly ConcurrentDictionary<int, List<ActorImage>> _imagesCache = new();

        static ActorRepository()
        {
            DataCache.DataInvalidated += InvalidateCache;
        }

        public static void InvalidateCache()
        {
            _imagesCache.Clear();
        }

        public static void InvalidateCache(int actorId)
        {
            _imagesCache.TryRemove(actorId, out _);
        }
        public List<Actor> GetAllByUser(int userId)
        {
            var actors = new List<Actor>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM Actors WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                actors.Add(MapActor(reader));
            }
            return actors;
        }

        public Actor? GetById(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Actors WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapActor(reader);
            }
            return null;
        }

        public Actor? GetByName(int userId, string name, int excludeId = 0)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Actors WHERE UserId = @UserId AND TRIM(Name) = @Name COLLATE NOCASE AND Id != @ExcludeId LIMIT 1";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Name", name.Trim());
            command.Parameters.AddWithValue("@ExcludeId", excludeId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapActor(reader);
            }
            return null;
        }

        public int Insert(Actor actor)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                INSERT INTO Actors (UserId, Name, AvatarPath, DateOfBirth, Nationality, Bio)
                VALUES (@UserId, @Name, @AvatarPath, @DateOfBirth, @Nationality, @Bio);
                SELECT last_insert_rowid();
            ";
            
            command.Parameters.AddWithValue("@UserId", actor.UserId);
            command.Parameters.AddWithValue("@Name", actor.Name);
            command.Parameters.AddWithValue("@AvatarPath", (object?)actor.AvatarPath ?? DBNull.Value);
            command.Parameters.AddWithValue("@DateOfBirth", (object?)actor.DateOfBirth ?? DBNull.Value);
            command.Parameters.AddWithValue("@Nationality", (object?)actor.Nationality ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bio", (object?)actor.Bio ?? DBNull.Value);

            var result = command.ExecuteScalar();
            if (result != null)
            {
                actor.Id = Convert.ToInt32(result);
                return actor.Id;
            }
            return 0;
        }

        public bool Update(Actor actor)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                UPDATE Actors 
                SET Name = @Name,
                    AvatarPath = @AvatarPath,
                    DateOfBirth = @DateOfBirth,
                    Nationality = @Nationality,
                    Bio = @Bio,
                    UpdatedAt = datetime('now','localtime')
                WHERE Id = @Id
            ";
            
            command.Parameters.AddWithValue("@Id", actor.Id);
            command.Parameters.AddWithValue("@Name", actor.Name);
            command.Parameters.AddWithValue("@AvatarPath", (object?)actor.AvatarPath ?? DBNull.Value);
            command.Parameters.AddWithValue("@DateOfBirth", (object?)actor.DateOfBirth ?? DBNull.Value);
            command.Parameters.AddWithValue("@Nationality", (object?)actor.Nationality ?? DBNull.Value);
            command.Parameters.AddWithValue("@Bio", (object?)actor.Bio ?? DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Actors WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            return command.ExecuteNonQuery() > 0;
        }

        public int DeleteAll(int userId)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Actors WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", userId);
            return command.ExecuteNonQuery();
        }

        // --- Actor Images ---
        public List<ActorImage> GetImages(int actorId)
        {
            if (_imagesCache.TryGetValue(actorId, out var cached))
            {
                return cached;
            }

            var images = new List<ActorImage>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM ActorImages WHERE ActorId = @ActorId ORDER BY SortOrder ASC, Id ASC";
            command.Parameters.AddWithValue("@ActorId", actorId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                images.Add(new ActorImage
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ActorId = reader.GetInt32(reader.GetOrdinal("ActorId")),
                    ImagePath = reader.GetString(reader.GetOrdinal("ImagePath")),
                    SortOrder = reader.IsDBNull(reader.GetOrdinal("SortOrder")) ? 0 : reader.GetInt32(reader.GetOrdinal("SortOrder"))
                });
            }

            _imagesCache[actorId] = images;
            return images;
        }

        public int AddImage(ActorImage image)
        {
            InvalidateCache(image.ActorId);

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                INSERT INTO ActorImages (ActorId, ImagePath, SortOrder)
                VALUES (@ActorId, @ImagePath, @SortOrder);
                SELECT last_insert_rowid();
            ";
            
            command.Parameters.AddWithValue("@ActorId", image.ActorId);
            command.Parameters.AddWithValue("@ImagePath", image.ImagePath);
            command.Parameters.AddWithValue("@SortOrder", image.SortOrder);

            var result = command.ExecuteScalar();
            if (result != null)
            {
                image.Id = Convert.ToInt32(result);
                return image.Id;
            }
            return 0;
        }

        public bool DeleteImage(int id)
        {
            InvalidateCache();

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ActorImages WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            return command.ExecuteNonQuery() > 0;
        }
        
        // --- Movie Actors Relation ---
        public List<Actor> GetActorsForMovie(int movieId)
        {
            var actors = new List<Actor>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                SELECT a.* FROM Actors a
                INNER JOIN MovieActors ma ON a.Id = ma.ActorId
                WHERE ma.MovieId = @MovieId
            ";
            command.Parameters.AddWithValue("@MovieId", movieId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                actors.Add(MapActor(reader));
            }
            return actors;
        }
        
        public bool AddActorToMovie(int movieId, int actorId, string? role = null)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = @"
                INSERT OR IGNORE INTO MovieActors (MovieId, ActorId, Role)
                VALUES (@MovieId, @ActorId, @Role);
            ";
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@ActorId", actorId);
            command.Parameters.AddWithValue("@Role", (object?)role ?? DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }
        
        public bool RemoveActorFromMovie(int movieId, int actorId)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = "DELETE FROM MovieActors WHERE MovieId = @MovieId AND ActorId = @ActorId";
            command.Parameters.AddWithValue("@MovieId", movieId);
            command.Parameters.AddWithValue("@ActorId", actorId);

            return command.ExecuteNonQuery() > 0;
        }

        public List<string> GetDistinctNationalities(int userId)
        {
            var set = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
                connection.Open();

                // Lấy từ bảng Nationalities
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Nationalities WHERE UserId = @UserId";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string val = r.GetString(0).Trim();
                        if (!string.IsNullOrEmpty(val)) set.Add(val);
                    }
                }

                // Lấy thêm từ bảng Actors
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT Nationality FROM Actors WHERE UserId = @UserId AND Nationality IS NOT NULL AND TRIM(Nationality) != ''";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string val = r.GetString(0).Trim();
                        if (!string.IsNullOrEmpty(val)) set.Add(val);
                    }
                }
            }
            catch { }
            return set.ToList();
        }

        public List<(string Name, int ActorCount)> GetNationalitiesWithCount(int userId)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
                connection.Open();

                // Đọc tất cả quốc tịch đã đăng ký
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Nationalities WHERE UserId = @UserId";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string val = r.GetString(0).Trim();
                        if (!string.IsNullOrEmpty(val) && !dict.ContainsKey(val))
                        {
                            dict[val] = 0;
                        }
                    }
                }

                // Đếm số diễn viên theo từng quốc tịch
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT Nationality, COUNT(*) FROM Actors WHERE UserId = @UserId AND Nationality IS NOT NULL AND TRIM(Nationality) != '' GROUP BY Nationality";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        string val = r.GetString(0).Trim();
                        int count = r.GetInt32(1);
                        if (!string.IsNullOrEmpty(val))
                        {
                            dict[val] = count;
                        }
                    }
                }
            }
            catch { }

            return dict.Select(kvp => (kvp.Key, kvp.Value)).OrderBy(x => x.Key).ToList();
        }

        public bool AddNationality(int userId, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            try
            {
                using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
                connection.Open();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT OR IGNORE INTO Nationalities (UserId, Name) VALUES (@UserId, @Name)";
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Name", name.Trim());
                return cmd.ExecuteNonQuery() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateNationality(int userId, string oldName, string newName)
        {
            if (string.IsNullOrWhiteSpace(oldName) || string.IsNullOrWhiteSpace(newName)) return false;
            oldName = oldName.Trim();
            newName = newName.Trim();

            try
            {
                using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                // 1. Cập nhật trong bảng Nationalities
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "UPDATE Nationalities SET Name = @NewName WHERE UserId = @UserId AND Name = @OldName COLLATE NOCASE";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@NewName", newName);
                    cmd.Parameters.AddWithValue("@OldName", oldName);
                    cmd.ExecuteNonQuery();
                }

                // 2. Cập nhật đồng bộ các diễn viên đang mang quốc tịch cũ
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "UPDATE Actors SET Nationality = @NewName WHERE UserId = @UserId AND Nationality = @OldName COLLATE NOCASE";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@NewName", newName);
                    cmd.Parameters.AddWithValue("@OldName", oldName);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteNationality(int userId, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            name = name.Trim();

            try
            {
                using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                // 1. Xóa khỏi bảng Nationalities
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "DELETE FROM Nationalities WHERE UserId = @UserId AND Name = @Name COLLATE NOCASE";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                }

                // 2. Chuyển quốc tịch của các diễn viên thành NULL
                using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    cmd.CommandText = "UPDATE Actors SET Nationality = NULL WHERE UserId = @UserId AND Nationality = @Name COLLATE NOCASE";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                return false;
            }
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

        private Actor MapActor(SqliteDataReader reader)
        {
            return new Actor
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                AvatarPath = reader.IsDBNull(reader.GetOrdinal("AvatarPath")) ? null : reader.GetString(reader.GetOrdinal("AvatarPath")),
                DateOfBirth = reader.IsDBNull(reader.GetOrdinal("DateOfBirth")) ? null : reader.GetString(reader.GetOrdinal("DateOfBirth")),
                Nationality = reader.IsDBNull(reader.GetOrdinal("Nationality")) ? null : reader.GetString(reader.GetOrdinal("Nationality")),
                Bio = reader.IsDBNull(reader.GetOrdinal("Bio")) ? null : reader.GetString(reader.GetOrdinal("Bio")),
                CreatedAt = ParseDateTimeSafe(reader["CreatedAt"]),
                UpdatedAt = ParseDateTimeNullable(reader["UpdatedAt"])
            };
        }
    }
}
