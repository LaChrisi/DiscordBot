using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands
{
    class Help
    {
        public class Get : ModuleBase<SocketCommandContext>
        {
            [Command("help"), Alias("h", "-h"), Summary("help for commands")]
            public async Task HelpModule()
            {
                try
                {
                    Log.Information($"command - help - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                    //user
                    fields.Add(Classes.Field.CreateFieldBuilder("hallo",
                        "!hallo"));
                    fields.Add(Classes.Field.CreateFieldBuilder("get",
                        "!get stats | stat| s" + "\n" +
                        "!get reactions | reaction | r **<MessageID>**"));



                    var user = Classes.User.GetById(Context.User.Id);

                    if (user != null)
                    {
                        if (user.privileg >= Classes.Privileg.moderator)
                        {
                            //moderator



                            if (user.privileg >= Classes.Privileg.admin)
                            {
                                //admin
                                fields.Add(Classes.Field.CreateFieldBuilder("check",
                                    "!check **<ChannelID>** **<StartMessageID>**"));
                                fields.Add(Classes.Field.CreateFieldBuilder("move",
                                    "!move **<MessageID>** **<ChannelID>**"));
                                fields.Add(Classes.Field.CreateFieldBuilder("setvote",
                                    "!setvote **<Number>** **[<ChannelID>** **<MessageID>]**"));
                                fields.Add(Classes.Field.CreateFieldBuilder("setvoteall",
                                    "!setvoteall **[<ServerID>** **<MessageID>]**"));
                                fields.Add(Classes.Field.CreateFieldBuilder("version",
                                    "!version"));
                                fields.Add(Classes.Field.CreateFieldBuilder("data user",
                                    "!data user get | g **<UserID>**" + "\n" +
                                    "!data user getall | ga"));
                                fields.Add(Classes.Field.CreateFieldBuilder("data vote",
                                    "!data vote get | g **<VoteID>**" + "\n" +
                                    "!data vote getall | ga"));
                                fields.Add(Classes.Field.CreateFieldBuilder("data vote_channel",
                                    "!data vote_channel | v_c getall | ga **[<ChannelID>]**"));
                                fields.Add(Classes.Field.CreateFieldBuilder("data event",
                                    "!data event getall | ga"));
                                fields.Add(Classes.Field.CreateFieldBuilder("data channel_event",
                                    "!data channel_event | c_e getall | ga **[<ChannelID>]**"));



                                if (user.privileg >= Classes.Privileg.owner)
                                {
                                    //owner
                                    fields.Add(Classes.Field.CreateFieldBuilder("get",
                                        "!get server"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("backdoor",
                                        "!backdoor **<ServerID>**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("say",
                                        "!say **<ChannelID>** **\"<What-to-Say>\"**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("delete",
                                        "!delete **<ChannelID>** **<MessageID>**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("status",
                                        "!status **<play | watch | listen>** **<\"Status-Message\">**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("data user",
                                        "!data user add | a **<UserID>** **<Name>** **<Privileg>** **[<Posts>]** **[<Upvotes>]** **[<Downvotes>]** **[<Karma>]**" + "\n" +
                                        "!data user delete | d **<UserID>**" + "\n" +
                                        "!data user set | s **<UserID>** **<Privileg>** **[<Name>]** **[<Posts>]** **[<Upvotes>]** **[<Downvotes>]** **[<Karma>]**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("data vote",
                                        "!data vote add | a **<Name>** **<\"What\">** **<\"How\">**" + "\n" +
                                        "!data vote delete | d **<VoteID>**" + "\n" +
                                        "!vote set | s **<VoteID>** **<Name>** **<\"What\">** **<\"How\">**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("data vote_channel",
                                        "!data vote_channel | v_c add | a **<VoteID>** **<ChannelID>** **[<Aktiv>]**" + "\n" +
                                        "!data vote_channel | v_c delete | d **<Vote_ChannelID>**" + "\n" +
                                        "!data vote_channel | v_c set | s **<Vote_ChannelID>** **<Aktiv>** **[<VoteID>]** **[<ChannelID>]**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("data event",
                                        "!data event add | a **<\"What\">** **<\"How\">**" + "\n" +
                                        "!dataevent delete | d **<EventID>**" + "\n" +
                                        "!data event set | s **<EventID>** **<\"What\">** **<\"How\">**"));
                                    fields.Add(Classes.Field.CreateFieldBuilder("data channel_event",
                                        "!data channel_event | c_e add | a **<ChannelID>** **<EventID>** **<\"When\">** **<Type>** **[<Aktiv>]**" + "\n" +
                                        "!data channel_event | c_e delete | d **<Channel_EventID>**" + "\n" +
                                        "!data channel_event | c_e set | s **<Channel_EventID>** **<Aktiv>** **[<ChannelID>]** **[<EventID>]** **[<\"When\">]** **[<Type>]**"));


                                }
                            }
                        }

                        var privileg = Classes.Privileg.GetById((ulong)user.privileg);
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Classes.Colors.information, "help", $"for {privileg.name}"));
                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Classes.Colors.information, "help - user"));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"command - help - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }
        }
    }
}
