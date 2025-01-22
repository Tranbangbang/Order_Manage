using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Repository;

namespace Order_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet("{senderId}/{receiverId}")]
        public IActionResult GetChatHistory(string senderId, string receiverId)
        {
            try
            {
                var messages = _messageRepository.GetChatHistoryAsync(senderId, receiverId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
