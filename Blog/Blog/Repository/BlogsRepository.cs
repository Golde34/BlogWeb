using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class BlogsRepository : IBlogsRepo
    {
        public async Task<Blogs> AddBlogs(Blogs Blogs) => await BlogsManagement.Instance.AddBlogs(Blogs);

        public void DeleteBlogs(Blogs Blogs) => BlogsManagement.Instance.DeleteBlogs(Blogs);

        public Blogs GetBlogById(int? id) => BlogsManagement.Instance.GetBlogsByID(id);

        public IEnumerable<Blogs> GetBlogs(string userId) => BlogsManagement.Instance.GetBlogsList(userId);

        public void UpdateBlogs(Blogs Blogs) => BlogsManagement.Instance.UpdateBlogs(Blogs);
    }
}