namespace Order_Manage.Dto.Request
{
    public class UpdateUserRequest
    {
        public string? Id { get; set; }
        public string? AccountName { get; set; }
        public string? Major { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? Email { get; set; }
    }
}
