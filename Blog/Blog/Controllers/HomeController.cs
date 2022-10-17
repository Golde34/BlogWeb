using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using Blog.Models.Enums;
using System.Security.Claims;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private AppDBContext _dbContext;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var chats = _dbContext.Chats.Include(o => o.Users).ThenInclude(o => o.User)
                .Where(o => o.Type == ChatType.Room)
                .ToList();
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["currentUser"] = _dbContext.Users.FirstOrDefault(o => o.Id == userId).UserName;
            return View(chats);
        }

        
        [HttpGet]
        public IActionResult Edit()
        {

            return View(new Blogs());
        }

        [HttpPost]
        public IActionResult Edit(Blogs blog)
        {

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