using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Order_Manage.Kafka.Dto;
using Order_Manage.Service;

namespace Order_Manage.Kafka.Impl
{
    public class KafkaConsumerService : IHostedService, IDisposable
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly KafkaConfiguration _kafkaConfiguration;
        private IConsumer<string, string> _consumer;
        private CancellationTokenSource _cts;
        private readonly IServiceScopeFactory _serviceScopeFactory; 

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IOptions<KafkaConfiguration> kafkaConfigurationOptions, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _kafkaConfiguration = kafkaConfigurationOptions?.Value ?? throw new ArgumentException(nameof(kafkaConfigurationOptions));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentException(nameof(serviceScopeFactory));

            Init();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service has started.");

            _consumer.Subscribe(new List<string>() { _kafkaConfiguration.Topic });

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task.Run(() => Consume(_cts.Token), _cts.Token);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service is stopping.");
            _cts.Cancel();
            _consumer.Close();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumer?.Dispose();
            _cts?.Dispose();
        }

        private void Init()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfiguration.Brokers,
                GroupId = _kafkaConfiguration.ConsumerGroup,
                SecurityProtocol = SecurityProtocol.Plaintext,
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetStatisticsHandler((_, kafkaStatistics) => LogKafkaStats(kafkaStatistics))
                .SetErrorHandler((_, e) => LogKafkaError(e))
                .Build();
        }

        private void LogKafkaStats(string kafkaStatistics)
        {
            var stats = JsonConvert.DeserializeObject<KafkaStatistics>(kafkaStatistics);
            if (stats?.topics != null && stats.topics.Count > 0)
            {
                foreach (var topic in stats.topics)
                {
                    foreach (var partition in topic.Value.Partitions)
                    {
                        var logMessage = $"KafkaStats Topic: {topic.Key} Partition: {partition.Key} ConsumerLag: {partition.Value.ConsumerLag}";
                        _logger.LogDebug(logMessage);
                    }
                }
            }
        }

        private void LogKafkaError(Error ex)
        {
            var error = $"Kafka Exception: ErrorCode:[{ex.Code}] Reason:[{ex.Reason}] Message:[{ex.ToString()}]";
            _logger.LogError(error);
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult?.Message == null) continue;

                    if (consumeResult.Topic.Equals(_kafkaConfiguration.Topic))
                    {
                        var json = consumeResult.Message.Value;
                        _logger.LogInformation($"Nhận order từ Kafka: {json}");
                        using var scope = _serviceScopeFactory.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                        await orderService.ProcessKafkaOrder(json);
                        _consumer.Commit(consumeResult);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka Consumer bị hủy.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý Kafka message.");
            }
        }
    }
}
