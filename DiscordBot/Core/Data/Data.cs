using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SQLite;
using MySql.Data.MySqlClient;
using System.Data;

namespace DiscordBot.Core.Data
{
    public static class Data
    {
        public static string Version()
        {
            
            string stm = "SELECT VERSION()";

            using var con = new MySqlConnection(Token.cs);
            con.Open();

            using var cmd = new MySqlCommand(stm, con);
            string version = cmd.ExecuteScalar().ToString();

            return version;
        }

        public static DataTable ExecuteRead(string query, Dictionary<string, object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using (var con = new MySqlConnection(Token.cs))
            {
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }

                    var da = new MySqlDataAdapter(cmd);

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

            using (var con = new MySqlConnection(Token.cs))
            {
                con.Open();

                using (var cmd = new MySqlCommand(query, con))
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
