# StackExchange.Redis.CosmosDB

**This library is a very early proof-of-concept and it is not advised to use it for anything serious for now!**

The goal of this library is to provide an easy way for developers using the StackExchange.Redis library to evaluate the usage of [Azure Cosmos DB](https://azure.microsoft.com/services/cosmos-db/) as a replacement of Redis.

## Usage

- Simply use `CosmosConnectionMultiplexer.Connect` instead of `ConnectionMultiplexer.Connect` and use the connection string of your Azure Cosmos DB account as the parameter.
- Call `CreateDatabaseIfNotExistsAsync()` to initialize the underlying Cosmos DB container

## Supported Redis commands

For now, a very limited subset of `string` commands are supported. See [CosmosDatabase.cs](./StackExchange.Redis.CosmosDB/CosmosDatabase.cs) for the commands that don't throw a `NotImplementedException`. 😊

More commands will be added with time, and a better way to track command coverage will be provided.