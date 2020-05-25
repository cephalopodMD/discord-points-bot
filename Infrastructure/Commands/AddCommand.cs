using System.Text.Json;
using PointsBot.Infrastructure.Models;

namespace PointsBot.Infrastructure.Commands
{
    public class AddCommand : ICommand
    {
        private readonly AddPointsMessage _message;

        public AddCommand(string originPlayer, string targetPlayerId, int amountOfPoints)
        {
            _message = new AddPointsMessage(originPlayer, targetPlayerId, amountOfPoints);
        }

        public string Serialize() => JsonSerializer.Serialize(_message);

        // Not sure how to make this private and still work with System.Text.Json;
        public class AddPointsMessage
        {
            public AddPointsMessage(string originPlayerId, string targetPlayerId, int amountOfPoints)
            {
                Payload = new PointsCommand
                {
                    OriginPlayerId = originPlayerId,
                    TargetPlayerId = targetPlayerId,
                    AmountOfPoints = amountOfPoints
                };
            }

            public string Action { get; } = "add";

            public PointsCommand Payload { get; }
        }
    }
}