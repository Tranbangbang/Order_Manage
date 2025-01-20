namespace Order_Manage.Dto.Request
{
    public class EmailRequest
    {
        public string? To { get; set; }
        public string? Subject { get; set; }
        public string? Content { get; set; }
    }
}
