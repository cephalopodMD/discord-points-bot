using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using PointsBot.Infrastructure.Commands;
using StackExchange.Redis;

namespace PointsBot.CLI
{
    public class Program
    {
        public static IConfiguration Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("./local.appsettings.json", true, true)
            .Build();

        private static async Task<int> Main(string[] args)
        {
            var command = args[0];
            Console.WriteLine(args[0]);

            if (command == "postbuild")
            {
                PostBuild.Execute(args[1]);
                return 0;
            }

            var sender = new CommandSender(new QueueClient(new ServiceBusConnectionStringBuilder(Configuration["CommandServiceBusConnectionString"])));

            switch (command)
            {
                case "add":
                    await sender.SendAdd(args[1], args[2], Int32.Parse(args[3]), "CLI");
                    break;
                case "remove":
                    await sender.SendRemove(args[1], args[2], Int32.Parse(args[3]), "CLI");
                    break;
                case "dumpredis":
                    await DumpRedis();
                    break;
                default: return 0;
            }

            return 0;
        }

        private static async Task DumpRedis()
        {
            var multiplexer = ConnectionMultiplexer.Connect(Configuration["RedisConnection"]);
            var database = multiplexer.GetDatabase();

            var amountOfPointsCommands = database.ListLength("points");
            var pointsTasks = new List<Task<RedisValue>>();

            for (int ii = 0; ii < amountOfPointsCommands; ii++)
            {
                pointsTasks.Add(database.ListGetByIndexAsync("points", ii));
            }

            var pointsCommandsAsJson = await Task.WhenAll(pointsTasks);
            var pointsCommands = pointsCommandsAsJson.Select(p => JsonSerializer.Deserialize<PointsCommand>(p));

            var dumpFile = File.Create($"C:\\points_bot_dump_{DateTime.Now.ToFileTime()}.json");
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pointsCommands));

            await dumpFile.WriteAsync(data);
        }
    }

    public class PointsCommand
    {
        public string OriginPlayerId { get; set; }

        public string TargetPlayerId { get; set; }

        public EventParameters EventParameters { get; set; }
    }

    public class EventParameters
    {
        public string Action { get; set; }

        public int Amount { get; set; }
    }
}
