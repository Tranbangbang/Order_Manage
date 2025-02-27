using Logging_Service.Config;
using Nest;

namespace Logging_Service.Service
{
    public class ElasticsearchService
    {
     
        private static IElasticClient _client;

        public static IElasticClient GetElasticClient()
        {
            if (_client == null)
            {
                var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                    .DefaultIndex("logs");

                _client = new ElasticClient(settings);
            }
            return _client;
        }

        public static void LogToElasticsearch(LogEntry log)
        {
            var client = GetElasticClient();

            var response = client.IndexDocument(log);

            if (response.IsValid)
            {
                Console.WriteLine("Log đã được gửi thành công!");
            }
            else
            {
                Console.WriteLine("Có lỗi khi gửi log: " + response.DebugInformation);
            }
        }
    }
}
