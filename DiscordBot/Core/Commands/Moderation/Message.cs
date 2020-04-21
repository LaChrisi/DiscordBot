using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Moderation
{
    public class Message : ModuleBase<SocketCommandContext>
    {
        [Command("say"), SummaryAttribute("writes as bot")]
        public async Task SayModule(ulong channelID = 0, String what = "")
        {
            if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.owner))
            {
                await Context.Channel.SendMessageAsync(":x: You are not my god!");
                return;
            }

            if (channelID == 0 || what == "")
            {
                await Context.Channel.SendMessageAsync("sudo!say **<ChannelID> <What to Say>**");
                return;
            }

            var channel = Context.Client.GetChannel(channelID) as IMessageChannel;
            await channel.SendMessageAsync(what);
        }

        [Command("delete"), SummaryAttribute("deletes messages")]
        public async Task DeleteModule(ulong channelID = 0, ulong messageID = 0)
        {
            if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.owner))
            {
                await Context.Channel.SendMessageAsync(":x: You are not my god!");
                return;
            }

            if (channelID == 0 || messageID == 0)
            {
                await Context.Channel.SendMessageAsync("sudo!delete **<ChannelID> <MessageID>**");
                return;
            }

            var channel = Context.Client.GetChannel(channelID) as IMessageChannel;
            await channel.DeleteMessageAsync(messageID);
        }
    }
}
