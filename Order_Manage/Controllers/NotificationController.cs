using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Dto.Request;
using Order_Manage.Service;

namespace Order_Manage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("create")]
        public  IActionResult CreateNotification([FromBody] NotificationRequest request)
        {
            var response =  _notificationService.CreateNotification(request);
            return StatusCode(response.Code, response);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetNotificationsByUser(int userId)
        {
            var response = _notificationService.GetNotificationsByUser(userId);
            return StatusCode(response.Code, response);
        }

        [HttpPost("read/{notificationId}")]
        public IActionResult MarkNotificationAsRead(int notificationId)
        {
            var response = _notificationService.MarkNotificationAsRead(notificationId);
            return StatusCode(response.Code, response);
        }
    }

}
