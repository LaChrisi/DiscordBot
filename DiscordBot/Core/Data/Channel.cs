using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Channel
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public ulong server { get; set; }

        public static string header = "id | name | server";

        public Channel(ulong id, string name, ulong server)
        {
            this.id = id;
            this.name = name;
            this.server = server;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name + " | " + this.server;
        }

        public static List<Channel> GetAll()
        {
            var query = "SELECT * FROM channel";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel> list = new List<Channel>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), (ulong)Convert.ToInt64(dt.Rows[i]["server"])));
                i++;
            }

            return list;
        }

        public static List<Channel> GetAllByServerId(ulong id)
        {
            var query = "SELECT * FROM channel WHERE server = @server";
            var args = new Dictionary<string, object>
            {
                {"@server", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel> list = new List<Channel>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), (ulong)Convert.ToInt64(dt.Rows[i]["server"])));
                i++;
            }

            return list;
        }

        public static Channel GetById(ulong id)
        {
            var query = "SELECT * FROM channel WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Channel((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]), (ulong)Convert.ToInt64(dt.Rows[0]["server"]));
        }

        public static int Add(Channel channel)
        {
            const string query = "INSERT INTO channel(id, name, server) VALUES(@id, @name, @server)";

            var args = new Dictionary<string, object>
            {
                {"@id", channel.id},
                {"@name", channel.name},
                {"@server", channel.server}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM channel WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByServerId(ulong id)
        {
            var query = "DELETE * FROM channel WHERE server = @id";
            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Channel channel)
        {
            const string query = "UPDATE channel SET name = @name, server = @server WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", channel.id},
                {"@name", channel.name},
                {"@server", channel.server}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
