﻿using Blog.Models;

namespace Blog.Repository.@interface
{
    public interface IBlogsRepo
    {
        IEnumerable<Blogs> GetBlogs(string userId);
        Task<Blogs> AddBlogs(Blogs Blogs);
        void UpdateBlogs(Blogs Blogs);
        void DeleteBlogs(Blogs Blogs);
        Blogs GetBlogById(int? id);
    }
}
