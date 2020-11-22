using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Role
    {
        public ulong id { get; set; }
        public string name { get; set; }

        public static string header = "id | name";

        public Role(ulong id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name;
        }

        public static List<Role> GetAll()
        {
            var query = "SELECT * FROM role";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Role> list = new List<Role>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                var role = new Role((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]));
                list.Add(role);
                i++;
            }

            return list;
        }

        public static Role GetById(ulong id)
        {
            var query = "SELECT * FROM role WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Role((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]));
        }

        public static int Add(Privileg privileg)
        {
            const string query = "INSERT INTO role(id, name) VALUES(@id, @name)";

            var args = new Dictionary<string, object>
            {
                {"@id", privileg.id},
                {"@name", privileg.name}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM role WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Privileg privileg)
        {
            const string query = "UPDATE role SET name = @name WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", privileg.id},
                {"@name", privileg.name}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
