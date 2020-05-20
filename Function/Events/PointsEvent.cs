using System.Text.Json.Serialization;

namespace CommandsFunction.Events
{
    public class PointsEvent
    {
        [JsonIgnore]
        public string Root => "points";

        public string OriginPlayerId { get; set; }

        public string TargetPlayerId { get; set; }

        public PointsEventParameters EventParameters { get; set; }
    }

    public class PointsEventParameters
    {
        public string Action { get; set; }

        public int Amount { get; set; }
    }
}