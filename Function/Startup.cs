using System;
using System.Text.Json;
using Function.Events;
using Function.Query;
using Function.Redis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PointsBot.Core;
using StackExchange.Redis;

namespace Function
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(
                provider => new Func<JsonDocument, PointsEvent>(
                    element => new EventFactory().Create(element)
                )
            );
            var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            builder.Services.AddSingleton(ConnectionMultiplexer.Connect(configuration["RedisConnection"]));
            builder.Services.AddSingleton<Func<int>>(() => Int32.Parse(configuration["MaxPointsPerAddOrSubtract"]));
            builder.Services.AddSingleton<IEventFeed<PointsEvent>, RedisPointsEventStorage>();
            builder.Services.AddSingleton<IEventWriter<PointsEvent>, RedisPointsEventStorage>();
            builder.Services.AddSingleton<GameState>();
        }
    }
}
