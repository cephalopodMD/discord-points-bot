using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PointsBot.Core;

namespace PointsBot.CLI
{
    public class Program
    {
        public static IConfiguration _configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("./local.appsettings.json", true, true)
            .Build();

        private static async Task Main(string[] args)
        {
            switch (args[0])
            {
                case "add":
                {
                    var sender = new CommandSender(_configuration["CommandServiceBusConnectionString"]);
                    await sender.AddPoints(args[1], Int32.Parse(args[2]));
                }
                break;
                case "remove":
                {
                    var sender = new CommandSender(_configuration["CommandServiceBusConnectionString"]);
                    await sender.RemovePoints(args[1], Int32.Parse(args[2]));
                }
                break;
                case "init":
                {
                    var sender = new CommandSender(_configuration["CommandServiceBusConnectionString"]);
                    await sender.AddPlayer(args[1]);
                }
                break;
                default: break;
            }
        }
    }
}
