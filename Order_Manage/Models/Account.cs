using Microsoft.AspNetCore.Identity;

namespace Order_Manage.Models
{
    public class Account : IdentityUser
    {
        public string? AccountName { get; set; }
        public string? Major {  get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime createAt { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
