using Order_Manage.Dto.Request;

namespace Order_Manage.Dto.Response
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailRequest> OrderDetails { get; set; } = new List<OrderDetailRequest>();
    }
}
