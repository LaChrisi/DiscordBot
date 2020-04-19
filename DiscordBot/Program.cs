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
                //await Data.SaveLikes(Message.Value.Author.Id, -1);
            }
            else if (Reaction.Emote.Name == "👎")
            {
                //await Data.SaveDislikes(Message.Value.Author.Id, -1);
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
                //await Data.SaveLikes(Message.Value.Author.Id, 1);
            }
            else if (Reaction.Emote.Name == "👎")
            {
                //await Data.SaveDislikes(Message.Value.Author.Id, 1);
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

            int ArgPos = 0;
            if (Message.HasStringPrefix("!", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))
            {
                var Result = await Commands.ExecuteAsync(Context, ArgPos, null);

                if (!Result.IsSuccess)
                    Console.WriteLine($"{DateTime.Now} at Commands] Somthing went wrong with executing a command.\nText: {Context.Message.Content}\nError: {Result.ErrorReason}");

                return;
            }
            
            if((Context.Guild.Id == ServerIDs.landfill && Message.Channel.Id != ServerIDs.landfill_memes) && (Message.Content.StartsWith("https://9gag.com/") || Message.Content.StartsWith("https://www.reddit.com/")))
            {
                var channel = Client.GetChannel(ServerIDs.landfill_memes) as ISocketMessageChannel;
                var x = await channel.SendMessageAsync($"meme from: {Message.Author.Mention}\n{Message.Content.ToString()}");

                await Context.Channel.DeleteMessageAsync(Message.Id);

                await x.AddReactionAsync(new Emoji("👍"));
                await x.AddReactionAsync(new Emoji("👎"));
                //await Data.SavePosts(Context.User.Id, 1);
                return;
            }

            if ((Message.Channel.Id == ServerIDs.saltyAutismKids_davidsBadComics || Message.Channel.Id == ServerIDs.saltyAutismKids_davidsBadMemes || Message.Channel.Id == ServerIDs.pabliSible_9gagnshit || Message.Channel.Id == ServerIDs.landfill_memes || Message.Channel.Id == ServerIDs.landfill_geheim || Message.Channel.Id == ServerIDs.pabliSible_test || Message.Channel.Id == ServerIDs.pabliSible_siftnstuff) && (Message.Content.ToString() == "" || Message.Content.Contains("https://") || Message.Content.Contains("http://")))
            {
                await Message.AddReactionAsync(new Emoji("👍"));
                await Message.AddReactionAsync(new Emoji("👎"));
                //await Data.SavePosts(Context.User.Id,1);
                return;
            }

            if (Message.Channel.Id == ServerIDs.landfill_geheim && (Message.Content.ToLower().Trim().Contains("r6") || Message.Content.ToLower().Trim().Contains("rainbowsix") || Message.Content.ToLower().Trim().Contains("rainbowshit") || Message.Content.ToLower().Trim().Contains("rainbow") || Message.Content.ToLower().Trim().Contains("siege")))
            {
                await Message.AddReactionAsync(new Emoji("🌈"));
                await Message.AddReactionAsync(new Emoji("💩"));
                return;
            }

            if (Message.Channel.Id == ServerIDs.pabliSible_siftnstuff && !Message.Content.Contains("Borderlands 3") && Message.Author.IsWebhook)
            {
                await Message.DeleteAsync();
            }
        }
    }
}
