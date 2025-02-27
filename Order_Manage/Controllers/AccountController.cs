using Microsoft.AspNetCore.Mvc;
using Serilog;
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
                Log.Warning("Email is required, but was not provided.");
                return BadRequest("Email is required");
            }

            Log.Information("Request to send code to email: {Email}", request.email);

            var response = _accountService.handleSendCodeToMail(request);
            Log.Information("Code sent to email: {Email} with response code: {ResponseCode}", request.email, response.Code);

            return StatusCode(response.Code, response);
        }
    }
}
