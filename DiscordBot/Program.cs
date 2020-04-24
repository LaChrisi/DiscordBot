using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using DiscordBot.Core.Data;

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient Client;
        private CommandService Commands;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;
            Client.ReactionAdded += Client_ReactionAdded;
            Client.ReactionRemoved += Client_ReactionRemoved;

            await Client.LoginAsync(TokenType.Bot, Token.token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (Reaction.User.Value.IsBot)
                return;

            if (Reaction.Emote.Name == "👍")
            {
                var mes = await Channel.GetMessageAsync(Message.Id) as IUserMessage;
                User user = User.GetById(mes.Author.Id);

                if (user != null)
                {
                    user.upvotes--;
                    User.Edit(user);
                }
            }
            else if (Reaction.Emote.Name == "👎")
            {
                var mes = await Channel.GetMessageAsync(Message.Id) as IUserMessage;
                User user = User.GetById(mes.Author.Id);

                if (user != null)
                {
                    user.downvotes--;
                    User.Edit(user);
                }
            }
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (Reaction.User.Value.IsBot)
                return;

            var userMessage = await Channel.GetMessageAsync(Message.Id) as IUserMessage;
            var Reactions = userMessage.Reactions;

            var channel_event_list = Channel_Event.GetAllByChannelIdAndType(Channel.Id, 'e');

            foreach (var channel_event in channel_event_list)
            {
                if (channel_event.aktiv != 1)
                    continue;

                var e = Event.GetById(channel_event.Event);

                foreach (var reaction in Reactions)
                {
                    if (reaction.Key.Name == e.what && reaction.Value.ReactionCount >= Convert.ToInt32(channel_event.when) - 1)
                    {
                        string[] how = e.how.Split(";");

                        if (how.Length == 1)
                        {
                            if (how[0] == "pin")
                            {
                                await userMessage.PinAsync();
                            }
                        }
                        else if (how.Length == 2)
                        {
                            if (how[0] == "emote")
                            {
                                await userMessage.AddReactionAsync(new Emoji(how[1]));
                            }
                        }
                    }
                }
            }
            
            if (Reaction.Emote.Name == "👍")
            {
                var mes = await Channel.GetMessageAsync(Message.Id) as IUserMessage;
                User user = User.GetById(mes.Author.Id);
            
                if (user != null)
                {
                    user.upvotes++;
                    User.Edit(user);
                }
            }
            else if (Reaction.Emote.Name == "👎")
            {
                var mes = await Channel.GetMessageAsync(Message.Id) as IUserMessage;
                User user = User.GetById(mes.Author.Id);
            
                if (user != null)
                {
                    user.downvotes++;
                    User.Edit(user);
                }
            }
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        private async Task Client_Log(LogMessage Message)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("an sich rum...", "", ActivityType.Playing);

            

            Timer timer = new Timer
            {
                AutoReset = false,
                Interval = 5 * 60 * 1000,
                Enabled = true
            };
            timer.Elapsed += Timer_Elapsed;
        }
        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Client.SetGameAsync("sich um...", "", ActivityType.Watching);
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            if (MessageParam.Source == MessageSource.System || MessageParam.Source == MessageSource.Bot)
                return;

            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            //Command - Feher in Console loggen
            int ArgPos = 0;
            if (Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))
            {
                var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

                if (!Result.IsSuccess)
                    Console.WriteLine($"{DateTime.Now} at Commands] Somthing went wrong with executing a command.\nText: {Context.Message.Content}\nError: {Result.ErrorReason}");

                return;
            }

            //channel_event handler
            {
                var channel_event_list = Channel_Event.GetAllByChannelId(Context.Channel.Id);

                foreach (var channel_event in channel_event_list)
                {
                    if (channel_event.aktiv != 1)
                        continue;

                    var e = Event.GetById(channel_event.Event);

                    //move
                    if (channel_event.type == 'm')
                    {
                        string[] when = channel_event.when.Split(";");

                        foreach (var when_item in when)
                        {
                            if (Context.Message.Content.Contains(when_item) && when_item != "" || when_item == "" && Context.Message.Attachments.Count > 0)
                            {
                                if (e.what == "move")
                                {
                                    var channel = Client.GetChannel((ulong)Convert.ToInt64(e.how)) as ISocketMessageChannel;
                                    var x = await channel.SendMessageAsync($"meme from: {Message.Author.Mention}\n{Message.Content}");

                                    await Context.Channel.DeleteMessageAsync(Message.Id);

                                    await x.AddReactionAsync(new Emoji("👍"));
                                    await x.AddReactionAsync(new Emoji("👎"));

                                    User user = User.GetById(Message.Author.Id);

                                    if (user != null)
                                    {
                                        user.posts++;
                                        User.Edit(user);
                                    }
                                    else
                                    {
                                        User.Add(new User(Message.Author.Id, Message.Author.Username, 0, 1));
                                    }
                                }

                                break;
                            }
                        }
                    }

                    //copy
                    if (channel_event.type == 'c')
                    {
                        string[] when = channel_event.when.Split(";");

                        foreach (var when_item in when)
                        {
                            if (Context.Message.Content.Contains(when_item) && when_item != "" || when_item == "" && Context.Message.Attachments.Count > 0)
                            {
                                if (e.what == "copy")
                                {
                                    var channel = Client.GetChannel((ulong)Convert.ToInt64(e.how)) as ISocketMessageChannel;
                                    var x = await channel.SendMessageAsync($"meme from: {Message.Author.Mention}\n{Message.Content}");

                                    await x.AddReactionAsync(new Emoji("👍"));
                                    await x.AddReactionAsync(new Emoji("👎"));

                                    User user = User.GetById(Message.Author.Id);

                                    if (user != null)
                                    {
                                        user.posts++;
                                        User.Edit(user);
                                    }
                                    else
                                    {
                                        User.Add(new User(Message.Author.Id, Message.Author.Username, 0, 1));
                                    }
                                }

                                break;
                            }
                        }
                    }

                    //delete
                    if (channel_event.type == 'd')
                    {
                        string[] when = channel_event.when.Split(";");

                        foreach (var when_item in when)
                        {
                            //nicht enthalten
                            if (e.what == "not")
                            {
                                if  (!Context.Message.Content.Contains(when_item))
                                {
                                    await Context.Channel.DeleteMessageAsync(Message.Id);
                                }

                                break;
                            }
                            else if (e.what == "in")
                            {
                                if (Context.Message.Content.Contains(when_item))
                                {
                                    await Context.Channel.DeleteMessageAsync(Message.Id);
                                }

                                break;
                            }
                        }
                    }


                }
            }

            //Vote erstellen
            {
                var vote_channel_list = Vote_Channel.GetAllByChannelId(Message.Channel.Id);

                if (vote_channel_list.Count > 0)
                {
                    foreach (var vote_channel in vote_channel_list)
                    {
                        if (vote_channel.aktiv == 1)
                        {
                            var vote = Vote.GetById(vote_channel.vote);
                            string[] what = vote.what.Split(';');

                            foreach (var what_item in what)
                            {
                                if (Message.Content.Contains(what_item) && what_item != "" || what_item == "" && Context.Message.Attachments.Count > 0)
                                {
                                    string[] how = vote.how.Split(';');

                                    foreach (var how_item in how)
                                    {
                                        await Message.AddReactionAsync(new Emoji(how_item));
                                    }

                                    if (vote.id == 1)
                                    {
                                        User user = User.GetById(Message.Author.Id);

                                        if (user != null)
                                        {
                                            user.posts++;
                                            User.Edit(user);
                                        }
                                        else
                                        {
                                            User.Add(new User(Message.Author.Id, Message.Author.Username, 0, 1));
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
