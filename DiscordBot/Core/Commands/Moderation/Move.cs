using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Move : ModuleBase<SocketCommandContext>
    {
        [Command("move"), Alias("m"), Summary("moves the message")]
        public async Task MoveModule(ulong messageID = 0, ulong channelID = 0)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.admin))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Classes.Colors.warning));
                    Classes.Log.Warning($"command - move - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Classes.Log.Information($"command - move - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Classes.Log.Error($"command - move - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}