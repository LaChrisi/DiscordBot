using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;

namespace DiscordBot.Core.Commands
{
    public class Set : ModuleBase<SocketCommandContext>
    {
        [Group("set"), Summary("Group to manage set commands")]
        public class GetGroup : ModuleBase<SocketCommandContext>
        {
            [Command("broadcast"), Alias("b"), Summary("sets current channel for broadcast")]
            public async Task SetBroadcastModule(int broadcast = 0)
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                    return;
                }

                var channel = Channel.GetById(Context.Channel.Id);
                channel.broadcast = broadcast;
                Channel.Edit(channel);
            }
        }
    }
}