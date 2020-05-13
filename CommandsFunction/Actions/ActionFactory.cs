using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using CommandsFunction.Actions.Management;
using CommandsFunction.Actions.Point;
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
                    return new AddPoints(payload.GetRawText(), Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"]));
                case "remove":
                    return new RemovePoints(payload.GetRawText(), Int32.Parse(_configuration["MaxPointsPerAddOrSubtract"]));
                case "init":
                    return new AddPlayer(payload.GetRawText());
                case "nuke":
                    return new ResetPoints();
                default:
                    return NoAction;
            }
        }
    }
}