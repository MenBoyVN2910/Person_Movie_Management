using System;
using System.Collections.Generic;

namespace Person_Movie_Management.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }

    public class ActorImage
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
    }

    public class MovieActor
    {
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        public string? Role { get; set; }
    }
}
