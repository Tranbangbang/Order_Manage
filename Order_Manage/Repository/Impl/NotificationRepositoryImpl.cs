using Dapper;
using Order_Manage.Common.Constants.Helper;
using Order_Manage.Models;
using Order_Manage.XML;

namespace Order_Manage.Repository.Impl
{
    public class NotificationRepositoryImpl : INotificationRepository
    {
        private readonly DapperContext _context;
        private readonly QueryLoader _queryLoader;

        public NotificationRepositoryImpl(DapperContext context, QueryLoader queryLoader)
        {
            _context = context;
            _queryLoader = queryLoader;
        }

        public int CreateNotification(Notification notification)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("insert-notification", out var insertNotificationQuery))
                throw new KeyNotFoundException("Query 'insert-notification' not found in XML file");

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var notificationId = connection.QuerySingle<int>(insertNotificationQuery, new
                {
                    notification.UserId,
                    notification.NotificationType,
                    notification.NotificationMessage,
                    notification.ReadFlg,
                    notification.RedirectUrl,
                    notification.UrlBody,
                    notification.CreatedAt,
                    notification.CreatedBy,

                }, transaction);
                transaction.Commit();
                return notificationId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public List<Notification> GetNotificationsByUser(int userId)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("get-user-notifications", out var getUserNotificationsQuery))
                throw new KeyNotFoundException("Query 'get-user-notifications' not found in XML file");

            using var connection = _context.CreateConnection();
            connection.Open();
            return connection.Query<Notification>(getUserNotificationsQuery, new { UserId = userId }).ToList();
        }


        public void MarkNotificationAsRead(int notificationId)
        {
            var sql = _queryLoader.Read_Xml();
            if (!sql.TryGetValue("mark-notification-read", out var markNotificationReadQuery))
                throw new KeyNotFoundException("Query 'mark-notification-read' not found in XML file");

            using var connection = _context.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(markNotificationReadQuery, new { NotificationId = notificationId }, transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

    }
}
