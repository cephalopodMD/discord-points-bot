using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace PointsBot.Core.Commands
{
    public class CommandSender
    {
        private readonly IQueueClient _client;

        public CommandSender(IQueueClient client)
        {
            _client = client;
        }

        public Task SendCommand(ICommand command)
        {
            var message = new Message
            {
                ContentType = "application/json",
                Body = Encoding.UTF8.GetBytes(command.Serialize())
            };

            return _client.SendAsync(message);
        }
    }
}