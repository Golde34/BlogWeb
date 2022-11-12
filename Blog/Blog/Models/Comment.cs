using Blog.Models.Enums;

namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int BlogID { get; set; }
        public virtual Blogs Blogs { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
}
