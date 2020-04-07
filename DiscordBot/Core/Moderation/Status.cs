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
        [Command("status"), SummaryAttribute("Status zu String")]
        public async Task StatusModule(String what, String newStatus)
        {
            if (!(Context.User.Id == Data.UserIDs.LaChrisi))
            {
                await Context.Channel.SendMessageAsync(":x: You are not my god!");
                return;
            }

            if (what.ToLower().Trim() == "play")
                await Context.Client.SetGameAsync(newStatus, "", ActivityType.Playing);
            else if (what.ToLower().Trim() == "watch")
                await Context.Client.SetGameAsync(newStatus, "", ActivityType.Watching);
            else if (what.ToLower().Trim() == "listen")
                await Context.Client.SetGameAsync(newStatus, "", ActivityType.Listening);
            else
                await Context.Channel.SendMessageAsync(":x: status not found!");
        }
    }
}