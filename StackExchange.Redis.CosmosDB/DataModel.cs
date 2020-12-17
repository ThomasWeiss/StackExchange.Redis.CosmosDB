using Newtonsoft.Json;

namespace StackExchange.Redis.CosmosDB
{
    class DataModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("val")]
        public object Value { get; set; }
        [JsonProperty("ttl")]
        public double? TimeToLive { get; set; }
    }
}
