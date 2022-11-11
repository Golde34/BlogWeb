using Blog.Models;

namespace Blog.Repository.@interface
{
    public interface IBlogsRepo
    {
        IEnumerable<Blogs> GetBlogs(string userId);
        Task<Blogs> AddBlogs(Blogs Blogs);
        Task<Blogs> UpdateBlogs(Blogs Blogs);
        Task<Blogs> DeleteBlogs(Blogs Blogs);
        Task<Blogs> GetBlogById(int? id);
        IEnumerable<Blogs> GetTop5Blogs(string userId);

    }
}
