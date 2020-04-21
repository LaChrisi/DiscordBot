using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBot.Core.Data;

namespace DiscordBot.Core.Moderation
{
    public class Database : ModuleBase<SocketCommandContext>
    {
        [Group("data"), Summary("Database commands")]
        public class DataGroup : ModuleBase<SocketCommandContext>
        {
            [Command("version")]
            public async Task VersionModule()
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                {
                    await Context.Channel.SendMessageAsync(":x: You need to be at least admin to use this command!");
                    return;
                }

                await Context.Channel.SendMessageAsync("Version: " + Data.Data.Version());
            }

            [Group("user"), Summary("Database commands for user")]
            public class UserGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong id, string name, int privileg, int posts = 0, int upvotes = 0, int downvotes = 0)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(":x: You are not my god!");
                        return;
                    }

                    try
                    {
                        User.Add(new User(id, name, privileg, posts, upvotes, downvotes));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }

                [Command("get"), Alias("g"), Summary("get by id")]
                public async Task GetModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(":x: You need to be at least admin to use this command!");
                        return;
                    }

                    try
                    {
                        var item = User.GetById(id);
                        await Context.Channel.SendMessageAsync(User.header + "\n" + item.ToString());
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule()
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(":x: You need to be at least admin to use this command!");
                        return;
                    }

                    try
                    {
                        var list = User.GetAll();
                        string ausgabe = User.header;

                        foreach (var item in list)
                        {
                            ausgabe += "\n" + item.ToString();
                        }

                        await Context.Channel.SendMessageAsync(ausgabe);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(":x: You are not my god!");
                        return;
                    }

                    try
                    {
                        User.DeleteById(id);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }

                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int privileg)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(":x: You are not my god!");
                        return;
                    }

                    try
                    {
                        var item = User.GetById(id);
                        item.privileg = privileg;
                        User.Edit(item);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }
            }

            [Group("vote"), Summary("Database commands for vote")]
            public class VoteGroup : ModuleBase<SocketCommandContext>
            {
                [Command("getall"), Alias("ga"), Summary("gets all")]
                public async Task GetAllModule()
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(":x: You need to be at least admin to use this command!");
                        return;
                    }

                    try
                    {
                        var list = Vote.GetAll();
                        string ausgabe = Vote.header;

                        foreach (var item in list)
                        {
                            ausgabe += "\n" + item.ToString();
                        }

                        await Context.Channel.SendMessageAsync(ausgabe);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }

                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(string name, string what, string how)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(":x: You are not my god!");
                        return;
                    }

                    try
                    {
                        Vote.Add(new Vote(name, what, how));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }
            }

            [Group("vote_channel"), Alias("v_c"), Summary("Database commands for vote_channel")]
            public class Vote_ChannelGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong vote, ulong channel, int aktiv = 1)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(":x: You are not my god!");
                        return;
                    }

                    try
                    {
                        Vote_Channel.Add(new Vote_Channel(aktiv, vote, channel));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule(ulong id = 0)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(":x: You need to be at least admin to use this command!");
                        return;
                    }

                    if (id == 0)
                    {
                        try
                        {
                            var list = Vote_Channel.GetAll();
                            string ausgabe = Vote_Channel.header;

                            foreach (var item in list)
                            {
                                ausgabe += "\n" + item.ToString();
                            }

                            await Context.Channel.SendMessageAsync(ausgabe);
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            var list = Vote_Channel.GetAllByChannelId(id);
                            string ausgabe = Vote_Channel.header;

                            foreach (var item in list)
                            {
                                ausgabe += "\n" + item.ToString();
                            }

                            await Context.Channel.SendMessageAsync(ausgabe);
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                        }
                    }
                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int aktiv, ulong vote = 0, ulong channel = 0)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(":x: You are not my god!");
                        return;
                    }

                    try
                    {
                        var item = Vote_Channel.GetById(id);
                        item.aktiv = aktiv;

                        if(vote != 0)
                            item.vote = vote;

                        if(channel != 0)
                            item.channel = channel;

                        Vote_Channel.Edit(item);
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                    }
                }
            }
        }
    }
}
