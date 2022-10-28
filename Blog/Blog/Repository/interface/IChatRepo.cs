using Blog.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Repository.@interface
{
    public interface IChatRepo
    {
        IEnumerable<Chat> GetChats();
        void AddChat(Chat Chat);
        void UpdateChat(Chat Chat);
        void DeleteChat(Chat Chat);

        List<Chat> FindRoomByName(string search);

        Chat GetSingleChat(int id);
    }
}
