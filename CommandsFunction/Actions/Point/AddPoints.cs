using System.Text.Json;

namespace CommandsFunction.Actions.Point
{
    public class AddPoints : Action<Points>
    {
        private readonly int _maxPointsPerAddAndSubtract;

        public AddPoints(string payloadElement, int maxPointsPerAddAndSubtract) : base(payloadElement)
        {
            _maxPointsPerAddAndSubtract = maxPointsPerAddAndSubtract;
        }

        public override void Execute(IGame game)
        {
            var player = game.GetPlayer(_payload.PlayerId);
            if (player == null) return;

            var amountToAdd = _payload.Amount > _maxPointsPerAddAndSubtract ? _maxPointsPerAddAndSubtract : _payload.Amount;
            player.TotalPoints += amountToAdd;
        }
    }
}