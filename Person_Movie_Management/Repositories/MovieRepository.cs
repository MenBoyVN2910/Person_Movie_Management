using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class MovieRepository
    {
        public List<Movie> GetAllByUser(int userId, int? sourceType = null)
        {
            var movies = new List<Movie>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            string sql = "SELECT * FROM Movies WHERE UserId = @UserId AND IsDeleted = 0";
            if (sourceType.HasValue)
            {
                sql += " AND SourceType = @SourceType";
            }
            sql += " ORDER BY CreatedAt DESC";

            command.CommandText = sql;
            command.Parameters.AddWithValue("@UserId", userId);
            if (sourceType.HasValue)
            {
                command.Parameters.AddWithValue("@SourceType", sourceType.Value);
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                movies.Add(MapMovie(reader));
            }
            return movies;
        }

        public async System.Threading.Tasks.Task<List<Movie>> GetAllByUserAsync(int userId, int? sourceType = null)
        {
            var movies = new List<Movie>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            
            string sql = "SELECT * FROM Movies WHERE UserId = @UserId AND IsDeleted = 0";
            if (sourceType.HasValue)
            {
                sql += " AND SourceType = @SourceType";
            }
            sql += " ORDER BY CreatedAt DESC";

            command.CommandText = sql;
            command.Parameters.AddWithValue("@UserId", userId);
            if (sourceType.HasValue)
            {
                command.Parameters.AddWithValue("@SourceType", sourceType.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movies.Add(MapMovie(reader));
            }
            return movies;
        }

        public Movie? GetById(int id, bool includeDeleted = false)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = includeDeleted ? "SELECT * FROM Movies WHERE Id = @Id" : "SELECT * FROM Movies WHERE Id = @Id AND IsDeleted = 0";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapMovie(reader);
            }
            return null;
        }

        public Movie? GetByCode(int userId, string movieCode)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Movies WHERE UserId = @UserId AND MovieCode = @MovieCode AND IsDeleted = 0";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@MovieCode", movieCode);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapMovie(reader);
            }
            return null;
        }

        public Movie? GetByMediaUrl(int userId, string mediaUrl)
        {
            if (string.IsNullOrWhiteSpace(mediaUrl)) return null;

            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Movies WHERE UserId = @UserId AND MediaUrl = @MediaUrl AND IsDeleted = 0 LIMIT 1";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@MediaUrl", mediaUrl);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapMovie(reader);
            }
            return null;
        }

        public List<Movie> Search(int userId, string keyword, int? sourceType = null)
        {
            var movies = new List<Movie>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            string sql = "SELECT * FROM Movies WHERE UserId = @UserId AND IsDeleted = 0 AND (MovieCode LIKE @Keyword OR Note LIKE @Keyword)";
            if (sourceType.HasValue)
            {
                sql += " AND SourceType = @SourceType";
            }
            sql += " ORDER BY CreatedAt DESC";

            command.CommandText = sql;
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
            if (sourceType.HasValue)
            {
                command.Parameters.AddWithValue("@SourceType", sourceType.Value);
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                movies.Add(MapMovie(reader));
            }
            return movies;
        }

        public List<Movie> GetFavorites(int userId)
        {
            var movies = new List<Movie>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM Movies WHERE UserId = @UserId AND IsFavorite = 1 AND IsDeleted = 0 ORDER BY CreatedAt DESC";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                movies.Add(MapMovie(reader));
            }
            return movies;
        }

        public async System.Threading.Tasks.Task<List<Movie>> GetFavoritesAsync(int userId)
        {
            var movies = new List<Movie>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM Movies WHERE UserId = @UserId AND IsFavorite = 1 AND IsDeleted = 0 ORDER BY CreatedAt DESC";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movies.Add(MapMovie(reader));
            }
            return movies;
        }

        public int Insert(Movie movie)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Movies (UserId, MovieCode, SourceType, MediaUrl, CoverImage, Note, Rating, IsFavorite)
                VALUES (@UserId, @MovieCode, @SourceType, @MediaUrl, @CoverImage, @Note, @Rating, @IsFavorite);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@UserId", movie.UserId);
            command.Parameters.AddWithValue("@MovieCode", movie.MovieCode);
            command.Parameters.AddWithValue("@SourceType", movie.SourceType);
            command.Parameters.AddWithValue("@MediaUrl", movie.MediaUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CoverImage", movie.CoverImage ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Note", movie.Note ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Rating", movie.Rating);
            command.Parameters.AddWithValue("@IsFavorite", movie.IsFavorite ? 1 : 0);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(Movie movie)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Movies 
                SET MovieCode = @MovieCode, SourceType = @SourceType, MediaUrl = @MediaUrl, CoverImage = @CoverImage, 
                    Note = @Note, Rating = @Rating, IsFavorite = @IsFavorite, UpdatedAt = datetime('now','localtime'),
                    WatchProgress = @WatchProgress, LastWatched = @LastWatched
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", movie.Id);
            command.Parameters.AddWithValue("@MovieCode", movie.MovieCode);
            command.Parameters.AddWithValue("@SourceType", movie.SourceType);
            command.Parameters.AddWithValue("@MediaUrl", movie.MediaUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CoverImage", movie.CoverImage ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Note", movie.Note ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Rating", movie.Rating);
            command.Parameters.AddWithValue("@IsFavorite", movie.IsFavorite ? 1 : 0);
            command.Parameters.AddWithValue("@WatchProgress", movie.WatchProgress);
            command.Parameters.AddWithValue("@LastWatched", movie.LastWatched ?? (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UpdateProgress(int id, int progress)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Movies SET WatchProgress = @Progress, LastWatched = datetime('now','localtime') WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Progress", progress);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Movies SET IsDeleted = 1, DeletedAt = datetime('now','localtime'), MovieCode = MovieCode || '_$DEL$_' || Id WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public void DeleteAll(int userId, int? sourceType = null)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            string sql = "UPDATE Movies SET IsDeleted = 1, DeletedAt = datetime('now','localtime'), MovieCode = MovieCode || '_$DEL$_' || Id WHERE UserId = @UserId AND IsDeleted = 0";
            if (sourceType.HasValue)
            {
                sql += " AND SourceType = @SourceType";
            }
            command.CommandText = sql;
            command.Parameters.AddWithValue("@UserId", userId);
            if (sourceType.HasValue)
            {
                command.Parameters.AddWithValue("@SourceType", sourceType.Value);
            }
            
            command.ExecuteNonQuery();
        }

        
        public void HardDeleteAll(int userId, int? sourceType = null)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            string subquery = "SELECT Id FROM Movies WHERE UserId = @UserId";
            if (sourceType.HasValue)
            {
                subquery += " AND SourceType = @SourceType";
            }

            command.CommandText = $@"
                DELETE FROM MovieTags WHERE MovieId IN ({subquery});
                DELETE FROM MovieImages WHERE MovieId IN ({subquery});
                DELETE FROM PlaylistItems WHERE ItemType = 0 AND ItemId IN ({subquery});
                DELETE FROM Movies WHERE UserId = @UserId {(sourceType.HasValue ? "AND SourceType = @SourceType" : "")};
            ";
            command.Parameters.AddWithValue("@UserId", userId);
            if (sourceType.HasValue)
            {
                command.Parameters.AddWithValue("@SourceType", sourceType.Value);
            }
            
            command.ExecuteNonQuery();
        }

        public bool HardDelete(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Movies WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public bool ToggleFavorite(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Movies SET IsFavorite = CASE WHEN IsFavorite = 1 THEN 0 ELSE 1 END WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public (int Total, int Online, int Local, int Favorites) GetStats(int userId)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT 
                    COUNT(*) as Total,
                    SUM(CASE WHEN SourceType = 0 THEN 1 ELSE 0 END) as Online,
                    SUM(CASE WHEN SourceType = 1 THEN 1 ELSE 0 END) as Local,
                    SUM(CASE WHEN IsFavorite = 1 THEN 1 ELSE 0 END) as Favorites
                FROM Movies
                WHERE UserId = @UserId AND IsDeleted = 0
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (
                    Convert.ToInt32(reader["Total"] != DBNull.Value ? reader["Total"] : 0),
                    Convert.ToInt32(reader["Online"] != DBNull.Value ? reader["Online"] : 0),
                    Convert.ToInt32(reader["Local"] != DBNull.Value ? reader["Local"] : 0),
                    Convert.ToInt32(reader["Favorites"] != DBNull.Value ? reader["Favorites"] : 0)
                );
            }
            return (0, 0, 0, 0);
        }

        public List<Movie> GetDeleted(int userId)
        {
            var movies = new List<Movie>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT * FROM Movies WHERE UserId = @UserId AND IsDeleted = 1 ORDER BY DeletedAt DESC";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                movies.Add(MapMovie(reader));
            }
            return movies;
        }

        private Movie MapMovie(SqliteDataReader reader)
        {
            return new Movie
            {
                Id = Convert.ToInt32(reader["Id"]),
                UserId = Convert.ToInt32(reader["UserId"]),
                MovieCode = reader["MovieCode"].ToString() ?? "",
                SourceType = Convert.ToInt32(reader["SourceType"]),
                MediaUrl = reader["MediaUrl"] != DBNull.Value ? reader["MediaUrl"].ToString() : null,
                CoverImage = reader["CoverImage"] != DBNull.Value ? reader["CoverImage"].ToString() : null,
                Note = reader["Note"] != DBNull.Value ? reader["Note"].ToString() : null,
                Rating = Convert.ToInt32(reader["Rating"]),
                IsFavorite = Convert.ToInt32(reader["IsFavorite"]) == 1,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                UpdatedAt = reader["UpdatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedAt"]) : null,
                IsDeleted = reader["IsDeleted"] != DBNull.Value ? Convert.ToInt32(reader["IsDeleted"]) == 1 : false,
                DeletedAt = reader["DeletedAt"] != DBNull.Value ? Convert.ToDateTime(reader["DeletedAt"]) : null,
                WatchProgress = reader["WatchProgress"] != DBNull.Value ? Convert.ToInt32(reader["WatchProgress"]) : 0,
                LastWatched = reader["LastWatched"] != DBNull.Value ? Convert.ToDateTime(reader["LastWatched"]) : null
            };
        }
    }
}
