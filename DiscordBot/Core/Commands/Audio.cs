using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;
using CliWrap;
using System.Collections.Generic;

namespace DiscordBot.Core.Commands
{
    public class Audio : ModuleBase<SocketCommandContext>
    {
        private static Dictionary<ulong, AudioClient> audioClients = new Dictionary<ulong, AudioClient>();

        [Command("leave", RunMode = RunMode.Async), Summary("leave the current voice channel")]
        public async Task LeaveModule()
        {
            try
            {
                Log.Information($"command - leave - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                audioClients[Context.Guild.Id].Stop();

                await audioClients[Context.Guild.Id].audioClient.SetSpeakingAsync(false);

                await audioClients[Context.Guild.Id].audioClient.StopAsync();

                //cleanup
                if (audioClients.ContainsKey(Context.Guild.Id))
                {
                    audioClients.Remove(Context.Guild.Id);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"command - leave - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("stop", RunMode = RunMode.Async), Summary("stop the current music")]
        public async Task StopModule()
        {
            try
            {
                Log.Information($"command - stop - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                audioClients[Context.Guild.Id].Stop();

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"stopped!"), Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - stop - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("queue", RunMode = RunMode.Async), Summary("stop the current music")]
        public async Task QueueModule()
        {
            try
            {
                Log.Information($"command - stop - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                var queue = audioClients[Context.Guild.Id].queue.ToArray();

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue", $"next 5 songs:\n[{queue[0].Title}]({queue[0].Url})\n[{queue[1].Title}]({queue[1].Url})\n[{queue[2].Title}]({queue[2].Url})\n[{queue[3].Title}]({queue[3].Url})\n[{queue[4].Title}]({queue[4].Url})\ntotal {audioClients[Context.Guild.Id].queue.Count} songs"), Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - stop - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("skip", RunMode = RunMode.Async), Summary("skip the current song")]
        public async Task SkipModule()
        {
            try
            {
                Log.Information($"command - skip - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                audioClients[Context.Guild.Id].Next();
            }
            catch (Exception ex)
            {
                Log.Error($"command - skip - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("play", RunMode = RunMode.Async), Summary("play music")]
        public async Task PlayModule(params string[] input)
        {
            try
            {
                Log.Information($"command - play - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                //join if not connected
                if (!audioClients.ContainsKey(Context.Guild.Id))
                {
                    var channel = (Context.User as IVoiceState).VoiceChannel;

                    audioClients.Add(Context.Guild.Id, new AudioClient(await channel.ConnectAsync(), (IMessageChannel) Context.Channel));
                }

                audioClients[Context.Guild.Id].Add(input);

                if (audioClients[Context.Guild.Id].playing == null)
                {
                    await audioClients[Context.Guild.Id].Start();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"command - play - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}