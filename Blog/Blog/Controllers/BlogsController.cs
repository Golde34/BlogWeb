using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Blog.Repository.@interface;
using Blog.Repository;
using Blog.ViewDTO;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Blog.Models.Enums;

namespace Blog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly AppDBContext _context;
        private IBlogsRepo _blogsRepo = new BlogsRepository();
        private IUserRepo _userRepo = new UserRepository();
        private INotificationRepo _notificationRepo = new NotificationRepository();
        public BlogsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<Blogs> blogList = _blogsRepo.GetTop5Blogs(userId);
            //var appDBContext = _context.Blogs.Include(b => b.User);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            List<IdentityUser> listUserBlogs = _userRepo.GetOthersBlogUser(userId);
            ViewData["listUserBlogs"] = listUserBlogs;
            IEnumerable<Blogs> allBlogs = _blogsRepo.GetAllBlogs(userId);
            ViewData["allBlogs"] = allBlogs;
            IEnumerable<Blogs> previewBlogList = _blogsRepo.GetTop5BlogsPreview(userId);
            ViewData["previewBlogList"] = previewBlogList;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            return View(blogList);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }
            Blogs blogs = await _blogsRepo.GetBlogById(id);
            //var blogs = _context.Blogs
            //    .Include(b => b.User)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogs);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id");
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, GenreId,Title,Url,Image,Intro,Body,Status,UserId")] BlogDTO blogDTO, [FromServices] IHostingEnvironment hostingEnvironment)
        {
            string uniqueFileName = null;
            if (blogDTO.Image != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + blogDTO.Image.FileName;
                using (var fs = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                {
                    await blogDTO.Image.CopyToAsync(fs);
                }
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blogs blogs = new Blogs
            {
                GenreId = blogDTO.GenreId,
                Title = blogDTO.Title,
                Url = blogDTO.Url,
                Image = uniqueFileName,
                Intro = blogDTO.Intro,
                Body = blogDTO.Body,
                UserId = userId,
            };
            if (blogDTO.Status == true)
            {
                blogs.Status = BlogStatus.Preview;
            } else
            {
                blogs.Status = BlogStatus.Public;
            }

            await _blogsRepo.AddBlogs(blogs);
            //_context.Add(blogs);
            //await _context.SaveChangesAsync();
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            return RedirectToAction(nameof(Index));
        }

        // GET: Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs == null)
            {
                return NotFound();
            }
            BlogDTO blogDTO = new BlogDTO
            {
                Id = (int)id,
                GenreId = blogs.GenreId,
                Title = blogs.Title,
                Url = blogs.Url,
                Intro = blogs.Intro,
                Body = blogs.Body,
                //Status = blogs.Status == BlogStatus.Preview ? true : false,
                UserId = userId,
            };
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", blogs.UserId);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogDTO);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GenreId,Title,Url,Image,Intro,Body,Status,UserId")] BlogDTO blogDTO)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id != blogDTO.Id)
            {
                return NotFound();
            }

            Blogs blogs = await _blogsRepo.GetBlogById(id);
            blogs.GenreId = blogDTO.GenreId;
            blogs.Title = blogDTO.Title;
            blogs.Url = blogDTO.Url;
            blogs.Intro = blogDTO.Intro;
            blogs.Body = blogDTO.Body;
            blogs.UserId = userId;
            blogs.Created = DateTime.Now;
            if (blogDTO.Status == true)
            {
                blogs.Status = BlogStatus.Public;
            } else
            {
                blogs.Status = BlogStatus.Preview;
            }

            await _blogsRepo.UpdateBlogs(blogs);
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", blogs.UserId);
            return RedirectToAction("Index", "Blogs");
        }

        // GET: Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogs = await _context.Blogs
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogs);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Blogs == null)
            {
                return Problem("Entity set 'AppDBContext.Blogs'  is null.");
            }
            var blogs = await _context.Blogs.FindAsync(id);
            if (blogs != null)
            {
                _context.Blogs.Remove(blogs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogsExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }

        public async Task<IActionResult> BlogPage(int?id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }

            var blogs = await _blogsRepo.GetBlogById(id);
            if (blogs == null)
            {
                return NotFound();
            }
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", blogs.UserId);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogs);
        }

        public async Task<IActionResult> Menu()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<Blogs> blogList = _blogsRepo.GetAllPublicBlogs();
            //var appDBContext = _context.Blogs.Include(b => b.User);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            var top5Lateset = _blogsRepo.GetTop5LatestPublicBLogs();
            ViewData["latestBlogs"] = top5Lateset;
            return View(blogList);
        }

        public async Task<IActionResult> IndexAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<Blogs> blogList = _blogsRepo.GetBlogs(userId);
            //var appDBContext = _context.Blogs.Include(b => b.User);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            List<IdentityUser> listUserBlogs = _userRepo.GetOthersBlogUser(userId);
            ViewData["listUserBlogs"] = listUserBlogs;
            IEnumerable<Blogs> allBlogs = _blogsRepo.GetAllBlogs(userId);
            ViewData["allBlogs"] = allBlogs;
            IEnumerable<Blogs> previewBlogList = _blogsRepo.GetTop5BlogsPreview(userId);
            ViewData["previewBlogList"] = previewBlogList;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            return View("Index", blogList);
        }

        public async Task<IActionResult> Public(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Blogs blogs = await _blogsRepo.GetBlogById(id);
            blogs.Status = BlogStatus.Public;
            await _blogsRepo.UpdateBlogs(blogs);
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", blogs.UserId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OthersBlog(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<Blogs> blogList = _blogsRepo.GetTop5Blogs(id);
            //var appDBContext = _context.Blogs.Include(b => b.User);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            List<IdentityUser> listUserBlogs = _userRepo.GetOthersBlogUser(id);
            ViewData["listUserBlogs"] = listUserBlogs;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            return View(blogList);
        }

        public async Task<IActionResult> IndexOtherAll(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<Blogs> blogList = _blogsRepo.GetBlogs(id);
            //var appDBContext = _context.Blogs.Include(b => b.User);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            List<IdentityUser> listUserBlogs = _userRepo.GetOthersBlogUser(id);
            ViewData["listUserBlogs"] = listUserBlogs;
            var notifications = _notificationRepo.GetNotifications(userId);
            ViewData["notifications"] = notifications;
            return View("OthersBlog", blogList);
        }
    }
}
