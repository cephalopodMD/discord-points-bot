using System.Text.Json;
using PointsBot.Infrastructure.Models;

namespace PointsBot.Infrastructure.Commands
{
    public class RemoveCommand : ICommand
    {
        private readonly RemovePointsMessage _message;

        public RemoveCommand(string originPlayer, string targetPlayerId, int amountOfPoints, string source)
        {
            _message = new RemovePointsMessage(originPlayer, targetPlayerId, amountOfPoints, source);
        }

        public string Serialize() => JsonSerializer.Serialize(_message);

        // Not sure how to make this private and still work with System.Text.Json;
        public class RemovePointsMessage
        {
            public RemovePointsMessage(string originPlayerId, string targetPlayerId, int amountOfPoints, string source)
            {
                Source = source;
                Payload = new PointsCommand
                {
                    OriginPlayerId = originPlayerId,
                    TargetPlayerId = targetPlayerId,
                    AmountOfPoints = amountOfPoints
                };
            }

            public string Action { get; } = "remove";

            public string Source { get; }

            public PointsCommand Payload { get; set; }
        }
    }
}