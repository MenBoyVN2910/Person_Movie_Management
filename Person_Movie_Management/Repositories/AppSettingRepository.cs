using System;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class AppSettingRepository
    {
        public string? Get(int userId, string key)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT SettingValue FROM AppSettings WHERE UserId = @UserId AND SettingKey = @SettingKey";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SettingKey", key);

            var result = command.ExecuteScalar();
            return result != DBNull.Value ? result?.ToString() : null;
        }

        public bool Set(int userId, string key, string value)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            
            // Upsert (Insert or Replace based on UNIQUE constraint)
            command.CommandText = @"
                INSERT INTO AppSettings (UserId, SettingKey, SettingValue)
                VALUES (@UserId, @SettingKey, @SettingValue)
                ON CONFLICT(UserId, SettingKey) 
                DO UPDATE SET SettingValue = @SettingValue;
            ";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@SettingKey", key);
            command.Parameters.AddWithValue("@SettingValue", value ?? (object)DBNull.Value);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
