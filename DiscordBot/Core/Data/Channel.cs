using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Channel
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public ulong server { get; set; }

        public Channel(ulong id, string name, ulong server)
        {
            this.id = id;
            this.name = name;
            this.server = server;
        }
    }
}
