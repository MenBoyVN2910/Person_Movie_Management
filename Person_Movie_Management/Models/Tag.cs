using System;

namespace Person_Movie_Management.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TagName { get; set; } = "";
        public string ColorHex { get; set; } = "#8b5cf6"; // e.g. "#FF5733"
    }

    public class MovieTag
    {
        public int MovieId { get; set; }
        public int TagId { get; set; }
    }

    public class AudioTag
    {
        public int AudioId { get; set; }
        public int TagId { get; set; }
    }
}
