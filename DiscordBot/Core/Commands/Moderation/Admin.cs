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
    public class Admin : ModuleBase<SocketCommandContext>
    {
        [Group("admin"), Summary("admin commands")]
        public class AdminGroup : ModuleBase<SocketCommandContext>
        {
            [Group("reset"), Summary("admin commands")]
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

                        var Role = Core.Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 573492166954057730));

                        Role = Core.Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 778203675687387156));

                        Role = Core.Classes.Role.GetById(6);
                        exclutedRoles.Add(channel.Guild.Roles.FirstOrDefault(x => x.Id == 356486959901835264));

                        Role = Core.Classes.Role.GetById(6);
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
        }
    }
}