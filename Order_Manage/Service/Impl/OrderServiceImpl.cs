using System.Security.Claims;
using Newtonsoft.Json;
using Order_Manage.Common.Constants.Helper;
using Order_Manage.Dto.Mapper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Exceptions;
using Order_Manage.Models;
using Order_Manage.Repository;

namespace Order_Manage.Service.Impl
{
    public class OrderServiceImpl : IOrderService
    {
        public readonly IOrderRepository _orderRepository;
        public readonly ILogger _logger;
        public readonly INotificationService _notificationService;
        public OrderServiceImpl(IOrderRepository orderRepository, ILogger<IOrderRepository> logger, INotificationService notificationService)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _notificationService = notificationService;
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
                var notificationRequest = new NotificationRequest
                {
                    UserId = accountId,
                    NotificationType = "Order",
                    NotificationMessage = $"Your order #{orderId} has been created successfully!",
                    RedirectUrl = $"/orders/{orderId}",
                    ReadFlg = false
                };
                var notificationResponse = _notificationService.CreateNotification(notificationRequest);

                if (notificationResponse == null)
                {
                    _logger.LogWarning("Notification not sent successfully for order #{OrderId}", orderId);
                }
                return ApiResponse<int>.Success(orderId, "Order created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order");
                return ApiResponse<int>.Error((int)ErrorCode.ORDER_CREATION_FAILED, ErrorCode.ORDER_CREATION_FAILED.GetMessage());
            }
        }

        public ApiResponse<int> ProcessKafkaOrderz(OrderCreatedEvent orderEvent)
        {
            try
            {
                if (orderEvent == null || orderEvent.OrderDetails == null || !orderEvent.OrderDetails.Any())
                {
                    return ApiResponse<int>.Error((int)ErrorCode.ORDER_DETAILS_MISSING, "Order details are missing");
                }
                var order = new Order
                {
                    OrderDate = orderEvent.OrderDate,
                    //TotalAmount = orderEvent.TotalAmount,
                    AccountId = "KafkaConsumer",
                    OrderDetails = orderEvent.OrderDetails.Select(d => new OrderDetail
                    {
                        ProductId = d.ProductId,
                        Quantity = d.Quantity
                    }).ToList() ?? new List<OrderDetail>()
                };

                var orderId = _orderRepository.CreateOrder(order);

                Console.WriteLine($"Đã lưu Order {orderId} vào DB từ Kafka");

                return ApiResponse<int>.Success(orderId, "Order processed from Kafka");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý order từ Kafka");
                return ApiResponse<int>.Error((int)ErrorCode.ORDER_CREATION_FAILED, "Failed to process Kafka order");
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

        public async Task ProcessKafkaOrder(string orderJson)
        {
            try
            {
                var orderRequest = JsonConvert.DeserializeObject<OrderRequest>(orderJson);

                if (orderRequest == null || orderRequest.OrderDetails == null || !orderRequest.OrderDetails.Any())
                {
                    _logger.LogWarning("Order rỗng hoặc không hợp lệ: {OrderJson}", orderJson);
                    return;
                }
                var order = new Order
                {
                    OrderDate = orderRequest.OrderDate,
                    OrderDetails = orderRequest.OrderDetails.Select(d => new OrderDetail
                    {
                        ProductId = d.ProductId,
                        Quantity = d.Quantity
                    }).ToList()
                };

                 _orderRepository.CreateOrder(order); 
                _logger.LogInformation("Đã xử lý và lưu Order: {OrderId}", order.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi xử lý Order từ Kafka");
            }
        }

    }
}
