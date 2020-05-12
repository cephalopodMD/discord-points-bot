using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using CommandsFunction.Actions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(
                provider => new Func<JsonDocument, IAction>(
                    element => new ActionFactory(provider.GetService<IConfiguration>()).Create(element)
                )
            );
        }
    }
}
