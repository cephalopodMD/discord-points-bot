using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using PointsBot.Core.Commands;
using PointsBot.Core.Models;

namespace Bot.Modules
{
    public class PointsModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandSender _sender;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PointsModule(CommandSender sender, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _sender = sender;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [Command("give")]
        public async Task GiveWithNoAmount(IGuildUser user)
        {
            await Context.Channel.SendMessageAsync($"Must specify an amount to give. Try '@PBot give @{user.Username} 420'");
        }

        [Command("give")]
        [RequireContext(ContextType.Guild)]
        public async Task GivePoints(IGuildUser user, string amountOfPoints)
        {
            await Context.Channel.SendMessageAsync($"Give only accepts numbers");
        }

        [Command("give")]
        [RequireContext(ContextType.Guild)]
        public async Task GivePoints(IGuildUser user, int amountOfPoints)
        {
            await Task.WhenAll(AddPoints(user, amountOfPoints));
        }

        [Command("give")]
        [RequireContext(ContextType.Guild)]
        public async Task GivePoints(IGuildUser user, int amountOfPoints, [Remainder] string theRest)
        {
            await Task.WhenAll(AddPoints(user, amountOfPoints));
        }

        private IEnumerable<Task> AddPoints(IUser user, int amountOfPoints) => new[]
        {
            _sender.SendCommand(new AddCommand(Context.User.Username, user.Username, amountOfPoints)),
            Context.Channel.SendMessageAsync("Transaction complete.")
        };

        [Command("take")]
        public async Task TakeWithNoAmount(IGuildUser user)
        {
            await Context.Channel.SendMessageAsync($"Must specify an amount to take. Try '@PBot give @{user.Username} 69'");
        }

        [Command("give")]
        [RequireContext(ContextType.Guild)]
        public async Task TakePoints(IGuildUser user, string amountOfPoints)
        {
            await Context.Channel.SendMessageAsync($"take only accepts numbers");
        }

        [Command("take")]
        [RequireContext(ContextType.Guild)]
        public async Task TakePoints(IGuildUser user, int amountOfPoints)
        {
            await Task.WhenAll(RemovePoints(user, amountOfPoints));
        }

        [Command("take")]
        [RequireContext(ContextType.Guild)]
        public async Task TakePoints(IGuildUser user, int amountOfPoints, [Remainder] string theRest)
        {
            await Task.WhenAll(RemovePoints(user, amountOfPoints));
        }

        private IEnumerable<Task> RemovePoints(IUser user, int amountOfPoints) => new[]
        {
            _sender.SendCommand(new RemoveCommand(Context.User.Username, user.Username, amountOfPoints)),
            Context.Channel.SendMessageAsync("Transaction complete.")
        };

        [Command("bank")]
        public async Task GetTotalForUser(IGuildUser user)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var playerPointsResult = await httpClient.GetAsync($"{_configuration["QueryBaseEndpoint"]}points/{user.Username}?code={_configuration["QueryKey"]}");
            var playerState =
                JsonSerializer.Deserialize<PlayerState>(await playerPointsResult.Content.ReadAsStringAsync(), JsonOptions);

            await Context.Channel.SendMessageAsync($"{playerState.TotalPoints}");
        }

        [Command("bank")]
        public async Task GetTotalForUser()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var playerPointsResult = await httpClient.GetAsync($"{_configuration["QueryBaseEndpoint"]}points/{Context.User.Username}?code={_configuration["QueryKey"]}");
            var playerState =
                JsonSerializer.Deserialize<PlayerState>(await playerPointsResult.Content.ReadAsStringAsync(), JsonOptions);

            await Context.Channel.SendMessageAsync($"{playerState.TotalPoints}");
        }
    }
}