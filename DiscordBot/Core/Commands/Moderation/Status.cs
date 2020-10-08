using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Status : ModuleBase<SocketCommandContext>
    {
        [Command("status"), SummaryAttribute("set status")]
        public async Task StatusModule(String what, String newStatus)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You are not my god!"), Classes.Colors.warning));
                    Classes.Log.Warning($"command - status - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Classes.Log.Information($"command - status - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (what.ToLower().Trim() == "play")
                    await Context.Client.SetGameAsync(newStatus, "", ActivityType.Playing);
                else if (what.ToLower().Trim() == "watch")
                    await Context.Client.SetGameAsync(newStatus, "", ActivityType.Watching);
                else if (what.ToLower().Trim() == "listen")
                    await Context.Client.SetGameAsync(newStatus, "", ActivityType.Listening);
                else
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("error", "status not found!"), Classes.Colors.error));
            }
            catch (Exception ex)
            {
                Classes.Log.Error($"command - status - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}