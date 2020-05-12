using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CommandsFunction.Actions
{
    public class ActionFactory
    {
        private readonly IConfiguration _configuration;

        public ActionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static readonly IAction NoAction = new NoAction();

        public IAction Create(JsonDocument document)
        {
            var action = document.RootElement.GetProperty("Action").GetString();
            var payload = document.RootElement.GetProperty("Payload");

            switch (action.ToLowerInvariant())
            {
                case "add":
                    return new AddPointsAction(payload, Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"]));
                case "remove":
                    return new RemovePointsAction(payload, Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"]));
                case "init":
                    return new AddPlayerAction(payload);
                default:
                    return NoAction;
            }
        }
    }
}