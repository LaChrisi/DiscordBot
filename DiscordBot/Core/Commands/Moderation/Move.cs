using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Move : ModuleBase<SocketCommandContext>
    {
        [Command("move"), Alias("m"), Summary("moves the message")]
        public async Task MoveModule(ulong messageID = 0, ulong channelID = 0)
        {
            if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.admin))
            {
                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Classes.Colors.warning));
                return;
            }

            if (messageID == 0 || channelID == 0)
            {
                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("try", "!move **<MessageID>** **<ChannelID>**"), Classes.Colors.error, "error"));
                return;
            }

            try
            {
                var message = await Context.Channel.GetMessageAsync(messageID) as IUserMessage;

                Program.Copy_Message(message, channelID, true);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("error", e.Message), Classes.Colors.error));
            }
        }
    }
}