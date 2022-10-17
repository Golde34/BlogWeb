using Microsoft.AspNetCore.Identity;
using System.Security;

namespace Blog.Models
{
    public class User : IdentityUser
    {
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Profession { get; set; }
        public string PhoneNumber { get; set; }

        public ICollection<ChatUser> Chats { get; set; }
        
    }
}
