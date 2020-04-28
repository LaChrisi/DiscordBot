using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Server
    {
        public ulong id { get; set; }
        public string name { get; set; }

        public static string header = "id | name";

        public Server(ulong id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name;
        }

        public static List<Server> GetAll()
        {
            var query = "SELECT * FROM server";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Server> list = new List<Server>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Server((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"])));
                i++;
            }

            return list;
        }

        public static Server GetById(ulong id)
        {
            var query = "SELECT * FROM server WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Server((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]));
        }

        public static int Add(Server server)
        {
            const string query = "INSERT INTO server(id, name) VALUES(@id, @name)";

            var args = new Dictionary<string, object>
            {
                {"@id", server.id},
                {"@name", server.name}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM server WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Server server)
        {
            const string query = "UPDATE server SET name = @name WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", server.id},
                {"@name", server.name}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
