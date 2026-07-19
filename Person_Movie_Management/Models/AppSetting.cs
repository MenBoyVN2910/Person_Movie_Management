namespace Person_Movie_Management.Models
{
    public class AppSetting
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string? SettingValue { get; set; }
    }
}
