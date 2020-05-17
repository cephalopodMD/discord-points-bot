using System;
using System.Text.Json;

namespace CommandsFunction.Events
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
                    var pointsPayload = JsonSerializer.Deserialize<Points>(payload.GetRawText());

                    return new PointsEvent
                    {
                        PlayerId = pointsPayload.PlayerId,
                        EventParameters = new PointsEventParameters
                        {
                            Action = action.ToLowerInvariant(),
                            Amount = pointsPayload.Amount
                        }
                    };
                }
                case "nuke":
                {
                    return new PointsEvent
                    {
                        EventParameters = new PointsEventParameters {Action = "nuke"}
                    };
                }
                default:
                    return null;
            }
        }
    }
}