using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBot.Core.Data;

namespace DiscordBot.Core.Moderation
{
    public class Data : ModuleBase<SocketCommandContext>
    {
        [Command("version")]
        public async Task VersionModule()
        {
            await Context.Channel.SendMessageAsync("Version: " + Core.Data.Data.Version());
        }

        [Command("user")]
        public async Task UserModule()
        {
            await Context.Channel.SendMessageAsync("User:\n" + Core.Data.Data.Read());
        }
    }
}
