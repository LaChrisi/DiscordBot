using Discord.WebSocket;
using System.Threading.Tasks;
using DiscordBot.Core.Commands;
using System;
using Discord;
using System.Collections.Generic;

namespace DiscordBot.Core.Classes
{
    public class SlashCommands
    {
        public static async Task SlashCommandHandler(SocketSlashCommand interaction)
        {
            if (interaction.Data.Name == "help")
            {
                Log.Information($"command - /help - start user:{interaction.User.Id} channel:{interaction.Channel.Id} command: /help");
                try
                {
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                    var user = User.GetById(interaction.User.Id);

                    if (interaction.Data.Options == null)
                    {
                        //user
                        fields.Add(Field.CreateFieldBuilder("hallo",
                            "!hallo"));
                        fields.Add(Field.CreateFieldBuilder("get",
                            "!get stats | stat| s" + "\n" +
                            "!get reactions | reaction | r **<MessageID>**"));

                        if (user != null)
                        {
                            if (user.privileg >= Privileg.moderator)
                            {
                                //moderator



                                if (user.privileg >= Privileg.admin)
                                {
                                    //admin
                                    fields.Add(Field.CreateFieldBuilder("check",
                                        "!check **<ChannelID>** **<StartMessageID>**"));
                                    fields.Add(Field.CreateFieldBuilder("move",
                                        "!move **<MessageID>** **<ChannelID>**"));
                                    fields.Add(Field.CreateFieldBuilder("setvote",
                                        "!setvote **<Number>** **[<ChannelID>** **<MessageID>]**"));
                                    fields.Add(Field.CreateFieldBuilder("setvoteall",
                                        "!setvoteall **[<ServerID>** **<MessageID>]**"));
                                    fields.Add(Field.CreateFieldBuilder("version",
                                        "!version"));
                                    fields.Add(Field.CreateFieldBuilder("admin reset roles",
                                        "!admin reset roles - **RESETS ALL ROLES EXCPECT ADMIN** - back to the rules"));
                                    fields.Add(Field.CreateFieldBuilder("say",
                                        "!say **<ChannelID>** **\"<What-to-Say>\"**"));
                                    fields.Add(Field.CreateFieldBuilder("delete",
                                        "!delete **<ChannelID>** **<MessageID>**"));
                                    fields.Add(Field.CreateFieldBuilder("edit",
                                        "!delete **<ChannelID>** **<MessageID>** **<What-to-Say>**"));

                                    if (user.privileg >= Privileg.owner)
                                    {
                                        //owner
                                        fields.Add(Field.CreateFieldBuilder("get",
                                            "!get server"));
                                        fields.Add(Field.CreateFieldBuilder("backdoor",
                                            "!backdoor **<ServerID>**"));
                                        fields.Add(Field.CreateFieldBuilder("status",
                                            "!status **<play | watch | listen>** **<\"Status-Message\">**"));
                                    }
                                }
                            }

                            var privileg = Privileg.GetById((ulong)user.privileg);
                            await interaction.RespondAsync(ephemeral: true, embed: Embed.New(interaction.User, fields, Colors.information, "help", $"for {privileg.name}"));
                        }
                        else
                        {
                            await interaction.RespondAsync(ephemeral: true, embed: Embed.New(interaction.User, fields, Colors.information, "help - user"));
                        }
                    }
                    else
                    {
                        foreach (var item in interaction.Data.Options)
                        {
                            if (item.Name == "type")
                            {
                                if (item.Value.ToString() == "data")
                                {
                                    if (user != null)
                                    {
                                        if (user.privileg >= Privileg.moderator)
                                        {
                                            //moderator



                                            if (user.privileg >= Privileg.admin)
                                            {
                                                //admin
                                                fields.Add(Field.CreateFieldBuilder("data user",
                                                    "!data user get | g **<UserID>**" + "\n" +
                                                    "!data user getall | ga"));
                                                fields.Add(Field.CreateFieldBuilder("data vote",
                                                    "!data vote get | g **<VoteID>**" + "\n" +
                                                    "!data vote getall | ga"));
                                                fields.Add(Field.CreateFieldBuilder("data vote_channel",
                                                    "!data vote_channel | v_c getall | ga **[<ChannelID>]**"));
                                                fields.Add(Field.CreateFieldBuilder("data event",
                                                    "!data event getall | ga"));
                                                fields.Add(Field.CreateFieldBuilder("data channel_event",
                                                    "!data channel_event | c_e getall | ga **[<ChannelID>]**"));

                                                if (user.privileg >= Privileg.owner)
                                                {
                                                    //owner
                                                    fields.Add(Field.CreateFieldBuilder("data user",
                                                        "!data user add | a **<UserID>** **<Name>** **<Privileg>** **[<Posts>]** **[<Upvotes>]** **[<Downvotes>]** **[<Karma>]**" + "\n" +
                                                        "!data user delete | d **<UserID>**" + "\n" +
                                                        "!data user set | s **<UserID>** **<Privileg>** **[<Name>]** **[<Posts>]** **[<Upvotes>]** **[<Downvotes>]** **[<Karma>]**"));
                                                    fields.Add(Field.CreateFieldBuilder("data vote",
                                                        "!data vote add | a **<Name>** **<\"What\">** **<\"How\">**" + "\n" +
                                                        "!data vote delete | d **<VoteID>**" + "\n" +
                                                        "!data vote set | s **<VoteID>** **<Name>** **<\"What\">** **<\"How\">**"));
                                                    fields.Add(Field.CreateFieldBuilder("data vote_channel",
                                                        "!data vote_channel | v_c add | a **<VoteID>** **<ChannelID>** **[<Aktiv>]**" + "\n" +
                                                        "!data vote_channel | v_c delete | d **<Vote_ChannelID>**" + "\n" +
                                                        "!data vote_channel | v_c set | s **<Vote_ChannelID>** **<Aktiv>** **[<VoteID>]** **[<ChannelID>]**"));
                                                    fields.Add(Field.CreateFieldBuilder("data event",
                                                        "!data event add | a **<\"What\">** **<\"How\">**" + "\n" +
                                                        "!data event delete | d **<EventID>**" + "\n" +
                                                        "!data event set | s **<EventID>** **<\"What\">** **<\"How\">**"));
                                                    fields.Add(Field.CreateFieldBuilder("data channel_event",
                                                        "!data channel_event | c_e add | a **<ChannelID>** **<EventID>** **<\"When\">** **<Type>** **[<Aktiv>]**" + "\n" +
                                                        "!data channel_event | c_e delete | d **<Channel_EventID>**" + "\n" +
                                                        "!data channel_event | c_e set | s **<Channel_EventID>** **<Aktiv>** **[<ChannelID>]** **[<EventID>]** **[<\"When\">]** **[<Type>]**"));
                                                }
                                            }
                                        }

                                        var privileg = Privileg.GetById((ulong)user.privileg);
                                        await interaction.RespondAsync(ephemeral: true, embed: Embed.New(interaction.User, fields, Colors.information, "help", $"for {privileg.name}"));
                                    }
                                    else
                                    {
                                        fields.Add(Field.CreateFieldBuilder("information", "no data commands for you - sorry"));
                                        await interaction.RespondAsync(ephemeral: true, embed: Embed.New(interaction.User, fields, Colors.information, "help - user"));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /help - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
        }
    }
}
