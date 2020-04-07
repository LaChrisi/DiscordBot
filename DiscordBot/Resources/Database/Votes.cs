using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Resources.Database
{
    public class Votes
    {
        [Key]
        public ulong UserId { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Posts { get; set; }
    }
}
