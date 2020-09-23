using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes.Database
{
    class Log
    {
        public ulong id { get; set; }
        public ulong channel { get; set; }
        public ulong user { get; set; }
        public char type { get; set; }
        public string text { get; set; }

        public static string header = "id | channel | user | type | text";

        public Log(ulong id, ulong channel, ulong user, char type, string text)
        {
            this.id = id;
            this.channel = channel;
            this.user = user;
            this.type = type;
            this.text = text;
        }

        public Log(ulong channel, ulong  user, char type, string text)
        {
            this.channel = channel;
            this.user = user;
            this.type = type;
            this.text = text;
        }

        public Log(ulong channel, char type, string text)
        {
            this.channel = channel;
            this.user = 1;
            this.type = type;
            this.text = text;
        }

        public Log(char type, string text)
        {
            this.user = 1;
            this.type = type;
            this.text = text;
        }

        public override string ToString()
        {
            return this.id + " | " + this.channel + " | " + this.user + " | " + this.type + " | " + this.text;
        }

        public static List<Log> GetAll()
        {
            var query = "SELECT * FROM log";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Log> list = new List<Log>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Log((ulong)Convert.ToInt64(dt.Rows[i]["id"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), (ulong)Convert.ToInt64(dt.Rows[i]["user"]), Convert.ToChar(dt.Rows[i]["type"]), Convert.ToString(dt.Rows[i]["text"])));
                i++;
            }

            return list;
        }

        public static Log GetById(ulong id)
        {
            var query = "SELECT * FROM log WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Log((ulong)Convert.ToInt64(dt.Rows[0]["id"]), (ulong)Convert.ToInt64(dt.Rows[0]["channel"]), (ulong)Convert.ToInt64(dt.Rows[0]["user"]), Convert.ToChar(dt.Rows[0]["type"]), Convert.ToString(dt.Rows[0]["text"]));
        }

        public static int Add(Log log)
        {
            const string query = "INSERT INTO log(channel, user, type, text) VALUES(@channel, @user, @type, @text)";

            var args = new Dictionary<string, object>
            {
                {"@channel", log.channel},
                {"@user", log.user},
                {"@type", log.type},
                {"@text", log.text}
            };

            return Data.ExecuteWrite(query, args);
        }

        /*
        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM log WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }
        */

        /*
        public static int Edit(Log log)
        {
            const string query = "UPDATE log SET channel = @channel, user = @user, type = @type, text = @text WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", log.id},
                {"@channel", log.channel},
                {"@user", log.user},
                {"@type", log.type},
                {"@text", log.text}
            };

            return Data.ExecuteWrite(query, args);
        }
        */

    }
}
