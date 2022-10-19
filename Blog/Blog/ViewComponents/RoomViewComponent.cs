using Blog.Models.Enums;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.ViewComponents
{
    [ViewComponent]
    public class RoomViewComponent : ViewComponent
    {
        private AppDBContext _ctx;
        public RoomViewComponent(AppDBContext ctx)
        {
            _ctx = ctx;
        }
        public IViewComponentResult Invoke(ChatType chatType, int chatId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chats = _ctx.ChatUsers
                .Include(x => x.Chat)
                .ThenInclude(x => x.Users)
                .ThenInclude(x => x.User)
                .Where(x => x.UserId == userId)
                .Where(x => x.Chat.Type == chatType)
                .Select(x => x.Chat)
                .ToList();
            ViewBag.ChatId = chatId;
            ViewBag.ChatType = chatType;
            return View(chats);
        }
    }
}
