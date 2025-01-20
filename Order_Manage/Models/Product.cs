using System.ComponentModel.DataAnnotations;

namespace Order_Manage.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
