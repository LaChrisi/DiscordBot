using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SQLite;
using System.Data;

namespace DiscordBot.Core.Data
{
    public static class Data
    {
        public static string Version()
        {
            string cs = "Data Source=..\\DiscordBot.db";
            string stm = "SELECT SQLITE_VERSION()";

            using var con = new SQLiteConnection(cs, true);
            con.Open();

            using var cmd = new SQLiteCommand(stm, con);
            string version = cmd.ExecuteScalar().ToString();

            return version;
        }

        public static DataTable ExecuteRead(string query, Dictionary<string, object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            //using (var con = new SQLiteConnection("Data Source=Core\\Data\\DiscordBot.db"))
            using (var con = new SQLiteConnection("Data Source=..\\DiscordBot.db"))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }

                    var da = new SQLiteDataAdapter(cmd);

                    var dt = new DataTable();
                    da.Fill(dt);

                    da.Dispose();
                    return dt;
                }
            }
        }

        public static int ExecuteWrite(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;

            using (var con = new SQLiteConnection("Data Source=..\\DiscordBot.db"))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(query, con))
                {
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }

                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }

                return numberOfRowsAffected;
            }
        }
    }
}
