using System.Security.Claims;
using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Mapper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Exceptions;
using Order_Manage.Repository;

namespace Order_Manage.Service.Impl
{
    public class OrderServiceImpl : IOrderService
    {
        public readonly IOrderRepository _orderRepository;
        public readonly ILogger _logger;

        public OrderServiceImpl(IOrderRepository orderRepository, ILogger<IOrderRepository> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }
        public ApiResponse<int> CreateOrder(OrderRequest orderRequest, ClaimsPrincipal user)
        {
            try
            {
                if (orderRequest.OrderDetails == null || !orderRequest.OrderDetails.Any())
                {
                    return ApiResponse<int>.Error((int)ErrorCode.ORDER_DETAILS_MISSING, ErrorCode.ORDER_DETAILS_MISSING.GetMessage());
                }

                var accountId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    return ApiResponse<int>.Error((int)ErrorCode.USER_NOT_AUTHORIZED, ErrorCode.USER_NOT_AUTHORIZED.GetMessage());
                }
                var order = OrderMapper.ToEntity(orderRequest);
                order.AccountId = accountId;

                var orderId = _orderRepository.CreateOrder(order);
                return ApiResponse<int>.Success(orderId, "Order created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order");
                return ApiResponse<int>.Error((int)ErrorCode.ORDER_CREATION_FAILED, ErrorCode.ORDER_CREATION_FAILED.GetMessage());
            }
        }


        public ApiResponse<List<OrderResponse>> GetUserOrders(ClaimsPrincipal user)
        {
            try
            {
                var accountId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(accountId))
                {
                    return ApiResponse<List<OrderResponse>>.Error((int)ErrorCode.USER_NOT_AUTHORIZED, ErrorCode.USER_NOT_AUTHORIZED.GetMessage());
                }
                var orders = _orderRepository.GetOrdersByUser(accountId);
                if(orders == null)
                    return ApiResponse<List<OrderResponse>>.Error((int)ErrorCode.ORDER_NOT_FOUND, ErrorCode.ORDER_NOT_FOUND.GetMessage());
                return ApiResponse<List<OrderResponse>>.Success(orders, "Order history retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving order history");
                return ApiResponse<List<OrderResponse>>.Error((int)ErrorCode.ORDER_HISTORY_FAILED, ErrorCode.ORDER_HISTORY_FAILED.GetMessage());
            }
        }
    }
}
