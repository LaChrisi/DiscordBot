﻿using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using Discord.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Group("admin"), Summary("admin commands")]
        public class AdminGroup : ModuleBase<SocketCommandContext>
        {
            [Group("reset"), Summary("reset group")]
            public class ResetGroup : ModuleBase<SocketCommandContext>
            {
                [Command("roles")]
                public async Task ResetRolesModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.admin))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You need to be at least admin to use this command!"), Colors.warning));
                            Log.Warning($"command - admin reset roles - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - admin reset roles - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        //code

                        var guild = Context.Guild as IGuild;
                        var channel = Context.Channel as IGuildChannel;

                        List<IRole> exclutedRoles = new List<IRole>();

                        var Role = Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 573492166954057730));

                        Role = Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 778203675687387156));

                        Role = Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 356486959901835264));

                        Role = Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 780036990158110730));
                        
                        List<IRole> roles = new List<IRole>();

                        foreach (var role in channel.Guild.Roles)
                        {
                            if (!role.Permissions.Administrator || !exclutedRoles.Contains(role))
                            {
                                roles.Add(role);
                            }
                        }

                        List<IRole> newRoles = new List<IRole>();

                        foreach (var role in roles)
                        {
                            await role.DeleteAsync();
                            newRoles.Add(await guild.CreateRoleAsync(role.Name, role.Permissions, role.Color, role.IsHoisted, null));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - admin reset roles - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }

            [Command("leave"), SummaryAttribute("leaves the server")]
            public async Task LeaveModule()
            {
                try
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        Log.Warning($"command - admin leave - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                        return;
                    }

                    Log.Information($"command - admin leave - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                    await Context.Guild.LeaveAsync();
                }
                catch (Exception ex)
                {
                    Log.Error($"command - admin leave - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Group("timer"), Summary("timer group")]
            public class TimerGroup : ModuleBase<SocketCommandContext>
            {
                [Command("restart"), SummaryAttribute("restarts all timer")]
                public async Task RestartModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - admin timer restart - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - admin timer restart - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        Repetitive_Timer.SetUpDailyTimer(new TimeSpan(Convert.ToInt32(Global.GetByName("daily_timer_hour").value), 0, 0));
                        Repetitive_Timer.SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
                        Repetitive_Timer.SetUp5MinutesTimer(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + 5, 0));

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("info", "restart done!"), Colors.information));
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - admin timer restart - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("hour"), SummaryAttribute("uses the hourly event")]
                public async Task TimerHourModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - admin timer hour - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - admin timer hour - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        Repetitive_Timer.Hourly_timer_Elapsed(null, null);

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("info", "timer hour done!"), Colors.information));
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - admin timer hour - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("day"), SummaryAttribute("uses the daily event")]
                public async Task TimerDayModule()
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - admin timer day - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        Log.Information($"command - admin timer day - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                        Repetitive_Timer.Daily_timer_Elapsed(null, null);

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("info", "timer day done!"), Colors.information));
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - admin timer day - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }
            }

            [Command("add"), SummaryAttribute("adds a slash command")]
            public async Task AddModule(string name, ulong guildID = 0)
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - admin add - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                try
                {
                    List<SlashCommandBuilder> slashCommands = new List<SlashCommandBuilder>();

                    var command = new SlashCommandBuilder();

                    //help
                    {
                        command.WithName("help");
                        command.WithDescription("displays all bots commands");

                        command.AddOption("type", ApplicationCommandOptionType.String, "type of help", false, false, false, null, choices: new ApplicationCommandOptionChoiceProperties { Name = "data", Value = "data" });

                        slashCommands.Add(command);
                    }

                    //get
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("get");
                        command.WithDescription("get various things");

                        var options = new List<ApplicationCommandOptionChoiceProperties>();
                        options.Add(new ApplicationCommandOptionChoiceProperties { Name = "stats", Value = "stats" });
                        options.Add(new ApplicationCommandOptionChoiceProperties { Name = "leaderboard", Value = "leaderboard" });

                        command.AddOption("what", ApplicationCommandOptionType.String, "what to get", true, false, false, null, choices: options.ToArray());

                        slashCommands.Add(command);
                    }

                    //play
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("play");
                        command.WithDescription("play music from youtube");
                        command.AddOption("what", ApplicationCommandOptionType.String, "what to play", isRequired: false);

                        slashCommands.Add(command);
                    }

                    //stop
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("stop");
                        command.WithDescription("stop music");

                        slashCommands.Add(command);
                    }

                    //skip
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("skip");
                        command.WithDescription("skip music track");

                        slashCommands.Add(command);
                    }

                    //leave
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("leave");
                        command.WithDescription("leave the connected voice channel");

                        slashCommands.Add(command);
                    }

                    //shuffle
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("shuffle");
                        command.WithDescription("shuffle music queue");

                        slashCommands.Add(command);
                    }

                    //queue
                    {
                        command = new SlashCommandBuilder();
                        command.WithName("queue");
                        command.WithDescription("list the 5 next music tracks");

                        slashCommands.Add(command);
                    }

                    foreach (var slashCommand in slashCommands)
                    {
                        if (guildID != 0)
                        {
                            if (slashCommand.Name == name)
                            {
                                await Program.Client.Rest.CreateGuildCommand(slashCommand.Build(), guildID);
                            }
                        }
                        else
                        {
                            if (slashCommand.Name == name)
                            {
                                await Program.Client.Rest.CreateGlobalCommand(slashCommand.Build());
                            }
                        }
                    }

                    await Context.Message.ReplyAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("info", "done!"), Colors.information));
                }
                catch (ApplicationCommandException ex)
                {
                    var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                    await Context.Message.ReplyAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("error", json), Colors.error));
                }
                catch(Exception ex)
                {
                    Log.Error($"command - !admin get - user:{Context.Message.Author.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Command("get"), SummaryAttribute("gets all slash commands")]
            public async Task GetModule()
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - admin get - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                try
                {
                    var commands = await Program.Client.Rest.GetGlobalApplicationCommands();

                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                    string output = "";

                    foreach (var command in commands)
                    {
                        output = output + command.Name + "\n";
                    }

                    if (output != "")
                    {
                        fields.Add(Field.CreateFieldBuilder("global slash commands", output));
                    }
                    else
                    {
                        fields.Add(Field.CreateFieldBuilder("global slash commands", "NULL"));
                    }

                    foreach (var server in Program.Client.Guilds)
                    {
                        try
                        {
                            var commands2 = await Program.Client.Rest.GetGuildApplicationCommands(server.Id);

                            output = "";

                            foreach (var command in commands2)
                            {
                                output = output + command.Name + "\n";
                            }

                            if (output != "")
                            {
                                fields.Add(Field.CreateFieldBuilder(server.Name, output));
                            }
                            else
                            {
                                fields.Add(Field.CreateFieldBuilder(server.Name, "NULL"));
                            }
                        }
                        catch
                        { 

                        }
                    }

                    await Context.Message.ReplyAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - !admin get - user:{Context.Message.Author.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
    }

            [Command("delete"), SummaryAttribute("delete all global slash commands")]
            public async Task DeleteModule(ulong guildID = 0)
            {
                try
                {
                    if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                        Log.Warning($"command - admin delete - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                        return;
                    }

                    if (guildID == 0)
                    {
                        await Program.Client.Rest.DeleteAllGlobalCommandsAsync();
                    }
                    else
                    {
                        var guild = Program.Client.GetGuild(guildID);
                        await guild.DeleteApplicationCommandsAsync();
                    }

                    await Context.Message.ReplyAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("info", "done!"), Colors.information));
                }
                catch (Exception ex)
                {
                    Log.Error($"command - !admin delete - user:{Context.Message.Author.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Group("sex"), Summary("sex group")]
            public class SexModule : ModuleBase<SocketCommandContext>
            {
                [Command("get"), SummaryAttribute("get a row by id")]
                public async Task GetByRowID(int rowID = 0)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - admin sex get - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        if (rowID != 0 && rowID > 0)
                        {
                            SpreadSheetConnector google = new SpreadSheetConnector();

                            google.ConnectToGoogle();

                            Discord.Embed embed = null;

                            var item = google.GetRow(rowID);

                            if (item != null)
                            {
                                string notes = item.notes;

                                var split = notes.Split("!alt - ");
                                notes = $"{split[0]}\n*{split[1]}*";

                                if (item.who != "")
                                {
                                    if (item.who == "Nadine")
                                    {
                                        embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{item.who} ist durch {item.type} gekommen!", $"{notes}"), Colors.nadine, item.when, footer: $"{rowID - 1}");
                                    }
                                    else if (item.who == "Christoph")
                                    {
                                        embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{item.who} ist durch {item.type} gekommen!", $"{notes}"), Colors.christoph, item.when, footer: $"{rowID - 1}");
                                    }
                                    else if (item.who == "Christoph, Nadine")
                                    {
                                        embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"Wir sind beide richtig geil durch {item.type} gekommen!", $"{notes}"), Colors.rumpfi, item.when, footer: $"{rowID - 1}");
                                    }
                                }
                                else
                                {
                                    embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"Wir hatten {item.type}!", $"{notes}"), Colors.gray, item.when, footer: $"{rowID - 1}");
                                }
                            }

                            await Context.Channel.SendMessageAsync(embed: embed);
                        }
                        else
                        {
                            await Context.Message.ReplyAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("wrong syntax", "try\n!admin sex get <[rowID]>"), Colors.warning));
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - !admin sex get - user:{Context.Message.Author.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }

                [Command("getall"), SummaryAttribute("get a row by id")]
                public async Task GetAllRows(int rowID = 0)
                {
                    try
                    {
                        if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                        {
                            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                            Log.Warning($"command - admin sex get - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                            return;
                        }

                        SpreadSheetConnector google = new SpreadSheetConnector();

                        int i = 2;

                        if (rowID != 0)
                        {
                            i = rowID;
                        }

                        google.ConnectToGoogle();

                        while (true) 
                        {
                            var item = google.GetRow(i);
                            Discord.Embed embed = null;

                            if (item == null)
                                break;

                            string notes = item.notes;

                            var split = notes.Split("!alt - ");
                            notes = $"{split[0]}\n*{split[1]}*";

                            if (item.who != "")
                            {
                                if (item.who == "Nadine")
                                {
                                    embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{item.who} ist durch {item.type} gekommen!", $"{notes}"), Colors.nadine, item.when, footer: $"{i - 1}");
                                }
                                else if (item.who == "Christoph")
                                {
                                    embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{item.who} ist durch {item.type} gekommen!", $"{notes}"), Colors.christoph, item.when, footer: $"{i - 1}");
                                }
                                else if (item.who == "Christoph, Nadine")
                                {
                                    embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"Wir sind beide richtig geil durch {item.type} gekommen!", $"{notes}"), Colors.rumpfi, item.when, footer: $"{i - 1}");
                                }
                            }
                            else
                            {
                                embed = Classes.Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"Wir hatten {item.type}!", $"{notes}"), Colors.gray, item.when, footer: $"{i - 1}");
                            }

                            await Context.Channel.SendMessageAsync(embed: embed);

                            i++;
                        }

                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("admin sex getall", "Done Successfully!"), Colors.information));
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"command - !admin sex getall - user:{Context.Message.Author.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                    }
                }


            }
        }
    }
}