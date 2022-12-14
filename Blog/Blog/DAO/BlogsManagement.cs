using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Blog.Models.Enums;

namespace Blog.DAO
{
    public class BlogsManagement
    {
        private static BlogsManagement instance;
        private static readonly object instancelock = new object();

        public BlogsManagement()
        {
        }

        public static BlogsManagement Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (instance == null) instance = new BlogsManagement();
                }
                return instance;
            }
        }

        public IEnumerable<Blogs> GetBlogsList(string id)
        {
            List<Blogs> Blogss;
            try
            {
                var context = new AppDBContext();
                Blogss = context.Blogs.Include(b => b.User)
                    .Where(b => b.UserId == id && b.Status == BlogStatus.Public)
                    .OrderByDescending(b => b.Created).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogss;
        }

        public IEnumerable<Blogs> GetTop5Blogs(string id)
        {
            List<Blogs> Blogss;
            try
            {
                var context = new AppDBContext();
                Blogss = context.Blogs.Include(b => b.User)
                    .Where(b => b.UserId == id && b.Status == BlogStatus.Public)
                    .OrderByDescending(b => b.Created).Take(5).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogss;
        }

        public async Task<Blogs?> GetBlogsByID(int? BlogsID)
        {
            Blogs Blogs = null;
            try
            {
                var context = new AppDBContext();
                Blogs = context.Blogs.Include(b => b.User).FirstOrDefault(m => m.Id == BlogsID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogs;
        }

        public async Task<Blogs> AddBlogs(Blogs Blogs)
        {
            try
            {
                Blogs _Blogs = await GetBlogsByID(Blogs.Id);
                if (_Blogs == null)
                {
                    var context = new AppDBContext();
                    context.Blogs.Add(Blogs);
                    await context.SaveChangesAsync();
                    return _Blogs; 
                }
                else
                {
                    throw new Exception("The Blogs is already exist");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Blogs> UpdateBlogs(Blogs Blogs)
        {
            try
            {
                Blogs _Blogs = await GetBlogsByID(Blogs.Id);
                if (_Blogs != null)
                {
                    var context = new AppDBContext();
                    context.Entry<Blogs>(Blogs).State = EntityState.Modified;
                    context.SaveChanges();
                    return _Blogs;
                }
                else
                {
                    throw new Exception("The Blogs does not already exist");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Blogs> DeleteBlogs(Blogs Blogs)
        {
            try
            {
                Blogs _Blogs = await GetBlogsByID(Blogs.Id);
                if (_Blogs != null)
                {
                    var context = new AppDBContext();
                    context.Blogs.Remove(Blogs);
                    context.SaveChanges();
                    return _Blogs;
                }
                else
                {
                    throw new Exception("The Blogs does not already exist");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public IEnumerable<Blogs> GetTop5BlogsPreview(string id)
        {
            List<Blogs> Blogss;
            try
            {
                var context = new AppDBContext();
                Blogss = context.Blogs.Include(b => b.User)
                    .Where(b => b.UserId == id && b.Status == BlogStatus.Preview)
                    .OrderByDescending(b => b.Created).Take(5).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogss;
        }

        public IEnumerable<Blogs> GetAllBlogs(string id)
        {
            List<Blogs> Blogs;
            try
            {
                var context = new AppDBContext();
                Blogs = context.Blogs.Include(b => b.User)
                    .Where(b => b.UserId == id).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogs;
        }

        public IEnumerable<Blogs> GetAllPublicBlogs()
        {
            List<Blogs> Blogs;
            try
            {
                var context = new AppDBContext();
                Blogs = context.Blogs.Include(u => u.User)
                    .Where(b => b.Status == BlogStatus.Public)
                    .OrderByDescending(c => c.Created).ToList();
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogs;
        }

        public IEnumerable<Blogs> GetTop5LatestPublicBLogs()
        {
            List<Blogs> blogs;
            try
            {
                var context = new AppDBContext();
                blogs = context.Blogs.Include(u => u.User)
                    .Where(b => b.Status == BlogStatus.Public)
                    .OrderByDescending(c => c.Created).Take(5)
                    .ToList();
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return blogs;
        }
    }
}
