using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class ChatRepository : IChatRepo
    {
        public async void AddChat(Chat Chat) => await ChatManagement.Instance.AddChatRoom(Chat);

        public void DeleteChat(Chat Chat) => ChatManagement.Instance.DeleteChat(Chat);

        public List<Chat> FindRoomByName(string search) => ChatManagement.Instance.FindRoomByName(search);

        public IEnumerable<Chat> GetChats() => ChatManagement.Instance.GetChatRooms();

        public void UpdateChat(Chat Chat) => ChatManagement.Instance.UpdateChat(Chat);

        public Chat GetPrivateChatById(int chatId) => ChatManagement.Instance.GetPrivateChatById(chatId);
    }
}
