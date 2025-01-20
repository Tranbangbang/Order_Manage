namespace Order_Manage.Dto.Request
{
    public class RegisterRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? AccountName { get; set; }
        public string? Major { get; set; }
        public string? Role { get; set; }
    }
}
