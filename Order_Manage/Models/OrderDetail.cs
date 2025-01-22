using System.ComponentModel.DataAnnotations;

namespace Order_Manage.Models
{
    public class OrderDetail 
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal TotalPrice => Quantity * Product.Price;
    }
}
