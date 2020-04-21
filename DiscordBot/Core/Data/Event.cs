using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Event
    {
        public ulong id { get; set; }
        public string what { get; set; }
        public string how { get; set; }

        public Event(ulong id, string what, string how)
        {
            this.id = id;
            this.what = what;
            this.how = how;
        }
    }
}
