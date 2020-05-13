using System.Text.Json;

namespace CommandsFunction.Actions
{
    public abstract class Action<T> : IAction
    {
        protected T _payload { get; }

        protected Action(string payload)
        {
            _payload = JsonSerializer.Deserialize<T>(payload);
        }

        public virtual void Execute(IGame game) { }
    }
}