using System.Text.Json.Serialization;

namespace Function.Events
{
    public class GameEvent
    {
        [JsonIgnore]
        public string Root => "points";

        public string OriginPlayerId { get; set; }

        public string TargetPlayerId { get; set; }

        public PointsEvent PointsEvent { get; set; }
    }

    public class PointsEvent
    {
        public string Action { get; set; }

        public int Amount { get; set; }
    }
}