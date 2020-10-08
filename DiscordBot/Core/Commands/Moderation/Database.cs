using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Threading.Tasks;

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
                try
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        Log.Warning($"command - data version - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                        return;
                    }

                    Log.Information($"command - data version - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("version", Data.Version()), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - data version - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Group("user"), Summary("Database commands for user")]
            public class UserGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong id, string name, int privileg, int posts = 0, int upvotes = 0, int downvotes = 0, int karma = -1)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data user add - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data user add - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("user", $"rows affected: {User.Add(new User(id, name, privileg, posts, upvotes, downvotes, karma))}"), Colors.information));

                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data user add - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("get"), Alias("g"), Summary("get by id")]
                public async Task GetModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data user get - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data user get - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            var item = User.GetById(id);
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(User.header, item.ToString()), Colors.information, "user"));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data user get - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data user getall - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data user getall - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data user getall - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data user delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data user delete - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("user", $"rows affected: {User.DeleteById(id)}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data user delete - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int privileg, string name = "", int posts = 0, int upvotes = 0, int downvotes = 0, int karma = -1)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data user set - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data user set - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }

            [Group("vote"), Summary("Database commands for vote")]
            public class VoteGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(string name, string what, string how)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data vote add - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote add - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote", $"rows affected: {Vote.Add(new Vote(name, what, how))}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote add - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("get"), Alias("g"), Summary("get by id")]
                public async Task GetModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data vote get - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote get - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            var item = Vote.GetById(id);
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder(Vote.header, item.ToString()), Colors.information, "vote"));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote get - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("gets all")]
                public async Task GetAllModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data vote getall - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote getall - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote getall - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data vote delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote delete - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote", $"rows affected: {Vote.DeleteById(id)}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote delete - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, string name, string what, string how)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data vote set - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote set - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote set - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }

            [Group("vote_channel"), Alias("v_c"), Summary("Database commands for vote_channel")]
            public class Vote_ChannelGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong vote, ulong channel, int aktiv = 1)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data vote_channel add - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote_channel add - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote_channel", $"rows affected: {Vote_Channel.Add(new Vote_Channel(aktiv, vote, channel))}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote_channel add - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule(ulong id = 0)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data vote_channel getall - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote_channel getall - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                                throw e;
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
                                throw e;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote_channel getall - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data vote_channel delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote_channel delete - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote_channel", $"rows affected: {Vote_Channel.DeleteById(id)}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote_channel delete - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int aktiv, ulong vote = 0, ulong channel = 0)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data vote_channel set - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data vote_channel set - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            var item = Vote_Channel.GetById(id);
                            item.aktiv = aktiv;

                            if (vote != 0)
                                item.vote = vote;

                            if (channel != 0)
                                item.channel = channel;

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("vote_channel", $"rows affected: {Vote_Channel.Edit(item)}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data vote_channel set - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }

            [Group("event"), Summary("Database commands for event")]
            public class EventGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(string what, string how)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data event add - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data event add - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("event", $"rows affected: {Event.Add(new Event(what, how))}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data event add - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data event getall - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data event getall - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data event getall - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data event delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data event delete - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("event", $"rows affected: {Event.DeleteById(id)}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data event delete - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, string what, string how)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data event set - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data event set - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data event set - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }

            [Group("channel_event"), Alias("c_e"), Summary("Database commands for channel_event")]
            public class Channel_EventGroup : ModuleBase<SocketCommandContext>
            {
                [Command("add"), Alias("a"), Summary("add new")]
                public async Task AddModule(ulong channel, ulong Event, string when, char type, int aktiv = 1)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data channel_event add - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data channel_event add - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("channel_event", $"rows affected: {Channel_Event.Add(new Channel_Event(aktiv, channel, Event, when, type))}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data channel_event add - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("getall"), Alias("ga"), Summary("get all")]
                public async Task GetAllModule(ulong id = 0)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - data channel_event getall - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data channel_event getall - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                                throw e;
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
                                throw e;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data channel_event getall - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("delete"), Alias("d"), Summary("delete by id")]
                public async Task DeleteModule(ulong id)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data channel_event delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data channel_event delete - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        try
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("channel_event", $"rows affected: {Channel_Event.DeleteById(id)}"), Colors.information));
                        }
                        catch (Exception e)
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", e.Message), Colors.error));
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data channel_event delete - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("set"), Alias("s"), Summary("set by id")]
                public async Task SetModule(ulong id, int aktiv, ulong channel = 0, ulong Event = 0, string when = "", char type = ' ')
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - data channel_event set - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - data channel_event set - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

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
                            throw e;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - data channel_event set - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }


        }
    }
}
