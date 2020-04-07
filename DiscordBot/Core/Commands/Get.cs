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
                if (UserID != 0 && Context.User.Id == Data.UserIDs.LaChrisi)
                    await Context.Channel.SendMessageAsync($"{Context.Client.GetUser(UserID).Mention} you have\n:+1: {Data.Data.GetLikes(UserID)} likes\n:-1: {Data.Data.GetDislikes(UserID)} dislikes\n:notepad_spiral: {Data.Data.GetPosts(UserID)} posts");
                else if(UserID != 0 && !(Context.User.Id == Data.UserIDs.LaChrisi))
                    await Context.Channel.SendMessageAsync($":x: You are not my god!");
                else
                    await Context.Channel.SendMessageAsync($"{Context.User.Mention} you have\n:+1: {Data.Data.GetLikes(Context.User.Id)} likes\n:-1: {Data.Data.GetDislikes(Context.User.Id)} dislikes\n:notepad_spiral: {Data.Data.GetPosts(Context.User.Id)} posts");
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
                if (!(Context.User.Id == Data.UserIDs.LaChrisi))
                {
                    await Context.Channel.SendMessageAsync(":x: You are not my god!");
                    return;
                }

                String ausgabe = "";

                foreach (var x in Context.Client.Guilds)
                {
                    ausgabe = ausgabe + x.Id + " - " + x.Name + "\n";
                }

                await Context.Channel.SendMessageAsync(ausgabe);
            }

        }
    }
}