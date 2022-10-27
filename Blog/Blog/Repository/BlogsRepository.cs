using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class BlogsRepository : IBlogsRepo
    {
        public async void AddBlogs(Blogs Blogs) => await BlogsManagement.Instance.AddBlogs(Blogs);

        public void DeleteBlogs(Blogs Blogs)
        {
            throw new NotImplementedException();
        }

        public Blogs GetBlogById(int? id) => BlogsManagement.Instance.GetBlogsByID(id);

        public IEnumerable<Blogs> GetBlogs() => BlogsManagement.Instance.GetBlogsList();

        public void UpdateBlogs(Blogs Blogs)
        {
            throw new NotImplementedException();
        }
    }
}