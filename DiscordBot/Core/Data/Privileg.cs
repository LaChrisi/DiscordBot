using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Privileg
    {
        public ulong id { get; set; }
        public string name { get; set; }

        public static int user = 0;
        public static int moderator = 1;
        public static int admin = 2;
        public static int owner = 3;

        public Privileg(ulong id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public static bool CheckById(ulong userId, int privileg)
        {
            User user = User.GetById(userId);

            if (user != null)
            {
                if (user.privileg >= privileg)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static List<Privileg> GetAll()
        {
            var query = "SELECT * FROM privileg";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Privileg> list = new List<Privileg>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                var privileg = new Privileg((ulong) Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]));
                list.Add(privileg);
                i++;
            }

            return list;
        }
    }
}
