namespace PointsBot.Core
{
    internal class ObjectPayload
    {
        public string OriginPlayerId { get; set; }

        public string TargetPlayerId { get; set; }

        public int AmountOfPoints { get; set; }
    }
}