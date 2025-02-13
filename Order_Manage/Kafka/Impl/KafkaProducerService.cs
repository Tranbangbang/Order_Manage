using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Order_Manage.Kafka.Dto;

namespace Order_Manage.Kafka.Impl
{
    public class KafkaProducerService
    {
        private readonly KafkaConfiguration _kafkaConfig;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly IProducer<string, string> _producer;

        public KafkaProducerService(IOptions<KafkaConfiguration> kafkaConfig, ILogger<KafkaProducerService> logger)
        {
            _kafkaConfig = kafkaConfig.Value;
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.Brokers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task SendMessageAsync(object order)
        {
            try
            {
                var message = JsonConvert.SerializeObject(order);
                var result = await _producer.ProduceAsync(_kafkaConfig.Topic, new Message<string, string>
                {
                    Key = _kafkaConfig.Key,
                    Value = message
                });

                _logger.LogInformation($"Kafka Producer: Gửi thành công order - {result.TopicPartitionOffset}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Kafka Producer lỗi: {ex.Message}");
            }
        }
    }
}
