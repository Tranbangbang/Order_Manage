using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Models;

namespace Order_Manage.Dto.Mapper
{
    public class NotificationMapper
    {
        public static NotificationResponse ToResponse(Notification notification)
        {
            return new NotificationResponse
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                NotificationType = notification.NotificationType,
                NotificationMessage = notification.NotificationMessage,
                ReadFlg = notification.ReadFlg,
                RedirectUrl = notification.RedirectUrl,
                UrlBody = notification.UrlBody,
                CreatedAt = notification.CreatedAt
            };
        }

        public static Notification ToEntity(NotificationRequest request)
        {
            return new Notification
            {
                NotificationId = request.NotificationId,
                UserId = request.UserId,
                NotificationType = request.NotificationType,
                NotificationMessage = request.NotificationMessage,
                ReadFlg = request.ReadFlg,
                RedirectUrl = request.RedirectUrl,
                UrlBody = request.UrlBody,
                CreatedAt = DateTime.UtcNow, 
                //CreatedBy = 1 
            };
        }
    }
}
