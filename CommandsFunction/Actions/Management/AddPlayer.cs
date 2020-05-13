namespace CommandsFunction.Actions.Management
{
    public class AddPlayer : Action<NewPlayer>
    {
        public AddPlayer(string payloadElement) : base(payloadElement) { }

        public override void Execute(IGame game)
        {
            if (game.HasPlayer(_payload.PlayerId)) return;
            game.AddPlayer(_payload.PlayerId);
        }
    }
}