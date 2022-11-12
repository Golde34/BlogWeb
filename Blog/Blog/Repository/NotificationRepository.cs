using Blog.DAO;
using Blog.Models;
using Blog.Repository.@interface;

namespace Blog.Repository
{
    public class NotificationRepository : INotificationRepo
    {
        public void AddNotification(Notification Notification)
        {
            throw new NotImplementedException();
        }

        public void DeleteNotification(Notification Notification)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notification> GetNotifications(string userId) => NotificationManagement.Instance.GetNotifications(userId);

        public void UpdateNotification(Notification Notification)
        {
            throw new NotImplementedException();
        }

        public List<Notification> ClearNotification(string userId) => NotificationManagement.Instance.ClearNotification(userId);
    }
}
