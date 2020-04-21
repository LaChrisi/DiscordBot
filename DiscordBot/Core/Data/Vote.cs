using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Vote
    {
        public ulong id { get; set; }
        public int aktiv { get; set; }
        public ulong channel { get; set; }
        public string what { get; set; }
        public string how { get; set; }

        public Vote(ulong id, int aktiv, ulong channel, string what, string how)
        {
            this.id = id;
            this.aktiv = aktiv;
            this.channel = channel;
            this.what = what;
            this.how = how;
        }
    }
}
