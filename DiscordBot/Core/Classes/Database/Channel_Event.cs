using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DiscordBot.Core.Classes
{
    class Channel_Event
    {
        public ulong id { get; set; }
        public int aktiv { get; set; }
        public ulong channel { get; set; }
        public ulong Event { get; set; }
        public string when { get; set; }
        public char type { get; set; }

        public static string header = "id | aktiv | channel | event | when | type";

        public override string ToString()
        {
            return this.id + " | " + this.aktiv + " | " + this.channel + " | " + this.Event + " | " + this.when + " | " + this.type;
        }

        public Channel_Event(ulong id, int aktiv, ulong channel, ulong eventId, string when, char type)
        {
            this.id = id;
            this.aktiv = aktiv;
            this.channel = channel;
            this.Event = eventId;
            this.when = when;
            this.type = type;
        }

        public Channel_Event(int aktiv, ulong channel, ulong eventId, string when, char type)
        {
            this.aktiv = aktiv;
            this.channel = channel;
            this.Event = eventId;
            this.when = when;
            this.type = type;
        }

        public static List<Channel_Event> GetAll()
        {
            var query = "SELECT * FROM channel_event";
            var args = new Dictionary<string, object>();
            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel_Event> list = new List<Channel_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToString(dt.Rows[i]["when"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }

        public static List<Channel_Event> GetAllByChannelId(ulong id)
        {
            var query = "SELECT * FROM channel_event WHERE channel = @channel";
            var args = new Dictionary<string, object>
            {
                {"@channel", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel_Event> list = new List<Channel_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToString(dt.Rows[i]["when"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }

        public static List<Channel_Event> GetAllByEventeId(ulong id)
        {
            var query = "SELECT * FROM channel_event WHERE event = @event";
            var args = new Dictionary<string, object>
            {
                {"@event", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel_Event> list = new List<Channel_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToString(dt.Rows[i]["when"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }

        public static Channel_Event GetById(ulong id)
        {
            var query = "SELECT * FROM channel_event WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            return new Channel_Event((ulong)Convert.ToInt64(dt.Rows[0]["id"]), Convert.ToInt16(dt.Rows[0]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[0]["channel"]), (ulong)Convert.ToInt64(dt.Rows[0]["event"]), Convert.ToString(dt.Rows[0]["when"]), Convert.ToChar(dt.Rows[0]["type"]));
        }

        public static int Add(Channel_Event channel_event)
        {
            const string query = "INSERT INTO channel_event(aktiv, channel, event, when, type) VALUES(@aktiv, @channel, @event, @when, @type)";

            var args = new Dictionary<string, object>
            {
                {"@aktiv", channel_event.aktiv},
                {"@channel", channel_event.channel},
                {"@event", channel_event.Event},
                {"@when", channel_event.when},
                {"@type", channel_event.type}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteById(ulong id)
        {
            const string query = "DELETE FROM channel_event WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int DeleteAllByChannelId(ulong id)
        {
            var query = "DELETE * FROM channel_event WHERE channel = @channel";
            var args = new Dictionary<string, object>
            {
                {"@channel", id}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static int Edit(Channel_Event channel_event)
        {
            const string query = "UPDATE channel_event SET aktiv = @aktiv, channel = @channel, event = @event, when = @when, type = @type WHERE id = @id";

            var args = new Dictionary<string, object>
            {
                {"@id", channel_event.id},
                {"@aktiv", channel_event.aktiv},
                {"@channel", channel_event.channel},
                {"@event", channel_event.Event},
                {"@when", channel_event.when},
                {"@type", channel_event.type}
            };

            return Data.ExecuteWrite(query, args);
        }

        public static List<Channel_Event> GetAllByChannelIdAndType(ulong id, char type)
        {
            var query = $"SELECT * FROM channel_event WHERE channel = @channel AND type = @type";
            var args = new Dictionary<string, object>
            {
                {"@channel", id},
                {"@type", type}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel_Event> list = new List<Channel_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToString(dt.Rows[i]["when"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }

        public static List<Channel_Event> GetAllByType(char type)
        {
            var query = $"SELECT * FROM channel_event WHERE type = @type";
            var args = new Dictionary<string, object>
            {
                {"@type", type}
            };

            DataTable dt = Data.ExecuteRead(query, args);

            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Channel_Event> list = new List<Channel_Event>();
            int i = 0;

            foreach (var item in dt.Rows)
            {
                list.Add(new Channel_Event((ulong)Convert.ToInt64(dt.Rows[i]["id"]), Convert.ToInt16(dt.Rows[i]["aktiv"]), (ulong)Convert.ToInt64(dt.Rows[i]["channel"]), (ulong)Convert.ToInt64(dt.Rows[i]["event"]), Convert.ToString(dt.Rows[i]["when"]), Convert.ToChar(dt.Rows[i]["type"])));
                i++;
            }

            return list;
        }
    }
}
