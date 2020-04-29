using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Event
    {
        public ulong id { get; set; }
        public string what { get; set; }
        public string how { get; set; }

        public static string header = "id | what | how";

        public Event(ulong id, string what, string how)
        {
            this.id = id;
            this.what = what;
            this.how = how;
        }

        public Event(string what, string how)
        {
            this.what = what;
            this.how = how;
        }

        public override string ToString()
        {
            return this.id + " | " + this.what + " | " + this.how;
        }

        public static List<Event> GetAll()
        {
            var query = "SELECT * FROM event";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Event> list = new List<Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["what"]), Convert.ToString(dt.Rows[i]["how"])));
                i++;
            }

            return list;
        }

        public static Event GetById(ulong id)
        {
            var query = "SELECT * FROM event WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Event((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["what"]), Convert.ToString(dt.Rows[0]["how"]));
        }

        public static Event GetRandomByWhat(string what)
        {
            var query = "SELECT * FROM event WHERE what = @what ORDER BY RAND() LIMIT 1";

            var args = new Dictionary<string, object>
            {
                {"@what", what}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Event((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["what"]), Convert.ToString(dt.Rows[0]["how"]));
        }

        public static int Add(Event Event)
        {
            const string query = "INSERT INTO event(id, what, how) VALUES(@id, @what, @how)";

            var args = new Dictionary<string, object>
            {
                {"@id", Event.id},
                {"@what", Event.what},
                {"@how", Event.how}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM event WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Event Event)
        {
            const string query = "UPDATE event SET what = @what, how = @how WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", Event.id},
                {"@what", Event.what},
                {"@how", Event.how}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
