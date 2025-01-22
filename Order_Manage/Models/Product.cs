using System.ComponentModel.DataAnnotations;
using Order_Manage.Common.Abstract;

namespace Order_Manage.Models
{
    public class Product : Auditable
    {
        [Key]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
