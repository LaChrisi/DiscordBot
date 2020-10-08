using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;

namespace DiscordBot.Core.Commands
{
    public class Get : ModuleBase<SocketCommandContext>
    {
        [Group("get"), Summary("Group to manage get commands")]
        public class GetGroup : ModuleBase<SocketCommandContext>
        {
            [Command("stats"), Alias("s", "stat"), Summary("returns your overall stats")]
            public async Task StatsModule(ulong UserID = 0)
            {
                try
                {
                    Log.Information($"command - get stats - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    if (UserID != 0 && Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        Classes.User user = User.GetById(UserID);

                        if (user != null)
                        {
                            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                            fields.Add(Field.CreateFieldBuilder(":+1: - upvotes", user.upvotes.ToString()));
                            fields.Add(Field.CreateFieldBuilder(":-1: - downvotes", user.downvotes.ToString()));
                            fields.Add(Field.CreateFieldBuilder(":notepad_spiral: - posts", user.posts.ToString()));

                            if (user.karma != -1)
                                fields.Add(Field.CreateFieldBuilder(":bar_chart: - karma", user.karma.ToString()));

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.GetUser(UserID), fields, Colors.information, "stats"));
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", "User not found!"), Colors.error));
                            throw new Exception("User not found!");
                        }
                    }
                    else if (UserID != 0 && !Privileg.CheckById(Context.User.Id, Privileg.admin))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                        Log.Warning($"command - get stats - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    }
                    else
                    {
                        User user = User.GetById(Context.Message.Author.Id);

                        if (user != null)
                        {
                            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                            fields.Add(Field.CreateFieldBuilder(":+1: - upvotes", user.upvotes.ToString()));
                            fields.Add(Field.CreateFieldBuilder(":-1: - downvotes", user.downvotes.ToString()));
                            fields.Add(Field.CreateFieldBuilder(":notepad_spiral: - posts", user.posts.ToString()));

                            if (user.karma != -1)
                                fields.Add(Field.CreateFieldBuilder(":bar_chart: - karma", user.karma.ToString()));

                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Colors.information, "stats"));
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", "User not found!"), Colors.error));
                            throw new Exception("User not found!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"command - get stats - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }
           
            [Command("reactions"), Alias("r", "reaction"), Summary("get reaction count command")]
            public async Task ReactionCountModule(ulong MessageID = 0)
            {
                try
                {
                    Log.Information($"command - get reactions - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    if (MessageID == 0)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("try", "!get reactions | reaction | r **<MessageID>**"), Colors.error, "error"));
                    }

                    var Message = await Context.Channel.GetMessageAsync(MessageID) as IUserMessage;
                    var Reactions = Message.Reactions;

                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                    foreach (var x in Reactions)
                    {
                        fields.Add(Field.CreateFieldBuilder(x.Key.Name, x.Value.ReactionCount.ToString()));
                    }
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Colors.information, "reactions"));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - get reactions - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Command("server"), Summary("get all server IDs")]
            public async Task ServerModule()
            {
                try
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        Log.Warning($"command - get server - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                        return;
                    }

                    Log.Information($"command - get server - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                    foreach (var x in Context.Client.Guilds)
                    {
                        fields.Add(Field.CreateFieldBuilder(x.Name, x.Id.ToString()));
                    }

                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Colors.information, "server"));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - get server - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Command("leaderboard"), Alias("lb"), Summary("get top 5 memers")]
            public async Task LeaderboardModule()
            {
                try
                {
                    Log.Information($"command - get leaderboard - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.GetLeaderboard());
                }
                catch (Exception ex)
                {
                    Log.Error($"command - get leaderboard - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }


        }
    }
}