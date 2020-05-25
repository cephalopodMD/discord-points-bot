using System.Text.Json.Serialization;

namespace Function.Command.Events
{
    public class GameEvent
    {
        [JsonIgnore]
        public string Root => "points";

        public PointsEvent PointsEvent { get; set; }
    }

    public class PointsEvent
    {
        public string TargetPlayerId { get; set; }

        public string Action { get; set; }

        public int Amount { get; set; }

        public string OriginPlayerId { get; set; }
    }
}