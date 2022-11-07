using Blog.Models;

namespace Blog.Repository.@interface
{
    public interface INotificationRepo
{
        IEnumerable<Notification> GetNotifications(string userId);
        void AddNotification(Notification Notification);
        void UpdateNotification(Notification Notification);
        void DeleteNotification(Notification Notification);

    }
}
