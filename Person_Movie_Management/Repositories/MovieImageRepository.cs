using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class MovieImageRepository
    {
        public List<MovieImage> GetByMovieId(int movieId)
        {
            var images = new List<MovieImage>();
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM MovieImages WHERE MovieId = @MovieId ORDER BY SortOrder";
            command.Parameters.AddWithValue("@MovieId", movieId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                images.Add(new MovieImage
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    MovieId = Convert.ToInt32(reader["MovieId"]),
                    ImagePath = reader["ImagePath"].ToString() ?? "",
                    SortOrder = Convert.ToInt32(reader["SortOrder"])
                });
            }
            return images;
        }

        public int Insert(MovieImage movieImage)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO MovieImages (MovieId, ImagePath, SortOrder)
                VALUES (@MovieId, @ImagePath, @SortOrder);
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("@MovieId", movieImage.MovieId);
            command.Parameters.AddWithValue("@ImagePath", movieImage.ImagePath);
            command.Parameters.AddWithValue("@SortOrder", movieImage.SortOrder);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public bool Delete(int id)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM MovieImages WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            return command.ExecuteNonQuery() > 0;
        }
        
        public bool DeleteByMovieId(int movieId)
        {
            using var connection = new SqliteConnection(DatabaseHelper.ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM MovieImages WHERE MovieId = @MovieId";
            command.Parameters.AddWithValue("@MovieId", movieId);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
