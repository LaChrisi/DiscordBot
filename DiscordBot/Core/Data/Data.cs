using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Resources.Database;
using System.Linq;

namespace DiscordBot.Core.Data
{
    public static class Data
    {
        public static int GetLikes(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                    return 0;
                return DbContext.Votes.Where(x => x.UserId == UserId).Select(x => x.Likes).FirstOrDefault();
            }
        }

        public static int GetDislikes(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                    return 0;
                return DbContext.Votes.Where(x => x.UserId == UserId).Select(x => x.Dislikes).FirstOrDefault();
            }
        }

        public static int GetPosts(ulong UserId)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                    return 0;
                return DbContext.Votes.Where(x => x.UserId == UserId).Select(x => x.Posts).FirstOrDefault();
            }
        }

        public static async Task SaveLikes(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Votes.Add(new Votes
                    {
                        UserId = UserId,
                        Likes = Amount,
                        Dislikes = 0,
                        Posts = 1
                    });
                }
                else
                {
                    Votes Current = DbContext.Votes.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Likes += Amount;
                    DbContext.Votes.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SaveDislikes(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Votes.Add(new Votes
                    {
                        UserId = UserId,
                        Dislikes = Amount,
                        Likes = 0,
                        Posts = 1
                    });
                }
                else
                {
                    Votes Current = DbContext.Votes.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Dislikes += Amount;
                    DbContext.Votes.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }

        public static async Task SavePosts(ulong UserId, int Amount)
        {
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Votes.Where(x => x.UserId == UserId).Count() < 1)
                {
                    DbContext.Votes.Add(new Votes
                    {
                        UserId = UserId,
                        Likes = 0,
                        Dislikes = 0,
                        Posts = Amount
                    });
                }
                else
                {
                    Votes Current = DbContext.Votes.Where(x => x.UserId == UserId).FirstOrDefault();
                    Current.Posts += Amount;
                    DbContext.Votes.Update(Current);
                }

                await DbContext.SaveChangesAsync();
            }
        }
    }
}
