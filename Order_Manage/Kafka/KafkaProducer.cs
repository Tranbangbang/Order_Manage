using System.Text.Json;
using Confluent.Kafka;
using Order_Manage.Dto.Response;

namespace Order_Manage.Kafka
{
    public class KafkaProducer
    {
        private readonly string _bootstrapServers = "localhost:9092";
        private readonly string _topic = "order-topic";

        public async Task SendMessageAsync(OrderCreatedEvent order)
        {
            var config = new ProducerConfig { BootstrapServers = _bootstrapServers };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    var message = JsonSerializer.Serialize(order);
                    Console.WriteLine($"Đang gửi order vào Kafka: {message}");

                    await producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });

                    Console.WriteLine($"Đã gửi order {order.OrderId} vào Kafka");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Kafka Producer: {ex.Message}");
            }
        }

    }
}
