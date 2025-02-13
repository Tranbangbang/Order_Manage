using Newtonsoft.Json;

namespace Order_Manage.Kafka.Dto
{
    public class TopicData
    {
        [JsonProperty("topic")]
        public string? Topic { get; set; }

        [JsonProperty("partitions")]
        public IReadOnlyDictionary<string, PartitionData>? Partitions { get; set; }

    }
}
