using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Core.Moderation
{
    public class Move : ModuleBase<SocketCommandContext>
    {
        [Command("move"), Alias("m"), Summary("moves the message")]
        public async Task MoveModule(ulong messageID = 0, ulong channelID = 0)
        {
            if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.admin))
            {
                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Data.Colors.warning));
                return;
            }

            if (messageID == 0 || channelID == 0)
            {
                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("try", "!move **<MessageID>** **<ChannelID>**"), Data.Colors.error, "error"));
                return;
            }

            try
            {
                var message = await Context.Channel.GetMessageAsync(messageID) as IUserMessage;

                Program.Copy_Message(message, channelID, true);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("error", e.Message), Data.Colors.error));
            }
        }
    }
}