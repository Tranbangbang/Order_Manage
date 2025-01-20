namespace Order_Manage.Dto.Request
{
    public class SignupRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? AccountName { get; set; }
        public int Code { get; set; }
    }
}
