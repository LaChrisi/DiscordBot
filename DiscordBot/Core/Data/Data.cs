using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SQLite;

namespace DiscordBot.Core.Data
{
    public static class Data
    {
        public static string Version()
        {
            string cs = "Data Source=DiscordBot.db";
            string stm = "SELECT SQLITE_VERSION()";

            using var con = new SQLiteConnection(cs, true);
            con.Open();

            using var cmd = new SQLiteCommand(stm, con);
            string version = cmd.ExecuteScalar().ToString();

            return version;
        }

        public static string Read()
        {
            string cs = "Data Source=DiscordBot.db";
            string stm = "SELECT * FROM user";

            using var con = new SQLiteConnection(cs, true);
            con.Open();

            using var cmd = new SQLiteCommand(stm, con);
            using SQLiteDataReader rdr = cmd.ExecuteReader();

            string ausgabe = "";

            while (rdr.Read())
            {
                ausgabe = ausgabe + rdr.GetInt64(0) + ";" + rdr.GetString(1) + ";" + rdr.GetInt32(2) + ";" + rdr.GetInt32(3) + ";" + rdr.GetInt32(4) + ";" + rdr.GetInt32(5) + "\n";
            }

            return ausgabe;
        }

        public static int Write(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;

            //setup the connection to the database
            using (var con = new SQLiteConnection("Data Source=test.db"))
            {
                con.Open();

                //open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    //set the arguments given in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }

                    //execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }

                return numberOfRowsAffected;
            }
        }
        /*
        public static int GetLikes(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                    return 0;
                return DbContext.Votes.Where(x => x.UserId == UserId).Select(x => x.Likes).FirstOrDefault();
            }
        }

        public static int GetDislikes(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                    return 0;
                return DbContext.Votes.Where(x => x.UserId == UserId).Select(x => x.Dislikes).FirstOrDefault();
            }
        }

        public static int GetPosts(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                    return 0;
                return DbContext.Votes.Where(x => x.UserId == UserId).Select(x => x.Posts).FirstOrDefault();
            }
        }

        public static async Task SaveLikes(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Votes.Add(new Votes
                    {
                        UserId = UserId,
                        Likes = Amount,
                        Dislikes = 0,
                        Posts = 1
                    });
                }
                else
                {
                    Votes Current = DbContext.Votes.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Likes += Amount;
                    DbContext.Votes.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SaveDislikes(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Votes.Add(new Votes
                    {
                        UserId = UserId,
                        Dislikes = Amount,
                        Likes = 0,
                        Posts = 1
                    });
                }
                else
                {
                    Votes Current = DbContext.Votes.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Dislikes += Amount;
                    DbContext.Votes.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SavePosts(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Votes.Add(new Votes
                    {
                        UserId = UserId,
                        Likes = 0,
                        Dislikes = 0,
                        Posts = Amount
                    });
                }
                else
                {
                    Votes Current = DbContext.Votes.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Posts += Amount;
                    DbContext.Votes.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }
        */
    }
}
