using Blog.Models.Enums;

namespace Blog.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public NotificationType Type { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
    }
}
