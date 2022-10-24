using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class MessageRepository : IMessageRepo
    {
        public async void AddMessage(Message Message) => await MessageManagement.Instance.AddMessage(Message);

        public void DeleteMessage(Message Message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetMessages()
        {
            throw new NotImplementedException();
        }

        public void UpdateMessage(Message Message)
        {
            throw new NotImplementedException();
        }
    }
}
