using Blog.Models;
using Blog.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.DAO
{
    public class ChatManagement
    {
        private static ChatManagement instance;
        private static readonly object instancelock = new object();

        public ChatManagement()
        {
        }

        public static ChatManagement Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (instance == null) instance = new ChatManagement();
                }
                return instance;
            }
        }

        public List<Chat> FindRoomByName(string search)
        {
            List<Chat> chats;
            try
            {
                var _context = new AppDBContext();
                chats = _context.Chats.Where(o => o.Type == ChatType.Room && o.Name.Contains(search)).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return chats;
        }

        public List<Chat> GetChatRooms()
        {
            List<Chat> chats;
            try
            {
                var _context = new AppDBContext();
                chats = _context.Chats.Include(o => o.Users).ThenInclude(o => o.User)
                    .Where(o => o.Type == ChatType.Room)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return chats;
        }

        public async Task<Chat> AddChatRoom(Chat chat)
        {
            try
            {
                var _context = new AppDBContext();
                _context.Chats.Add(chat);
                await _context.SaveChangesAsync();
                return chat;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateChat(Chat Chat)
        {
            try
            {
                Chat _Chat = GetChatByID(Chat.Id);
                if (_Chat != null)
                {
                    var context = new AppDBContext();
                    context.Entry<Chat>(Chat).State = EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The Chat does not already exist");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteChat(Chat Chat)
        {
            try
            {
                Chat _Chat = GetChatByID(Chat.Id);
                if (_Chat != null)
                {
                    var context = new AppDBContext();
                    context.Chats.Remove(Chat);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The Chat does not already exist");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Chat GetPrivateChatById(int chatId)
        {
            Chat chat; ;
            try
            {
                var _context = new AppDBContext();
                chat = _context.Chats.Include(o => o.Users).ThenInclude(o => o.User)
                    .FirstOrDefault(m => m.Id == chatId && m.Type == ChatType.Private);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return chat;
        }

        public Chat GetChatByID(int chatId)
        {
            Chat chat; ;
            try
            {
                var _context = new AppDBContext();
                chat = _context.Chats.Include(o => o.Users).ThenInclude(o => o.User)
                    .FirstOrDefault(m => m.Id == chatId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return chat;
        }
    }
}
