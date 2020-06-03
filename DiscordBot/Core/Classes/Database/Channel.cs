using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Channel
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public ulong server { get; set; }
        public int broadcast { get; set; }

        public static string header = "id | name | server | broadcast";

        public Channel(ulong id, string name, ulong server, int broadcast = 0)
        {
            this.id = id;
            this.name = name;
            this.server = server;
            this.broadcast = broadcast;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name + " | " + this.server + " | " + this.broadcast;
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
                list.Add(new Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), (ulong)Convert.ToInt64(dt.Rows[i]["server"]), Convert.ToInt16(dt.Rows[i]["broadcast"])));
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
                list.Add(new Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), (ulong)Convert.ToInt64(dt.Rows[i]["server"]), Convert.ToInt16(dt.Rows[i]["broadcast"])));
                i++;
            }

            return list;
        }

        public static List<Channel> GetAllByBroadcast(int broadcast)
        {
            var query = "SELECT * FROM channel WHERE broadcast = @broadcast";
            var args = new Dictionary<string, object>
            {
                {"@broadcast", broadcast}
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
                list.Add(new Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), (ulong)Convert.ToInt64(dt.Rows[i]["server"]), Convert.ToInt16(dt.Rows[i]["broadcast"])));
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

            return new Channel((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]), (ulong)Convert.ToInt64(dt.Rows[0]["server"]), Convert.ToInt16(dt.Rows[0]["broadcast"]));
        }

        public static int Add(Channel channel)
        {
            const string query = "INSERT INTO channel(id, name, server, broadcast) VALUES(@id, @name, @server, @broadcast)";

            var args = new Dictionary<string, object>
            {
                {"@id", channel.id},
                {"@name", channel.name},
                {"@server", channel.server},
                {"@broadcast", channel.broadcast}
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
            const string query = "UPDATE channel SET name = @name, server = @server, broadcast = @broadcast WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", channel.id},
                {"@name", channel.name},
                {"@server", channel.server},
                {"@broadcast", channel.broadcast}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
