using Microsoft.AspNetCore.SignalR;
using Order_Manage.Common.Constants.Helper;
using Order_Manage.Common.Hubs;
using Order_Manage.Dto.Mapper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Models;
using Order_Manage.Repository;

namespace Order_Manage.Service.Impl
{
    public class NotificationServiceImpl : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;


        public NotificationServiceImpl(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public ApiResponse<int> CreateNotification(NotificationRequest request)
        {
            try
            {
                var notification = NotificationMapper.ToEntity(request);
                var notificationId = _notificationRepository.CreateNotification(notification);
                _hubContext.Clients.User(request.UserId)
                    .SendAsync("ReceiveNotification", new
                    {
                        message = request.NotificationMessage,
                        type = request.NotificationType,
                        redirectUrl = request.RedirectUrl
                    });
                return ApiResponse<int>.Success(notificationId, "Notification created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.Error(500, $"An error occurred: {ex.Message}");
            }
        }

        public ApiResponse<List<NotificationResponse>> GetNotificationsByUser(int userId)
        {
            try
            {
                var notifications = _notificationRepository.GetNotificationsByUser(userId)
                    .Select(NotificationMapper.ToResponse)
                    .ToList();
                return ApiResponse<List<NotificationResponse>>.Success(notifications, "Notifications retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<NotificationResponse>>.Error(500, $"An error occurred: {ex.Message}");
            }
        }

        public ApiResponse<bool> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                _notificationRepository.MarkNotificationAsRead(notificationId);
                return ApiResponse<bool>.Success(true, "Notification marked as read");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
