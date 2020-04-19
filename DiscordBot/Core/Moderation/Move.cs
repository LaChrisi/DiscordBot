using System;
using System.Collections.Generic;
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
            if (Context.User.Id != Core.Data.UserIDs.LaChrisi)
            {
                await Context.Channel.SendMessageAsync($":x: You are not my god!");
                return;
            }

            if (messageID == 0 || channelID == 0)
            {
                await Context.Channel.SendMessageAsync($"move **<MessageID>** **<ChatID>**");
                return;
            }

            try
            {
                var message = await Context.Channel.GetMessageAsync(messageID) as IUserMessage;
                var channel = Context.Client.GetChannel(channelID) as ISocketMessageChannel;

                var x = await channel.SendMessageAsync($"meme from: {message.Author.Mention}\n{message.Content.ToString()}");
                await x.AddReactionAsync(new Emoji("👍"));
                await x.AddReactionAsync(new Emoji("👎"));

                await Context.Channel.DeleteMessageAsync(messageID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}