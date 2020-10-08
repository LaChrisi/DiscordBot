using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Check : ModuleBase<SocketCommandContext>
    {
        //alt
        [Command("check"), SummaryAttribute("checkt den Channel und pinnt Nachrichten im Nachhinein.")]
        public async Task CheckModule(ulong ChannelID = 0, ulong MessageID = 0)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.admin))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Classes.Colors.warning));
                    Classes.Log.Warning($"command - check - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Classes.Log.Information($"command - check - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (ChannelID == 0 || MessageID == 0)
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("try", "!check **<ChannelID>** **<StartMessageID>**"), Classes.Colors.error, "error"));
                }
                else
                {
                    var pruneMessage = await Context.Channel.SendMessageAsync("in progress, give me a moment...");

                    try
                    {
                        var Channel = Context.Client.GetChannel(ChannelID) as ISocketMessageChannel;
                        var startMessage = await Channel.GetMessageAsync(MessageID) as IUserMessage;

                        var oldMessages = await Context.Channel.GetMessagesAsync(pruneMessage, Direction.Before, 1).FlattenAsync();

                        while (true)
                        {
                            var messages = await Channel.GetMessagesAsync(startMessage, Direction.Before, 1, CacheMode.AllowDownload).FlattenAsync();

                            if (!messages.Equals(oldMessages))
                                oldMessages = messages;
                            else
                                break;

                            foreach (var message in messages)
                            {
                                var x = (IUserMessage)await Channel.GetMessageAsync(message.Id);

                                var userMessage = x as IUserMessage;
                                var Reactions = userMessage.Reactions;

                                foreach (var y in Reactions)
                                {
                                    if (y.Key.Name == "👍" && y.Value.ReactionCount >= 5)
                                    {
                                        await userMessage.AddReactionAsync(new Emoji("⭐"));
                                        await userMessage.PinAsync();
                                    }
                                    else if (y.Key.Name == "👎" && y.Value.ReactionCount >= 5)
                                    {
                                        await userMessage.AddReactionAsync(new Emoji("💩"));
                                    }
                                }

                                startMessage = x;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("error", e.Message), Classes.Colors.error));
                        throw e;
                    }
                    finally
                    {
                        await pruneMessage.ModifyAsync(x => x.Content = "completed, this message will self destruct in 5 seconds!");
                        await Task.Factory.StartNew(async () =>
                        {
                            await Task.Delay(5000);
                            await pruneMessage.DeleteAsync();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Classes.Log.Error($"command - check - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}