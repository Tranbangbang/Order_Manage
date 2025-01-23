using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Service;

namespace Order_Manage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private readonly IQrCodeService _qrCodeService;

        public QrCodeController(IQrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        [HttpGet("generate")]
        public IActionResult GenerateQrCode([FromQuery] string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return BadRequest("Content is required to generate a QR code.");
                }
                var qrCodeBytes = _qrCodeService.GenerateQrCode(content);
                return File(qrCodeBytes, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
