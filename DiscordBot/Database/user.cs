using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace DiscordBot.Database
{
    class user
    {
        public ulong id { get; set; }
        public string name { get; set; }
        public int posts { get; set; }
        public int upvotes { get; set; }
        public int downvotes { get; set; }
        public int privileg { get; set; }

    }
}
