﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using DiscordBot.Core.Data;
using System.Collections.Generic;
using System.Linq;
using Discord.Rest;
using System.ComponentModel.DataAnnotations;

namespace DiscordBot
{
    public class Program
    {
        private static DiscordSocketClient Client;
        private CommandService Commands;

        private Timer daily_timer;
        private Timer hourly_timer;

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

            Client.JoinedGuild += Client_JoinedGuild;
            Client.GuildUpdated += Client_GuildUpdated;
            Client.LeftGuild += Client_LeftGuild;

            Client.ChannelCreated += Client_ChannelCreated;
            Client.ChannelUpdated += Client_ChannelUpdated;
            Client.ChannelDestroyed += Client_ChannelDestroyed;

            Client.UserJoined += Client_UserJoined;
            Client.UserUpdated += Client_UserUpdated;
            
#if DEBUG
            await Client.LoginAsync(TokenType.Bot, Token.token_test);
#else
            await Client.LoginAsync(TokenType.Bot, Token.token_prod);
#endif

            SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
            SetUpDailyTimer(new TimeSpan(5, 0, 0));

            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private void SetUpHourlyTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;

            hourly_timer = new Timer
            {
                AutoReset = false,
                Interval = timeToGo.TotalMilliseconds
            };
            hourly_timer.Elapsed += Hourly_timer_Elapsed;

            hourly_timer.Start();
        }
        
