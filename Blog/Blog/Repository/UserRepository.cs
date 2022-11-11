using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;
using Microsoft.AspNetCore.Identity;

namespace Blog.Repository
{
    public class UserRepository : IUserRepo
    {
        public void AddUser(User User)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(User User)
        {
            throw new NotImplementedException();
        }

        public IdentityUser GetCurrentUser(string userId) => UserManagement.Instance.GetCurrentUser(userId);

        public List<IdentityUser> GetUserBySearchString(string search, string userId) => UserManagement.Instance.GetUserBySearchString(search, userId);

        public IEnumerable<User> GetUsers()
        {
            throw new NotImplementedException();
        }

        //public List<IdentityUser> GetUsersButNoCurrentBlogUser(string userId, string currentUserId) => UserManagement.Instance.GetUsersButNoCurrentBlogUser(userId, currentUserId);

        public void UpdateUser(User User)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IdentityUser> IUserRepo.GetUsers()
        {
            throw new NotImplementedException();
        }
        public ChatUser GetNotificationUser(int chatId, string userId) => UserManagement.Instance.GetUserNotification(chatId, userId);

        public List<IdentityUser> GetOthersBlogUser(string userId) => UserManagement.Instance.GetOthersBlogsUser(userId);

        public List<IdentityUser> GetUsersbutNoCurrentUser(string userId) => UserManagement.Instance.GetUsersbutNoCurrentUser(userId);
    }
}
