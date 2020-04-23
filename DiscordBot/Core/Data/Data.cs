using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Data;

namespace DiscordBot.Core.Data
{
    public static class Data
    {
        public static string Version()
        {
            try
            {
                string sql = "SELECT VERSION()";

                using var con = new MySqlConnection(Token.cs);
                con.Open();

                using var cmd = new MySqlCommand(sql, con);

                return cmd.ExecuteScalar().ToString();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static DataTable ExecuteRead(string query, Dictionary<string, object> args)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static int ExecuteWrite(string query, Dictionary<string, object> args)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
