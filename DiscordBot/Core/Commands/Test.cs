using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Classes;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace DiscordBot.Core.Commands
{
    public class Test : ModuleBase<SocketCommandContext>
    {
        [Command("hallo"), SummaryAttribute("Hallo Welt")]
        public async Task HalloModule()
        {
            try
            {
                Log.Information($"command - hallo - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Classes.Field.CreateFieldBuilder("hallo", $"Hallo {Context.User.Username}!"), Classes.Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - hallo - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("test"), SummaryAttribute("test command")]
        public async Task TestModule(ulong id = 0)
        {
            try
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - test - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - test - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                //start test here

                var channel_event_list = Channel_Event.GetAllByType('s');

                foreach (var channel_event in channel_event_list)
                {
                    if (channel_event.aktiv == 1)
                    {
                        var channel = Program.Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                        var e = Event.GetById(channel_event.Event);

                        if (e.what == "say")
                        {
                            if (e.how == "todays_birthday")
                            {
                                var message_list = Message.GetAllByChannelAndType(channel.Id, 't');

                                if (message_list != null)
                                {
                                    foreach (var message in message_list)
                                    {
                                        await channel.DeleteMessageAsync(message.message);
                                        Message.DeleteById(message.id);
                                    }
                                }

                                var items = Birthday.GetNextBirthday();

                                if (items != null)
                                {
                                    string content = "";

                                    foreach (var item in items)
                                    {
                                        if (item.Start.Date == DateTime.Now.Date.ToString("yyyy-MM-dd"))
                                        {
                                            content = content + item.Summary + "\n";
                                        }
                                    }

                                    if (content != "")
                                    {
                                        content = content + "\nHappy Birthday!";

                                        var message = await channel.SendMessageAsync(embed: Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder(Birthday.DateToSting(items[0].Start.Date), content), Colors.information, "todays birthday"));

                                        Message.Add(new Message(Program.Client.CurrentUser.Id, message.Id, channel.Id, 't'));
                                    }
                                }
                            }
                        }
                    }
                }

                //test end
            }
            catch (Exception ex)
            {
                Log.Error($"command - test - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}