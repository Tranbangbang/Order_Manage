using System.Text;
using Newtonsoft.Json;
using Order_Manage.Kafka.Dto;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Order_Manage.Common
{
    public class HttpLogSink : ILogEventSink
    {
        private readonly HttpClient _httpClient;
        private readonly string _logServiceUrl;

        public HttpLogSink(string logServiceUrl)
        {
            _httpClient = new HttpClient();
            _logServiceUrl = logServiceUrl;
        }

        public void Emit(LogEvent logEvent)
        {
            var logEntry = new LogEntry
            {
                Timestamp = logEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                Service = "Order_Manage",
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage(),
                RequestId = logEvent.Properties.ContainsKey("RequestId") ? logEvent.Properties["RequestId"].ToString() : null,
                StackTrace = logEvent.Exception?.ToString()
            };

            var content = new StringContent(JsonConvert.SerializeObject(logEntry), Encoding.UTF8, "application/json");

            // Gửi log tới LoggingService
            var response = _httpClient.PostAsync(_logServiceUrl, content).Result;

            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Failed to send log to Logging Service. StatusCode: {StatusCode}, Reason: {Reason}", response.StatusCode, response.ReasonPhrase);
            }
        }


    }
}
