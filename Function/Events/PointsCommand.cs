namespace Function.Events
{
    public class PointsCommand
    {
        public string OriginPlayerId { get; set; }

        public string TargetPlayerId { get; set; }

        public int AmountOfPoints { get; set; }
    }
}