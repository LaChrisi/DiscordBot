using System;
using System.Collections.Generic;
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
                await Context.Channel.SendMessageAsync($"get stats | stat | s\nget reactions | reaction | r **<MessageID>**");
            }

            [Command("stats"), Alias("s", "stat"), Summary("returns your overall stats")]
            public async Task StatsModule(ulong UserID = 0)
            {
                if (UserID != 0 && Data.Privileg.CheckById(Context.User.Id, Data.Privileg.admin))
                {
                    Data.User user = Data.User.GetById(UserID);
                    if (user != null)
                        await Context.Channel.SendMessageAsync($"Stats for {Context.Client.GetUser(UserID).Mention}:\n:+1: - {user.upvotes}\n:-1: - {user.downvotes}\n in {user.posts} posts");
                    else
                        await Context.Channel.SendMessageAsync($"User not found");
                }
                else if(UserID != 0 && !Data.Privileg.CheckById(Context.User.Id, Data.Privileg.admin))
                    await Context.Channel.SendMessageAsync($":x: You need to be at least admin to use this command!");
                else
                {
                    Data.User user = Data.User.GetById(Context.Message.Author.Id);
                    if (user != null)
                        await Context.Channel.SendMessageAsync($"Stats for {Context.Client.GetUser(Context.Message.Author.Id).Mention}:\n:+1: - {user.upvotes}\n:-1: - {user.downvotes}\n in {user.posts} posts");
                    else
                        await Context.Channel.SendMessageAsync($"User not found");
                }
            }
           
            [Command("reactions"), Alias("r", "reaction"), Summary("get reaction count command")]
            public async Task ReactionCountModule(ulong MessageID = 0)
            {
                if (MessageID == 0)
                {
                    await Context.Channel.SendMessageAsync($"reaction **<MessageID>**");
                }

                var Message = await Context.Channel.GetMessageAsync(MessageID) as IUserMessage;
                var Reactions = Message.Reactions;

                foreach (var x in Reactions)
                {
                    await Context.Channel.SendMessageAsync($"{x.Key.Name} reaction count: {x.Value.ReactionCount}");
                }
            }

            [Command("server"), Summary("get all server IDs")]
            public async Task ServerModule()
            {
                if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(":x: You are not my god!");
                    return;
                }

                string ausgabe = "";

                foreach (var x in Context.Client.Guilds)
                {
                    ausgabe = ausgabe + x.Id + " - " + x.Name + "\n";
                }

                await Context.Channel.SendMessageAsync(ausgabe);
            }

        }
    }
}