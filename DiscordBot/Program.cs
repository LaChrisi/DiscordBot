using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Data.SQLite;

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

            foreach (var x in Reactions)
            {
                if (x.Key.Name == "👍" && x.Value.ReactionCount >= 5)
                {
                    await userMessage.AddReactionAsync(new Emoji("⭐"));
                    await userMessage.PinAsync();
                }
                else if (x.Key.Name == "👎" && x.Value.ReactionCount >= 5)
                {
                    await userMessage.AddReactionAsync(new Emoji("💩"));
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

        private async Task Client_Log(LogMessage Message)
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
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            if (Context.User.IsBot)
                return;

            //Feher in Console loggen
            int ArgPos = 0;
            if (Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))
            {
                var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

                if (!Result.IsSuccess)
                    Console.WriteLine($"{DateTime.Now} at Commands] Somthing went wrong with executing a command.\nText: {Context.Message.Content}\nError: {Result.ErrorReason}");

                return;
            }
            
            //memes verschieben
            if((Context.Guild.Id == ServerIDs.landfill && Message.Channel.Id != ServerIDs.landfill_memes) && (Message.Content.StartsWith("https://9gag.com/") || Message.Content.StartsWith("https://www.reddit.com/")))
            {
                var channel = Client.GetChannel(ServerIDs.landfill_memes) as ISocketMessageChannel;
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

                return;
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
                                if (Message.Content.Contains(what_item) && what_item != "" || Message.Content.ToString().Trim() == what_item)
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
            //Borderlands 3
            if (Message.Channel.Id == ServerIDs.pabliSible_siftnstuff && !Message.Content.Contains("Borderlands 3") && Message.Author.IsWebhook)
            {
                await Message.DeleteAsync();
            }
        }
    }
}
