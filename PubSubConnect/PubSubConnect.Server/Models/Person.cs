namespace PubSubConnect.Server.Models
{
    public class Person
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
        public bool PendingConfirmation { get; set; } = false;
        public HashSet<string> BlockedUsers { get; set; } = new();
    }
}
