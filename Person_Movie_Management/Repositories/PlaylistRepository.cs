using System;
using System.Collections.Generic;
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

        public List<Playlist> GetAllByUser(int userId)
        {
            var playlists = new List<Playlist>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, Name, Description, CreatedAt FROM Playlists WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                playlists.Add(new Playlist
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Name = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                });
            }

            return playlists;
        }

        public Playlist? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, Name, Description, CreatedAt FROM Playlists WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Playlist
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Name = reader.GetString(2),
                    Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4)
                };
            }
            return null;
        }

        public int Insert(Playlist playlist)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Playlists (UserId, Name, Description, CreatedAt)
                VALUES (@UserId, @Name, @Description, @CreatedAt);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@UserId", playlist.UserId);
            command.Parameters.AddWithValue("@Name", playlist.Name);
            command.Parameters.AddWithValue("@Description", playlist.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", playlist.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

            var newId = command.ExecuteScalar();
            return newId != null ? Convert.ToInt32(newId) : 0;
        }

        public bool Update(Playlist playlist)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Playlists SET Name = @Name, Description = @Description WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", playlist.Id);
            command.Parameters.AddWithValue("@Name", playlist.Name);
            command.Parameters.AddWithValue("@Description", playlist.Description ?? (object)DBNull.Value);

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
