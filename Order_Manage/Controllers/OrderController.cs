using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Service;

namespace Order_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public IOrderService _orderService;

        public OrderController(IOrderService orderService) {
            _orderService = orderService;
        }

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

        [HttpGet("history")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{UserRoles.User}")]
        public IActionResult GetOrderHistory()
        {
            var response = _orderService.GetUserOrders(User);
            return StatusCode(response.Code, response);
        }

    }
}
