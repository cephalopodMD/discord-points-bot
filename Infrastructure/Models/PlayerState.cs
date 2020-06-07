namespace PointsBot.Infrastructure.Models
{
    public class PlayerState
    {
        public static PlayerState Unknown = new PlayerState("unknown_to_pbot", 0);
        public string PlayerId { get; }

        public int TotalPoints { get; }

        public PlayerState(string playerId, int totalPoints)
        {
            PlayerId = playerId;
            TotalPoints = totalPoints;
        }
    }
}