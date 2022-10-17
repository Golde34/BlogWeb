using Blog.Models.Enums;

namespace Blog.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsDeleted { get; set; }
        public MessageType MessageType { get; set; }
        public int ChatID { get; set; }
        public Chat Chat { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
    }
}
