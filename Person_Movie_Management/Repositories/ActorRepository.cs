using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Person_Movie_Management.Data;
using Person_Movie_Management.Models;

namespace Person_Movie_Management.Repositories
{
    public class ActorRepository
    {
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

        // --- Actor Images ---
        public List<ActorImage> GetImages(int actorId)
        {
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
            return images;
        }

        public int AddImage(ActorImage image)
        {
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
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
