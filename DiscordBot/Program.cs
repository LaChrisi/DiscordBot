using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Collections.Generic;
using System.Linq;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using DiscordBot.Core.Classes;
using System.Net;
using System.IO;
using Discord.Audio;
using System.Threading;
using Google.Protobuf;
using Discord.Rest;
using Microsoft.VisualBasic;
using AngleSharp.Dom;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Utilities;

namespace DiscordBot
{
    public class Program
    {
        public static DiscordSocketClient Client;
        private CommandService Commands;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }


        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            if (MessageParam.Source == MessageSource.System || MessageParam.Source == MessageSource.Bot)
                return;

            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            //command handler - console log
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
            try
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
                                if (Message.Content.Contains(what_item) && what_item != "" || what_item == "" && Context.Message.Attachments.Count == 1)
                                {
                                    Log.Information($"system - vote handler - start vote_channel:{vote_channel.id} message:{Context.Message.Id} channel:{Context.Channel.Id}");

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
                                            {
                                                int karma_threshold_to_remind = Convert.ToInt32(Global.GetByName("karma_threshold_to_remind").value);
                                                int karma_per_post = Convert.ToInt32(Global.GetByName("karma_per_post").value);

                                                if (user.karma < karma_threshold_to_remind && user.karma + karma_per_post >= karma_threshold_to_remind)
                                                {
                                                    var message_list = Core.Classes.Message.GetAllByUserAndType(user.id, 'k');

                                                    if (message_list != null)
                                                    {
                                                        foreach (var message in message_list)
                                                        {
                                                            var channel = Client.GetChannel(message.channel) as ISocketMessageChannel;
                                                            await channel.DeleteMessageAsync(message.message);
                                                            Core.Classes.Message.DeleteById(message.id);
                                                        }
                                                    }
                                                }

                                                user.karma += karma_per_post;
                                            }

                                            User.Edit(user);
                                        }
                                        else
                                        {
                                            User.Add(new User(Message.Author.Id, Message.Author.Username, 0, 1));
                                        }
                                    }

                                    break;
                                }
                                /**/
                                else if (Message.Content.Contains(what_item) && what_item != "" || what_item == "" && Context.Message.Attachments.Count > 1)
                                {
                                    Log.Information($"system - vote handler - start vote_channel:{vote_channel.id} message:{Context.Message.Id} channel:{Context.Channel.Id}");

                                    string[] how = vote.how.Split(';');
                                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                                    using var webclient = new WebClient();
                                    {
                                        foreach (var attachment in Context.Message.Attachments)
                                        {
                                            string fileName = Context.Channel.Id + "-" + Context.Message.Id + "--" + attachment.Filename;

                                            webclient.DownloadFile(new Uri(attachment.Url), fileName);

                                            var new_message = await Context.Channel.SendFileAsync(fileName, embed: Core.Classes.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: $"attachment://{fileName}", footer: Message.Author.Id.ToString()));

                                            File.Delete(fileName);

                                            foreach (var how_item in how)
                                            {
                                                await new_message.AddReactionAsync(new Emoji(how_item));
                                            }

                                            if (vote.id == 1)
                                            {
                                                User user = User.GetById(Message.Author.Id);

                                                if (user != null)
                                                {
                                                    user.posts++;

                                                    if (user.karma != -1)
                                                    {
                                                        int karma_threshold_to_remind = Convert.ToInt32(Global.GetByName("karma_threshold_to_remind").value);
                                                        int karma_per_post = Convert.ToInt32(Global.GetByName("karma_per_post").value);

                                                        if (user.karma < karma_threshold_to_remind && user.karma + karma_per_post >= karma_threshold_to_remind)
                                                        {
                                                            var message_list = Core.Classes.Message.GetAllByUserAndType(user.id, 'k');

                                                            if (message_list != null)
                                                            {
                                                                foreach (var message in message_list)
                                                                {
                                                                    var channel = Client.GetChannel(message.channel) as ISocketMessageChannel;
                                                                    await channel.DeleteMessageAsync(message.message);
                                                                    Core.Classes.Message.DeleteById(message.id);
                                                                }
                                                            }
                                                        }

                                                        user.karma += karma_per_post;
                                                    }

                                                    User.Edit(user);
                                                }
                                                else
                                                {
                                                    User.Add(new User(Message.Author.Id, Message.Author.Username, 0, 1));
                                                }
                                            }
                                        }
                                    }
                                    await Context.Message.DeleteAsync();

                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR] " + e.Message);
                Log.Error($"system - vote handler - message:{Context.Message.Id} channel:{Context.Channel.Id} error:{e.Message}");
            }

            //channel_event handler
            try
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
                                    Log.Information($"system - channel_event handler - start channel_event:{channel_event.id} message:{Context.Message.Id} channel:{Context.Channel.Id}");

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
                                    Log.Information($"system - channel_event handler - start channel_event:{channel_event.id} message:{Context.Message.Id} channel:{Context.Channel.Id}");

                                    if (e.what == "copy")
                                    {
                                        if (e.how == "broadcast")
                                        {
                                            Broadcast_Message(Message);
                                        }
                                        else
                                        {
                                            var channel = Client.GetChannel((ulong)Convert.ToInt64(e.how)) as ISocketMessageChannel;

                                            Copy_Message(Message, channel.Id);
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
                                //not in
                                if (e.what == "not")
                                {
                                    if (!Context.Message.Content.Contains(when_item))
                                    {
                                        Log.Information($"system - channel_event handler - start channel_event:{channel_event.id} message:{Context.Message.Id} channel:{Context.Channel.Id}");

                                        await Context.Channel.DeleteMessageAsync(Message.Id);
                                    }

                                    break;
                                }
                                //in
                                else if (e.what == "in")
                                {
                                    if (Context.Message.Content.Contains(when_item))
                                    {
                                        Log.Information($"system - channel_event handler - start channel_event:{channel_event.id} message:{Context.Message.Id} channel:{Context.Channel.Id}");

                                        await Context.Channel.DeleteMessageAsync(Message.Id);
                                    }

                                    break;
                                }
                            }
                        }

                        //attachment
                        if (channel_event.type == 'a')
                        {
                            foreach (var attachment in Context.Message.Attachments)
                            {
                                //EndsWith
                                if (channel_event.when == "EndsWith")
                                {
                                    if (attachment.Filename.EndsWith(e.what))
                                    {
                                        string fileName = Context.Channel.Id + "-" + Context.Message.Id + "--" + attachment.Filename + e.how;

                                        using var webclient = new WebClient();
                                        {
                                            webclient.DownloadFile(new Uri(attachment.Url), fileName);
                                        }

                                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                                        await Context.Channel.SendFileAsync(fileName, embed: Core.Classes.Embed.New((SocketUser)Message.Author, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: $"attachment://{fileName}", footer: Message.Author.Id.ToString()));

                                        await Context.Channel.DeleteMessageAsync(Message.Id);

                                        File.Delete(fileName);
                                    }
                                }
                            }

                            break;
                        }


                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR] " + e.Message);
                Log.Error($"system - channel_event handler - message:{Context.Message.Id} channel:{Context.Channel.Id} error:{e.Message}");
            }

        }
        
        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> Message, Cacheable<IMessageChannel, ulong> Channelarg, SocketReaction Reaction)
        {
            try
            {
                if (Reaction.User.IsSpecified)
                {
                    if (Reaction.User.Value.IsBot)
                        return;
                }

                var Channel = await Client.GetChannelAsync(Channelarg.Id) as ISocketMessageChannel;

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
                        string[] what_list = e.what.Split(";");
                        string[] how = e.how.Split(";");

                        //reaction update event
                        if (Reaction.Emote.Name == e.what)
                        {
                            Log.Information($"system - reaction added channel_event - start channel_event:{channel_event.id} channel:{Channel.Id}");

                            if (how[0] == "present")
                            {
                                var message = Core.Classes.Message.GetByMessageAndChannelAndType(userMessage.Id, Channel.Id, 'u');

                                if (message != null)
                                {
                                    message = Core.Classes.Message.GetById((ulong)Convert.ToInt64(message.reference));
                                    var embed_message = await Channel.GetMessageAsync(message.message) as IUserMessage;

                                    try
                                    {
                                        await embed_message.ModifyAsync(x => { x.Embed = Core.Classes.Embed.UpdatePresent(embed_message, Reaction); });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    var x = await Channel.SendMessageAsync(embed: Core.Classes.Embed.CreatePresent(Reaction));
                                    Core.Classes.Message.Add(new Core.Classes.Message(x.Author.Id, x.Id, Channel.Id, 'u'));
                                    message = Core.Classes.Message.GetByMessageAndChannelAndType(x.Id, Channel.Id, 'u');
                                    Core.Classes.Message.Add(new Core.Classes.Message(userMessage.Author.Id, userMessage.Id, Channel.Id, 'u', message.id));
                                }
                            }
                            else if (how[0] == "present_time")
                            {
                                var message = Core.Classes.Message.GetByMessageAndChannelAndType(userMessage.Id, Channel.Id, 'u');

                                if (message != null)
                                {
                                    message = Core.Classes.Message.GetById((ulong)Convert.ToInt64(message.reference));
                                    var embed_message = await Channel.GetMessageAsync(message.message) as IUserMessage;

                                    try
                                    {
                                        await embed_message.ModifyAsync(x => { x.Embed = Core.Classes.Embed.UpdatePresentTime(embed_message, Reaction); });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        throw ex;
                                    }
                                }
                            }
                            else if (how[0] == "role")
                            {
                                try
                                {
                                    var message = Core.Classes.Message.GetByMessageAndChannelAndType(userMessage.Id, Channel.Id, 'r');
                                    var role = Core.Classes.Role.GetById(message.reference);

                                    ulong userId = Reaction.UserId;
                                    var guildChannel = userMessage.Channel as IGuildChannel;

                                    var guildUser = await guildChannel.GetUserAsync(userId) ?? await Client.Rest.GetGuildUserAsync(guildChannel.Guild.Id, userId);

                                    var guildRole = guildChannel.Guild.Roles.FirstOrDefault(x => x.Name == role.name);

                                    await guildUser.AddRoleAsync(guildRole);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    throw ex;
                                }
                            }
                        }

                        //reaction at count event
                        foreach (var reaction in Reactions)
                        {
                            if (what_list.Length == 1)
                            {
                                if (reaction.Key.Name == Reaction.Emote.Name)
                                {
                                    if (reaction.Key.Name == e.what && reaction.Value.ReactionCount == Convert.ToInt32(channel_event.when) + 1)
                                    {
                                        Log.Information($"system - reaction added channel_event - start channel_event:{channel_event.id} channel:{Channel.Id}");

                                        if (how.Length == 1)
                                        {
                                            if (how[0] == "pin")
                                            {
                                                await userMessage.PinAsync();
                                            }
                                        }
                                        else if (how.Length >= 2)
                                        {
                                            if (how[0] == "emote")
                                            {
                                                string[] emote_list = how.Skip(1).ToArray();

                                                foreach (var emote in emote_list)
                                                {
                                                    await userMessage.AddReactionAsync(new Emoji(emote));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else if (what_list.Length == 2)
                            {
                                if (reaction.Key.Name == Reaction.Emote.Name)
                                {
                                    if (reaction.Key.Name == what_list[0] && reaction.Value.ReactionCount == Convert.ToInt32(channel_event.when) + 1)
                                    {
                                        Log.Information($"system - reaction added channel_event - start channel_event:{channel_event.id} channel:{Channel.Id}");

                                        if (how.Length == 1)
                                        {
                                            if (what_list[1] == "copy")
                                            {
                                                if (how[0] == "broadcast")
                                                {
                                                    Broadcast_Message(userMessage);
                                                }
                                                else
                                                {
                                                    Copy_Message(userMessage, (ulong)Convert.ToInt64(how[0]));
                                                }
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
                        {
                            int karma_threshold_to_remind = Convert.ToInt32(Global.GetByName("karma_threshold_to_remind").value);
                            int karma_per_upvote = Convert.ToInt32(Global.GetByName("karma_per_upvote").value);

                            if (user.karma < karma_threshold_to_remind && user.karma + karma_per_upvote >= karma_threshold_to_remind)
                            {
                                var message_list = Core.Classes.Message.GetAllByUserAndType(user.id, 'k');

                                foreach (var message in message_list)
                                {
                                    var channel = Client.GetChannel(message.channel) as ISocketMessageChannel;
                                    await channel.DeleteMessageAsync(message.message);
                                    Core.Classes.Message.DeleteById(message.id);
                                }
                            }

                            user.karma += karma_per_upvote;
                        }

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
                            user.karma += Convert.ToInt32(Global.GetByName("karma_per_downvote").value);

                        User.Edit(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"system - reaction added - message:{Message.Id} channel:{Channelarg.Id} error:{ex.Message}");
            }
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> Message, Cacheable<IMessageChannel, ulong> Channelarg, SocketReaction Reaction)
        {
            try
            {
                if (Reaction.User.IsSpecified)
                {
                    if (Reaction.User.Value.IsBot)
                        return;
                }

                var Channel = await Client.GetChannelAsync(Channelarg.Id) as ISocketMessageChannel;

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
                        string[] what_list = e.what.Split(";");
                        string[] how = e.how.Split(";");

                        //reaction update event
                        if (Reaction.Emote.Name == e.what)
                        {
                            Log.Information($"system - reaction removed channel_event - start channel_event:{channel_event.id} channel:{Channel.Id}");

                            if (how[0] == "present")
                            {
                                var message = Core.Classes.Message.GetByMessageAndChannelAndType(userMessage.Id, Channel.Id, 'u');

                                if (message != null)
                                {
                                    message = Core.Classes.Message.GetById((ulong)Convert.ToInt64(message.reference));
                                    var embed_message = await Channel.GetMessageAsync(message.message) as IUserMessage;

                                    try
                                    {
                                        await embed_message.ModifyAsync(x => { x.Embed = Core.Classes.Embed.UpdatePresent(embed_message, Reaction, true); });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        throw ex;
                                    }
                                }
                            }
                            else if (how[0] == "present_time")
                            {
                                var message = Core.Classes.Message.GetByMessageAndChannelAndType(userMessage.Id, Channel.Id, 'u');

                                if (message != null)
                                {
                                    message = Core.Classes.Message.GetById((ulong)Convert.ToInt64(message.reference));
                                    var embed_message = await Channel.GetMessageAsync(message.message) as IUserMessage;

                                    try
                                    {
                                        await embed_message.ModifyAsync(x => { x.Embed = Core.Classes.Embed.UpdatePresentTime(embed_message, Reaction, true); });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                        throw ex;
                                    }
                                }
                            }
                            else if (how[0] == "role")
                            {
                                try
                                {
                                    var message = Core.Classes.Message.GetByMessageAndChannelAndType(userMessage.Id, Channel.Id, 'r');
                                    var role = Core.Classes.Role.GetById(message.reference);

                                    ulong userId = Reaction.UserId;
                                    var guildChannel = userMessage.Channel as IGuildChannel;

                                    var guildUser = await guildChannel.GetUserAsync(userId) ?? await Client.Rest.GetGuildUserAsync(guildChannel.Guild.Id, userId);

                                    var guildRole = guildChannel.Guild.Roles.FirstOrDefault(x => x.Name == role.name);

                                    await guildUser.RemoveRoleAsync(guildRole);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    throw ex;
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
                        user.upvotes--;

                        if (user.karma != -1)
                            user.karma -= Convert.ToInt32(Global.GetByName("karma_per_upvote").value);

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
                            user.karma -= Convert.ToInt32(Global.GetByName("karma_per_downvote").value);

                        User.Edit(user);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"system - reaction removed - message:{Message.Id} channel:{Channelarg.Id} error:{ex.Message}");
            }
        }

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                AlwaysDownloadUsers = true,
                GatewayIntents = GatewayIntents.All
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

            Client.InteractionCreated += Client_InteractionCreated;

            Client.UserVoiceStateUpdated += Client_UserVoiceStateUpdated;

#if DEBUG
            await Client.LoginAsync(TokenType.Bot, Token.token_test);
#else
            await Client.LoginAsync(TokenType.Bot, Token.token_prod);
#endif

            Repetitive_Timer.SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
            Repetitive_Timer.SetUpDailyTimer(new TimeSpan(Convert.ToInt32(Global.GetByName("daily_timer_hour").value), 0, 0));
            Repetitive_Timer.SetUp5MinutesTimer(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + 5, 0));

            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_UserVoiceStateUpdated(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
        {
            try
            {
                if (user.IsBot)
                    return;

                var guild = state2.VoiceChannel?.Guild ?? state1.VoiceChannel?.Guild;

                if (guild == null)
                    return;

                //hier mit Nadine

                if (guild.Id == 1201924759353557042)
                {
                    var channel = (ITextChannel) Client.GetChannel(1215366832815607819);

                    if (state1.VoiceChannel == null && state2.VoiceChannel != null)
                    {
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                        var embed = Core.Classes.Embed.New(user, fields, Colors.information, "joined voice");

                        await channel.SendMessageAsync(embed: embed);
                    }
                    else if (state1.VoiceChannel != null && state2.VoiceChannel == null)
                    {
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                        var embed = Core.Classes.Embed.New(user, fields, Colors.information, "left voice");

                        await channel.SendMessageAsync(embed: embed);
                    }
                }


                if (!Core.Commands.Audio.audioClients.ContainsKey(guild.Id))
                    return;

                if (Core.Commands.Audio.audioClients[guild.Id].voiceChannel == state1.VoiceChannel & state2.VoiceChannel != state1.VoiceChannel)
                {
                    if (state1.VoiceChannel.Users.Count == 1)
                    {
                        Core.Commands.Audio.audioClients[guild.Id].Stop();
                        Core.Commands.Audio.audioClients[guild.Id].CleanUp();

                        if (Core.Commands.Audio.audioClients.ContainsKey(guild.Id))
                        {
                            Core.Commands.Audio.audioClients.Remove(guild.Id);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private async Task Client_InteractionCreated(SocketInteraction interaction)
        {
            switch (interaction)
            {
                // Slash commands
                case SocketSlashCommand commandInteraction:
                    await SlashCommands.SlashCommandHandler(commandInteraction);
                    break;

                // Button clicks/selection dropdowns
                case SocketMessageComponent componentInteraction:
                    await ButtonSelectionHandler(componentInteraction);
                    break;

                case SocketModal commandInteraction:
                    await ModalSelectionHandler(commandInteraction);
                    break;
                // Unused or Unknown/Unsupported
                default:
                    break;
            }
        }

        private async Task ModalSelectionHandler(SocketModal interaction)
        {
            try
            {
                await ModalNotesTask(interaction);
            }
            catch (Exception ex)
            {
                Log.Error($"modal - clicked - user:{interaction.User.Id} error:{ex.Message}");
            }
        }

        private async Task ButtonSelectionHandler(SocketMessageComponent interaction)
        {
            try
            {
                var customId = interaction.Data.CustomId;
                var user = (SocketGuildUser)interaction.User;
                var guild = user.Guild;
                var builder = new ComponentBuilder();
                var modal = new ModalBuilder();
                string answers = "";
                string[] split = null;

                switch (customId)
                {
                    case "home":

                        builder = new ComponentBuilder()
                            .WithButton("Der Akt wurde vollzogen", "start", ButtonStyle.Success)
                            .WithButton("letzten Akt bearbeiten", "edit", ButtonStyle.Primary)
                            .WithButton("letzten Akt löschen", "delete", ButtonStyle.Danger)
                            ;

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder("Neuaufnahme", $"Hier kann Sex aufgezeichnet werden:"), Colors.information);
                        });

                        break;

                    case "start":

                        builder = new ComponentBuilder()
                            .WithButton("Keiner", "none", ButtonStyle.Secondary)
                            .WithButton("Christoph", "christoph", ButtonStyle.Primary)
                            .WithButton("Nadine", "nadine", ButtonStyle.Danger)
                            .WithButton("Christoph und Nadine", "both", ButtonStyle.Success)
                            .WithButton("Home", "home", ButtonStyle.Secondary);

                        await interaction.UpdateAsync(msgProps =>
                            {
                                msgProps.Components = builder.Build();
                                msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder("Wer ist gekommen?", $"Antworten:"), Colors.information);
                            });

                        break;

                    case "edit":

                        SpreadSheetConnector google = new SpreadSheetConnector();
                        
                        google.ConnectToGoogle();
                        
                        var global = Global.GetByName("sex_id");
                        var item = google.GetRow(Convert.ToInt32(global.value) -1);

                        modal = new ModalBuilder()
                            .WithTitle("Bearbeiten")
                            .WithCustomId("edit")
                            .AddTextInput("Wer ist gekommen?", "who", placeholder: "Christoph | Nadine", required: true, value: item.who)
                            .AddTextInput("Was für ein Akt wurde vollzogen?", "type", placeholder: "Sex´| Oral | Masturbation", required: true, value: item.type)
                            .AddTextInput("Bemerkungen", "note", TextInputStyle.Paragraph, required: true, value: item.notes)
                            ;

                        await interaction.RespondWithModalAsync(modal.Build());

                        break;

                    case "delete":

                        builder = new ComponentBuilder()
                            .WithButton("Löschen", "deleteyes", ButtonStyle.Danger)
                            .WithButton("Home", "home", ButtonStyle.Secondary);

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder("Den letzten Akt wirklich löschen?", $"Diese Aktion kann nicht rückgängig gemacht werden!"), Colors.information);
                        });

                        break;

                    case "deleteyes":

                        await ButtonDeleteYesTask();

                        break;

                    case "none":

                        builder = new ComponentBuilder()
                            .WithButton("Oral", "oral", ButtonStyle.Secondary)
                            .WithButton("Sex", "sex", ButtonStyle.Primary)
                            .WithButton("Masturbation", "masturbation", ButtonStyle.Success)
                            .WithButton("Home", "home", ButtonStyle.Secondary);

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder($"Was wurde durchgeführt?", $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nKeiner"), Colors.information);
                        });

                        break;

                    case "christoph":

                        builder = new ComponentBuilder()
                            .WithButton("Oral", "oral", ButtonStyle.Secondary)
                            .WithButton("Sex", "sex", ButtonStyle.Primary)
                            .WithButton("Masturbation", "masturbation", ButtonStyle.Success)
                            .WithButton("Home", "home", ButtonStyle.Secondary);

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder($"Was wurde durchgeführt?", $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nChristoph"), Colors.information);
                        });
                        
                        break;

                    case "nadine":

                        builder = new ComponentBuilder()
                            .WithButton("Oral", "oral", ButtonStyle.Secondary)
                            .WithButton("Sex", "sex", ButtonStyle.Primary)
                            .WithButton("Masturbation", "masturbation", ButtonStyle.Success)
                            .WithButton("Home", "home", ButtonStyle.Secondary);

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder($"Was wurde durchgeführt?", $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nNadine"), Colors.information);
                        });

                        break;

                    case "both":

                        builder = new ComponentBuilder()
                            .WithButton("Oral", "oral", ButtonStyle.Secondary)
                            .WithButton("Sex", "sex", ButtonStyle.Primary)
                            .WithButton("Masturbation", "masturbation", ButtonStyle.Success)
                            .WithButton("Home", "home", ButtonStyle.Secondary);

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(user, Field.CreateFieldBuilder($"Was wurde durchgeführt?", $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nChristoph und Nadine"), Colors.information);
                        });

                        break;

                    case "oral":

                        answers = $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nOral";
                        split = answers.Split("\n");
                        answers = $"{split[1]}\n{split[2]}";

                        modal = new ModalBuilder()
                            .WithTitle("Bemerkungen")
                            .WithCustomId("notes")
                            .AddTextInput("Was hat dir gut gefallen, oder war Besonders?", "note", placeholder: "Ich liebe dich!", required: false, value: "")
                            .AddTextInput("Antworten", "answers", TextInputStyle.Paragraph, required: true, value: answers)
                            ;

                        await interaction.RespondWithModalAsync(modal.Build());

                        break;

                    case "sex":

                        answers = $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nSex";
                        split = answers.Split("\n");
                        answers = $"{split[1]}\n{split[2]}";

                        modal = new ModalBuilder()
                            .WithTitle("Bemerkungen")
                            .WithCustomId("notes")
                            .AddTextInput("Was hat dir gut gefallen, oder war Besonders?", "note", placeholder: "Ich liebe dich!", required: false, value: "")
                            .AddTextInput("Antworten", "answers", TextInputStyle.Paragraph, required: true, value: answers)
                            ;

                        await interaction.RespondWithModalAsync(modal.Build());

                        break;

                    case "masturbation":

                        answers = $"{interaction.Message.Embeds.FirstOrDefault().Fields.FirstOrDefault().Value}\nMasturbation";
                        split = answers.Split("\n");
                        answers = $"{split[1]}\n{split[2]}";

                        modal = new ModalBuilder()
                            .WithTitle("Bemerkungen")
                            .WithCustomId("notes")
                            .AddTextInput("Was hat dir gut gefallen, oder war Besonders?", "note", placeholder: "Ich liebe dich!", required: false, value: "")
                            .AddTextInput("Antworten", "answers", TextInputStyle.Paragraph, required: true, value: answers)
                            ;

                        await interaction.RespondWithModalAsync(modal.Build());

                        break;

                    default:

                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"button - clicked - user:{interaction.User.Id} error:{ex.Message}");
            }
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            try
            {
                if (arg.IsBot)
                    return;

                Log.Information($"system - user joined - start user:{arg.Id}");

                var user = User.GetById(arg.Id);

                if (user == null)
                    User.Add(new User(user.id, user.name, 0));
            }
            catch (Exception ex)
            {
                Log.Error($"system - user joined - user:{arg.Id} error:{ex.Message}");
            }
        }

        private async Task Client_UserUpdated(SocketUser user_before, SocketUser user_after)
        {
            try
            {
                if (user_before.IsBot)
                    return;

                Log.Information($"system - user updated - start user:{user_before.Id}");

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
            catch (Exception ex)
            {
                Log.Error($"system - user updated - user:{user_before.Id} error:{ex.Message}");
            }
        }

        private async Task Client_ChannelCreated(SocketChannel arg)
        {
            try
            {
                Log.Information($"system - channel created - start channel:{arg.Id}");

                var channel = arg as IGuildChannel;
                Channel.Add(new Channel(channel.Id, channel.Name, channel.GuildId));
            }
            catch (Exception ex)
            {
                Log.Error($"system - channel created - channel:{arg.Id} error:{ex.Message}");
            }
        }

        private async Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2)
        {
            try
            {
                Log.Information($"system - channel updated - start channel:{arg1.Id}");

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
            catch (Exception ex)
            {
                Log.Error($"system - channel updated - channel:{arg1.Id} error:{ex.Message}");
            }
        }

        private async Task Client_ChannelDestroyed(SocketChannel channel)
        {
            try
            {
                Log.Information($"system - channel destroyed - start channel:{channel.Id}");

                Channel.DeleteById(channel.Id);
            }
            catch (Exception ex)
            {
                Log.Error($"system - channel destroyed - channel:{channel.Id} error:{ex.Message}");
            }
        }

        private async Task Client_JoinedGuild(SocketGuild guild)
        {
            try
            {
                Log.Information($"system - joined guild - start guild:{guild.Id}");

                Server.Add(new Server(guild.Id, guild.Name));

                foreach (var channel in guild.Channels)
                {
                    Channel.Add(new Channel(channel.Id, channel.Name, guild.Id));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"system - joined guild - guild:{guild.Id} error:{ex.Message}");
            }
        }

        private async Task Client_GuildUpdated(SocketGuild server_before, SocketGuild server_after)
        {
            try
            {
                Log.Information($"system - guild updated - start guild:{server_before.Id}");

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
            catch (Exception ex)
            {
                Log.Error($"system - guild updated - guild:{server_before.Id} error:{ex.Message}");
            }
        }

        private async Task Client_LeftGuild(SocketGuild guild)
        {
            try
            {
                Log.Information($"system - left guild - start guild:{guild.Id}");

                Channel.DeleteAllByServerId(guild.Id);
                Server.DeleteById(guild.Id);
            }
            catch (Exception ex)
            {
                Log.Error($"system - left guild - guild:{guild.Id} error:{ex.Message}");
            }
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.

        private async Task Client_Ready()
        {
            try
            {
                Log.Information($"system - ready - start");

                await Client.SetGameAsync(Convert.ToString(Global.GetByName("ready_initial_activity_playing").value), "", ActivityType.Playing);

                System.Timers.Timer ready_timer = new System.Timers.Timer
                {
                    AutoReset = false,
                    Interval = Convert.ToInt32(Global.GetByName("ready_timer_minute").value) * 60 * 1000,
                    Enabled = true
                };
                ready_timer.Elapsed += Ready_Timer;
            }
            catch (Exception ex)
            {
                Log.Error($"system - ready - error:{ex.Message}");
            }
        }
        private async void Ready_Timer(object sender, ElapsedEventArgs e)
        {
            try
            {
                Log.Information($"system - ready timer - start");

                await Client.SetGameAsync(Convert.ToString(Global.GetByName("ready_timer_elapsed_watching_activity").value), "", ActivityType.Watching);
            }
            catch (Exception ex)
            {
                Log.Error($"system - ready timer - error:{ex.Message}");
            }
        }

        public static async void Copy_Message(IUserMessage Message, ulong channelId, bool move = false)
        {
            try
            {
                Log.Information($"system - copy message - start message:{Message.Id} channel:{channelId}");

                var channel = Client.GetChannel(channelId) as ISocketMessageChannel;
                var attachments = Message.Attachments;
                var embeds = Message.Embeds;
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                var x = Message;
                var restUser = await Client.Rest.GetUserAsync(Message.Author.Id);

                if (Core.Classes.Message.GetByMessageAndChannelAndType(Message.Id, channel.Id, 'c') == null && !move || move)
                {
                    try
                    {
                        if (embeds.Count == 1 && Message.Author.Id == Client.CurrentUser.Id)
                        {
                            x = await channel.SendMessageAsync(embed: embeds.FirstOrDefault().ToEmbedBuilder().Build());
                        }
                        else if (embeds.Count == 1 && !(Message.Author.Id == Client.CurrentUser.Id))
                        {
                            if (embeds.FirstOrDefault().Image != null)
                            {
                                if (Message.Content != "")
                                    fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                                x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: embeds.FirstOrDefault().Image.Value.Url, footer: Message.Author.Id.ToString()));
                            }
                            else if (embeds.FirstOrDefault().Thumbnail != null)
                            {
                                if (Message.Content != "")
                                    fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                                x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: embeds.FirstOrDefault().Thumbnail.Value.Url, footer: Message.Author.Id.ToString()));
                            }

                        }
                        else
                        {
                            if (attachments.Count == 1)
                            {
                                if (Message.Content != "")
                                    fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                                x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: attachments.First().Url, footer: Message.Author.Id.ToString()));
                            }
                            else
                            {
                                if (Message.Content.EndsWith(".jpg") || Message.Content.EndsWith(".jpeg") || Message.Content.EndsWith(".png"))
                                {
                                    fields.Add(Field.CreateFieldBuilder("message", Message.Content));
                                    x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: Message.Content, footer: Message.Author.Id.ToString()));
                                }
                                else
                                {
                                    fields.Add(Field.CreateFieldBuilder("message", Message.Content));
                                    x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", footer: Message.Author.Id.ToString()));
                                }
                            }
                        }

                        if (!move)
                            Core.Classes.Message.Add(new Message(Message.Author.Id, Message.Id, channel.Id, 'c'));
                    }
                    finally
                    {
                        await x.AddReactionAsync(new Emoji("👍"));
                        await x.AddReactionAsync(new Emoji("👎"));
                    }
                }

                if (move)
                    await Message.DeleteAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"system - copy message - message:{Message.Id} channel:{channelId} error:{ex.Message}");
            }
        }

        public static async void Broadcast_Message(IUserMessage Message)
        {
            try
            {
                Log.Information($"system - broadcast message - start message:{Message.Id}");

                var broadcast_list = Channel.GetAllByBroadcast(1);

                foreach (var broadcast in broadcast_list)
                {
                    var channel = Client.GetChannel(broadcast.id) as ISocketMessageChannel;
                    var attachments = Message.Attachments;
                    var embeds = Message.Embeds;
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                    var x = Message;
                    var restUser = await Client.Rest.GetUserAsync(Message.Author.Id);

                    if (Core.Classes.Message.GetByMessageAndChannelAndType(Message.Id, channel.Id, 'b') == null)
                    {
                        try
                        {
                            if (embeds.Count == 1 && Message.Author.Id == Client.CurrentUser.Id)
                            {
                                x = await channel.SendMessageAsync(embed: embeds.FirstOrDefault().ToEmbedBuilder().Build());
                            }
                            else if (embeds.Count == 1 && !(Message.Author.Id == Client.CurrentUser.Id))
                            {
                                if (embeds.FirstOrDefault().Image != null)
                                {
                                    if (Message.Content != "")
                                        fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                                    x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: embeds.FirstOrDefault().Image.Value.Url, footer: Message.Author.Id.ToString()));
                                }
                                else if (embeds.FirstOrDefault().Thumbnail != null)
                                {
                                    if (Message.Content != "")
                                        fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                                    x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: embeds.FirstOrDefault().Thumbnail.Value.Url, footer: Message.Author.Id.ToString()));
                                }

                            }
                            else
                            {
                                if (attachments.Count == 1)
                                {
                                    if (Message.Content != "")
                                        fields.Add(Field.CreateFieldBuilder("message", Message.Content));

                                    x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: attachments.First().Url, footer: Message.Author.Id.ToString()));
                                }
                                else
                                {
                                    if (Message.Content.EndsWith(".jpg") || Message.Content.EndsWith(".jpeg") || Message.Content.EndsWith(".png"))
                                    {
                                        fields.Add(Field.CreateFieldBuilder("message", Message.Content));
                                        x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", imgURL: Message.Content, footer: Message.Author.Id.ToString()));
                                    }
                                    else
                                    {
                                        fields.Add(Field.CreateFieldBuilder("message", Message.Content));
                                        x = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(restUser, fields, Colors.meme, description: $"from [{Message.Channel.Name}]({Message.GetJumpUrl()})", footer: Message.Author.Id.ToString()));
                                    }
                                }
                            }

                        }
                        finally
                        {
                            await x.AddReactionAsync(new Emoji("👍"));
                            await x.AddReactionAsync(new Emoji("👎"));
                        }
                    }
                }

                Core.Classes.Message.Add(new Message(Message.Author.Id, Message.Id, Message.Channel.Id, 'b'));
            }
            catch (Exception ex)
            {
                Log.Error($"system - broadcast message - message:{Message.Id} error:{ex.Message}");
            }
        }

        public static async Task ModalNotesTask(SocketModal interaction)
        {
            var customId = interaction.Data.CustomId;
            var answer = $"{interaction.Data.Components.LastOrDefault().Value}\n{interaction.Data.Components.FirstOrDefault().Value}";
            var answers = answer.Split("\n");

            SpreadSheetConnector google = new SpreadSheetConnector();
            google.ConnectToGoogle();

            switch (customId)
            {
                case "notes":
                    try
                    {
                        var item = new Item(answers[0], answers[1], answers[2], DateTime.Now);

                        google.EditRow(item);

                        Repetitive_Timer.Minutes_5_timer_Elapsed(null, null);

                        //Back to the beginning
                        var builder = new ComponentBuilder()
                                .WithButton("Der Akt wurde vollzogen", "start", ButtonStyle.Success)
                                .WithButton("letzten Akt bearbeiten", "edit", ButtonStyle.Primary)
                                .WithButton("letzten Akt löschen", "delete", ButtonStyle.Danger)
                        ;

                        await interaction.UpdateAsync(msgProps =>
                        {
                            msgProps.Components = builder.Build();
                            msgProps.Embed = Core.Classes.Embed.New(interaction.User, Field.CreateFieldBuilder("Neuaufnahme", $"Hier kann Sex aufgezeichnet werden:"), Colors.information);
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"system - ModalNotes notes - error:{ex.Message}");
                    }

                    break;
                case "edit":

                    try
                    {
                        var test = interaction.Data.Components;

                        var global = Global.GetByName("sex_id");

                        var lastItem = google.GetRow(Convert.ToInt32(global.value) - 1);
                        

                        string note = "";
                        string type = "";
                        string who = "";

                        foreach (var x in interaction.Data.Components)
                        {
                            if (x.CustomId == "who")
                            {
                                who = x.Value;
                            }
                            else if (x.CustomId == "type")
                            {
                                type = x.Value;
                            }
                            else if (x.CustomId == "note")
                            {
                                note = x.Value;
                            }
                        }

                        var newItem = new Item(who, type, note, lastItem.when);

                        //edit discord
                        var embed = SexGetEmbed(newItem, (Convert.ToInt32(global.value) - 1).ToString());

                        var editChannel = (IMessageChannel)Program.Client.GetChannel(1242565685146816584);
                        var editMessage = (IUserMessage)await editChannel.GetMessageAsync((ulong)Convert.ToInt64(Global.GetByName("sex_last_message_id").value));

                        await editMessage.ModifyAsync(msgProps =>
                        {
                            msgProps.Embed = embed;
                        });

                        //edit spreadsheet
                        google.EditRow(newItem, Convert.ToInt32(global.value) - 1);

                        //edit stats
                        AddToSexStats(lastItem, -1);
                        AddToSexStats(newItem, +1);

                        //Update stats
                        UpdateSexStats();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"system - ModalNotes edit - error:{ex.Message}");
                    }

                    break;

                default:

                    break;
            }
        }

        public static void AddToSexStats(Item item, int toAdd)
        {
            var global = Global.GetByName("sex_id");        

            if (item.type == "Sex")
            {
                global = Global.GetByName("sex_stats_sex");
                global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                Global.Edit(global);


                if (item.who == "Christoph")
                {
                    global = Global.GetByName("sex_stats_sex_christoph");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
                else if (item.who == "Nadine")
                {
                    global = Global.GetByName("sex_stats_sex_nadine");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
                else if (item.who == "Christoph und Nadine")
                {
                    global = Global.GetByName("sex_stats_sex_christoph");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);

                    global = Global.GetByName("sex_stats_sex_nadine");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
            }
            else if (item.type == "Oral")
            {
                global = Global.GetByName("sex_stats_oral");
                global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                Global.Edit(global);


                if (item.who == "Christoph")
                {
                    global = Global.GetByName("sex_stats_oral_christoph");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
                else if (item.who == "Nadine")
                {
                    global = Global.GetByName("sex_stats_oral_nadine");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
                else if (item.who == "Christoph und Nadine")
                {
                    global = Global.GetByName("sex_stats_oral_christoph");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);

                    global = Global.GetByName("sex_stats_oral_nadine");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
            }
            else if (item.type == "Masturbation")
            {
                if (item.who == "Christoph")
                {
                    global = Global.GetByName("sex_stats_masturbation_christoph");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
                else if (item.who == "Nadine")
                {
                    global = Global.GetByName("sex_stats_masturbation_nadine");
                    global.value = (Convert.ToInt32(global.value) + toAdd).ToString();
                    Global.Edit(global);
                }
            }
        }

        public static async Task ButtonDeleteYesTask()
        {
            try
            {
                var sex_last_message_id = (ulong)Convert.ToInt64(Global.GetByName("sex_last_message_id").value);
                var sex_id = Global.GetByName("sex_id");

                var channel = (ISocketMessageChannel)Client.GetChannel(1242565685146816584);
                var message = await channel.GetMessageAsync(sex_last_message_id);

                if (message == null)
                    return;

                await message.DeleteAsync();

                sex_id.value = (Convert.ToInt32(sex_id.value) - 1).ToString();
                Global.Edit(sex_id);

                SpreadSheetConnector google = new SpreadSheetConnector();
                google.ConnectToGoogle();

                var item = google.GetRow(Convert.ToInt32(sex_id.value));

                if (item.type == "Sex")
                {
                    var global = Global.GetByName("sex_stats_sex");
                    global.value = (Convert.ToInt32(global.value) - 1).ToString();
                    Global.Edit(global);


                    if (item.who == "Christoph")
                    {
                        global = Global.GetByName("sex_stats_sex_christoph");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                    else if (item.who == "Nadine")
                    {
                        global = Global.GetByName("sex_stats_sex_nadine");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                    else if (item.who == "Christoph und Nadine")
                    {
                        global = Global.GetByName("sex_stats_sex_christoph");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);

                        global = Global.GetByName("sex_stats_sex_nadine");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                }
                else if (item.type == "Oral")
                {
                    var global = Global.GetByName("sex_stats_oral");
                    global.value = (Convert.ToInt32(global.value) - 1).ToString();
                    Global.Edit(global);


                    if (item.who == "Christoph")
                    {
                        global = Global.GetByName("sex_stats_oral_christoph");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                    else if (item.who == "Nadine")
                    {
                        global = Global.GetByName("sex_stats_oral_nadine");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                    else if (item.who == "Christoph und Nadine")
                    {
                        global = Global.GetByName("sex_stats_oral_christoph");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);

                        global = Global.GetByName("sex_stats_oral_nadine");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                }
                else if (item.type == "Masturbation")
                {
                    if (item.who == "Christoph")
                    {
                        var global = Global.GetByName("sex_stats_masturbation_christoph");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                    else if (item.who == "Nadine")
                    {
                        var global = Global.GetByName("sex_stats_masturbation_nadine");
                        global.value = (Convert.ToInt32(global.value) - 1).ToString();
                        Global.Edit(global);
                    }
                }

                google.DeleteRow(Convert.ToInt32(sex_id.value));

                try
                {
                    UpdateSexStats();
                }
                catch (Exception)
                {

                }
            }
            catch (Exception ex)
            {
                Log.Error($"system - system ButtonDeleteYesTask - error:{ex.Message}");
            }
        }

        public static async void SexSendStatsDiscord(Item item)
        {
            var channel = Program.Client.GetChannel(1242565685146816584) as ISocketMessageChannel;
            var global = Global.GetByName("sex_id");

            var embed = SexGetEmbed(item, $"{Convert.ToInt32(global.value) - 1}");

            var message = await channel.SendMessageAsync(embed: embed);

            global = Global.GetByName("sex_last_message_id");
            global.value = message.Id.ToString();
            Global.Edit(global);
        }

        public static Discord.Embed SexGetEmbed(Item item, string footer)
        {
            Discord.Embed embed = null;

            if (item.who != "")
            {
                if (item.who == "Nadine")
                {
                    embed = Core.Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{item.who} ist durch {item.type} gekommen!", $"{item.notes}"), Colors.nadine, item.when, footer: footer);
                }
                else if (item.who == "Christoph")
                {
                    embed = Core.Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{item.who} ist durch {item.type} gekommen!", $"{item.notes}"), Colors.christoph, item.when, footer: footer);
                }
                else if (item.who == "Christoph, Nadine")
                {
                    embed = Core.Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"Wir sind beide richtig geil durch {item.type} gekommen!", $"{item.notes}"), Colors.rumpfi, item.when, footer: footer);
                }
            }
            else
            {
                embed = Core.Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"Wir hatten {item.type}!", $"{item.notes}"), Colors.gray, item.when, footer: footer);
            }

            return embed;
        }

        public static async void UpdateSexStats()
        {
            var statsChannel = (IMessageChannel)Program.Client.GetChannel(1242853004395282513);
            var statsMessage = (IUserMessage)await statsChannel.GetMessageAsync(1243887965722378279);

            await statsMessage.ModifyAsync(msgProps =>
            {
                msgProps.Embed = Core.Classes.Embed.GetSexStats();
            });
        }
    }
}
