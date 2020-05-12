namespace CommandsFunction
{
    public interface IGame
    {
        Player GetPlayer(string playerId);
        void AddPlayer(string playerId);
        bool HasPlayer(string playerId);
    }
}