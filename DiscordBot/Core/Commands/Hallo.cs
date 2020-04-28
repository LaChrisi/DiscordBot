using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Commands
{
    public class Hello : ModuleBase<SocketCommandContext>
    {
        [Command("hallo"), SummaryAttribute("Hallo Welt")]
        public async Task HalloModule()
        {
            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Classes.Field.CreateFieldBuilder("hallo", $"Hallo {Context.User.Username}!"), Classes.Colors.information));
        }
    }
}