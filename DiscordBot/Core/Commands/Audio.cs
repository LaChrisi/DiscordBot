using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands
{
    public class Audio : ModuleBase<SocketCommandContext>
    {
        private static IVoiceChannel channel;

        [Command("join"), Summary("join the current voice channel")]
        public async Task JoinModule()
        {
            try
            {
                Log.Information($"command - join - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                channel = (Context.User as IGuildUser).VoiceChannel;
                await channel.ConnectAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"command - join - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}
