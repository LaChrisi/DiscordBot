using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Embed
    {
        public static Discord.Embed New(SocketUser author, EmbedFieldBuilder field, Color color, string title = "", string description = "", string imgURL = "", string thumbnailUrl = "", string footer = "")
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithAuthor(author);
            embed.WithTitle(title);
            embed.WithDescription(description);
            embed.WithImageUrl(imgURL);
            embed.WithThumbnailUrl(thumbnailUrl);
            embed.AddField(field);
            embed.WithColor(color);
            embed.WithFooter(f => f.Text = footer);
            embed.WithTimestamp(DateTimeOffset.Now);

            return embed.Build();
        }

        public static Discord.Embed New(SocketUser author, List<EmbedFieldBuilder> fields, Color color, string title = "", string description = "", string imgURL = "", string thumbnailUrl = "", string footer = "")
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithAuthor(author);
            embed.WithTitle(title);
            embed.WithDescription(description);
            embed.WithImageUrl(imgURL);
            embed.WithThumbnailUrl(thumbnailUrl);

            foreach (var field in fields)
            {
                embed.AddField(field);
            }
            
            embed.WithColor(color);
            embed.WithFooter(f => f.Text = footer);
            embed.WithTimestamp(DateTimeOffset.Now);

            return embed.Build();
        }

        public static Discord.Embed New(EmbedAuthor author, List<EmbedFieldBuilder> fields, Color color, string title = "", string description = "", string imgURL = "", string thumbnailUrl = "", string footer = "")
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.WithAuthor(author.Name, author.IconUrl, author.Url);
            embed.WithTitle(title);
            embed.WithDescription(description);
            embed.WithImageUrl(imgURL);
            embed.WithThumbnailUrl(thumbnailUrl);

            foreach (var field in fields)
            {
                embed.AddField(field);
            }

            embed.WithColor(color);
            embed.WithFooter(f => f.Text = footer);
            embed.WithTimestamp(DateTimeOffset.Now);

            return embed.Build();
        }

        public static Discord.Embed GetLeaderboard()
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

            var user_list = User.GetTop5();
            int i = 1;

            foreach (var user in user_list)
            {
                string title = "";

                if (i == 1)
                    title += "🥇" + " - ";
                else if (i == 2)
                    title += "🥈" + " - ";
                else if (i == 3)
                    title += "🥉" + " - ";

                title += user.name;

                //fields.Add(Field.CreateFieldBuilder(title, $"👍 {user.upvotes}\n👎 {user.downvotes}\n🗒️ {user.posts}\n📊 {user.karma}"));
                fields.Add(Field.CreateFieldBuilder(title, $"👍 {user.upvotes}\n👎 {user.downvotes}\n🗒️ {user.posts}"));
                i++;
            }

            return Embed.New(Program.Client.CurrentUser, fields, Colors.information, "top 5 memers", "ordered by upvotes");
        }

        public static Discord.Embed CreatePresent(SocketReaction reaction)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

            fields.Add(Field.CreateFieldBuilder(reaction.User.Value.Username, $"{DateTime.Now.ToString("dd.M. - HH:mm:ss")}"));

            return Embed.New(Program.Client.CurrentUser, fields, Colors.information, "present members");
        }

        public static Discord.Embed UpdatePresent(IUserMessage message, SocketReaction reaction, bool remove = false)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

            foreach (var embed in message.Embeds)
            {
                fields.Add(embed.Fields.);//????????????????????????????????????????

                fields.Add(Field.CreateFieldBuilder(reaction.User.Value.Username, $"{DateTime.Now.ToString("dd.M. - HH:mm:ss")}"));
            }
            return Embed.New(Program.Client.CurrentUser, fields, Colors.information, "present members");
        }
    }

    class Colors
    {
        public static Color meme = Color.Green;
        public static Color information = Color.Blue;
        public static Color warning = Color.Gold;
        public static Color error = Color.Red;
    }

    class Field
    {
        public static EmbedFieldBuilder CreateFieldBuilder(string header, string[] content)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();

            string value = "";

            foreach (string content_list in content)
            {
                value += content_list + "\n";
            }

            builder.Name = header;

            builder.Value = value;

            return builder;
        }

        public static EmbedFieldBuilder CreateFieldBuilder(string header, string content)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();

            builder.Name = header;

            builder.Value = content;

            return builder;
        }

        public static EmbedFieldBuilder CreatMarkdowneTable(string header, string[] content, string title = "")
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            string zwischenspeicher = "|";

            string[] header_list = header.Split(" | ");

            foreach (string item in header_list)
            {
                zwischenspeicher += "**" + item + "**|";
            }

            zwischenspeicher += "\n";

            for (int i = 0; i < header_list.Length; i++)
            {
                zwischenspeicher += "|---";
            }

            zwischenspeicher += "|\n";

            foreach (string content_list in content)
            {
                string[] content_list_list = content_list.Split(" | ");

                zwischenspeicher += "|";

                foreach (string item in content_list_list)
                {
                    zwischenspeicher += item + "|";
                }

                zwischenspeicher += "\n";
            }

            builder.Name = title;

            builder.Value = zwischenspeicher;

            return builder;
        }

        public static EmbedFieldBuilder CreateMarkdownTable(string title, string header, string content)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            string zwischenspeicher = "|";

            string[] header_list = header.Split(" | ");

            foreach (string item in header_list)
            {
                zwischenspeicher += "**" + item + "**|";
            }

            zwischenspeicher += "\n";

            for (int i = 0; i < header_list.Length; i++)
            {
                zwischenspeicher += "|---";
            }

            zwischenspeicher += "|\n";

            string[] content_list = content.Split(" | ");

            zwischenspeicher += "|";

            foreach (string item in content_list)
            {
                zwischenspeicher += item + "|";
            }

            zwischenspeicher += "\n";

            builder.Name = title;

            builder.Value = zwischenspeicher;

            return builder;
        }
    }
}
