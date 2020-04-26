using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Moderation
{
    public class Status : ModuleBase<SocketCommandContext>
    {
        [Command("status"), SummaryAttribute("set status")]
        public async Task StatusModule(String what, String newStatus)
        {
            if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.owner))
            {
                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("warning", "You are not my god!"), Data.Colors.warning));
                return;
            }

            if (what.ToLower().Trim() == "play")
                await Context.Client.SetGameAsync(newStatus, "", ActivityType.Playing);
            else if (what.ToLower().Trim() == "watch")
                await Context.Client.SetGameAsync(newStatus, "", ActivityType.Watching);
            else if (what.ToLower().Trim() == "listen")
                await Context.Client.SetGameAsync(newStatus, "", ActivityType.Listening);
            else
                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("error", "status not found!"), Data.Colors.error));
        }
    }
}