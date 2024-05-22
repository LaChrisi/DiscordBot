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
                //Repetitive_Timer.Minutes_5_timer_Elapsed(null, null);

                var builder = new ComponentBuilder()
                    //.WithSelectMenu(new SelectMenuBuilder()
                    //.WithCustomId("who")
                    //.WithPlaceholder("Wer ist gekommen?")
                    //.AddOption(new SelectMenuOptionBuilder().WithLabel("Christoph").WithValue("Christoph"))
                    //.AddOption(new SelectMenuOptionBuilder().WithLabel("Nadine").WithValue("Nadine"))
                    //)
                    //.WithSelectMenu(new SelectMenuBuilder()
                    //.WithCustomId("type")
                    //.WithPlaceholder("Durch was?")
                    //.AddOption(new SelectMenuOptionBuilder().WithLabel("Sex").WithValue("Sex"))
                    //.AddOption(new SelectMenuOptionBuilder().WithLabel("Oral").WithValue("Oral"))
                    //.AddOption(new SelectMenuOptionBuilder().WithLabel("Masturbation").WithValue("Masturbation"))
                    //)
                    //.WithButton("Christoph", "christoph", ButtonStyle.Primary)
                    //.WithButton("Nadine", "nadine", ButtonStyle.Danger)
                    //.WithButton("Nadine", "nadine", ButtonStyle.Danger)
                    //.WithButton("Nadine", "nadine", ButtonStyle.Danger)
                    .WithButton("Der Akt wurde vollzogen", "start", ButtonStyle.Secondary)
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