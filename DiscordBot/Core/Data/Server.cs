using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
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
    }
}
