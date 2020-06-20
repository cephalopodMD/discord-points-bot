using System;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Commands;
using Function.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PointsBot.Core;
using PointsBot.Infrastructure;

namespace Function
{
    public class Startup : FunctionsStartup
    {
        private const string CosmosDatabaseName = "points_bot";

        private static readonly CosmosClientOptions CosmosClientOptions = new CosmosClientOptions
        {
            ApplicationName = "PBot",
            ConsistencyLevel = ConsistencyLevel.ConsistentPrefix
        };

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(
                provider => new Func<JsonDocument, PointsEvent>(
                    element => new EventFactory().Create(element)
                )
            );
            var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            builder.Services.AddSingleton<Func<int>>(() => Int32.Parse(configuration["MaxPointsPerAddOrSubtract"]));
            builder.Services.AddSingleton<Func<string, Task<Container>>>(async containerName =>
            {
                var client = new CosmosClient(configuration["CosmosConnectionString"], CosmosClientOptions);

                var databaseResponse = await client.CreateDatabaseAsync(CosmosDatabaseName);
                var containerResponse = await databaseResponse.Database.CreateContainerAsync(new ContainerProperties(),
                    ThroughputProperties.CreateAutoscaleThroughput(400));

                return containerResponse.Container;
            });

            builder.Services.AddSingleton<IEventWriter<PointsEvent>, CosmosEventWriter>();
            builder.Services.AddSingleton<IGameTimer, CosmosTimer>();
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<PlayerCache>();

            builder.Services.AddOptions<IOptions<TimeoutOptions>>()
                .Bind(configuration.GetSection("Timeout"));
            builder.Services.AddOptions<IOptions<EventFeedOptions>>()
                .Bind(configuration.GetSection("EventFeed"));

            builder.Services.AddSingleton(
                CloudStorageAccount.Parse(configuration["PointsReadModelConnectionString"]));

            builder.Services.AddSingleton<Func<string, CloudTable>>(serviceProvider =>
            {
                var cloudStorageAccount = serviceProvider.GetRequiredService<CloudStorageAccount>();
                var tableClient = cloudStorageAccount.CreateCloudTableClient();

                return tableName => tableClient.GetTableReference(tableName);
            });
        }
    }
}
