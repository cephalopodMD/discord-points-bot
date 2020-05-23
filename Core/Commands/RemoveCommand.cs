using System.Text.Json;

namespace PointsBot.Core.Commands
{
    public class RemoveCommand : ICommand
    {
        private readonly RemovePointsMessage _message;

        public RemoveCommand(string originPlayer, string targetPlayerId, int amountOfPoints)
        {
            _message = new RemovePointsMessage(originPlayer, targetPlayerId, amountOfPoints);
        }

        public string Serialize() => JsonSerializer.Serialize(_message);

        // Not sure how to make this private and still work with System.Text.Json;
        public class RemovePointsMessage
        {
            public RemovePointsMessage(string originPlayerId, string targetPlayerId, int amountOfPoints)
            {
                Payload = new ObjectPayload
                {
                    OriginPlayerId = originPlayerId,
                    TargetPlayerId = targetPlayerId,
                    AmountOfPoints = amountOfPoints
                };
            }

            public string Action { get; } = "remove";

            public ObjectPayload Payload { get; set; }
        }
    }
}