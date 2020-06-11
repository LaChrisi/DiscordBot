using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Classes;

namespace DiscordBot.Core.Commands
{
    public class Hello : ModuleBase<SocketCommandContext>
    {
        [Command("hallo"), SummaryAttribute("Hallo Welt")]
        public async Task HalloModule()
        {
            await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Classes.Field.CreateFieldBuilder("hallo", $"Hallo {Context.User.Username}!"), Classes.Colors.information));
        }

        [Command("test"), SummaryAttribute("test command")]
        public async Task TestModule(ulong id)
        {
            if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
            {
                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                return;
            }

            Channel_Event.Add(new Channel_Event(1, Context.Channel.Id, 15, $"{id}", 'r'));


        }
    }
}