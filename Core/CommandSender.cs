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

        public Task AddPoints(string player, int numberOfPoints)
        {
            var sender = new MessageSender(_connection, "commands");
            var messageBody = MessageTemplates.AddPoints(player, numberOfPoints);

            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(messageBody)
            };

            return sender.SendAsync(message);
        }

        public Task RemovePoints(string player, int numberOfPoints)
        {
            var sender = new MessageSender(_connection, "commands");
            var messageBody = MessageTemplates.RemovePoints(player, numberOfPoints);

            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(messageBody)
            };

            return sender.SendAsync(message);
        }

        public Task AddPlayer(string playerId)
        {
            var sender = new MessageSender(_connection, "commands");
            var messageBody = MessageTemplates.AddPlayer(playerId);

            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(messageBody)
            };

            return sender.SendAsync(message);
        }

        public Task ResetPoints()
        {
            var sender = new MessageSender(_connection, "commands");
            var messageBody = MessageTemplates.NukeIt();

            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(messageBody)
            };

            return sender.SendAsync(message);
        }
    }
}