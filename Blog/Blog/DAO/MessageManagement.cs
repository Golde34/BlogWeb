using Blog.Models;

namespace Blog.DAO
{
    public class MessageManagement
    {
        private static MessageManagement instance;
        private static readonly object instancelock = new object();

        public MessageManagement()
        {
        }

        public static MessageManagement Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (instance == null) instance = new MessageManagement();
                }
                return instance;
            }
        }

        public async Task<Message> AddMessage(Message message)
        {
            try
            {
                var _context = new AppDBContext();
                _context.Add(message);
                await _context.SaveChangesAsync();
                return message;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
