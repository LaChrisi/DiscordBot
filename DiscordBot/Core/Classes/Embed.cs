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

        public static Discord.Embed New(Discord.Rest.RestUser author, EmbedFieldBuilder field, Color color, string title = "", string description = "", string imgURL = "", string thumbnailUrl = "", string footer = "")
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

        public static Discord.Embed New(Discord.Rest.RestUser author, List<EmbedFieldBuilder> fields, Color color, string title = "", string description = "", string imgURL = "", string thumbnailUrl = "", string footer = "")
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

            return Embed.New(Program.Client.Rest.CurrentUser, fields, Colors.information, "top 5 memers", "ordered by upvotes");
        }

        public static Discord.Embed CreatePresent(SocketReaction reaction)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

            fields.Add(Field.CreateFieldBuilder(reaction.User.Value.Username, $"{DateTime.Now.ToString("dd.M. - HH:mm:ss")}"));

            return Embed.New(Program.Client.Rest.CurrentUser, fields, Colors.information, "present members", "current: 1");
        }

        public static Discord.Embed UpdatePresent(IUserMessage message, SocketReaction reaction, bool remove = false)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            int current = 0;

            if (remove)
            {
                foreach (var embed in message.Embeds)
                {
                    foreach (var field in embed.Fields)
                    {
                        if (field.Name != reaction.User.Value.Username)
                        {
                            fields.Add(Field.CreateFieldBuilder(field));
                            current++;
                        }
                    }
                }

                return Embed.New(Program.Client.Rest.CurrentUser, fields, Colors.information, "present members", "current: " + current);
            }
            else
            {
                foreach (var embed in message.Embeds)
                {
                    foreach (var field in embed.Fields)
                    {
                        fields.Add(Field.CreateFieldBuilder(field));
                        current++;
                    }

                    fields.Add(Field.CreateFieldBuilder(reaction.User.Value.Username, $"{DateTime.Now.ToString("dd.M. - HH:mm:ss")}"));
                    current++;
                }
                return Embed.New(Program.Client.Rest.CurrentUser, fields, Colors.information, "present members", "current: " + current);
            }
        }

        public static Discord.Embed UpdatePresentTime(IUserMessage message, SocketReaction reaction, bool remove = false)
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            int current = 0;

            if (remove)
            {
                foreach (var embed in message.Embeds)
                {
                    foreach (var field in embed.Fields)
                    {
                        if (field.Name == reaction.User.Value.Username)
                        {
                            fields.Add(Field.CreateFieldBuilder(field.Name, PresentTime(field.Value, reaction.Emote.Name, true)));
                            current++;
                        }
                        else
                        {
                            fields.Add(Field.CreateFieldBuilder(field));
                            current++;
                        }
                    }
                }

                return Embed.New(Program.Client.Rest.CurrentUser, fields, Colors.information, "present members", "current: " + current);
            }
            else
            {
                foreach (var embed in message.Embeds)
                {
                    foreach (var field in embed.Fields)
                    {
                        if (field.Name == reaction.User.Value.Username)
                        {
                            fields.Add(Field.CreateFieldBuilder(field.Name, PresentTime(field.Value, reaction.Emote.Name)));
                            current++;
                        }
                        else
                        {
                            fields.Add(Field.CreateFieldBuilder(field));
                            current++;
                        }
                    }
                }

                return Embed.New(Program.Client.Rest.CurrentUser, fields, Colors.information, "present members", "current: " + current);
            }
        }

        private static string PresentTime(string fieldValue, string emote, bool remove = false)
        {
            string[] parts = fieldValue.Split("\n");
            string partTime = "";
            string output = "";

            if (remove)
            {
                if (parts.Length == 2)
                {
                    string time = GetTime(emote);
                    partTime = parts[1].Substring(5);

                    string[] times = partTime.Split(", ");

                    foreach (var item in times)
                    {
                        if (item != time)
                        {
                            if (output != "")
                                output += ", " + item;
                            else
                                output = item;
                        }
                    }
                }

                if (output == "")
                    return parts[0];
                else
                    return parts[0] + "\nfrom " + output;
            }
            else
            {
                if (parts.Length == 1)
                {
                    output = GetTime(emote);
                }
                else if (parts.Length == 2)
                {
                    partTime = parts[1].Substring(5);
                    output = partTime + ", " + GetTime(emote);
                }

                return parts[0] + "\nfrom " + output;
            }
        }

        private static string GetTime(string emote)
        {
            string time = "";

            switch (emote)
            {
                case "4️⃣":
                    time = "16";
                    break;
                case "5️⃣":
                    time = "17";
                    break;
                case "6️⃣":
                    time = "18";
                    break;
                case "7️⃣":
                    time = "19";
                    break;
                case "8️⃣":
                    time = "20";
                    break;
                case "9️⃣":
                    time = "21";
                    break;
                default:
                    break;
            }

            return time;
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
        public static EmbedFieldBuilder CreateFieldBuilder(EmbedField embedField)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            
            builder.Name = embedField.Name;

            builder.Value = embedField.Value;

            return builder;
        }

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
            string cache = "|";

            string[] header_list = header.Split(" | ");

            foreach (string item in header_list)
            {
                cache += "**" + item + "**|";
            }

            cache += "\n";

            for (int i = 0; i < header_list.Length; i++)
            {
                cache += "|---";
            }

            cache += "|\n";

            foreach (string content_list in content)
            {
                string[] content_list_list = content_list.Split(" | ");

                cache += "|";

                foreach (string item in content_list_list)
                {
                    cache += item + "|";
                }

                cache += "\n";
            }

            builder.Name = title;

            builder.Value = cache;

            return builder;
        }

        public static EmbedFieldBuilder CreateMarkdownTable(string title, string header, string content)
        {
            EmbedFieldBuilder builder = new EmbedFieldBuilder();
            string cache = "|";

            string[] header_list = header.Split(" | ");

            foreach (string item in header_list)
            {
                cache += "**" + item + "**|";
            }

            cache += "\n";

            for (int i = 0; i < header_list.Length; i++)
            {
                cache += "|---";
            }

            cache += "|\n";

            string[] content_list = content.Split(" | ");

            cache += "|";

            foreach (string item in content_list)
            {
                cache += item + "|";
            }

            cache += "\n";

            builder.Name = title;

            builder.Value = cache;

            return builder;
        }
    }
}
