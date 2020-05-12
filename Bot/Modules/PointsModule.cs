using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PointsBot.Core;

namespace Bot.Modules
{
    public class PointsModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandSender _sender;

        public PointsModule(CommandSender sender)
        {
            _sender = sender;
        }

        [Command("give")]
        public async Task GiveWithNoAmount(IGuildUser user)
        {
            await Context.Channel.SendMessageAsync($"Must specify an amount to give. Try '@PBot give @{user.Username} 420'");
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
            _sender.AddPoints(user.Username, amountOfPoints),
            Context.Channel.SendMessageAsync("Transaction complete.")
        };

        [Command("take")]
        public async Task TakeWithNoAmount(IGuildUser user)
        {
            await Context.Channel.SendMessageAsync($"Must specify an amount to take. Try '@PBot give @{user.Username} 69'");
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
            _sender.RemovePoints(user.Username, amountOfPoints),
            Context.Channel.SendMessageAsync("Transaction complete.")
        };
    }
}