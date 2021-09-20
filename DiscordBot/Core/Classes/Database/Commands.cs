using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes.Database
{
    class Commands
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public ulong server { get; set; }

        public static string header = "id | name | server";

        public Commands(ulong id, string name, ulong server = 0)
        {
            this.id = id;
            this.name = name;
            this.server = server;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name + " | " + this.server;
        }

        public static List<Commands> GetAll()
        {
            var query = "SELECT * FROM commands";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Commands> list = new List<Commands>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Commands((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), (ulong)Convert.ToInt64(dt.Rows[i]["server"])));
                i++;
            }

            return list;
        }

        public static Commands GetById(ulong id)
        {
            var query = "SELECT * FROM commands WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Commands((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]), (ulong)Convert.ToInt64(dt.Rows[0]["server"]));
        }

        public static int Add(Commands commands)
        {
            const string query = "INSERT INTO commands(name, server) VALUES(@name, @server)";

            var args = new Dictionary<string, object>
            {
                {"@name", commands.name},
                {"@server", commands.server}
            };

            return Data.ExecuteWrite(query, args);
        }
        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM commands WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
