using System.ComponentModel.DataAnnotations;

namespace Order_Manage.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string? AccountId { get; set; }
        public Account? Account { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
