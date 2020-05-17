using System.Text.Json.Serialization;

namespace CommandsFunction.Events
{
    public class PointsEvent
    {
        [JsonIgnore]
        public string Root => "points";

        public string PlayerId { get; }

        public PointsEventParameters EventParameters { get; }

        public PointsEvent(string playerId, PointsEventParameters eventParameters)
        {
            PlayerId = playerId;
            EventParameters = eventParameters;
        }
    }

    public class PointsEventParameters
    {
        public string Action { get; set; }

        public int Amount { get; set; }
    }
}