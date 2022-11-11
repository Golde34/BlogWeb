using Blog.Models;
using Microsoft.Build.Framework;

namespace Blog.ViewDTO
{
    public class BlogDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int GenreId { get; set; }
        [Required]
        public string Title { get; set; } = "";
        [Required]
        public string Url { get; set; } = "";
        [Required]
        public IFormFile Image { get; set; } = null;
        [Required]
        public string Intro { get; set; } = "";
        [Required]
        public string Body { get; set; } = "";
        [Required]

        public bool Status { get; set; }

        public string UserId { get; set; }
    }
}
