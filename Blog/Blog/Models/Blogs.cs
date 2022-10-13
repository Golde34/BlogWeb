namespace Blog.Models
{
    public class Blogs
    {
        public int BlogId { get; set; }
        public int GenreId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string Intro { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

            
    }
}
