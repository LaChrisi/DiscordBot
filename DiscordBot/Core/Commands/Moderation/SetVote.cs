using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Classes;

namespace DiscordBot.Core.Commands.Moderation
{
    public class SetVote : ModuleBase<SocketCommandContext>
    {
        //funktioniert nicht! - alt

        [Command("setvote"), SummaryAttribute("UP/DOWN Vote Reaction for x Messages in the channel")]
        public async Task SetVoteModule(int numberUP = 0, ulong channelID = 0, ulong messageID = 0)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.admin))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Classes.Colors.warning));
                    Log.Warning($"command - setvote - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - setvote - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (numberUP <= 0)
                    numberUP = 1;

                var pruneMessage = await Context.Channel.SendMessageAsync("in progress, give me a moment...");

                try
                {
                    //ohne serverID und channelID
                    if (channelID == 0 && messageID == 0)
                    {
                        var messages = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, numberUP).FlattenAsync();

                        foreach (var message in messages)
                        {
                            var x = (IUserMessage)await Context.Channel.GetMessageAsync(message.Id);

                            if (!(x.Content.ToString() == "" || x.Content.Contains("https://") || x.Content.Contains("http://")))
                            {
                                continue;
                            }

                            await x.AddReactionAsync(new Emoji("👍"));
                            await x.AddReactionAsync(new Emoji("👎"));
                        }
                    }
                    //mit serverID und channelID
                    else if (channelID != 0 && messageID != 0)
                    {
                        var channel = Context.Client.GetChannel(channelID) as IMessageChannel;
                        var messages = await channel.GetMessagesAsync(await channel.GetMessageAsync(messageID), Direction.Before, numberUP).FlattenAsync();

                        foreach (var message in messages)
                        {
                            var x = (IUserMessage)await channel.GetMessageAsync(message.Id);

                            if (!(x.Content.ToString() == "" || x.Content.Contains("https://") || x.Content.Contains("http://")))
                            {
                                continue;
                            }

                            await x.AddReactionAsync(new Emoji("👍"));
                            await x.AddReactionAsync(new Emoji("👎"));
                        }
                    }
                    //falsche Argumente
                    else
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("try", "!setvote **<Number>** **[<ChannelID>** **<MessageID>]**"), Classes.Colors.error, "error"));
                    }
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
            catch (Exception ex)
            {
                Log.Error($"command - setvote - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("setvoteall"), SummaryAttribute("UP/DOWN Vote Button for all Messages in the channel")]
        public async Task SetVoteAllModule(ulong ChannelID = 0, ulong MessageID = 0)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.admin))
                {
                    await Context.Channel.SendMessageAsync(":x: You need to be at least admin to use this command!");
                    Log.Warning($"command - setvoteall - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - servoteall - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                //ohne Argumente
                if (ChannelID == 0 && MessageID == 0)
                {
                    var pruneMessage = await Context.Channel.SendMessageAsync("in progress, give me a moment...");

                    try
                    {
                        var startMessage = (IUserMessage)Context.Message;

                        var oldMessages = await Context.Channel.GetMessagesAsync(pruneMessage, Direction.Before, 1).FlattenAsync();

                        while (true)
                        {
                            var messages = await Context.Channel.GetMessagesAsync(startMessage, Direction.Before, 1).FlattenAsync();

                            if (!messages.Equals(oldMessages))
                            {
                                oldMessages = messages;
                            }
                            else
                                break;

                            foreach (var message in messages)
                            {
                                var x = (IUserMessage)await Context.Channel.GetMessageAsync(message.Id);

                                startMessage = x;

                                if (!(x.Attachments.Count > 0 || x.Content.StartsWith("https://") || x.Content.StartsWith("http://")))
                                {
                                    continue;
                                }

                                await x.AddReactionAsync(new Emoji("👍"));
                                await x.AddReactionAsync(new Emoji("👎"));
                            }
                        }
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
                //mit fehlenden Argumenten
                else if (ChannelID != 0 && MessageID == 0 || ChannelID == 0 && MessageID != 0)
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("try", "!setvoteall **[<ServerID>** **<MessageID>]**"), Classes.Colors.error, "error"));
                    return;
                }
                //mit Argumenten
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

                                startMessage = x;

                                if (!(x.Attachments.Count > 0 || x.Content.Contains("https://") || x.Content.Contains("http://")))
                                {
                                    continue;
                                }

                                await x.AddReactionAsync(new Emoji("👍"));
                                await x.AddReactionAsync(new Emoji("👎"));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error:" + e.ToString());
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
                Log.Error($"command - setvoteall - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}