using Blog.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.Metrics;

namespace Blog.Repository.@interface
{
    public interface IUserRepo
    {
        IEnumerable<IdentityUser> GetUsers();
        void AddUser(User User);
        void UpdateUser(User User);
        void DeleteUser(User User);
        List<IdentityUser> GetUserBySearchString(string search, string userId);

        List<IdentityUser> GetUsersbutNoCurrentUser(string userId);
        IdentityUser GetCurrentUser(string userId);
    }
}
