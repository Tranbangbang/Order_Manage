using Order_Manage.Dto.Response;
using Order_Manage.Models;

namespace Order_Manage.Repository
{
    public interface IOrderRepository
    {
        int CreateOrder(Order order);
        List<OrderResponse> GetOrdersByUser(string accountId);
    }
}
