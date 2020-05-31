using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using PointsBot.Infrastructure.Commands;

namespace PointsBot.CLI
{
    public class Program
    {
        public static IConfiguration Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddJsonFile("./local.appsettings.json", true, true)
            .Build();

        private static Task Main(string[] args)
        {
            var sender = new CommandSender(new QueueClient(new ServiceBusConnectionStringBuilder(Configuration["CommandServiceBusConnectionString"])));

            switch (args[0])
            {
                case "add":
                    return sender.SendAdd(args[1], args[2], Int32.Parse(args[3]), "CLI");
                case "remove":
                    return sender.SendRemove(args[1], args[2], Int32.Parse(args[3]), "CLI");
                default: return Task.CompletedTask;
            }
        }
    }
}
