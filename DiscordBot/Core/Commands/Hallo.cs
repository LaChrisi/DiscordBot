﻿using System;
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
            try
            {
                Log.Information($"command - hallo - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Classes.Field.CreateFieldBuilder("hallo", $"Hallo {Context.User.Username}!"), Classes.Colors.information));
            }
            catch (Exception ex)
            {
                Log.Error($"command - hallo - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        [Command("test"), SummaryAttribute("test command")]
        public async Task TestModule(ulong id)
        {
            try
            {
                if (!Privileg.CheckById(Context.User.Id, Privileg.owner))
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Message.Author, Field.CreateFieldBuilder("warning", "You are not my god!"), Colors.warning));
                    Log.Warning($"command - test - user:{Context.User.Id} channel:{Context.Channel.Id} privileg to low");
                    return;
                }

                Log.Information($"command - test - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");

                Channel_Event.Add(new Channel_Event(1, Context.Channel.Id, 15, $"{id}", 'r'));
            }
            catch (Exception ex)
            {
                Log.Error($"command - test - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}