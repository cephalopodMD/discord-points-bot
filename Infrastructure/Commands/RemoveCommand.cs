using System.Text.Json;
using PointsBot.Infrastructure.Models;

namespace PointsBot.Infrastructure.Commands
{
    internal class RemoveCommand
    {
        public  string Action { get; } = "remove";

        public string Source { get; }

        public PointsCommand Payload { get; set; }

        public RemoveCommand(string originPlayerId, string targetPlayerId, int amountOfPoints, string source)
        {
            Source = source;
            Payload = new PointsCommand
            {
                OriginPlayerId = originPlayerId,
                TargetPlayerId = targetPlayerId,
                AmountOfPoints = amountOfPoints
            };
        }

        public string Serialize() => JsonSerializer.Serialize(this);
    }
}