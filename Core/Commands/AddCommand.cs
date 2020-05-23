using System.Text.Json;

namespace PointsBot.Core.Commands
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
                Payload = new ObjectPayload
                {
                    OriginPlayerId = originPlayerId,
                    TargetPlayerId = targetPlayerId,
                    AmountOfPoints = amountOfPoints
                };
            }

            public string Action { get; } = "add";

            public ObjectPayload Payload { get; }
        }
    }
}