using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBot.Core.Classes;

namespace DiscordBot.Core.Commands.Moderation
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
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                    return;
                }

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("version", Data.Version()), Colors.information));
            }

            [Group("user"), Summary("Database commands for user")]
            public class UserGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong id, string name, int privileg, int posts = 0, int upvotes = 0, int downvotes = 0, int karma = -1)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("user", $"rows affected: {User.Add(new User(id, name, privileg, posts, upvotes, downvotes, karma))}"), Colors.information));
                        
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("get"), Alias("g"), Summary("get by id")]
                public async Task GetModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var item = User.GetById(id);
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(User.header, item.ToString()), Colors.information, "user"));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule()
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var list = User.GetAll();
                        string ausgabe = "";

                        foreach (var item in list)
                        {
                            ausgabe += item.ToString() + "\n";
                        }

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(User.header, ausgabe), Colors.information, "user"));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("user", $"rows affected: {User.DeleteById(id)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }

                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int privileg, string name = "", int posts = 0, int upvotes = 0, int downvotes = 0, int karma = -1)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var item = User.GetById(id);

                        item.privileg = privileg;

                        if (name != "")
                            item.name = name;

                        if (posts != 0)
                            item.posts = posts;

                        if (upvotes != 0)
                            item.upvotes = upvotes;

                        if (downvotes != 0)
                            item.downvotes = downvotes;

                        if (karma != -1)
                            item.karma = karma;

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("user", $"rows affected: {User.Edit(item)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }
            }

            [Group("vote"), Summary("Database commands for vote")]
            public class VoteGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(string name, string what, string how)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote", $"rows affected: {Vote.Add(new Vote(name, what, how))}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("get"), Alias("g"), Summary("get by id")]
                public async Task GetModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var item = Vote.GetById(id);
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Vote.header, item.ToString()), Colors.information, "vote"));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("getall"), Alias("ga"), Summary("gets all")]
                public async Task GetAllModule()
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var list = Vote.GetAll();
                        string ausgabe = "";

                        foreach (var item in list)
                        {
                            ausgabe += item.ToString() + "\n";
                        }

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Vote.header, ausgabe), Colors.information, "vote"));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));

                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }
                    
                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote", $"rows affected: {Vote.DeleteById(id)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }

                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, string name, string what, string how)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var item = Vote.GetById(id);
                        item.name = name;
                        item.what = what;
                        item.how = how;
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote", $"rows affected: {Vote.Edit(item)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
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
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote_channel", $"rows affected: {Vote_Channel.Add(new Vote_Channel(aktiv, vote, channel))}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule(ulong id = 0)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    if (id == 0)
                    {
                        try
                        {
                            var list = Vote_Channel.GetAll();
                            string ausgabe = "";

                            foreach (var item in list)
                            {
                                ausgabe += item.ToString() + "\n";
                            }

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Vote_Channel.header, ausgabe), Colors.information, "vote_channel"));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                        }
                    }
                    else
                    {
                        try
                        {
                            var list = Vote_Channel.GetAllByChannelId(id);
                            string ausgabe = "";

                            foreach (var item in list)
                            {
                                ausgabe += item.ToString() + "\n";
                            }

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Vote_Channel.header, ausgabe), Colors.information, "vote_channel"));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                        }
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote_channel", $"rows affected: {Vote_Channel.DeleteById(id)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }

                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int aktiv, ulong vote = 0, ulong channel = 0)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
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

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote_channel", $"rows affected: {Vote_Channel.Edit(item)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }
            }

            [Group("event"), Summary("Database commands for event")]
            public class EventGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(string what, string how)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("event", $"rows affected: {Event.Add(new Event(what, how))}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule()
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var list = Event.GetAll();
                        string ausgabe = "";

                        foreach (var item in list)
                        {
                            ausgabe += item.ToString() + "\n";
                        }

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Event.header, ausgabe), Colors.information, "event"));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("event", $"rows affected: {Event.DeleteById(id)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }

                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, string what, string how)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var item = Event.GetById(id);
                        item.what = what;
                        item.how = how;

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("event", $"rows affected: {Event.Edit(item)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }
            }

            [Group("channel_event"), Alias("c_e"), Summary("Database commands for channel_event")]
            public class Channel_EventGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong channel, ulong Event, string when, char type, int aktiv = 1)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("channel_event", $"rows affected: {Channel_Event.Add(new Channel_Event(aktiv, channel, Event, when, type))}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule(ulong id = 0)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        return;
                    }

                    if (id == 0)
                    {
                        try
                        {
                            var list = Channel_Event.GetAll();
                            string ausgabe = "";

                            foreach (var item in list)
                            {
                                ausgabe += item.ToString() + "\n";
                            }

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Channel_Event.header, ausgabe), Colors.information, "channel_event"));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                        }
                    }
                    else
                    {
                        try
                        {
                            var list = Channel_Event.GetAllByChannelId(id);
                            string ausgabe = "";

                            foreach (var item in list)
                            {
                                ausgabe += item.ToString() + "\n";
                            }

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Channel_Event.header, ausgabe), Colors.information, "channel_event"));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                        }
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("channel_event", $"rows affected: {Channel_Event.DeleteById(id)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }

                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int aktiv, ulong channel = 0, ulong Event = 0, string when = "", char type = ' ')
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        return;
                    }

                    try
                    {
                        var item = Channel_Event.GetById(id);
                        item.aktiv = aktiv;

                        if (channel != 0)
                            item.channel = channel;

                        if (Event != 0)
                            item.Event = Event;

                        if (when != "")
                            item.when = when;

                        if (type != ' ')
                            item.type = type;

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("channel_event", $"rows affected: {Channel_Event.Edit(item)}"), Colors.information));
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                    }
                }
            }


        }
    }
}
