using System.Text.Json;
using System.Windows.Input;
using PointsBot.Infrastructure.Models;

namespace PointsBot.Infrastructure.Commands
{
    internal class AddCommand 
    {
        public string Action { get; } = "add";

        public string Source { get; }

        public PointsCommand Payload { get; }

        public AddCommand(string originPlayerId, string targetPlayerId, int amountOfPoints, string source)
        {
            Source = source;
            Payload = new PointsCommand
            {
                OriginPlayerId = originPlayerId,
                TargetPlayerId = targetPlayerId,
                AmountOfPoints = amountOfPoints
            };
        }
    }
}