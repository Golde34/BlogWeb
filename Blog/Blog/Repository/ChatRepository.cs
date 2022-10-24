using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class ChatRepository : IChatRepo
    {
        public async void AddChat(Chat Chat) => await ChatManagement.Instance.AddChatRoom(Chat);

        public void DeleteChat(Chat Chat)
        {
            throw new NotImplementedException();
        }

        public List<Chat> FindRoomByName(string search) => ChatManagement.Instance.FindRoomByName(search);

        public IEnumerable<Chat> GetChats() => ChatManagement.Instance.GetChatRooms();

        public Chat GetSingleChat(int id) => ChatManagement.Instance.GetSingleChat(id);

        public void UpdateChat(Chat Chat)
        {
            throw new NotImplementedException();
        }
    }
}
