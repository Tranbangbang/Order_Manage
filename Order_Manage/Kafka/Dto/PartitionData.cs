using Newtonsoft.Json;

namespace Order_Manage.Kafka.Dto
{
    public class PartitionData
    {
        [JsonProperty("partition")]
        public int Partition { get; set; }

        [JsonProperty("consumer_lag")]
        public int ConsumerLag { get; set; }
    }
}
