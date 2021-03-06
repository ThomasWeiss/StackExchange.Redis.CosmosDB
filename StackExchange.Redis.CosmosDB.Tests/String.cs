using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace StackExchange.Redis.CosmosDB.Tests
{
    [TestClass]
    public class String
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext testContext)
        {
            //var redis = ConnectionMultiplexer.Connect("localhost");
            var redis = CosmosConnectionMultiplexer.Connect("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==");
            //await redis.CreateDatabaseIfNotExistsAsync();
            _database = redis.GetDatabase();
        }

        [TestMethod]
        public async Task StringSet_sets_value()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            var setResult = await _database.StringSetAsync(key, value);

            Assert.IsTrue(setResult);

            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(value, (string)getResult);
        }

        [TestMethod]
        public async Task StringSet_overwrites_existing_value()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, Guid.NewGuid().ToString());
            await _database.StringSetAsync(key, value);
            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(value, (string)getResult);
        }

        [TestMethod]
        public async Task StringSet_honors_WhenExists()
        {
            var setResult = await _database.StringSetAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), when: When.Exists);

            Assert.IsFalse(setResult);

            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, Guid.NewGuid().ToString());
            setResult = await _database.StringSetAsync(key, value, when: When.Exists);

            Assert.IsTrue(setResult);
        }

        [TestMethod]
        public async Task StringSet_honors_WhenNotExists()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, Guid.NewGuid().ToString());
            var setResult = await _database.StringSetAsync(key, value, when: When.NotExists);

            Assert.IsFalse(setResult);

            setResult = await _database.StringSetAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), when: When.NotExists);

            Assert.IsTrue(setResult);
        }

        [TestMethod]
        public async Task StringIncrement_increments_value()
        {
            var key = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, "1");
            await _database.StringIncrementAsync(key);
            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("2", (string)getResult);

            await _database.StringIncrementAsync(key, 3);
            getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("5", (string)getResult);

            await _database.StringIncrementAsync(key, 4.75);
            getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("9.75", (string)getResult);
        }

        [TestMethod]
        public async Task StringIncrement_initializes_to_zero_if_key_does_not_exist()
        {
            var key = Guid.NewGuid().ToString();
            await _database.StringIncrementAsync(key);
            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("1", (string)getResult);
        }

        [TestMethod]
        public async Task StringIncrement_fails_if_value_is_not_a_number()
        {
            var key = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, "string");

            await Assert.ThrowsExceptionAsync<RedisServerException>(async () => await _database.StringIncrementAsync(key));
        }

        [TestMethod]
        public async Task StringDecrement_decrements_value()
        {
            var key = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, "1");
            await _database.StringDecrementAsync(key);
            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("0", (string)getResult);

            await _database.StringDecrementAsync(key, 3);
            getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("-3", (string)getResult);

            await _database.StringDecrementAsync(key, 4.75);
            getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("-7.75", (string)getResult);
        }

        [TestMethod]
        public async Task StringDecrement_initializes_to_zero_if_key_does_not_exist()
        {
            var key = Guid.NewGuid().ToString();
            await _database.StringDecrementAsync(key);
            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual("-1", (string)getResult);
        }

        [TestMethod]
        public async Task StringDecrement_fails_if_value_is_not_a_number()
        {
            var key = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, "string");

            await Assert.ThrowsExceptionAsync<RedisServerException>(async () => await _database.StringDecrementAsync(key));
        }

        [TestMethod]
        public async Task StringLength_returns_correct_length()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, value);
            var lengthResult = await _database.StringLengthAsync(key);

            Assert.AreEqual(value.Length, lengthResult);
        }

        [TestMethod]
        public async Task StringGetSet_returns_previous_value()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, value);
            var getSetResult = await _database.StringGetSetAsync(key, newValue);

            Assert.AreEqual(value, (string)getSetResult);
            
            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(newValue, (string)getResult);
        }

        [TestMethod]
        public async Task StringGetSet_returns_null_if_key_does_not_exist()
        {
            var key = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();
            var getSetResult = await _database.StringGetSetAsync(key, newValue);

            Assert.AreEqual(null, (string)getSetResult);
        }

        [TestMethod]
        public async Task StringGetRange_returns_string_range()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, value);
            var getRangeResult = await _database.StringGetRangeAsync(key, 10, 20);

            Assert.AreEqual(value.Substring(10, 11), (string)getRangeResult);
        }

        [TestMethod]
        public async Task StringAppend_appends_string()
        {
            var key = Guid.NewGuid().ToString();
            var value1 = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, value1);
            var value2 = Guid.NewGuid().ToString();
            var appendResult = await _database.StringAppendAsync(key, value2);

            Assert.AreEqual(value1.Length + value2.Length, appendResult);

            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(value1 + value2, (string)getResult);
        }

        [TestMethod]
        public async Task StringAppend_creates_key_if_it_does_not_exist()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString();
            var appendResult = await _database.StringAppendAsync(key, value);

            Assert.AreEqual(value.Length, appendResult);

            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(value, (string)getResult);
        }

        [TestMethod]
        public async Task StringSetRange_inserts_string()
        {
            var key = Guid.NewGuid().ToString();
            var value1 = Guid.NewGuid().ToString();
            await _database.StringSetAsync(key, value1);
            var value2 = Guid.NewGuid().ToString().Substring(0, 4);
            var setRangeResult = await _database.StringSetRangeAsync(key, 8, value2);

            Assert.AreEqual(value1.Length, setRangeResult);

            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(value1.Substring(0, 8) + value2 + value1.Substring(12), (string)getResult);
        }

        [TestMethod]
        public async Task StringSetRange_pads_if_string_is_shorter()
        {
            var key = Guid.NewGuid().ToString();
            var value1 = Guid.NewGuid().ToString().Substring(0, 6);
            await _database.StringSetAsync(key, value1);
            var value2 = Guid.NewGuid().ToString().Substring(0, 4);
            var setRangeResult = await _database.StringSetRangeAsync(key, value1.Length + 2, value2);

            Assert.AreEqual(value1.Length + 2 + value2.Length, setRangeResult);

            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(value1 + new string(char.ConvertFromUtf32(0)[0], 2) + value2, (string)getResult);
        }

        [TestMethod]
        public async Task StringSetRange_creates_and_pads_string_if_key_does_not_exist()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToString().Substring(0, 4);
            var setRangeResult = await _database.StringSetRangeAsync(key, 4, value);

            Assert.AreEqual(4 + value.Length, setRangeResult);

            var getResult = await _database.StringGetAsync(key);

            Assert.AreEqual(new string(char.ConvertFromUtf32(0)[0], 4) + value, (string)getResult);
        }

        private static IDatabase _database;
    }
}
