using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;
using Discord.Audio;
using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.IO;
using CliWrap;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using DiscordBot.Core.Commands;

namespace DiscordBot.Core.Classes
{
    class AudioClient
    {
        public IAudioClient audioClient { get; set; }

        public string playing { get; set; }
        private Queue<string> queue { get; set; }

        private CancellationTokenSource Cancel { get; set; }
        private CancellationTokenSource Skip { get; set; }

        public AudioClient(IAudioClient Client)
        {
            audioClient = Client;
            playing = null;
            queue = new Queue<string>();
            Cancel = null;
            Skip = null;
        }

        public void Stop()
        {
            Cancel?.Cancel();
            Skip?.Cancel();
            playing = null;
        }

        public void Next()
        {
            Skip?.Cancel();
            playing = null;
        }

        public void Shuffle()
        {
            var Rand = new Random();
            var NewQueue = new Queue<string>();

            foreach (var Song in queue.ToArray().OrderBy(x => Rand.Next()))
                NewQueue.Enqueue(Song);

            queue = NewQueue;
        }

        public void Add(string input)
        {
            queue.Enqueue(input);
        }

        public async Task Start()
        {
            Cancel = new CancellationTokenSource();

            while (!Cancel.IsCancellationRequested)
            {
                try
                {
                    Skip = new CancellationTokenSource();

                    while (queue.Count == 0 && !Cancel.IsCancellationRequested)
                        await Task.Delay(10);

                    playing = queue.Dequeue();

                    if (playing != null)
                    {
                        PlaySong(playing);
                    }
                    
                    playing = null;
                }
                catch (Exception)
                {
                    
                }
            }
        }

        private async void PlaySong(string input)
        {
            await audioClient.SetSpeakingAsync(true);

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

                using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    try { await discord.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length, Skip.Token); }
                    finally { await discord.FlushAsync(); }
                }
            }
            else
            {
                string path = "music/" + input + ".mp3";

                using (var ffmpeg = Process.Start(new ProcessStartInfo
                {
#if DEBUG
                    FileName = "ffmpeg.exe",
#else
                    FileName = "ffmpeg",
#endif
                    Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }))
                using (var output = ffmpeg.StandardOutput.BaseStream)
                using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    try { await output.CopyToAsync(discord, Skip.Token); }
                    finally { await discord.FlushAsync(); }
                }
            }

            await audioClient.SetSpeakingAsync(false);
        }
    }
}
