namespace Function.Command.Events
{
    public class PointsEvent
    {
        public string TargetPlayerId { get; set; }

        public string Action { get; set; }

        public int Amount { get; set; }

        public string OriginPlayerId { get; set; }
    }
}