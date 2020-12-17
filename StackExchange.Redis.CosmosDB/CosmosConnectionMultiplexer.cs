using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos.Scripts;
using StackExchange.Redis.Profiling;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace StackExchange.Redis.CosmosDB
{
    public class CosmosConnectionMultiplexer : IConnectionMultiplexer
    {
        public static CosmosConnectionMultiplexer Connect(string connectionString, bool bulkExecution = false)
        {
            return new CosmosConnectionMultiplexer(connectionString, bulkExecution);
        }

        private CosmosConnectionMultiplexer(string connectionString, bool bulkExecution)
        {
            _cosmosClient = new CosmosClientBuilder(connectionString)
                .WithSerializerOptions(new CosmosSerializationOptions
                {
                    IgnoreNullValues = true,
                    Indented = false,
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.Default
                })
                .WithBulkExecution(bulkExecution)
                .Build();
        }

        private CosmosClient _cosmosClient;

        public async Task CreateDatabaseIfNotExistsAsync(int db = -1, int? throughput = null)
        {
            db = db == -1 ? 0 : db;

            await _cosmosClient.CreateDatabaseIfNotExistsAsync($"d{db}");

            var containerDefinition = _cosmosClient.GetDatabase($"d{db}").DefineContainer("c", "/id")
                .WithIndexingPolicy()
                    .WithExcludedPaths().Path("/*")
                    .Attach()
                .Attach()
                .WithDefaultTimeToLive(-1);
            if (throughput.HasValue)
            {
                await containerDefinition.CreateIfNotExistsAsync(throughput.Value);
            }
            else
            {
                await containerDefinition.CreateIfNotExistsAsync();
            }

            await UpsertStoredProcedure(_cosmosClient.GetContainer($"d{db}", "c"), "runStringCommand", StoredProcedures.runStringCommand);
        }

        public async Task UpdateDatabaseAsync(int db = -1)
        {
            db = db == -1 ? 0 : db;

            await UpsertStoredProcedure(_cosmosClient.GetContainer($"d{db}", "c"), "runStringCommand", StoredProcedures.runStringCommand);
        }

        public string ClientName => throw new NotImplementedException();

        public string Configuration => throw new NotImplementedException();

        public int TimeoutMilliseconds => throw new NotImplementedException();

        public long OperationCount => throw new NotImplementedException();

        public bool PreserveAsyncOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsConnected => throw new NotImplementedException();

        public bool IsConnecting => throw new NotImplementedException();

        public bool IncludeDetailInExceptions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int StormLogThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<RedisErrorEventArgs> ErrorMessage;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed;
        public event EventHandler<InternalErrorEventArgs> InternalError;
        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored;
        public event EventHandler<EndPointEventArgs> ConfigurationChanged;
        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast;
        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved;

        public void Close(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public bool Configure(TextWriter log = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfigureAsync(TextWriter log = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void ExportConfiguration(Stream destination, ExportOptions options = (ExportOptions)(-1))
        {
            throw new NotImplementedException();
        }

        public ServerCounters GetCounters()
        {
            throw new NotImplementedException();
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            db = db == -1 ? 0 : db;

            return new CosmosDatabase(this, _cosmosClient.GetContainer($"d{db}", "c"), db);
        }

        public EndPoint[] GetEndPoints(bool configuredOnly = false)
        {
            throw new NotImplementedException();
        }

        public int GetHashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(string host, int port, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(string hostAndPort, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(IPAddress host, int port)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(EndPoint endpoint, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public string GetStatus()
        {
            throw new NotImplementedException();
        }

        public void GetStatus(TextWriter log)
        {
            throw new NotImplementedException();
        }

        public string GetStormLog()
        {
            throw new NotImplementedException();
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public int HashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public void RegisterProfiler(Func<ProfilingSession> profilingSessionProvider)
        {
            throw new NotImplementedException();
        }

        public void ResetStormLog()
        {
            throw new NotImplementedException();
        }

        public void Wait(Task task)
        {
            throw new NotImplementedException();
        }

        public T Wait<T>(Task<T> task)
        {
            throw new NotImplementedException();
        }

        public void WaitAll(params Task[] tasks)
        {
            throw new NotImplementedException();
        }

        private async Task UpsertStoredProcedure(Container container, string storedProcedureId, string storedProcedureBody)
        {
            var storedProcedureExists = true;
            try
            {
                var storedProcedure = await container.Scripts.ReadStoredProcedureAsync(storedProcedureId);
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    storedProcedureExists = false;
                }
                else
                {
                    throw;
                }
            }

            if (storedProcedureExists)
            {
                await container.Scripts.ReplaceStoredProcedureAsync(new StoredProcedureProperties { Id = storedProcedureId, Body = storedProcedureBody });
            }
            else
            {
                await container.Scripts.CreateStoredProcedureAsync(new StoredProcedureProperties { Id = storedProcedureId, Body = storedProcedureBody });
            }
        }
    }
}
