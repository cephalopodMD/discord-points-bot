namespace PointsBot.Core
{
    internal static class MessageTemplates
    {
        public static string AddPoints(string player, int amount) => $"{{ \"Action\": \"add\", \"Payload\": {{ \"PlayerId\": \"{player}\", \"Amount\": {amount} }} }}";
        public static string RemovePoints(string player, int amount) => $"{{ \"Action\": \"remove\", \"Payload\": {{ \"PlayerId\": \"{player}\", \"Amount\": {amount} }} }}";
        public static string AddPlayer(string player) => $"{{ \"Action\": \"init\", \"Payload\": {{ \"PlayerId\": \"{player}\" }} }}";
        public static string NukeIt() => $"{{ \"Action\": \"reset\" }}";
    }
}