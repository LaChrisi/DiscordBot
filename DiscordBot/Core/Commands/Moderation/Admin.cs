using Discord;
using Discord.Commands;
using DiscordBot.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                try
                {
                    List<SlashCommandBuilder> slashCommands = new List<SlashCommandBuilder>();

                    var command = new SlashCommandBuilder();
                    command.WithName("help");
                    command.WithDescription("displays all bots commands");

                    command.AddOption("type", ApplicationCommandOptionType.String, "type of help", false, false, true, null, new ApplicationCommandOptionChoiceProperties { Name = "data", Value = "data" });

                    slashCommands.Add(command);

                    command = new SlashCommandBuilder();
                    command.WithName("get");
                    command.WithDescription("get various things");

                    var options = new List<ApplicationCommandOptionChoiceProperties>();
                    options.Add(new ApplicationCommandOptionChoiceProperties { Name = "stats", Value = "stats" });
                    options.Add(new ApplicationCommandOptionChoiceProperties { Name = "leaderboard", Value = "leaderboard" });

                    command.AddOption("what", ApplicationCommandOptionType.String, "what to get", true, false, true, null, options.ToArray());

                    slashCommands.Add(command);

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
                catch (Exception ex)
                {
                    Log.Error($"command - !admin get - user:{Context.Message.Author.Id} channel:{Context.Channel.Id} error:{ex.Message}");
                }
            }

            [Command("get"), SummaryAttribute("gets all slash commands")]
            public async Task GetModule()
            {
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
        }
    }
}