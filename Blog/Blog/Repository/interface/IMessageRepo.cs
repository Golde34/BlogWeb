
using Blog.Models;

namespace Blog.Repository.@interface
{
    public interface IMessageRepo
    {
        IEnumerable<Message> GetMessages();
        void AddMessage(Message Message);
        void UpdateMessage(Message Message);
        void DeleteMessage(Message Message);
    }
}
