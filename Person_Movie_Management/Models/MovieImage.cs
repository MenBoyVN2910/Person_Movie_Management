namespace Person_Movie_Management.Models
{
    public class MovieImage
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
    }
}
