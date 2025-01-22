using System.ComponentModel.DataAnnotations;
using Order_Manage.Common.Abstract;

namespace Order_Manage.Models
{
    public class Notification : Auditable
    {
        [Key]
        public int NotificationId { get; set; }
        public string? UserId { get; set; }
        public string? NotificationType { get; set; } 
        public string? NotificationMessage { get; set; } 
        public bool ReadFlg { get; set; }
        public string? RedirectUrl { get; set; }
        public string? UrlBody { get; set; }
        public Account? Account { get; set; }
    }
}
