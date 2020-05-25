namespace PointsBot.Core.Models
{
    public class PlayerState
    {
        public string PlayerId { get; }

        public int TotalPoints { get; }

        public PlayerState(string playerId, int totalPoints)
        {
            PlayerId = playerId;
            TotalPoints = totalPoints;
        }
    }
}