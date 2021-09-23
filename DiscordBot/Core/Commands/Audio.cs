using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;
using Discord.Audio;
using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Converter;
using System.IO;
using CliWrap;
using System.Collections.Generic;
using Discord.WebSocket;

namespace DiscordBot.Core.Commands
{
    public class Audio : ModuleBase<SocketCommandContext>
    {
        private static Dictionary<ulong, IAudioClient> audioClients = new Dictionary<ulong, IAudioClient>();
        private static Dictionary<ulong, Queue<string>> queues = new Dictionary<ulong, Queue<string>>();

        [Command("leave", RunMode = RunMode.Async), Summary("leave the current voice channel")]
        public async Task LeaveModule()
        {
            try
            {
                Log.Information($"command - leave - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                await audioClients[Context.Guild.Id].SetSpeakingAsync(false);

                await audioClients[Context.Guild.Id].StopAsync();

                //cleanup
                if (audioClients.ContainsKey(Context.Guild.Id))
                {
                    audioClients.Remove(Context.Guild.Id);
                }

                if (queues.ContainsKey(Context.Guild.Id))
                {
                    queues.Remove(Context.Guild.Id);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"command - leave - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("play", RunMode = RunMode.Async), Summary("play voice")]
        public async Task SendModule(string input)
        {
            try
            {
                Log.Information($"command - play - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (!audioClients.ContainsKey(Context.Guild.Id))
                {
                    var channel = (Context.User as IVoiceState).VoiceChannel;

                    audioClients.Add(Context.Guild.Id, await channel.ConnectAsync());
                }

                if (queues[Context.Guild.Id].Count == 0)
                {
                    PlaySong(Context.Guild.Id, input);
                    
                }
                else
                {
                    queues[Context.Guild.Id].Enqueue(input);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"command - play - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
#if DEBUG
                FileName = "ffmpeg.exe",
#else
                FileName = "ffmpeg",
#endif
                Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
        }

        private async void PlaySong(ulong guildID, string input)
        {
            await audioClients[guildID].SetSpeakingAsync(true);

            if (input.StartsWith("https://www.youtube.com/watch?v="))
            {
                YoutubeClient youtube = new YoutubeClient();

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(input.Substring(32));
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                var memoryStream = new MemoryStream();
                await Cli.Wrap("ffmpeg")
                    .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                    .WithStandardInputPipe(PipeSource.FromStream(stream))
                    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                    .ExecuteAsync();

                using (var discord = audioClients[guildID].CreatePCMStream(AudioApplication.Mixed))
                {
                    try { await discord.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length); }
                    finally { await discord.FlushAsync(); }
                }
            }
            else
            {
                string path = "music/" + input + ".mp3";

                using (var ffmpeg = CreateStream(path))
                using (var output = ffmpeg.StandardOutput.BaseStream)
                using (var discord = audioClients[guildID].CreatePCMStream(AudioApplication.Mixed))
                {
                    try { await output.CopyToAsync(discord); }
                    finally { await discord.FlushAsync(); }
                }
            }

            await audioClients[Context.Guild.Id].SetSpeakingAsync(false);
        }

        private Task CreateTask()
        {
            Action action = () =>
            {
                
            };

            return new Task(action);
        }
    }
}