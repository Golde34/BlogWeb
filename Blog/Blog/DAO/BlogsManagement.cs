using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

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

        public IEnumerable<Blogs> GetBlogsList()
        {
            List<Blogs> Blogss;
            try
            {
                var context = new AppDBContext();
                Blogss = context.Blogs.Include(b => b.User).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Blogss;
        }

        public Blogs? GetBlogsByID(int? BlogsID)
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
                Blogs _Blogs = GetBlogsByID(Blogs.Id);
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

        public void UpdateBlogs(Blogs Blogs)
        {
            try
            {
                Blogs _Blogs = GetBlogsByID(Blogs.Id);
                if (_Blogs != null)
                {
                    var context = new AppDBContext();
                    context.Entry<Blogs>(Blogs).State = EntityState.Modified;
                    context.SaveChanges();
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

        public void DeleteBlogs(Blogs Blogs)
        {
            try
            {
                Blogs _Blogs = GetBlogsByID(Blogs.Id);
                if (_Blogs != null)
                {
                    var context = new AppDBContext();
                    context.Blogs.Remove(Blogs);
                    context.SaveChanges();
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
    }
}
