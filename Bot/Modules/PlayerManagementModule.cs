using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PointsBot.Core;

namespace Bot.Modules
{
    public class PlayerManagementModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandSender _sender;

        public PlayerManagementModule(CommandSender sender)
        {
            _sender = sender;
        }

        [Command("addPlayer")]
        [RequireContext(ContextType.Guild)]
        public async Task AddPlayer(IGuildUser user)
        {
            await _sender.AddPlayer(user.Username);
            await Context.Channel.SendMessageAsync("Player Added.");
        }
    }
}