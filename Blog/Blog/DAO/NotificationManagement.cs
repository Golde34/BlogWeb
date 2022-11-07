using Blog.Models.Enums;
using Blog.Models;

namespace Blog.DAO
{
    public class NotificationManagement
    {
        private static NotificationManagement instance;
        private static readonly object instancelock = new object();

        public NotificationManagement()
        {
        }

        public static NotificationManagement Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (instance == null) instance = new NotificationManagement();
                }
                return instance;
            }
        }

        public List<Notification> GetNotifications(string userId)
        {
            List<Notification> Notifications;
            try
            {
                var _context = new AppDBContext();
                Notifications = _context.Notifications
                    .Where(s => s.UserID == userId)
                    .OrderByDescending(s => s.Created)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Notifications;
        }
    }
}
