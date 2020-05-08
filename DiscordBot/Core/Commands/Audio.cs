using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands
{
    public class Audio : ModuleBase<SocketCommandContext>
    {
        private static IVoiceChannel channel;

        [Command("join"), Summary("join the current voice channel")]
        public async Task JoinModule()
        {
            channel = (Context.User as IGuildUser).VoiceChannel;
            await channel.ConnectAsync();
        }
    }
}
