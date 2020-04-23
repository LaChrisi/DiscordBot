using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Vote_Event
    {
        public ulong id { get; set; }
        public int aktiv { get; set; }
        public ulong vote { get; set; }
        public ulong Event { get; set; }
        public int when { get; set; }

        public static string header = "id | aktiv | vote | event | when";

        public override string ToString()
        {
            return this.id + " | " + this.aktiv + " | " + this.vote + " | " + this.Event + " | " + this.when;
        }

        public Vote_Event(ulong id, int aktiv, ulong vote, ulong eventId, int when)
        {
            this.id = id;
            this.aktiv = aktiv;
            this.vote = vote;
            this.Event = eventId;
            this.when = when;
        }

        public Vote_Event(int aktiv, ulong vote, ulong eventId, int when)
        {
            this.aktiv = aktiv;
            this.vote = vote;
            this.Event = eventId;
            this.when = when;
        }

        public static List<Vote_Event> GetAll()
        {
            var query = "SELECT * FROM vote_event";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote_Event> list = new List<Vote_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["vote"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToInt32(dt.Rows[i]["when"])));
                i++;
            }

            return list;
        }

        public static List<Vote_Event> GetAllByVoteId(ulong id)
        {
            var query = "SELECT * FROM vote_event WHERE vote = @vote";
            var args = new Dictionary<string, object>
            {
                {"@vote", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote_Event> list = new List<Vote_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["vote"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToInt32(dt.Rows[i]["when"])));
                i++;
            }

            return list;
        }

        public static List<Vote_Event> GetAllByEventeId(ulong id)
        {
            var query = "SELECT * FROM vote_event WHERE event = @event";
            var args = new Dictionary<string, object>
            {
                {"@event", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Vote_Event> list = new List<Vote_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Vote_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["vote"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToInt32(dt.Rows[i]["when"])));
                i++;
            }

            return list;
        }

        public static Vote_Event GetById(ulong id)
        {
            var query = "SELECT * FROM vote_event WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Vote_Event((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToInt16(dt.Rows[0]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[0]["vote"]), (ulong)Convert.ToInt64(dt.Rows[0]["event"]), Convert.ToInt32(dt.Rows[0]["when"]));
        }

        public static int Add(Vote_Event vote_event)
        {
            const string query = "INSERT INTO vote_event(aktiv, vote, event, when) VALUES(@aktiv, @vote, @event, @when)";

            var args = new Dictionary<string, object>
            {
                {"@aktiv", vote_event.aktiv},
                {"@vote", vote_event.vote},
                {"@event", vote_event.Event},
                {"@when", vote_event.when}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM vote_event WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByVoteId(ulong id)
        {
            var query = "DELETE * FROM vote_event WHERE vote = @vote";
            var args = new Dictionary<string, object>
            {
                {"@vote", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Vote_Event vote_event)
        {
            const string query = "UPDATE vote_event SET aktiv = @aktiv, vote = @vote, event = @event, when = @when WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", vote_event.id},
                {"@aktiv", vote_event.aktiv},
                {"@vote", vote_event.vote},
                {"@event", vote_event.Event},
                {"@when", vote_event.when}
            };

            return Data.ExecuteWrite(query, args);
        }
    }
}
