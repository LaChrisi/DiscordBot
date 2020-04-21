using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace DiscordBot.Core.Data
{
    public class User
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public int posts { get; set; }
        public int upvotes { get; set; }
        public int downvotes { get; set; }
        public int privileg { get; set; }

        public User(ulong id, string name, int privileg, int posts = 0, int upvotes = 0, int downvotes = 0)
        {
            this.id = id;
            this.name = name;
            this.privileg = privileg;
            this.posts = posts;
            this.upvotes = upvotes;
            this.downvotes = downvotes;
        }

        public string WriteHead()
        {
            string ausgabe = "id | name | posts | upvotes | downvotes | privileg";

            return ausgabe;
        }

        public string Write()
        {
            string ausgabe = this.id + " | " + this.name + " | " + this.posts + " | " + this.upvotes + " | " + this.downvotes + " | " + this.privileg;

            return ausgabe;
        }

        public static List<User> GetAll()
        {
            var query = "SELECT * FROM user";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<User> list = new List<User>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                var user = new User((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), Convert.ToInt16(dt.Rows[i]["privileg"]), Convert.ToInt32(dt.Rows[i]["posts"]), Convert.ToInt32(dt.Rows[i]["upvotes"]), Convert.ToInt32(dt.Rows[i]["downvotes"]));
                list.Add(user);
                i++;
            }

            return list;
        }

        public static User GetById(ulong id)
        {
            var query = "SELECT * FROM user WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            var user = new User((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]), Convert.ToInt16(dt.Rows[0]["privileg"]), Convert.ToInt32(dt.Rows[0]["posts"]), Convert.ToInt32(dt.Rows[0]["upvotes"]), Convert.ToInt32(dt.Rows[0]["downvotes"]));

            return user;
        }

        public static int Add(User user)
        {
            const string query = "INSERT INTO user(id, name, posts, upvotes, downvotes, privileg) VALUES(@id, @name, @posts, @upvotes, @downvotes, @privileg)";

            var args = new Dictionary<string, object>
            {
                {"@id", user.id},
                {"@name", user.name},
                {"@posts", user.posts},
                {"@upvotes", user.upvotes},
                {"@downvotes", user.downvotes},
                {"@privileg", user.privileg}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE from user WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(User user)
        {
            const string query = "UPDATE user SET name = @name, privileg = @privileg, posts = @posts, upvotes = @upvotes, downvotes = @downvotes WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", user.id},
                {"@name", user.name},
                {"@posts", user.posts},
                {"@upvotes", user.upvotes},
                {"@downvotes", user.downvotes},
                {"@privileg", user.privileg}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
