using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Core.Data
{
    class Server
    {
        public ulong id { get; set; }
        public string name { get; set; }

        public Server(ulong id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
