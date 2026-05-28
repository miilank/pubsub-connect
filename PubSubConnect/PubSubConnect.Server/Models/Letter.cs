namespace PubSubConnect.Server.Models
{
    public class Letter
    {
        public string SenderUsername { get; set; } = string.Empty;
        public string SenderCity { get; set; } = string.Empty;
        public int SenderAge { get; set; }
        public string? SenderPhone { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
