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
        public static void Information(string text)
        {
            try
            {
                Database.Log.Add(new Database.Log(DateTime.Now, 'i', text));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Warning(string text)
        {
            try
            {
            Database.Log.Add(new Database.Log(DateTime.Now, 'w', text));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void Error(string text)
        {
            try
            {
            Database.Log.Add(new Database.Log(DateTime.Now, 'e', text));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}