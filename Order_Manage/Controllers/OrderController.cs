using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Common.Constants.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Dto.Response;
using Order_Manage.Kafka;
using Order_Manage.Kafka.Impl;
using Order_Manage.Service;

namespace Order_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public IOrderService _orderService;
        private readonly KafkaProducerService _kafkaProducer;
        public OrderController(IOrderService orderService, KafkaProducerService kafkaProducer) {
            _orderService = orderService;
            _kafkaProducer = kafkaProducer;
        }
        /*
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{UserRoles.User}")]
        public IActionResult CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = _orderService.CreateOrder(orderRequest, User);
            return StatusCode(response.Code, response);
        }
        */

        /*
        
        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderEvent = new OrderCreatedEvent
            {
                OrderId = Guid.NewGuid(),
                OrderDate = orderRequest.OrderDate,
                //TotalAmount = orderRequest.OrderDetails.Sum(d => d.Quantity * 1000),
                OrderDetails = orderRequest.OrderDetails
            };

            // Gửi order vào Kafka
            await _kafkaProducer.SendMessageAsync(orderEvent);
            Console.WriteLine($"Đã gửi order vào Kafka: {orderEvent.OrderId}");

            return Ok(new { message = "Order đã được gửi vào Kafka và đang được xử lý!", orderId = orderEvent.OrderId });
        }
        */
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _kafkaProducer.SendMessageAsync(orderRequest);

            return Ok(new { message = "Order đã gửi vào Kafka", orderId = orderRequest.OrderDetails});
        }

        /*
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreatedEvent order)
        {
            order.OrderId = Guid.NewGuid();
            await _kafkaProducer.SendMessageAsync(order);
            return Ok(new { message = "Order đã gửi vào Kafka", orderId = order.OrderId });
        }
        */


        [HttpGet("history")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{UserRoles.User}")]
        public IActionResult GetOrderHistory()
        {
            var response = _orderService.GetUserOrders(User);
            return StatusCode(response.Code, response);
        }

    }
}
