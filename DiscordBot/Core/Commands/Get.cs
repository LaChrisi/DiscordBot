using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace DiscordBot.Core.Commands
{
    public class Get : ModuleBase<SocketCommandContext>
    {
        [Group("get"), Summary("Group to manage get commands")]
        public class GetGroup : ModuleBase<SocketCommandContext>
        {
            [Command("help"), Alias("", "h", "-h"), Summary("help for get commands")]
            public async Task HelpModule()
            {
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                //user
                fields.Add(Data.Field.CreateFieldBuilder("hallo", "!hallo"));
                fields.Add(Data.Field.CreateFieldBuilder("get", "!get stats | stat| s" + "\n" + "!get reactions | reaction | r **<MessageID>**"));

                var user = Data.User.GetById(Context.User.Id);

                if (user != null)
                {
                    if (user.privileg >= Data.Privileg.moderator)
                    {
                        //moderator

                        if (user.privileg >= Data.Privileg.admin)
                        {
                            //admin


                            if (user.privileg >= Data.Privileg.owner)
                            {
                                //owner

                            }
                        }
                    }

                    var privileg = Data.Privileg.GetById((ulong)user.privileg);
                    await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, fields, Data.Colors.information, "help", $"for {privileg.name}"));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, fields, Data.Colors.information, "help - user"));
                }
            }

            [Command("stats"), Alias("s", "stat"), Summary("returns your overall stats")]
            public async Task StatsModule(ulong UserID = 0)
            {
                if (UserID != 0 && Data.Privileg.CheckById(Context.User.Id, Data.Privileg.admin))
                {
                    Data.User user = Data.User.GetById(UserID);

                    if (user != null)
                    {
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                        fields.Add(Data.Field.CreateFieldBuilder(":+1: - upvotes", user.upvotes.ToString()));
                        fields.Add(Data.Field.CreateFieldBuilder(":-1: - downvotes", user.downvotes.ToString()));
                        fields.Add(Data.Field.CreateFieldBuilder(":notepad_spiral: - posts", user.posts.ToString()));

                        await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Client.GetUser(UserID), fields, Data.Colors.information, "stats"));
                    }
                    else
                        await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("error", "User not found!"), Data.Colors.error));
                }
                else if(UserID != 0 && !Data.Privileg.CheckById(Context.User.Id, Data.Privileg.admin))
                    await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Data.Colors.warning));
                else
                {
                    Data.User user = Data.User.GetById(Context.Message.Author.Id);

                    if (user != null)
                    {
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                        fields.Add(Data.Field.CreateFieldBuilder(":+1: - upvotes", user.upvotes.ToString()));
                        fields.Add(Data.Field.CreateFieldBuilder(":-1: - downvotes", user.downvotes.ToString()));
                        fields.Add(Data.Field.CreateFieldBuilder(":notepad_spiral: - posts", user.posts.ToString()));

                        await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, fields, Data.Colors.information, "stats"));
                    }
                    else
                        await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("error", "User not found!"), Data.Colors.error));
                }
            }
           
            [Command("reactions"), Alias("r", "reaction"), Summary("get reaction count command")]
            public async Task ReactionCountModule(ulong MessageID = 0)
            {
                if (MessageID == 0)
                {
                    await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("try", "!get reactions | reaction | r **<MessageID>**"), Data.Colors.error, "error"));
                }

                var Message = await Context.Channel.GetMessageAsync(MessageID) as IUserMessage;
                var Reactions = Message.Reactions;

                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                foreach (var x in Reactions)
                {
                    fields.Add(Data.Field.CreateFieldBuilder(x.Key.Name, x.Value.ReactionCount.ToString()));
                }
                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, fields, Data.Colors.information, "reactions"));
            }

            [Command("server"), Summary("get all server IDs")]
            public async Task ServerModule()
            {
                if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, Data.Field.CreateFieldBuilder("warning", "You are not my god!"), Data.Colors.warning));
                    return;
                }

                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                foreach (var x in Context.Client.Guilds)
                {
                    fields.Add(Data.Field.CreateFieldBuilder(x.Name, x.Id.ToString()));
                }

                await Context.Channel.SendMessageAsync(embed: Data.Embed.New(Context.Message.Author, fields, Data.Colors.information, "server"));
            }

        }
    }
}