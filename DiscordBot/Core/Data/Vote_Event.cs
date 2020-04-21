using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Vote_Event
    {
        public ulong id { get; set; }
        public int aktiv { get; set; }
        public ulong vote { get; set; }
        public ulong eventId { get; set; }
        public int when { get; set; }

        public Vote_Event(ulong id, int aktiv, ulong vote, ulong eventId, int when)
        {
            this.id = id;
            this.aktiv = aktiv;
            this.vote = vote;
            this.eventId = eventId;
            this.when = when;
        }
    }
}
