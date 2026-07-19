using System;

namespace Person_Movie_Management.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum PlaylistItemType
    {
        Movie = 1,
        Audio = 2
    }

    public class PlaylistItem
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int ItemId { get; set; } // MovieId or AudioId
        public PlaylistItemType ItemType { get; set; }
        public int SortOrder { get; set; }
    }
}
