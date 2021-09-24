using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;
using Discord.Audio;
using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using System.IO;
using CliWrap;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using DiscordBot.Core.Commands;
using System.Web;

namespace DiscordBot.Core.Classes
{
    class AudioClient
    {
        public IAudioClient audioClient { get; set; }
        public YoutubeExplode.Videos.Video playing { get; set; }
        public Queue<YoutubeExplode.Videos.Video> queue { get; set; }
        private CancellationTokenSource Cancel { get; set; }
        private CancellationTokenSource Skip { get; set; }
        private IMessageChannel channel { get; set; }

        public AudioClient(IAudioClient Client, IMessageChannel Channel)
        {
            audioClient = Client;
            playing = null;
            queue = new Queue<YoutubeExplode.Videos.Video>();
            Cancel = null;
            Skip = null;
            channel = Channel;
        }

        public void Stop()
        {
            try
            {
                Cancel?.Cancel();
                Skip?.Cancel();
                playing = null;
            }
            catch (Exception)
            {

            }
            
        }

        public void Next()
        {
            try
            {
                Skip?.Cancel();
                playing = null;
            }
            catch (Exception)
            {

            }
            
        }

        public async void Shuffle()
        {
            try
            {
                var Rand = new Random();
                var NewQueue = new Queue<YoutubeExplode.Videos.Video>();

                foreach (var Song in queue.ToArray().OrderBy(x => Rand.Next()))
                    NewQueue.Enqueue(Song);

                queue = NewQueue;

                await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue", $"shuffling done!"), Colors.information));
            }
            catch (Exception)
            {

            }
        }

        public async void Add(string[] input)
        {
            try
            {
                var youtube = new YoutubeClient();

                if (input.Length > 0)
                {
                    if (input.FirstOrDefault().StartsWith("https://www.youtube.com/watch?v="))
                    {
                        var video = await youtube.Videos.GetAsync(input.FirstOrDefault());

                        queue.Enqueue(video);

                        await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue added", $"[{video.Title}]({video.Url})\n{video.Duration}"), Colors.information));
                    }
                    else if (input.FirstOrDefault().StartsWith("https://www.youtube.com/playlist?list="))
                    {
                        var playlist = await youtube.Playlists.GetAsync(input.FirstOrDefault());

                        var videos = await youtube.Playlists.GetVideosAsync(playlist.Id).CollectAsync();

                        foreach (var video in videos)
                        {
                            queue.Enqueue(await youtube.Videos.GetAsync(video.Id));
                        }

                        await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue", $"added {videos.Count} songs"), Colors.information));
                    }
                    else
                    {
                        string searchString = "";

                        foreach (var item in input)
                        {
                            searchString = searchString + item + " ";
                        }

                        var search = await youtube.Search.GetVideosAsync(searchString).CollectAsync(1);
                        var video = await youtube.Videos.GetAsync(search.FirstOrDefault().Id);

                        queue.Enqueue(video);

                        await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue added", $"[{video.Title}]({video.Url})\n{video.Duration}"), Colors.information));
                    }
                }
            }
            catch(Exception)
            {

            }
        }

        public async Task Start()
        {
            try
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
                            await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("now playing", $"[{playing.Title}]({playing.Url})\n{playing.Duration}"), Colors.information));
                            await PlaySong(playing);
                        }

                        playing = null;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private async Task PlaySong(YoutubeExplode.Videos.Video input)
        {
            try
            {
                await audioClient.SetSpeakingAsync(true);

                YoutubeClient youtube = new YoutubeClient();

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(input.Id);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                var memoryStream = new MemoryStream();
                await Cli.Wrap("ffmpeg")
                    .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                    .WithStandardInputPipe(PipeSource.FromStream(stream))
                    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                    .ExecuteAsync();

                using (var discord = audioClient.CreatePCMStream(AudioApplication.Music))
                {
                    try { await discord.WriteAsync(memoryStream.ToArray(), 0, (int)memoryStream.Length, Skip.Token); }
                    finally { await discord.FlushAsync(); }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
