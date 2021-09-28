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
            else if (interaction.Data.Name == "get")
            {
                Log.Information($"command - /get - start user:{interaction.User.Id} channel:{interaction.Channel.Id} command: /get");
                try
                {
                    foreach (var item in interaction.Data.Options)
                    {
                        if (item.Name == "what")
                        {
                            if (item.Value.ToString() == "stats")
                            {
                                User user = User.GetById(interaction.User.Id);

                                if (user != null)
                                {
                                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                                    fields.Add(Field.CreateFieldBuilder(":+1: - upvotes", user.upvotes.ToString()));
                                    fields.Add(Field.CreateFieldBuilder(":-1: - downvotes", user.downvotes.ToString()));
                                    fields.Add(Field.CreateFieldBuilder(":notepad_spiral: - posts", user.posts.ToString()));

                                    if (user.karma != -1)
                                        fields.Add(Field.CreateFieldBuilder(":bar_chart: - karma", user.karma.ToString()));

                                    await interaction.RespondAsync(ephemeral: true, embed: Embed.New(interaction.User, fields, Colors.information, "stats"));
                                }
                                else
                                {
                                    await interaction.RespondAsync(ephemeral: true, embed: Embed.New(interaction.User, Field.CreateFieldBuilder("error", "User not found!"), Colors.error));
                                    throw new Exception("User not found!");
                                }
                            }
                            else if (item.Value.ToString() == "leaderboard")
                            {
                                await interaction.RespondAsync(ephemeral: true, embed: Embed.GetLeaderboard());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /get - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
            else if (interaction.Data.Name == "play")
            {
                List<string> input = new List<string>();

                if (interaction.Data.Options != null)
                {
                    foreach (var item in interaction.Data.Options)
                    {
                        if (item.Name == "what")
                        {
                            input.Add(item.Value.ToString());
                        }
                    }
                }

                try
                {
                    Log.Information($"command - /play - start user:{interaction.User.Id} channel:{interaction.Channel.Id} what:{input.ToString()}");

                    PlayMusic(interaction, input);

                     await interaction.RespondAsync(ephemeral: false, embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"playing!"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /play - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
            else if (interaction.Data.Name == "stop")
            {
                try
                {
                    Log.Information($"command - /stop - start user:{interaction.User.Id} channel:{interaction.Channel.Id}");

                    var channelVoice = (interaction.User as IVoiceState).VoiceChannel;
                    var channel = interaction.Channel as IGuildChannel;

                    if (channelVoice == null)
                        return;
                    else if (Audio.audioClients[channel.GuildId].voiceChannel != channelVoice)
                        return;

                    Audio.audioClients[channel.GuildId].Stop();
                    await interaction.RespondAsync(ephemeral: false, embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"stopped!"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /stop - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
            else if (interaction.Data.Name == "skip")
            {
                try
                {
                    Log.Information($"command - /skip - start user:{interaction.User.Id} channel:{interaction.Channel.Id}");

                    var channelVoice = (interaction.User as IVoiceState).VoiceChannel;
                    var channel = interaction.Channel as IGuildChannel;

                    if (channelVoice == null)
                        return;
                    else if (Audio.audioClients[channel.GuildId].voiceChannel != channelVoice)
                        return;

                    Audio.audioClients[channel.GuildId].Next();

                    await interaction.RespondAsync(ephemeral: false, embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"skipped!"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /skip - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
            else if (interaction.Data.Name == "leave")
            {
                try
                {
                    Log.Information($"command - /leave - start user:{interaction.User.Id} channel:{interaction.Channel.Id}");

                    var channelVoice = (interaction.User as IVoiceState).VoiceChannel;
                    var channel = interaction.Channel as IGuildChannel;

                    if (channelVoice == null)
                        return;
                    else if (Audio.audioClients[channel.GuildId].voiceChannel != channelVoice)
                        return;

                    Audio.audioClients[channel.GuildId].Stop();

                    Audio.audioClients[channel.GuildId].CleanUp();

                    //cleanup
                    if (Audio.audioClients.ContainsKey(channel.GuildId))
                    {
                        Audio.audioClients.Remove(channel.GuildId);
                    }

                    await interaction.RespondAsync(ephemeral: false, embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"left!"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /leave - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
            else if (interaction.Data.Name == "shuffle")
            {
                try
                {
                    Log.Information($"command - /shuffle - start user:{interaction.User.Id} channel:{interaction.Channel.Id}");

                    var channelVoice = (interaction.User as IVoiceState).VoiceChannel;
                    var channel = interaction.Channel as IGuildChannel;

                    if (channelVoice == null)
                        return;
                    else if (Audio.audioClients[channel.GuildId].voiceChannel != channelVoice)
                        return;

                    Audio.audioClients[channel.GuildId].Shuffle();

                    await interaction.RespondAsync(ephemeral: false, embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("audio", $"queue shuffled!"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /shuffle - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }
            else if (interaction.Data.Name == "queue")
            {
                try
                {
                    Log.Information($"command - /queue - start user:{interaction.User.Id} channel:{interaction.Channel.Id}");

                    var channelVoice = (interaction.User as IVoiceState).VoiceChannel;
                    var channel = interaction.Channel as IGuildChannel;

                    if (channelVoice == null)
                        return;
                    else if (Audio.audioClients[channel.GuildId].voiceChannel != channelVoice)
                        return;

                    string queueOutput = "";

                    if (Audio.audioClients[channel.GuildId].videoQueue != null)
                    {
                        foreach (var video in Audio.audioClients[channel.GuildId].videoQueue)
                        {
                            queueOutput = queueOutput + $"[{video.Title}]({video.Url})\n";
                        }
                    }

                    await interaction.RespondAsync(ephemeral: false, embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder("queue", $"next 5 songs:\n{queueOutput}total {Audio.audioClients[channel.GuildId].videoQueue.Count + Audio.audioClients[channel.GuildId].IdQueue.Count} songs"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - /queue - user:{interaction.User.Id} channel:{interaction.Channel.Id} error:{ex.Message}");
                }
            }

        }

        private static async Task PlayMusic(SocketSlashCommand interaction, List<string> input)
        {
            try
            {
                var channelVoice = (interaction.User as IVoiceState).VoiceChannel;
                var channel = interaction.Channel as IGuildChannel;

                //join if not connected
                if (!Audio.audioClients.ContainsKey(channel.GuildId))
                {
                    Audio.audioClients.Add(channel.GuildId, new AudioClient(await channelVoice.ConnectAsync(), interaction.Channel, channelVoice));
                }

                if (channelVoice == null)
                    return;
                else if (Audio.audioClients[channel.GuildId].voiceChannel != channelVoice)
                    return;

                Audio.audioClients[channel.GuildId].Add(input.ToArray());

                if (Audio.audioClients[channel.GuildId].playing == null)
                {
                    await Audio.audioClients[channel.GuildId].Start();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
