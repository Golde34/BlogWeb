using Blog.Hubs;
using Blog.Models;
using Blog.Models.Enums;
using Blog.Repository;
using Blog.Repository.@interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text.Json;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Blog.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : Controller
    {
        private IHubContext<ChatHub> _hubContext;
        private IHubContext<NotificationHub> _hubNotiContext;
        private readonly ILogger<HomeController> _logger;
        private AppDBContext _dbContext;
        private IChatRepo _chatRepo = new ChatRepository();
        private IUserRepo _userRepo = new UserRepository();
        private INotificationRepo _notificationRepo = new NotificationRepository();
        public ChatController(IHubContext<ChatHub> hubContext, IHubContext<NotificationHub> hubNotiContext,
            ILogger<HomeController> logger, AppDBContext dbContext)
        {
            _hubContext = hubContext;
            _hubNotiContext = hubNotiContext;
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> JoinRoom(string connectionId, string roomId, [FromServices] UserManager<User> _userManager)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, roomId);
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            string userJson = JsonSerializer.Serialize(user);
            await _hubContext.Clients.Groups(roomId).SendAsync("UserAdded", userJson);
            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> JoinRoom(int id, [FromServices] UserManager<User> _uaserManager)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _uaserManager.FindByIdAsync(userId);
            Chat chats = _dbContext.Chats.Include(o => o.Users).FirstOrDefault(o => o.Id == id);
            if (chats.Users.FirstOrDefault(o => o.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value) != null)
            {
                return RedirectToAction("Chat", "Home", new { id = id });
            }
            var chatuser = new ChatUser
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ChatId = id,
            };
            _dbContext.ChatUsers.Add(chatuser);
            var msg = new Message
            {
                ChatID = id,
                Text = user.UserName + " has joined the group.",
                UserID = userId,
                Timestamp = DateTime.Now,
                MessageType = MessageType.Notification
            };
            _dbContext.Add(msg);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Groups(id.ToString()).SendAsync("NewUserJoined", msg.Text);
            return RedirectToAction("Chat", "Home", new { id = id });
        }

        [HttpPost("[action]/{connectionId}/{roomId}")]
        public async Task<IActionResult> LeaveRoom(string connectionId, string roomId, [FromServices] UserManager<User> _userManager)
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, roomId);
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            string userJson = JsonSerializer.Serialize(user);
            await _hubContext.Clients.Groups(roomId).SendAsync("UserRemoved", userJson);
            return Ok();
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> LeaveRoom(int id, [FromServices] UserManager<User> _userManager)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            Chat chats = _dbContext.Chats
                .Include(x => x.Users)
                .FirstOrDefault(x => x.Id == id);
            var msg = new Message
            {
                ChatID = id,
                Text = user.UserName + " has left the group.",
                UserID = userId,
                Timestamp = DateTime.Now,
                MessageType = MessageType.Notification
            };
            _dbContext.Messages.Add(msg);
            var chatUser = _dbContext.ChatUsers.Where(x => (x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.ChatId == id)).FirstOrDefault();
            _dbContext.ChatUsers.Remove(chatUser);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Groups(id.ToString()).SendAsync("UserLeft", msg.Text);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(string message, int chatId,
            string roomId, IFormFile image, [FromServices] IHostingEnvironment hostingEnvironment,
            [FromServices] UserManager<User> _userManager)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var chatRoom = _chatRepo.GetPrivateChatById(chatId);
            var userNoti = _userRepo.GetNotificationUser(chatId, userId);
            Notification notification = new Notification();
            if (chatRoom == null)
            {
                notification = null;
            } else
            {
                notification = new Notification()
                {
                    Content = user.UserName + " has text for you, " + userNoti.User.UserName,
                    UserID = userNoti.UserId,
                    Created = DateTime.Now,
                    Type = NotificationType.PrivateMessage,
                };
            }

            if (image != null)
            {
                string uniqueFileName;
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                using (var fs = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                {
                    await image.CopyToAsync(fs);
                }
                var imagemsg = new Message
                {
                    ChatID = chatId,
                    Text = uniqueFileName,
                    UserID = userId,
                    Timestamp = DateTime.Now,
                    MessageType = MessageType.Image
                };
                var imagemsgDate = imagemsg.Timestamp.ToString("dd/MM/yyyy hh:mm:ss");
                _dbContext.Messages.Add(imagemsg);
                if (notification != null)
                {
                    _dbContext.Notifications.Add(notification);
                }
                await _dbContext.SaveChangesAsync();
                await _hubNotiContext.Clients.Groups(roomId)
                    .SendAsync("GetNotification", notification.Content, userNoti.User.ToString(), notification.Created.ToString());
                await _hubContext.Clients.Groups(roomId)
                    .SendAsync("ReceiveMessage", imagemsg, user, imagemsgDate);
            }
            if (message == null)
            {
                return Ok();
            }
            if (message.Trim().Length > 0)
            {

                var msg = new Message
                {
                    ChatID = chatId,
                    Text = message,
                    UserID = userId,
                    Timestamp = DateTime.Now,
                    MessageType = MessageType.Text
                };
                var date = msg.Timestamp.ToString("dd/MM/yyyy hh:mm:ss");
                _dbContext.Messages.Add(msg);
                if (notification != null)
                {
                    _dbContext.Notifications.Add(notification);
                }
                await _dbContext.SaveChangesAsync();
                await _hubNotiContext.Clients.Groups(roomId)
                    .SendAsync("GetNotification", notification.Content, userNoti.User.ToString(), notification.Created.ToString());
                await _hubContext.Clients.Groups(roomId)
                    .SendAsync("ReceiveMessage", msg, user, date);

            }
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Rename(string newName, int chatId, string roomId,
            [FromServices] UserManager<User> _userManager)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var chat = _dbContext.Chats.FirstOrDefault(x => x.Id == chatId);
            chat.Name = newName;
            var msg = new Message
            {
                ChatID = chatId,
                Text = user.UserName + " named the group " + newName + ".",
                UserID = userId,
                Timestamp = DateTime.Now,
                MessageType = MessageType.Notification
            };
            var date = msg.Timestamp.ToString("dd/MM/yyyy hh:mm:ss");
            _dbContext.Messages.Add(msg);
            await _dbContext.SaveChangesAsync();
            await _hubContext.Clients.Groups(roomId)
                .SendAsync("Rename", msg.Text, newName, chatId);
            return Ok();
        }
    }
}
