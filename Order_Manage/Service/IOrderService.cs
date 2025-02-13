using System.Security.Claims;
using Order_Manage.Common.Constants.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;

namespace Order_Manage.Service
{
    public interface IOrderService
    {
        ApiResponse<int> CreateOrder(OrderRequest orderRequest, ClaimsPrincipal user);
        ApiResponse<int> ProcessKafkaOrderz(OrderCreatedEvent orderEvent);

        Task ProcessKafkaOrder(string orderJson);
        ApiResponse<List<OrderResponse>> GetUserOrders(ClaimsPrincipal user);
    }
}
