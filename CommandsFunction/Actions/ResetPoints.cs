using System;
using System.Linq;

namespace CommandsFunction.Actions
{
    public class ResetPoints : Action<NoPayload>
    {
        public ResetPoints() : base(String.Empty) { }

        public override void Execute(IGame game)
        {
            var allPlayers = game.GetPlayers().ToList();
            foreach (var player in allPlayers) player.TotalPoints = 0;
        }
    }
}