using System.Text.Json;

namespace Function.Events
{
    public class EventFactory
    {
        public GameEvent Create(JsonDocument document)
        {
            var action = document.RootElement.GetProperty("Action").GetString();
            var payload = document.RootElement.GetProperty("Payload");

            switch (action.ToLowerInvariant())
            {
                case "add":
                case "remove":
                {
                    var pointsPayload = JsonSerializer.Deserialize<PointsCommand>(payload.GetRawText());
                    return new GameEvent
                    {
                        OriginPlayerId = pointsPayload.OriginPlayerId,
                        TargetPlayerId = pointsPayload.TargetPlayerId,
                        PointsEvent = new PointsEvent
                        {
                            Action = action.ToLowerInvariant(),
                            Amount = pointsPayload.AmountOfPoints
                        }
                    };
                }
                default:
                    return null;
            }
        }
    }
}