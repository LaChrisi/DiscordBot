using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes.Database
{
    class Birthdays
    {
        public ulong id { get; set; }
        public ulong user { get; set; }
        public DateTime date { get; set; }

        public static string header = "id | user | date";

        public Birthdays(ulong id, ulong user, DateTime date)
        {
            this.id = id;
            this.user = user;
            this.date = date;
        }

        public override string ToString()
        {
            return this.id + " | " + this.user + " | " + this.date;
        }

        public static List<Birthdays> GetAll()
        {
            var query = "SELECT * FROM birthdays";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Birthdays> list = new List<Birthdays>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Birthdays((ulong)Convert.ToInt64(dt.Rows[i]["id"]), (ulong)Convert.ToInt64(dt.Rows[i]["user"]), (DateTime)Convert.ToDateTime(dt.Rows[i]["date"])));
                i++;
            }

            return list;
        }

        public static Birthdays GetById(ulong id)
        {
            var query = "SELECT * FROM birthdays WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Birthdays((ulong)Convert.ToInt64(dt.Rows[0]["id"]), (ulong)Convert.ToInt64(dt.Rows[1]["user"]), (DateTime)Convert.ToDateTime(dt.Rows[2]["date"]));
        }

        public static Birthdays GetNext()
        {
            var query = "SELECT TOP 1 * FROM birthdays WHERE date < @CurrentDate ORDER BY date DESC";

            var args = new Dictionary<string, object>();

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Birthdays((ulong)Convert.ToInt64(dt.Rows[0]["id"]), (ulong)Convert.ToInt64(dt.Rows[1]["user"]), (DateTime)Convert.ToDateTime(dt.Rows[2]["date"]));
        }

        public static int Add(Birthdays birthdays)
        {
            const string query = "INSERT INTO birthdays(id, user, date) VALUES(@id, @user, @date)";

            var args = new Dictionary<string, object>
            {
                {"@id", birthdays.id},
                {"@user", birthdays.user},
                {"@date", birthdays.date}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM birthdays WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByServerId(ulong id)
        {
            var query = "DELETE * FROM birthdays WHERE server = @id";
            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Birthdays birthdays)
        {
            const string query = "UPDATE birthdays SET user = @user, date = @date WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", birthdays.id},
                {"@name", birthdays.user},
                {"@server", birthdays.date}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
