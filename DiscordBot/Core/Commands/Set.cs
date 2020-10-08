using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands
{
    public class Set : ModuleBase<SocketCommandContext>
    {
        [Group("set"), Summary("Group to manage set commands")]
        public class GetGroup : ModuleBase<SocketCommandContext>
        {
            [Command("broadcast"), Alias("b"), Summary("sets current channel for broadcast")]
            public async Task SetBroadcastModule(int broadcast = 0)
            {
                try
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        Log.Warning($"command - set broadcast - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                        return;
                    }

                    Log.Information($"command - set broadcast - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    var channel = Channel.GetById(Context.Channel.Id);
                    channel.broadcast = broadcast;
                    Channel.Edit(channel);
                }
                catch (Exception ex)
                {
                    Log.Error($"command - set broadcast - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Command("leaderboard"), Alias("lb"), Summary("adds the current channel a renewing leaderboard")]
            public async Task SetLeaderboardModule()
            {
                try
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.moderator))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least moderator to use this command!"), Colors.warning));
                        Log.Warning($"command - set leaderboard - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                        return;
                    }

                    Log.Information($"command - set leaderboard - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    var channel_event_list = Channel_Event.GetAllByChannelIdAndType(Context.Channel.Id, 'r');

                    if (channel_event_list != null)
                    {
                        foreach (var channel_event in channel_event_list)
                        {
                            Channel_Event.DeleteById(channel_event.id);
                        }
                    }

                    var message = await Context.Channel.SendMessageAsync(embed: Classes.Embed.GetLeaderboard());

                    Channel_Event.Add(new Channel_Event(1, Context.Channel.Id, 15, $"{message.Id}", 'r'));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - set leaderboard - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }
        }
    }
}