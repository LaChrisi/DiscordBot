using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Global
    {
        public string name { get; set; }
        public string value { get; set; }

        public static string header = "name | value";

        public Global(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return this.name + " | " + this.value;
        }

        public static List<Global> GetAll()
        {
            var query = "SELECT * FROM global";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Global> list = new List<Global>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Global(Convert.ToString(dt.Rows[i]["name"]), Convert.ToString(dt.Rows[i]["value"])));
                i++;
            }

            return list;
        }

        public static Global GetByName(string name)
        {
            var query = "SELECT * FROM global WHERE name = @name";

            var args = new Dictionary<string, object>
            {
                {"@name", name}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Global(Convert.ToString(dt.Rows[0]["name"]), Convert.ToString(dt.Rows[0]["value"]));
        }

        public static int Add(Global global)
        {
            const string query = "INSERT INTO global(name, value) VALUES(@name, @value)";

            var args = new Dictionary<string, object>
            {
                {"@name", global.name},
                {"@value", global.value}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteByName(string name)
        {
            const string query = "DELETE FROM global WHERE name = @name";

            var args = new Dictionary<string, object>
            {
                {"@name", name}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Global global)
        {
            const string query = "UPDATE global SET value = @value WHERE name = @name";

            var args = new Dictionary<string, object>
            {
                {"@name", global.name},
                {"@value", global.value}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
