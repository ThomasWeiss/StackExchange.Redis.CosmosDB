using System.Threading.Tasks;

namespace StackExchange.Redis.CosmosDB.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var redis = ConnectionMultiplexer.Connect("localhost");

            // Use CosmosConnectionMultiplexer.Connect with the connection string of your Cosmos DB account.
            var redis = CosmosConnectionMultiplexer.Connect("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==;");

            // This method initializes the underlying Cosmos DB container if it doesn't exist.            
            await redis.CreateDatabaseIfNotExistsAsync();
            
            // This method updates the underlying Cosmos DB container and should be called whenever you update to the latest version of the library.
            await redis.UpdateDatabaseAsync();

            var db = redis.GetDatabase();
            await db.StringSetAsync("id", "val");
            var value1 = await db.StringGetAsync("id");
            var value2 = await db.StringIncrementAsync("unknown");
            var value3 = await db.StringIncrementAsync("num", 5.2);
            var value4 = await db.StringGetSetAsync("id", "new value");
            var values = await db.StringGetAsync(new RedisKey[] { "id", "num", "test" });
        }
    }
}
