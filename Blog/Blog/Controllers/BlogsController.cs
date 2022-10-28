using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace Blog.Controllers
{
    public class BlogsController : Controller
    {
        private readonly AppDBContext _context;
        private IBlogsRepo _blogsRepo = new BlogsRepository();
        private IUserRepo _userRepo = new UserRepository();
        public BlogsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            IEnumerable<Blogs> blogList = _blogsRepo.GetBlogs(userId);
            //var appDBContext = _context.Blogs.Include(b => b.User);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogList);
        }

        // GET: Blogs/Details/5
        public IActionResult Details(int? id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (id == null || _context.Blogs == null)
            {
                return NotFound();
            }
            Blogs blogs = _blogsRepo.GetBlogById(id);
            //var blogs = _context.Blogs
            //    .Include(b => b.User)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (blogs == null)
            {
                return NotFound();
            }
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogs);
        }

        // GET: Blogs/Create
        public IActionResult Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id");
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,Title,Url,Image,Intro,Body,UserId")] BlogDTO blogDTO, [FromServices] IHostingEnvironment hostingEnvironment)
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

            _blogsRepo.AddBlogs(blogs);
            //_context.Add(blogs);
            //await _context.SaveChangesAsync();
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
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", blogs.UserId);
            ViewData["currentUser"] = _userRepo.GetCurrentUser(userId);
            return View(blogs);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GenreId,Title,Url,Image,Intro,Body,Created,UserId")] Blogs blogs)
        {
            if (id != blogs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogsExists(blogs.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Set<User>(), "Id", "Id", blogs.UserId);
            return View(blogs);
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
    }
}
