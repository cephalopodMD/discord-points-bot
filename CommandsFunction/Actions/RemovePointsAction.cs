using System;
using System.Text.Json;

namespace CommandsFunction.Actions
{
    public class RemovePointsAction : IAction
    {
        private readonly JsonElement _payloadElement;
        private readonly int _maxPoints;

        private int _amount => _payloadElement.GetProperty("Amount").GetInt32();
        private string _playerId => _payloadElement.GetProperty("PlayerId").GetString();

        public RemovePointsAction(JsonElement payloadElement, int maxPoints)
        {
            _payloadElement = payloadElement;
            _maxPoints = maxPoints;
        }

        public void Execute(IGame game)
        {
            var player = game.GetPlayer(_playerId);
            if (player == null) return;

            var amountToRemove = _amount > _maxPoints ? _maxPoints : _amount;
            player.TotalPoints -= amountToRemove;
        }
    }
}