using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using DiscordBot.Core.Data;

namespace DiscordBot.Core.Moderation
{
    public class DB : ModuleBase<SocketCommandContext>
    {
        [Command("version")]
        public async Task VersionModule()
        {
            await Context.Channel.SendMessageAsync("Version: " + Data.Data.Version());
        }

        [Group("user"), Summary("Database commands for user")]
        public class UserGroup : ModuleBase<SocketCommandContext>
        {
            [Command("add"), Alias("a"), Summary("adds to the database")]
            public async Task AddUserModule(ulong id, string name, int privileg, int posts = 0, int upvotes = 0, int downvotes = 0)
            {
                try
                {
                    User.Add(new User(id, name, privileg, posts, upvotes, downvotes));
                }
                catch (Exception e)
                {
                    await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                }
            }

            [Command("get"), Alias("g"), Summary("gets user by id")]
            public async Task GetUserModule(ulong id)
            {
                try
                {
                    var user = Data.User.GetById(id);
                    await Context.Channel.SendMessageAsync(user.WriteHead() + "\n" + user.Write());
                }
                catch (Exception e)
                {
                    await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                }
                
            }

            [Command("getall"), Alias("ga"), Summary("gets all user")]
            public async Task GetAllUserModule()
            {
                try
                {
                    var userList = User.GetAll();
                    string ausgabe = userList[0].WriteHead();

                    foreach (var user in userList)
                    {
                        ausgabe += "\n" + user.Write();
                    }
                    await Context.Channel.SendMessageAsync(ausgabe);
                }
                catch (Exception e)
                {
                    await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                }

            }

            [Command("delete"), Alias("d"), Summary("delete user by id")]
            public async Task DeleteUserModule(ulong id)
            {
                try
                {
                    User.DeleteById(id);
                }
                catch (Exception e)
                {
                    await Context.Channel.SendMessageAsync($"Error:\n{e.Message}");
                }

            }
        }
    }
}
