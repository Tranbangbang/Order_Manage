namespace Order_Manage.Dto.Response
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public OrderDetailResponse? OrderDetail { get; set; }
    }
}
