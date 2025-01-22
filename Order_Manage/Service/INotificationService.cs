using Order_Manage.Common.Constants.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Models;

namespace Order_Manage.Service
{
    public interface INotificationService
    {
        ApiResponse<int> CreateNotification(NotificationRequest request);
        ApiResponse<List<NotificationResponse>> GetNotificationsByUser(int userId);
        ApiResponse<bool> MarkNotificationAsRead(int notificationId);
    }
}
