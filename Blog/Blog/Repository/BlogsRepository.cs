using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class BlogsRepository : IBlogsRepo
    {
        public async Task<Blogs> AddBlogs(Blogs Blogs) => await BlogsManagement.Instance.AddBlogs(Blogs);

        public async Task<Blogs> DeleteBlogs(Blogs Blogs) => await BlogsManagement.Instance.DeleteBlogs(Blogs);

        public async Task<Blogs> GetBlogById(int? id) => await BlogsManagement.Instance.GetBlogsByID(id);

        public IEnumerable<Blogs> GetBlogs(string userId) => BlogsManagement.Instance.GetBlogsList(userId);

        public async Task<Blogs> UpdateBlogs(Blogs Blogs) => await BlogsManagement.Instance.UpdateBlogs(Blogs);

        public IEnumerable<Blogs> GetTop5Blogs(string userId) => BlogsManagement.Instance.GetTop5Blogs(userId);
    }
}