namespace CommandsFunction.Actions.Point
{
    public class RemovePoints : Action<Points>
    {
        private readonly int _maxPointsToAddOrSubtract;

        public RemovePoints(string payloadElement, int maxPointsToAddOrSubtract) : base(payloadElement)
        {
            _maxPointsToAddOrSubtract = maxPointsToAddOrSubtract;
        }

        public void Execute(IGame game)
        {
            var player = game.GetPlayer(_payload.PlayerId);
            if (player == null) return;

            var amountToRemove = _payload.Amount > _maxPointsToAddOrSubtract ? _maxPointsToAddOrSubtract : _payload.Amount;
            player.TotalPoints -= amountToRemove;
        }
    }
}