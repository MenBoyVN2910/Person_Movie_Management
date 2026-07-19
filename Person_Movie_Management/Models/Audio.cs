using System;
using System.Text.Json.Serialization;

namespace Person_Movie_Management.Models
{
    public class Audio
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AudioCode { get; set; } = string.Empty;
        
        [JsonIgnore] // We might want to handle this manually for export to avoid massive JSON in memory at once if not needed, but since we are zipping it, maybe we will keep it or serialize it separately. Let's ignore it here and serialize separately, OR we can let it be exported as base64 string.
        public byte[]? AudioData { get; set; }
        
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
