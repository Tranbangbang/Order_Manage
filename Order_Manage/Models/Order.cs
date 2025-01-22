using System.ComponentModel.DataAnnotations;
using Order_Manage.Common.Abstract;

namespace Order_Manage.Models
{
    public class Order : Auditable
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public string? AccountId { get; set; }
        public Account? Account { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
