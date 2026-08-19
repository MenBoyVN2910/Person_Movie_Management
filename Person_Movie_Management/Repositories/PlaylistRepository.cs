using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class PlaylistRepository
    {
        private readonly string _connectionString;

        public PlaylistRepository()
        {
            _connectionString = DatabaseHelper.ConnectionString;
        }

        private static Playlist MapPlaylist(SqliteDataReader reader)
        {
            return new Playlist
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                CreatedAt = DateTime.TryParse(reader.GetString(4), out var dt) ? dt : DateTime.Now,
                CoverImage = reader.IsDBNull(5) ? null : reader.GetString(5),
                IsPrivate = reader.GetInt32(6) == 1
            };
        }

        public List<Playlist> GetAllByUser(int userId, string sortBy = "newest")
        {
            var playlists = new List<Playlist>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            string orderClause = sortBy switch
            {
                "oldest" => "CreatedAt ASC",
                "name_az" => "Name COLLATE NOCASE ASC",
                "name_za" => "Name COLLATE NOCASE DESC",
                _ => "CreatedAt DESC" // newest
            };

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT Id, UserId, Name, Description, CreatedAt, CoverImage, IsPrivate FROM Playlists WHERE UserId = @UserId ORDER BY {orderClause}";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                playlists.Add(MapPlaylist(reader));
            }

            return playlists;
        }

        public List<Playlist> GetAllByUserSortedByCount(int userId)
        {
            var playlists = GetAllByUser(userId, "newest");
            // Sort by item count (descending) in memory
            return playlists
                .OrderByDescending(p => GetItemCount(p.Id))
                .ThenByDescending(p => p.CreatedAt)
                .ToList();
        }

        public Playlist? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, Name, Description, CreatedAt, CoverImage, IsPrivate FROM Playlists WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapPlaylist(reader);
            }
            return null;
        }

        public int Insert(Playlist playlist)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Playlists (UserId, Name, Description, CreatedAt, CoverImage, IsPrivate)
                VALUES (@UserId, @Name, @Description, @CreatedAt, @CoverImage, @IsPrivate);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@UserId", playlist.UserId);
            command.Parameters.AddWithValue("@Name", playlist.Name);
            command.Parameters.AddWithValue("@Description", (object?)playlist.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", playlist.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@CoverImage", (object?)playlist.CoverImage ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsPrivate", playlist.IsPrivate ? 1 : 0);

            var newId = command.ExecuteScalar();
            return newId != null ? Convert.ToInt32(newId) : 0;
        }

        public bool Update(Playlist playlist)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Playlists SET Name = @Name, Description = @Description, CoverImage = @CoverImage, IsPrivate = @IsPrivate WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", playlist.Id);
            command.Parameters.AddWithValue("@Name", playlist.Name);
            command.Parameters.AddWithValue("@Description", (object?)playlist.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@CoverImage", (object?)playlist.CoverImage ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsPrivate", playlist.IsPrivate ? 1 : 0);

            return command.ExecuteNonQuery() > 0;
        }

        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Playlists WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public int DeleteAll(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Playlists WHERE UserId = @UserId";
            command.Parameters.AddWithValue("@UserId", userId);
            return command.ExecuteNonQuery();
        }

        // Playlist Items
        public List<PlaylistItem> GetItems(int playlistId)
        {
            var items = new List<PlaylistItem>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, PlaylistId, ItemId, ItemType, SortOrder FROM PlaylistItems WHERE PlaylistId = @PlaylistId ORDER BY SortOrder ASC, Id ASC";
            command.Parameters.AddWithValue("@PlaylistId", playlistId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new PlaylistItem
                {
                    Id = reader.GetInt32(0),
                    PlaylistId = reader.GetInt32(1),
                    ItemId = reader.GetInt32(2),
                    ItemType = (PlaylistItemType)reader.GetInt32(3),
                    SortOrder = reader.GetInt32(4)
                });
            }

            return items;
        }

        public bool AddItem(PlaylistItem item)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO PlaylistItems (PlaylistId, ItemId, ItemType, SortOrder) VALUES (@PlaylistId, @ItemId, @ItemType, @SortOrder)";
            command.Parameters.AddWithValue("@PlaylistId", item.PlaylistId);
            command.Parameters.AddWithValue("@ItemId", item.ItemId);
            command.Parameters.AddWithValue("@ItemType", (int)item.ItemType);
            command.Parameters.AddWithValue("@SortOrder", item.SortOrder);

            return command.ExecuteNonQuery() > 0;
        }

        public bool RemoveItem(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM PlaylistItems WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }

        public int GetItemCount(int playlistId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM PlaylistItems WHERE PlaylistId = @PlaylistId";
            command.Parameters.AddWithValue("@PlaylistId", playlistId);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>Returns (movieCount, audioCount) for a playlist</summary>
        public (int movieCount, int audioCount) GetStats(int playlistId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT ItemType, COUNT(*) 
                FROM PlaylistItems 
                WHERE PlaylistId = @PlaylistId 
                GROUP BY ItemType";
            command.Parameters.AddWithValue("@PlaylistId", playlistId);

            int movieCount = 0, audioCount = 0;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var type = (PlaylistItemType)reader.GetInt32(0);
                int count = reader.GetInt32(1);
                if (type == PlaylistItemType.Movie) movieCount = count;
                else if (type == PlaylistItemType.Audio) audioCount = count;
            }
            return (movieCount, audioCount);
        }

        /// <summary>Returns cover image paths for the first N items (for mosaic generation)</summary>
        public List<(string coverPath, PlaylistItemType type)> GetCoverThumbnails(int playlistId, int maxCount = 4)
        {
            var result = new List<(string, PlaylistItemType)>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT pi.ItemId, pi.ItemType,
                    CASE pi.ItemType 
                        WHEN 1 THEN m.CoverImage 
                        WHEN 2 THEN a.CoverImage 
                    END AS CoverPath
                FROM PlaylistItems pi
                LEFT JOIN Movies m ON pi.ItemType = 1 AND pi.ItemId = m.Id
                LEFT JOIN Audios a ON pi.ItemType = 2 AND pi.ItemId = a.Id
                WHERE pi.PlaylistId = @PlaylistId
                ORDER BY pi.SortOrder ASC, pi.Id ASC
                LIMIT @MaxCount";
            command.Parameters.AddWithValue("@PlaylistId", playlistId);
            command.Parameters.AddWithValue("@MaxCount", maxCount);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string path = reader.IsDBNull(2) ? "" : reader.GetString(2);
                var type = (PlaylistItemType)reader.GetInt32(1);
                result.Add((path, type));
            }

            return result;
        }

        public bool ItemExists(int playlistId, int itemId, PlaylistItemType itemType)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM PlaylistItems WHERE PlaylistId = @PlaylistId AND ItemId = @ItemId AND ItemType = @ItemType";
            command.Parameters.AddWithValue("@PlaylistId", playlistId);
            command.Parameters.AddWithValue("@ItemId", itemId);
            command.Parameters.AddWithValue("@ItemType", (int)itemType);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public void UpdateSortOrder(int itemId, int newSortOrder)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE PlaylistItems SET SortOrder = @SortOrder WHERE Id = @Id";
            command.Parameters.AddWithValue("@SortOrder", newSortOrder);
            command.Parameters.AddWithValue("@Id", itemId);

            command.ExecuteNonQuery();
        }

        public int GetNextSortOrder(int playlistId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COALESCE(MAX(SortOrder), 0) + 1 FROM PlaylistItems WHERE PlaylistId = @PlaylistId";
            command.Parameters.AddWithValue("@PlaylistId", playlistId);

            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}
