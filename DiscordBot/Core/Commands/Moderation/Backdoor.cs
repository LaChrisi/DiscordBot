using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Classes;

namespace DiscordBot.Core.Commands.Moderation
{
    public class Backdoor : ModuleBase<SocketCommandContext>
    {
        [Command("backdoor"), SummaryAttribute("Server invite erstellen lassen")]
        public async Task BackdoorModule(ulong GuildId)
        {
            try
            {
                if (!Classes.Privileg.CheckById(Context.User.Id, Classes.Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("warning", "You are not my god!"), Classes.Colors.warning));
                    Log.Warning($"command - backdoor - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - backdoor - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                if (Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1)
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder("error", "guild not found!"), Classes.Colors.error));
                    return;
                }

                SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
                var invites = await Guild.GetInvitesAsync();

                if (invites.Count() < 1)
                {
                    try
                    {
                        await Guild.SystemChannel.CreateInviteAsync();
                    }
                    catch (Exception e)
                    {
                        await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Classes.Field.CreateFieldBuilder($"Creating an invit for guild {Guild.Name} went wrong", e.Message), Classes.Colors.error, "error"));
                        throw e;
                    }
                }

                invites = null;
                invites = await Guild.GetInvitesAsync();

                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                foreach (var invite in invites)
                {
                    fields.Add(Classes.Field.CreateFieldBuilder($"{Guild.Name} - {invite.ChannelName}", $"[{invite.Url}]({invite.Url})"));
                }

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, fields, Classes.Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - backdoor - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}
