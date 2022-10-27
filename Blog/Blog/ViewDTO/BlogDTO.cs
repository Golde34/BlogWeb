using Blog.Models;
using Microsoft.Build.Framework;

namespace Blog.ViewDTO
{
    public class BlogDTO
    {
        [Required]
        public int GenreId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public string Intro { get; set; }
        [Required]
        public string Body { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
