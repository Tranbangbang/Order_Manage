using Logging_Service.Config;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using Logging_Service.Service;

namespace Logging_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;
        private static readonly Dictionary<string, Serilog.ILogger> _serviceLoggers = new();

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/logs")]
        public IActionResult ReceiveLog([FromBody] LogEntry logEntry)
        {

            var log = new LogEntry
            {
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"), // Sử dụng thời gian hiện tại
                Level = logEntry.Level, // Sử dụng cấp độ log từ người dùng
                Message = logEntry.Message // Sử dụng thông điệp từ người dùng
               
            };

            // Gửi log vào Elasticsearch
            ElasticsearchService.LogToElasticsearch(log);

            // Trả về phản hồi thành công
            return Ok(new { Message = "Log đã được ghi vào Elasticsearch" });
            //// Kiểm tra xem log có hợp lệ không
            //if (logEntry == null)
            //{
            //    return BadRequest("Log entry cannot be null");
            //}

            //if (string.IsNullOrEmpty(logEntry.Message) || string.IsNullOrEmpty(logEntry.Service))
            //{
            //    return BadRequest("Log entry data is invalid");
            //}

            //// Đảm bảo log sẽ được ghi vào Elasticsearch
            //Log.Information("Service: {Service}", logEntry.Service);

            //// Nếu chưa có logger cho service này, tạo mới
            //if (!_serviceLoggers.ContainsKey(logEntry.Service))
            //{
            //    var serviceLogger = new LoggerConfiguration()
            //        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://localhost:9200"))
            //        {
            //            IndexFormat = "logs-{0:yyyy.MM.dd}",
            //            AutoRegisterTemplate = true, // Đảm bảo template tự động được đăng ký
            //            ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "Cs_8EaS9wjqJDv9TpR7Z") // (Nếu có xác thực)
            //        })
            //        .Enrich.FromLogContext()
            //        .CreateLogger();

            //    _serviceLoggers[logEntry.Service] = serviceLogger;
            //}

            //// Ghi log vào Elasticsearch
            //_serviceLoggers[logEntry.Service].Information("Log received from {RequestId} {Service}: {Message} at {Timestamp}",
            //    logEntry.RequestId, logEntry.Service, logEntry.Message, logEntry.Timestamp);

            //return Ok();
        }
    }
}
