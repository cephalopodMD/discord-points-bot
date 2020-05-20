namespace PointsBot.Core
{
    internal static class MessageTemplates
    {
        public static string AddPoints(string originPlayer, string targetPlayer, int amount) => $"{{ \"Action\": \"add\", \"Payload\": {{ \"OriginPlayerId\": \"{originPlayer}\", \"TargetPlayerId\": \"{targetPlayer}\", \"Amount\": {amount} }} }}";
        public static string RemovePoints(string originPlayer, string targetPlayer, int amount) => $"{{ \"Action\": \"remove\", \"Payload\": {{ \"OriginPlayerId\": \"{originPlayer}\", \"TargetPlayerId\": \"{targetPlayer}\", \"Amount\": {amount} }} }}";
    }
}