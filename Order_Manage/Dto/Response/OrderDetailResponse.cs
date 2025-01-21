namespace Order_Manage.Dto.Response
{
    public class OrderDetailResponse
    {
        public int? OrderDetailId { get; set; }
        public List<ProductResponse>? ListProducts { get; set; }
    }
}
