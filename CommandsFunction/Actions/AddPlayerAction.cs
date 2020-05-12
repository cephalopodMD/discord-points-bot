using System.Text.Json;

namespace CommandsFunction.Actions
{
    public class AddPlayerAction : IAction
    {
        private readonly JsonElement _payloadElement;

        private string _newPlayer => _payloadElement.GetProperty("PlayerId").GetString();

        public AddPlayerAction(JsonElement payloadElement)
        {
            _payloadElement = payloadElement;
        }

        public void Execute(IGame game)
        {
            if (game.HasPlayer(_newPlayer)) return;
            game.AddPlayer(_newPlayer);
        }
    }
}