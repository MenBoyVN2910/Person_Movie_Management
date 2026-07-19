using System;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class UserRepository
    {
        public User? GetByUsername(string username)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Username = @Username";
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapUser(reader);
            }
            return null;
        }

        public User? GetById(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapUser(reader);
            }
            return null;
        }

        public int Insert(User user)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Username, DisplayName, Email, PasswordHash, AvatarPath, IsActive)
                VALUES (@Username, @DisplayName, @Email, @PasswordHash, @AvatarPath, @IsActive);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@Username", user.Username);
            command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@AvatarPath", user.AvatarPath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive ? 1 : 0);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Update(User user)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users 
                SET DisplayName = @DisplayName, Email = @Email, PasswordHash = @PasswordHash, AvatarPath = @AvatarPath, IsActive = @IsActive
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            command.Parameters.AddWithValue("@Email", user.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@AvatarPath", user.AvatarPath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IsActive", user.IsActive ? 1 : 0);

            return command.ExecuteNonQuery() > 0;
        }

        public bool UsernameExists(string username)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
            command.Parameters.AddWithValue("@Username", username);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        private User MapUser(SqliteDataReader reader)
        {
            return new User
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString() ?? "",
                DisplayName = reader["DisplayName"].ToString() ?? "",
                Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : null,
                PasswordHash = reader["PasswordHash"].ToString() ?? "",
                AvatarPath = reader["AvatarPath"] != DBNull.Value ? reader["AvatarPath"].ToString() : null,
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                IsActive = Convert.ToInt32(reader["IsActive"]) == 1
            };
        }
    }
}
