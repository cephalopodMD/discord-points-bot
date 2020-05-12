using System;
using System.Text.Json;

namespace CommandsFunction.Actions
{
    public class AddPointsAction : IAction
    {
        private readonly JsonElement _payloadElement;
        private readonly int _maxPointsPerAddAndSubtract;

        private int _amount => _payloadElement.GetProperty("Amount").GetInt32();
        private string _playerId => _payloadElement.GetProperty("PlayerId").GetString();

        public AddPointsAction(JsonElement payloadElement, int maxPointsPerAddAndSubtract)
        {
            _payloadElement = payloadElement;
            _maxPointsPerAddAndSubtract = maxPointsPerAddAndSubtract;
        }

        public void Execute(IGame game)
        {
            var player = game.GetPlayer(_playerId);
            if (player == null) return;

            var amountToAdd = _amount > _maxPointsPerAddAndSubtract ? _maxPointsPerAddAndSubtract : _amount;
            player.TotalPoints += amountToAdd;
        }
    }
}