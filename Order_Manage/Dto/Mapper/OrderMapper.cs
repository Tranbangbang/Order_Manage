using Order_Manage.Dto.Request;
using Order_Manage.Models;

namespace Order_Manage.Dto.Mapper
{
    public static class OrderMapper
    {
        public static Order ToEntity(OrderRequest orderRequest)
        {
            if (orderRequest == null) throw new ArgumentNullException(nameof(orderRequest));

            return new Order
            {
                OrderDate = orderRequest.OrderDate,
                OrderDetails = orderRequest.OrderDetails?.Select(detail => new OrderDetail
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity
                }).ToList()
            };
        }
        public static OrderRequest ToRequest(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            return new OrderRequest
            {
                OrderDate = order.OrderDate,
                OrderDetails = order.OrderDetails?.Select(detail => new OrderDetailRequest
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity
                }).ToList()
            };
        }
    }
}
