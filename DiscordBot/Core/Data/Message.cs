using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Message
    {
        public ulong id { get; set; }
        public ulong user { get; set; }
        public ulong message { get; set; }
        public ulong channel { get; set; }
        public char type { get; set; }

        public static string header = "id | user | message | channel | type";

        public override string ToString()
        {
            return this.id + " | " + this.user + " | " + this.message + " | " + this.channel + " | " + this.type;
        }

        public Message(ulong id, ulong user, ulong message, ulong channel, char type)
        {
            this.id = id;
            this.user = user;
            this.message = message;
            this.channel = channel;
            this.type = type;
        }

        public Message(ulong user, ulong message, ulong channel, char type)
        {
            this.user = user;
            this.message = message;
            this.channel = channel;
            this.type = type;
        }

        public static List<Message> GetAll()
        {
            var query = "SELECT * FROM message";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Message> list = new List<Message>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Message((ulong)Convert.ToInt64(dt.Rows[i]["id"]), (ulong)Convert.ToInt64(dt.Rows[i]["user"]), (ulong)Convert.ToInt64(dt.Rows[i]["message"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }

        public static List<Message> GetAllByUserAndType(ulong user, char type)
        {
            var query = "SELECT * FROM message WHERE user = @user AND type = @type";
            var args = new Dictionary<string, object>
            {
                {"@user", user},
                {"@type", type}
            };
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Message> list = new List<Message>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Message((ulong)Convert.ToInt64(dt.Rows[i]["id"]), (ulong)Convert.ToInt64(dt.Rows[i]["user"]), (ulong)Convert.ToInt64(dt.Rows[i]["message"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }

        public static Message GetById(ulong id)
        {
            var query = "SELECT * FROM message WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Message((ulong)Convert.ToInt64(dt.Rows[0]["id"]), (ulong)Convert.ToInt64(dt.Rows[0]["user"]), (ulong)Convert.ToInt64(dt.Rows[0]["message"]), (ulong)Convert.ToInt64(dt.Rows[0]["channel"]), Convert.ToChar(dt.Rows[0]["type"]));
        }

        public static int Add(Message message)
        {
            const string query = "INSERT INTO message(user, message, channel, type) VALUES(@user, @message, @channel, @type)";

            var args = new Dictionary<string, object>
            {
                {"@user", message.user},
                {"@message", message.message},
                {"@channel", message.channel},
                {"@type", message.type}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM message WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByUserAndTypeId(ulong id, char type)
        {
            var query = "DELETE * FROM message WHERE user = @user AND type = @type";
            var args = new Dictionary<string, object>
            {
                {"@user", id},
                {"@type", type}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Message message)
        {
            const string query = "UPDATE message SET user = @user, message = @message, channel = @channel, type = @type WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", message.id},
                {"@user", message.user},
                {"@message", message.message},
                {"@channel", message.channel},
                {"@type", message.type}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
