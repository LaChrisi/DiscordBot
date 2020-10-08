using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Message : ModuleBase<SocketCommandContext>
    {
        [Command("say"), SummaryAttribute("writes as bot")]
        public async Task SayModule(ulong channelID = 0, String what = "")
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You are not my god!"), Classes.Colors.warning));
                    Classes.Log.Warning($"command - say - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Classes.Log.Information($"command - say - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (channelID == 0 || what == "")
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("try", "!say **<ChannelID>** **\"<What to Say>\"**"), Classes.Colors.error, "error"));
                    return;
                }

                var channel = Context.Client.GetChannel(channelID) as IMessageChannel;
                await channel.SendMessageAsync(what);
            }
            catch (Exception ex)
            {
                Classes.Log.Error($"command - say - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("delete"), SummaryAttribute("deletes messages")]
        public async Task DeleteModule(ulong channelID = 0, ulong messageID = 0)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You are not my god!"), Classes.Colors.warning));
                    Classes.Log.Warning($"command - delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Classes.Log.Information($"command - delete - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (channelID == 0 || messageID == 0)
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("try", "!delete **<ChannelID>** **<MessageID>**"), Classes.Colors.error, "error"));
                    return;
                }

                var channel = Context.Client.GetChannel(channelID) as IMessageChannel;
                await channel.DeleteMessageAsync(messageID);
            }
            catch (Exception ex)
            {
                Classes.Log.Error($"command - delete - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}
