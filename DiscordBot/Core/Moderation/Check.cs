﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Core.Moderation
{
    public class Check : ModuleBase<SocketCommandContext>
    {
        [Command("check"), SummaryAttribute("checkt den Channel und pinnt Nachrichten im Nachhinein.")]
        public async Task CheckModule(ulong ChannelID = 0, ulong MessageID = 0)
        {
            if (!(Context.User.Id == Core.Data.UserIDs.LaChrisi))
            {
                await Context.Channel.SendMessageAsync(":x: You are not my god!");
                return;
            }

            if (ChannelID == 0 || MessageID == 0)
            {
                await Context.Channel.SendMessageAsync(":x: Something went wrong!\ntry: ``sudo!check <ChannelID> <StartMessageID>``");
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
                    Console.WriteLine("Error:" + e.ToString());
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
    }
}