        //hourly_timer event
        private async void Hourly_timer_Elapsed(object sender, ElapsedEventArgs eArgs)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay + "hourly_timer event");

            var channel_event_list = Channel_Event.GetAllByType('k');

            foreach (var channel_event in channel_event_list)
            {
                var user = User.GetById((ulong)Convert.ToInt64(channel_event.when));
                var e = Event.GetById(channel_event.Event);

                if (e.what == "say")
                {
                    if (e.how == "remind")
                    {
                        var channel = Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                        var reminder = await channel.SendMessageAsync(embed: Core.Data.Embed.New(Client.GetUser(user.id), Field.CreateFieldBuilder("warning", $"Your karma is {user.karma}.\nYou should post some new memes!"), Colors.warning, "friendly reminder"));
                        Message.Add(new Message(user.id,reminder.Id, channel.Id, 'k'));
                    }
                }
            }

            SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
        }

        private void SetUpDailyTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
            {
                timeToGo = new TimeSpan(24, 0, 0) + timeToGo;
            }

            daily_timer = new Timer
            {
                AutoReset = false,
                Interval = timeToGo.TotalMilliseconds
            };
            daily_timer.Elapsed += Daily_timer_Elapsed;

            daily_timer.Start();
        }

        //daily_timer event
        private void Daily_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay + "daily_timer event");

            var user_list = User.GetAllWithKarma();

            foreach (var user in user_list)
            {
                user.karma -= 100;

                if (user.karma < -100)
                    user.karma = -100;

                User.Edit(user);
            }

            SetUpDailyTimer(new TimeSpan(5, 0, 0));
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            if (arg.IsBot)
                return;

            var user = User.GetById(arg.Id);

            if (user == null)
                User.Add(new User(user.id, user.name, 0));
        }

        private async Task Client_UserUpdated(SocketUser user_before, SocketUser user_after)
        {
            if (user_before.IsBot)
                return;

            var user = User.GetById(user_before.Id);

            if (user != null)
            {
                user.name = user_after.ToString();
                User.Edit(user);
            }
            else
            {
                User.Add(new User(user_before.Id, user_after.ToString(), 0));
            }
        }

        private async Task Client_ChannelCreated(SocketChannel arg)
        {
            var channel = arg as IGuildChannel;
            Channel.Add(new Channel(channel.Id, channel.Name, channel.GuildId));
        }

        private async Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            var channel_after = arg2 as IGuildChannel;

            var channel = Channel.GetById(arg1.Id);

            if (channel != null)
            {
                channel.name = channel_after.Name;
                Channel.Edit(channel);
            }
            else
            {
                Channel.Add(new Channel(channel_after.Id, channel_after.Name, channel_after.GuildId));
            }
        }

        private async Task Client_ChannelDestroyed(SocketChannel channel)
        {
            Channel.DeleteById(channel.Id);
        }

        private async Task Client_JoinedGuild(SocketGuild guild)
        {
            Server.Add(new Server(guild.Id, guild.Name));

            foreach (var channel in guild.Channels)
            {
                Channel.Add(new Channel(channel.Id, channel.Name, guild.Id));
            }
        }

        private async Task Client_GuildUpdated(SocketGuild server_before, SocketGuild server_after)
        {
            var server = Server.GetById(server_before.Id);

            if (server != null)
            {
                server.name = server_after.Name;
                Server.Edit(server);
            }
            else
            {
                Server.Add(new Server(server_after.Id, server_after.Name));
            }
        }

        private async Task Client_LeftGuild(SocketGuild guild)
        {
            Channel.DeleteAllByServerId(guild.Id);
            Server.DeleteById(guild.Id);
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("an sich rum...", "", ActivityType.Playing);

            Timer ready_timer = new Timer
            {
                AutoReset = false,
                Interval = 5 * 60 * 1000,
                Enabled = true
            };
            ready_timer.Elapsed += Ready_Timer;
        }
        private async void Ready_Timer(object sender, ElapsedEventArgs e)
        {
            await Client.SetGameAsync("sich um...", "", ActivityType.Watching);
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            if (MessageParam.Source == MessageSource.System || MessageParam.Source == MessageSource.Bot)
                return;

            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            //command handler - log
            {
                int ArgPos = 0;
                if (Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))
                {
                    var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

                    if (!Result.IsSuccess)
                        Console.WriteLine($"{DateTime.Now} at Commands] Somthing went wrong with executing a command.\nText: {Context.Message.Content}\nError: {Result.ErrorReason}");

                    return;
                }
            }

            //vote handler
            {
                var vote_channel_list = Vote_Channel.GetAllByChannelId(Message.Channel.Id);

                if (vote_channel_list != null)
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

                                            if (user.karma != -1)
                                                user.karma += 20;

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

            //channel_event handler
            {
                var channel_event_list = Channel_Event.GetAllByChannelId(Context.Channel.Id);

                if (channel_event_list != null)
                {
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

                                        Copy_Message(Message, channel.Id, true);
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

                                        Copy_Message(Message, channel.Id);
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
                                    if (!Context.Message.Content.Contains(when_item))
                                    {
                                        await Context.Channel.DeleteMessageAsync(Message.Id);
                                    }

                                    break;
                                }
                                //enthalten
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
            }
            
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (Reaction.User.Value.IsBot)
                return;

            var userMessage = await Channel.GetMessageAsync(Message.Id) as IUserMessage;
            var Reactions = userMessage.Reactions;

            var channel_event_list = Channel_Event.GetAllByChannelIdAndType(Channel.Id, 'e');

            if (channel_event_list != null)
            {
                foreach (var channel_event in channel_event_list)
                {
                    if (channel_event.aktiv != 1)
                        continue;

                    var e = Event.GetById(channel_event.Event);


                    foreach (var reaction in Reactions)
                    {
                        string[] what_list = e.what.Split(";");

                        if (what_list.Length == 1)
                        {
                            if (reaction.Key.Name == e.what && reaction.Value.ReactionCount >= Convert.ToInt32(channel_event.when) + 1)
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
                        else if (what_list.Length == 2)
                        {
                            if (reaction.Key.Name == what_list[0] && reaction.Value.ReactionCount == Convert.ToInt32(channel_event.when) + 1)
                            {
                                string[] how = e.how.Split(";");

                                if (how.Length == 1)
                                {
                                    if (what_list[1] == "copy")
                                    {
                                        Copy_Message(userMessage, (ulong)Convert.ToInt64(how[0]));
                                    }
                                    else if (what_list[1] == "move")
                                    {
                                        Copy_Message(userMessage, (ulong)Convert.ToInt64(how[0]), true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Reaction.Emote.Name == "👍")
            {
                User user = null;

                if (userMessage.Embeds.Count == 1 && userMessage.Embeds.FirstOrDefault().Color == Colors.meme)
                {
                    user = User.GetById((ulong)Convert.ToInt64(userMessage.Embeds.FirstOrDefault().Footer.Value.Text));
                }
                else
                {
                    user = User.GetById(userMessage.Author.Id);
                }

                if (user != null)
                {
                    user.upvotes++;

                    if (user.karma != -1)
                        user.karma += 20;

                    User.Edit(user);
                }
            }
            else if (Reaction.Emote.Name == "👎")
            {
                User user = null;

                if (userMessage.Embeds.Count == 1 && userMessage.Embeds.FirstOrDefault().Color == Colors.meme)
                {
                    user = User.GetById((ulong)Convert.ToInt64(userMessage.Embeds.FirstOrDefault().Footer.Value.Text));
                }
                else
                {
                    user = User.GetById(userMessage.Author.Id);
                }

                if (user != null)
                {
                    user.downvotes++;

                    if (user.karma != -1)
                        user.karma -= 20;

                    User.Edit(user);
                }
            }
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (Reaction.User.Value.IsBot)
                return;

            var userMessage = await Channel.GetMessageAsync(Message.Id) as IUserMessage;

            if (Reaction.Emote.Name == "👍")
            {
                User user = null;

                if (userMessage.Embeds.Count == 1 && userMessage.Embeds.FirstOrDefault().Color == Colors.meme)
                {
                    user = User.GetById((ulong)Convert.ToInt64(userMessage.Embeds.FirstOrDefault().Footer.Value.Text));
                }
                else
                {
                    user = User.GetById(userMessage.Author.Id);
                }

                if (user != null)
                {
                    user.upvotes--;

                    if (user.karma != -1)
                        user.karma -= 20;

                    User.Edit(user);
                }
            }
            else if (Reaction.Emote.Name == "👎")
            {
                User user = null;

                if (userMessage.Embeds.Count == 1 && userMessage.Embeds.FirstOrDefault().Color == Colors.meme)
                {
                    user = User.GetById((ulong)Convert.ToInt64(userMessage.Embeds.FirstOrDefault().Footer.Value.Text));
                }
                else
                {
                    user = User.GetById(userMessage.Author.Id);
                }

                if (user != null)
                {
                    user.downvotes--;

                    if (user.karma != -1)
                        user.karma += 20;

                    User.Edit(user);
                }
            }
        }

        public static async void Copy_Message(IUserMessage Message, ulong channelId, bool move = false)
        {
            var channel = Client.GetChannel(channelId) as ISocketMessageChannel;
            var attachments = Message.Attachments;
            var embeds = Message.Embeds;
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            var x = Message;

            try
            {
                if (embeds.Count == 1 && Message.Author == Client.CurrentUser)
                {
                    x = await channel.SendMessageAsync(embed: embeds.FirstOrDefault().ToEmbedBuilder().Build());
                }
                else if (embeds.Count == 1 && !(Message.Author == Client.CurrentUser))
                {
                    if(embeds.FirstOrDefault().Image != null)
                    {
                        if (Message.Content != "")
                            fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                        x = await channel.SendMessageAsync(embed: Core.Data.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"meme from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: embeds.FirstOrDefault().Image.Value.Url, footer: Message.Author.Id.ToString()));
                    }  
                    else if (embeds.FirstOrDefault().Thumbnail != null)
                    {
                        if (Message.Content != "")
                            fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                        x = await channel.SendMessageAsync(embed: Core.Data.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"meme from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: embeds.FirstOrDefault().Thumbnail.Value.Url, footer: Message.Author.Id.ToString()));
                    }

                }
                else
                {
                    if (attachments.Count == 1)
                    {
                        if (Message.Content != "")
                            fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                        x = await channel.SendMessageAsync(embed: Core.Data.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"meme from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: attachments.First().Url, footer: Message.Author.Id.ToString()));
                    }
                    else
                    {
                        if (Message.Content.EndsWith(".jpg") || Message.Content.EndsWith(".jpeg") || Message.Content.EndsWith(".png"))
                        {
                            fields.Add(Field.CreateFieldBuilder("message", Message.Content));
                            x = await channel.SendMessageAsync(embed: Core.Data.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"meme from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: Message.Content, footer: Message.Author.Id.ToString()));
                        }
                        else
                        {
                            fields.Add(Field.CreateFieldBuilder("message", Message.Content));
                            x = await channel.SendMessageAsync(embed: Core.Data.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"meme from [{Message.Channel.Name}]({Message.GetJumpUrl()})", footer: Message.Author.Id.ToString()));
                        }
                    }
                }
            }
            finally
            {
                await x.AddReactionAsync(new Emoji("👍"));
                await x.AddReactionAsync(new Emoji("👎"));
            }

            if (move)
                await Message.DeleteAsync();
        }
    }
}
