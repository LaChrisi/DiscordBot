using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Log
    {
        public async void Information(string text, string title = "", string description = "", SocketCommandContext context = null)
        {
            if (context != null)
            {
                Database.Log.Add(new Database.Log(context.Channel.Id, 'i', text));

                await context.Channel.SendMessageAsync(embed: Embed.New(context.Message.Author, Field.CreateFieldBuilder("information", text), Colors.information, title, description));
            }
            else
            {
                Database.Log.Add(new Database.Log('i', text));
            }
        }

        public async void Warning(string text, string title = "", string description = "", SocketCommandContext context = null)
        {
            if (context != null)
            {
                Database.Log.Add(new Database.Log(context.Channel.Id, 'w', text));

                await context.Channel.SendMessageAsync(embed: Embed.New(context.Message.Author, Field.CreateFieldBuilder("warning", text), Colors.warning, title, description));
            }
            else
            {
                Database.Log.Add(new Database.Log('w', text));
            }
        }

        public async void Error(string text, string title = "", string description = "", SocketCommandContext context = null)
        {
            if (context != null)
            {
                Database.Log.Add(new Database.Log(context.Channel.Id, 'e', text));

                await context.Channel.SendMessageAsync(embed: Embed.New(context.Message.Author, Field.CreateFieldBuilder("error", text), Colors.error, title, description));
            }
            else
            {
                Database.Log.Add(new Database.Log('e', text));
            }
        }
    }
}