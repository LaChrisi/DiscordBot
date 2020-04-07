using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace DiscordBot.Resources.Database
{
    public class SqliteDbContext : DbContext
    {
        public DbSet<Votes> Votes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            Options.UseSqlite(@"Data Source=SQLiteDB.db");
        }
    }
}
