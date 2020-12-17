using Newtonsoft.Json;

namespace StackExchange.Redis.CosmosDB
{
    class SprocResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("result")]
        public object Result { get; set; }
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
