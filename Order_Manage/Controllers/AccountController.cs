using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Dto.Request;
using Order_Manage.Service;

namespace Order_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("send-code")]
        public IActionResult SendCodeToEmail([FromBody] ViaCodeRequest request)
        {
            if (string.IsNullOrEmpty(request.email))
            {
                return BadRequest("Email is required");
            }
            var response = _accountService.handleSendCodeToMail(request);
            return StatusCode(response.Code, response);
        }
    }
}
