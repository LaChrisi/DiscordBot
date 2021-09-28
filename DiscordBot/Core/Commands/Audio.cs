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
        internal static Dictionary<ulong, AudioClient> audioClients = new Dictionary<ulong, AudioClient>();

        [Command("leave", RunMode = RunMode.Async), Summary("leave the current voice channel")]
        public async Task LeaveModule()
        {
            try
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - leave - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - leave - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                audioClients[Context.Guild.Id].Stop();

                audioClients[Context.Guild.Id].CleanUp();

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
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - stop - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - stop - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                audioClients[Context.Guild.Id].Stop();

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"stopped!"), Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - stop - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("shuffle", RunMode = RunMode.Async), Summary("shuffle the current music queue")]
        public async Task ShuffleModule()
        {
            try
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - shuffle - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - shuffle - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                audioClients[Context.Guild.Id].Shuffle();

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"queue shuffled!"), Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - shuffle - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("queue", RunMode = RunMode.Async), Summary("stop the current music")]
        public async Task QueueModule()
        {
            try
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - stop - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - stop - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                string queueOutput = "";

                foreach (var video in audioClients[Context.Guild.Id].videoQueue)
                {
                    queueOutput = queueOutput + $"[{video.Title}]({video.Url})\n";
                }

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue", $"next 5 songs:\n{queueOutput}total {audioClients[Context.Guild.Id].videoQueue.Count + audioClients[Context.Guild.Id].IdQueue.Count} songs"), Colors.information));
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
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - skip - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

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
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - play - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - play - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                //join if not connected
                if (!audioClients.ContainsKey(Context.Guild.Id))
                {
                    var channel = (Context.User as IVoiceState).VoiceChannel;

                    audioClients.Add(Context.Guild.Id, new AudioClient(await channel.ConnectAsync(), Context.Channel, channel));
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