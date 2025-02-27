namespace Order_Manage.Kafka.Dto
{
    public class LogEntry
    {
        public string? Timestamp { get; set; }
        public string? Service { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? RequestId { get; set; }
        public string? StackTrace { get; set; }
    }
}
