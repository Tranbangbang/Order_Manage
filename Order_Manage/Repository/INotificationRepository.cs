using Order_Manage.Models;

namespace Order_Manage.Repository
{
    public interface INotificationRepository
    {
        int CreateNotification(Notification notification);
        List<Notification> GetNotificationsByUser(int userId);
        void MarkNotificationAsRead(int notificationId);
    }
}
