using System;
using System.Text.Json;
using Function.Command.Events;
using Function.Query;
using Function.Query.Redis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            builder.Services.AddSingleton<IEventFeed<PointsEvent>, RedisPointsEventStorage>();
            builder.Services.AddSingleton<IEventWriter<PointsEvent>, RedisPointsEventStorage>();
            builder.Services.AddSingleton<GameState>();
        }
    }
}
