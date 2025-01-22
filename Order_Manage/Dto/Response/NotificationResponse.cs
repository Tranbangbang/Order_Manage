namespace Order_Manage.Dto.Response
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string? UserId { get; set; }
        public string? NotificationType { get; set; }
        public string? NotificationMessage { get; set; } 
        public bool ReadFlg { get; set; }
        public string? RedirectUrl { get; set; }
        public string? UrlBody { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
