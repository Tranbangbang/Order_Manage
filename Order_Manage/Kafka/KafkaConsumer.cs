using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order_Manage.Dto.Response;
using Order_Manage.Service;

namespace Order_Manage.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly string _bootstrapServers = "localhost:9092";
        private readonly string _topic = "order-topic";
        private readonly string _groupId = "order-consumer-group";
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public KafkaConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _bootstrapServers,
                GroupId = _groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topic);
            Console.WriteLine("Kafka Consumer đang chạy...");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    if (result != null)
                    {
                        Console.WriteLine($"Nhận order từ Kafka: {result.Value}");

                        var order = JsonSerializer.Deserialize<OrderCreatedEvent>(result.Value);
                        if (order != null)
                        {
                            using var scope = _serviceScopeFactory.CreateScope();
                            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                            orderService.ProcessKafkaOrderz(order);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Consumer bị hủy.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi xử lý Kafka: {ex.Message}");
                }
            }
            Console.WriteLine("Consumer đã dừng.");
        }
    }
}
