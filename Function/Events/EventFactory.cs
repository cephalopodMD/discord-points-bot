using System.Text.Json;
using CommandsFunction.Events;

namespace Function.Events
{
    public class EventFactory
    {
        public PointsEvent Create(JsonDocument document)
        {
            var action = document.RootElement.GetProperty("Action").GetString();
            var payload = document.RootElement.GetProperty("Payload");

            switch (action.ToLowerInvariant())
            {
                case "add":
                case "remove":
                {
                    var pointsPayload = JsonSerializer.Deserialize<PointsCommand>(payload.GetRawText());
                    return new PointsEvent
                    {
                        OriginPlayerId = pointsPayload.OriginPlayerId,
                        TargetPlayerId = pointsPayload.TargetPlayerId,
                        EventParameters = new PointsEventParameters
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