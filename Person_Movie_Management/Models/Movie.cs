using System;

namespace Person_Movie_Management.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MovieCode { get; set; } = string.Empty;
        public int SourceType { get; set; } = 0; // 0: Online, 1: Local
        public string? MediaUrl { get; set; }
        public string? CoverImage { get; set; }
        public string? Note { get; set; }
        public int Rating { get; set; } = 0; // 1-5
        public bool IsFavorite { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        
        // Feature 3: Watch History & Progress
        public int WatchProgress { get; set; } = 0;
        public DateTime? LastWatched { get; set; }
    }
}
