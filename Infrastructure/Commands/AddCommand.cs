using System.Text.Json;
using PointsBot.Infrastructure.Models;

namespace PointsBot.Infrastructure.Commands
{
    public class AddCommand : ICommand
    {
        private readonly AddPointsMessage _message;

        public AddCommand(string originPlayer, string targetPlayerId, int amountOfPoints, string source)
        {
            _message = new AddPointsMessage(originPlayer, targetPlayerId, amountOfPoints, source);
        }

        public string Serialize() => JsonSerializer.Serialize(_message);

        // Not sure how to make this private and still work with System.Text.Json;
        public class AddPointsMessage
        {
            public AddPointsMessage(string originPlayerId, string targetPlayerId, int amountOfPoints, string source)
            {
                Source = source;
                Payload = new PointsCommand
                {
                    OriginPlayerId = originPlayerId,
                    TargetPlayerId = targetPlayerId,
                    AmountOfPoints = amountOfPoints
                };
            }

            public string Action { get; } = "add";

            public string Source { get; }

            public PointsCommand Payload { get; }
        }
    }
}