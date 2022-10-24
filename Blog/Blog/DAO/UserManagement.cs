using Blog.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using Blog.Models.Enums;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Blog.DAO
{
    public class UserManagement
    {
        private static UserManagement instance;
        private static readonly object instancelock = new object();

        public UserManagement()
        {
        }

        public static UserManagement Instance
        { 
            get 
            { 
                lock (instancelock)
                {
                    if (instance == null) instance = new UserManagement();
                }
                return instance; 
            } 
        }

        public List<IdentityUser> GetUserBySearchString(string search, string userId)
        {
            List<IdentityUser>  users = null;
            try
            {
                var _context = new AppDBContext();
                users = _context.Users
                    .Where(o => o.Id != userId)
                    .Where(o => o.UserName.Contains(search) || o.Email.Contains(search))
                    .ToList();
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return users;
        }

        public List<IdentityUser> GetUserById(string search, string userId)
        {
            List<IdentityUser> users = null;
            try
            {
                var _context = new AppDBContext();
                users = _context.Users
                    .Where(o => o.Id != userId)
                    .Where(o => o.UserName.Contains(search) || o.Email.Contains(search))
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return users;
        }

        internal List<IdentityUser> GetUsersButNoCurrentUser(string userId)
        {
            List<IdentityUser> users = null;
            try
            {
                var _context = new AppDBContext();
                users = _context.Users
                    .Where(o => o.Id != userId)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return users;
        }

        internal IdentityUser GetCurrentUser(string userId)
        {
            IdentityUser user = null;
            try
            {
                var _context = new AppDBContext();
                user = _context.Users.FirstOrDefault(o => o.Id == userId);
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return user;
        }
    }
}
