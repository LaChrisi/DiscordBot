using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Vote_Channel
    {
        public ulong id { get; set; }
        public int aktiv { get; set; }
        public ulong vote { get; set; }
        public ulong channel { get; set; }

        public static string header = "id | aktiv | vote | channel";

        public override string ToString()
        {
            return this.id + " | " + this.aktiv + " | " + this.vote + " | " + this.channel;
        }

        public Vote_Channel(ulong id, int aktiv, ulong vote, ulong channel)
        {
            this.id = id;
            this.aktiv = aktiv;
            this.vote = vote;
            this.channel = channel;
        }

        public Vote_Channel(int aktiv, ulong vote, ulong channel)
        {
            this.aktiv = aktiv;
            this.vote = vote;
            this.channel = channel;
        }

        public static List<Vote_Channel> GetAll()
        {
            var query = "SELECT * FROM vote_channel";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote_Channel> list = new List<Vote_Channel>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote_Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["vote"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"])));
                i++;
            }

            return list;
        }

        public static List<Vote_Channel> GetAllByVoteId(ulong id)
        {
            var query = "SELECT * FROM vote_channel WHERE vote = @vote";
            var args = new Dictionary<string, object>
            {
                {"@vote", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote_Channel> list = new List<Vote_Channel>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote_Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["vote"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"])));
                i++;
            }

            return list;
        }

        public static List<Vote_Channel> GetAllByChannelId(ulong id)
        {
            var query = "SELECT * FROM vote_channel WHERE channel = @channel";
            var args = new Dictionary<string, object>
            {
                {"@channel", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote_Channel> list = new List<Vote_Channel>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote_Channel((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["vote"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"])));
                i++;
            }

            return list;
        }

        public static Vote_Channel GetById(ulong id)
        {
            var query = "SELECT * FROM vote_channel WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Vote_Channel((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToInt16(dt.Rows[0]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[0]["vote"]), (ulong)Convert.ToInt64(dt.Rows[0]["channel"]));
        }

        public static int Add(Vote_Channel vote_channel)
        {
            const string query = "INSERT INTO vote_channel(aktiv, vote, channel) VALUES(@aktiv, @vote, @channel)";

            var args = new Dictionary<string, object>
            {
                {"@aktiv", vote_channel.aktiv},
                {"@vote", vote_channel.vote},
                {"@channel", vote_channel.channel}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM vote_channel WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByVoteId(ulong id)
        {
            var query = "DELETE * FROM vote_channel WHERE vote = @id";
            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByChannelId(ulong id)
        {
            var query = "DELETE * FROM vote_channel WHERE channel = @id";
            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Vote_Channel vote_channel)
        {
            const string query = "UPDATE vote_channel SET aktiv = @aktiv, vote = @vote, channel = @channel WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", vote_channel.id},
                {"@aktiv", vote_channel.aktiv},
                {"@vote", vote_channel.vote},
                {"@channel", vote_channel.channel}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
