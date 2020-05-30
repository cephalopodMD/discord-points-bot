using System.Text.Json;
using PointsBot.Core;
using PointsBot.Infrastructure.Models;

namespace Function.Events
{
    public class EventFactory
    {
        public PointsEvent Create(JsonDocument document)
        {
            var action = document.RootElement.GetProperty("Action").GetString();
            var source = document.RootElement.GetProperty("Source").GetString();
            var payload = document.RootElement.GetProperty("Payload");

            var pointsPayload = JsonSerializer.Deserialize<PointsCommand>(payload.GetRawText());
            return new PointsEvent
            {
                OriginPlayerId = pointsPayload.OriginPlayerId,
                TargetPlayerId = pointsPayload.TargetPlayerId,
                Action = action.ToLowerInvariant(),
                Amount = pointsPayload.AmountOfPoints
            };
        }
    }
}