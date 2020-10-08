using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes.Database
{
    class Log
    {
        public ulong id { get; set; }
        public DateTime datetime { get; set; }
        public char type { get; set; }
        public string text { get; set; }

        public static string header = "id | datetime | type | text";

        public Log(ulong id, DateTime datetime, char type, string text)
        {
            this.id = id;
            this.datetime = datetime;
            this.type = type;
            this.text = text;
        }

        public Log(DateTime datetime, char type, string text)
        {
            this.datetime = datetime;
            this.type = type;
            this.text = text;
        }

        public override string ToString()
        {
            return this.id + " | " + this.datetime.ToString("yyyy-MM-dd HH:mm:ss") + " | " + this.type + " | " + this.text;
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
                list.Add(new Log((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToDateTime(dt.Rows[i]["datetime"]), Convert.ToChar(dt.Rows[i]["type"]), Convert.ToString(dt.Rows[i]["text"])));
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

            return new Log((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToDateTime(dt.Rows[0]["datetime"]), Convert.ToChar(dt.Rows[0]["type"]), Convert.ToString(dt.Rows[0]["text"]));
        }

        public static int Add(Log log)
        {
            const string query = "INSERT INTO log(datetime, type, text) VALUES(@datetime, @type, @text)";

            var args = new Dictionary<string, object>
            {
                {"@datetime", log.datetime},
                {"@type", log.type},
                {"@text", log.text}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
