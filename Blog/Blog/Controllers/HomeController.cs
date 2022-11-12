using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using Blog.Models.Enums;
using System.Security.Claims;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Blog.Repository;
using Blog.Repository.@interface;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDBContext _dbContext;
        private IUserRepo _userRepo = new UserRepository();
        private IChatRepo _chatRepo = new ChatRepository();
        private IMessageRepo _messageRepo = new MessageRepository();
        private INotificationRepo _notificationRepo = new NotificationRepository();

        public HomeController(ILogger<HomeController> logger, AppDBContext dBContext)
        {
            _logger = logger;
            _dbContext = dBContext;
        }

        public IActionResult Index()
        {
            var chats = _chatRepo.GetChats();
            //var chats = _dbContext.Chats.Include(o => o.Users).ThenInclude(o => o.User)
            //    .Where(o => o.Type == ChatType.Room)
            //    .ToList();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            //ViewData["currentUser"] = _dbContext.Users.FirstOrDefault(o => o.Id == userId).UserName;
            return View(chats);
        }

        public IActionResult GetRoomViewComponent(int chatId)
        {
            return ViewComponent("Room", new { ChatType = ChatType.Room, chatId = chatId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room,
            };
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
            //_chatRepo.AddChat(chat);
            _dbContext.Chats.Add(chat);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("{id}")]
        public IActionResult Chat(int id)
        {
            Chat chat = _dbContext.Chats.Include(o => o.Users).Include(o => o.Messages).ThenInclude(o => o.User)
                .FirstOrDefault(o => o.Id == id);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            //ViewData["currentUser"] = _dbContext.Users.FirstOrDefault(o => o.Id == userId).UserName;
            if (chat != null)
            {
                if (chat.Users.Any(o => o.UserId == userId))
                {
                    ViewBag.ChatType = chat.Type;
                    ViewBag.ChatId = chat.Id;
                    return View(chat);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message, IFormFile image, [FromServices] IHostingEnvironment hostingEnvironment)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (image == null)
            {
                var msg = new Message
                {
                    ChatID = chatId,
                    Text = message,
                    UserID = userId,
                    Timestamp = DateTime.Now,
                    MessageType = MessageType.Text,
                };
                _messageRepo.AddMessage(msg);
                //_dbContext.Add(msg);
                //await _dbContext.SaveChangesAsync();
                return RedirectToAction("Chat", new {id = chatId});
            } else
            {
                string uniqueFileName;
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                using (var fs = new FileStream(Path.Combine(uploadFolder, uniqueFileName), FileMode.Create))
                {
                    await image.CopyToAsync(fs);
                }
                var msg = new Message
                {
                    ChatID = chatId,
                    Text = message,
                    UserID = userId,
                    Timestamp = DateTime.Now,
                    MessageType = MessageType.Image,
                };
                _messageRepo.AddMessage(msg);
                //_dbContext.Add(msg);
                //await _dbContext.SaveChangesAsync();
                return RedirectToAction("Chat", new { id = chatId });
            }
        }

        public IActionResult Find()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = _userRepo.GetUsersbutNoCurrentUser(userId);
            //var users = _dbContext.Users.Where(o => o.Id != userId).ToList();
            return View(users);
        }

        public IActionResult FindUserByName(string search)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = _userRepo.GetUserBySearchString(search, userId);
            //var users = _dbContext.Users
            //            .Where(o => o.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            //            .Where(o => o.UserName.Contains(search) || o.Email.Contains(search))
            //            .ToList();
            return Json(users);
        }

        public IActionResult FindRoomByName(string search)
        {
            var rooms = _chatRepo.FindRoomByName(search);
            //var rooms = _dbContext.Chats.Where(o => o.Type == ChatType.Room && o.Name.Contains(search)).ToList();
            return Json(rooms);
        }

        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var currentUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<ChatUser> duplicateChat = _dbContext.ChatUsers.Include(o => o.Chat)
                .Where(o => (o.User.Id == userId || o.UserId == currentUser) && o.Chat.Type == ChatType.Private).ToList();
            List<int> chats = duplicateChat.GroupBy(o => o.ChatId)
                .Where(o => o.Count() == 2)
                .Select(o => o.Key).ToList();
            if (chats.Count() > 0)
            {
                int existedChat = chats.First();
                return RedirectToAction("Chat", new { id = existedChat });
            }
            var chat = new Chat
            {
                Type = ChatType.Private,
                Name = "private" + currentUser + "_" + userId,
            };
            chat.Users.Add(new ChatUser
            {
                UserId = userId
            });
            chat.Users.Add(new ChatUser
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            });
            _userRepo.GetCurrentUser(userId);
            _dbContext.Add(chat);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Chat", new { id = chat.Id });
        }  

        public IActionResult Private()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var chats = _userRepo.GetUsersbutNoCurrentUser(userId);
            //var chats = _dbContext.Users.Where(x => x.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId).UserName;
            //ViewData["currentUser"] = _dbContext.Users.FirstOrDefault(o => o.Id == userId).UserName;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            return View(chats);
        }

        public IActionResult ClearNoti()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var users = _userRepo.GetUsersbutNoCurrentUser(userId);
            _notificationRepo.ClearNotification(userId);
            //var users = _dbContext.Users.Where(o => o.Id != userId).ToList();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}