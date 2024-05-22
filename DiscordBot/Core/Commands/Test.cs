using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.Classes;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace DiscordBot.Core.Commands
{
    public class Test : ModuleBase<SocketCommandContext>
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
        public async Task TestModule(int id = 0)
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

                string output = "";

                //start test here

                var builder = new ComponentBuilder()
                    .WithButton("Der Akt wurde vollzogen", "start", ButtonStyle.Secondary)
                    .WithButton("letzten Akt löschen", "delete", ButtonStyle.Secondary)
                    ;

                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Field.CreateFieldBuilder("Neuaufnahme", $"Hier kann Sex aufgezeichnet werden:"), Colors.information), components: builder.Build());

               

                //test end

                if (output == "")
                {
                    //await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Field.CreateFieldBuilder("test", $"Test successful!"), Colors.information));
                }
                else
                {
                    await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Field.CreateFieldBuilder("output", $"{output}"), Colors.information));
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync(embed: Classes.Embed.New(Context.Client.CurrentUser, Field.CreateFieldBuilder("ERROR", $"{ex.Message}"), Colors.error));
                Log.Error($"command - test - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }
    }
}