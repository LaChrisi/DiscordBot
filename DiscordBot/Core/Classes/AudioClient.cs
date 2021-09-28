using Discord;
using System;
using System.Threading.Tasks;
using Discord.Audio;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
using System.IO;
using CliWrap;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace DiscordBot.Core.Classes
{
    class AudioClient
    {
        public IAudioClient audioClient { get; set; }
        public YoutubeExplode.Videos.Video playing { get; set; }
        public Queue<YoutubeExplode.Videos.Video> videoQueue { get; set; }
        public Queue<string> IdQueue { get; set; }
        public IVoiceChannel voiceChannel { get; set; }
        private CancellationTokenSource Cancel { get; set; }
        private CancellationTokenSource Skip { get; set; }
        private IMessageChannel channel { get; set; }
        private YoutubeClient youtube { get; set; }

        public AudioClient(IAudioClient Client, IMessageChannel Channel, IVoiceChannel VoiceChannel)
        {
            audioClient = Client;
            playing = null;
            videoQueue = new Queue<YoutubeExplode.Videos.Video>();
            IdQueue = new Queue<string>();
            Cancel = null;
            Skip = null;
            channel = Channel;
            youtube = new YoutubeClient();
            voiceChannel = VoiceChannel;
        }

        public async void CleanUp()
        {
            await audioClient.StopAsync();
            audioClient = null;
            videoQueue = null;
            IdQueue = null;
            channel = null;
            youtube = null;
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
                Queue<string> allQueue = IdQueue;

                foreach (var item in videoQueue)
                {
                    allQueue.Enqueue(item.Id);
                }

                videoQueue = new Queue<YoutubeExplode.Videos.Video>();

                var Rand = new Random();
                var NewQueue = new Queue<string>();

                foreach (var Song in allQueue.ToArray().OrderBy(x => Rand.Next()))
                    NewQueue.Enqueue(Song);

                IdQueue = NewQueue;

                for (int i = 0; i < 5; i++)
                {
                    videoQueue.Enqueue(await youtube.Videos.GetAsync(IdQueue.Dequeue()));
                }
            }
            catch (Exception)
            {

            }
        }

        public async void Add(string[] input)
        {
            try
            {
                if (input.Length > 0)
                {
                    if (input.FirstOrDefault().StartsWith("https://www.youtube.com/watch?v="))
                    {
                        var video = await youtube.Videos.GetAsync(input.FirstOrDefault());

                        if (videoQueue.Count < 5)
                        {
                            videoQueue.Enqueue(video);
                        }
                        else
                        {
                            IdQueue.Enqueue(video.Id);
                        }

                        await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue added", $"[{video.Title}]({video.Url})\n{video.Duration}"), Colors.information));
                    }
                    else if (input.FirstOrDefault().StartsWith("https://www.youtube.com/playlist?list="))
                    {
                        var playlist = await youtube.Playlists.GetAsync(input.FirstOrDefault());

                        var videos = await youtube.Playlists.GetVideosAsync(playlist.Id).CollectAsync();

                        foreach (var video in videos)
                        {
                            if (videoQueue.Count < 5)
                            {
                                videoQueue.Enqueue(await youtube.Videos.GetAsync(video.Id));
                            }
                            else
                            {
                                IdQueue.Enqueue(video.Id);
                            }
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

                        if (videoQueue.Count < 5)
                        {
                            videoQueue.Enqueue(video);
                        }
                        else
                        {
                            IdQueue.Enqueue(video.Id);
                        }

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

                        while (videoQueue.Count == 0 && !Cancel.IsCancellationRequested)
                            await Task.Delay(10);

                        playing = videoQueue.Dequeue();

                        while (videoQueue.Count < 5)
                        {
                            if (IdQueue.Count > 0)
                            {
                                videoQueue.Enqueue(await youtube.Videos.GetAsync(IdQueue.Dequeue()));
                            }
                            else
                            {
                                break;
                            }
                        }

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

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(input.Id, Skip.Token);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo, Skip.Token);

                var memoryStream = new MemoryStream();
                await Cli.Wrap("ffmpeg")
                    .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
                    .WithStandardInputPipe(PipeSource.FromStream(stream))
                    .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
                    .ExecuteAsync(Skip.Token);

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
