using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DiscordBot.Core.Classes
{
    public class User
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public int posts { get; set; }
        public int upvotes { get; set; }
        public int downvotes { get; set; }
        public int privileg { get; set; }
        public int karma { get; set; }

        public static string header = "id | name | posts | upvotes | downvotes | privileg | karma";

        public User(ulong id, string name, int privileg, int posts = 0, int upvotes = 0, int downvotes = 0, int karma = 0)
        {
            this.id = id;
            this.name = name;
            this.privileg = privileg;
            this.posts = posts;
            this.upvotes = upvotes;
            this.downvotes = downvotes;
            this.karma = karma;
        }

        public override string ToString()
        {
            return this.id + " | " + this.name + " | " + this.posts + " | " + this.upvotes + " | " + this.downvotes + " | " + this.privileg + " | " + this.karma;
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
                list.Add(new User((ulong) Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), Convert.ToInt16(dt.Rows[i]["privileg"]), Convert.ToInt32(dt.Rows[i]["posts"]), Convert.ToInt32(dt.Rows[i]["upvotes"]), Convert.ToInt32(dt.Rows[i]["downvotes"]), Convert.ToInt32(dt.Rows[i]["karma"])));
                i++;
            }

            return list;
        }

        public static List<User> GetAllWithKarma()
        {
            var query = "SELECT * FROM user WHERE NOT karma = -1";
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
                list.Add(new User((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), Convert.ToInt16(dt.Rows[i]["privileg"]), Convert.ToInt32(dt.Rows[i]["posts"]), Convert.ToInt32(dt.Rows[i]["upvotes"]), Convert.ToInt32(dt.Rows[i]["downvotes"]), Convert.ToInt32(dt.Rows[i]["karma"])));
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

            return new User((ulong) Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToString(dt.Rows[0]["name"]), Convert.ToInt16(dt.Rows[0]["privileg"]), Convert.ToInt32(dt.Rows[0]["posts"]), Convert.ToInt32(dt.Rows[0]["upvotes"]), Convert.ToInt32(dt.Rows[0]["downvotes"]), Convert.ToInt32(dt.Rows[0]["karma"]));
        }

        public static int Add(User user)
        {
            const string query = "INSERT INTO user(id, name, posts, upvotes, downvotes, privileg, karma) VALUES(@id, @name, @posts, @upvotes, @downvotes, @privileg, @karma)";

            var args = new Dictionary<string, object>
            {
                {"@id", user.id},
                {"@name", user.name},
                {"@posts", user.posts},
                {"@upvotes", user.upvotes},
                {"@downvotes", user.downvotes},
                {"@privileg", user.privileg},
                {"@karma", user.karma}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM user WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(User user)
        {
            const string query = "UPDATE user SET name = @name, privileg = @privileg, posts = @posts, upvotes = @upvotes, downvotes = @downvotes, karma = @karma WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", user.id},
                {"@name", user.name},
                {"@posts", user.posts},
                {"@upvotes", user.upvotes},
                {"@downvotes", user.downvotes},
                {"@privileg", user.privileg},
                {"@karma", user.karma}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static List<User> GetTop5Karma()
        {
            var query = "SELECT * FROM user ORDER BY karma DESC, upvotes DESC LIMIT 5";
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
                list.Add(new User((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToString(dt.Rows[i]["name"]), Convert.ToInt16(dt.Rows[i]["privileg"]), Convert.ToInt32(dt.Rows[i]["posts"]), Convert.ToInt32(dt.Rows[i]["upvotes"]), Convert.ToInt32(dt.Rows[i]["downvotes"]), Convert.ToInt32(dt.Rows[i]["karma"])));
                i++;
            }

            return list;
        }
        

    }
}
