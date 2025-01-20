namespace Order_Manage.Dto.Response
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? AccName { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
