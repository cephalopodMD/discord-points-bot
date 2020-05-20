using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace PointsBot.Core
{
    public class CommandSender
    {
        private readonly ServiceBusConnection _connection;
        public CommandSender(string connectionString)
        {
            _connection = new ServiceBusConnection(
                new ServiceBusConnectionStringBuilder(connectionString));
        }

        public Task AddPoints(string originPlayer, string player, int numberOfPoints)
        {
            var sender = new MessageSender(_connection, "commands");
            var messageBody = MessageTemplates.AddPoints(originPlayer, player, numberOfPoints);

            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(messageBody)
            };

            return sender.SendAsync(message);
        }

        public Task RemovePoints(string originPlayer, string player, int numberOfPoints)
        {
            var sender = new MessageSender(_connection, "commands");
            var messageBody = MessageTemplates.RemovePoints(originPlayer, player, numberOfPoints);

            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(messageBody)
            };

            return sender.SendAsync(message);
        }
    }
}