using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Vote
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public string what { get; set; }
        public string how { get; set; }

        public static string header = "id | name | what | how";

        public Vote(ulong id, string name, string what, string how)
        {
            this.id = id;
            this.name = name;
            this.what = what;
            this.how = how;
        }

        public Vote(string name, string what, string how)
        {
            this.name = name;
            this.what = what;
            this.how = how;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name + " | " + this.what + " | " + this.how;
        }

        public static List<Vote> GetAll()
        {
            var query = "SELECT * FROM vote";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote> list = new List<Vote>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), Convert.ToString(dt.Rows[i]["what"]), Convert.ToString(dt.Rows[i]["how"])));
                i++;
            }

            return list;
        }

        public static Vote GetById(ulong id)
        {
            var query = "SELECT * FROM vote WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Vote((ulong) Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]), Convert.ToString(dt.Rows[0]["what"]), Convert.ToString(dt.Rows[0]["how"]));
        }

        public static int Add(Vote vote)
        {
            const string query = "INSERT INTO vote(name, what, how) VALUES(@name, @what, @how)";

            var args = new Dictionary<string, object>
            {
                {"@name", vote.name},
                {"@what", vote.what},
                {"@how", vote.how}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
