using Newtonsoft.Json;

namespace Order_Manage.Kafka.Dto
{
    public class KafkaStatistics
    {
        /// <summary>
        ///     Handle instance name.
        /// </summary>
        [JsonProperty("name")]
        public string? name { get; set; }

        [JsonProperty("topics")]
        public IReadOnlyDictionary<string, TopicData>? topics { get; set; }

    }
}
