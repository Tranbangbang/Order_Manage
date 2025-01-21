using System.ComponentModel.DataAnnotations;

namespace Order_Manage.Dto.Request
{
    public class OrderRequest
    {
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Order must contain at least one OrderDetail")]
        [MinLength(1, ErrorMessage = "Order must contain at least one OrderDetail")]
        public List<OrderDetailRequest> OrderDetails { get; set; } = new List<OrderDetailRequest>();
    }
}
