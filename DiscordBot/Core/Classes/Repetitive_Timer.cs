using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Discord;
using Discord.WebSocket;
using MySql.Data.MySqlClient.Memcached;

namespace DiscordBot.Core.Classes
{
    class Repetitive_Timer
    {
        public static Timer daily_timer;
        public static Timer hourly_timer;

        public static void SetUpDailyTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
            {
                timeToGo = new TimeSpan(24, 0, 0) + timeToGo;
            }

            daily_timer = new Timer
            {
                AutoReset = false,
                Interval = timeToGo.TotalMilliseconds
            };
            daily_timer.Elapsed += Daily_timer_Elapsed;

            daily_timer.Start();
        }

        public static void SetUpHourlyTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;

            hourly_timer = new Timer
            {
                AutoReset = false,
                Interval = timeToGo.TotalMilliseconds
            };
            hourly_timer.Elapsed += Hourly_timer_Elapsed;

            hourly_timer.Start();
        }

        //hourly_timer event
        private static async void Hourly_timer_Elapsed(object sender, ElapsedEventArgs eArgs)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay + "hourly_timer event");

            //karma reminder event

            var channel_event_list = Channel_Event.GetAllByType('k');

            foreach (var channel_event in channel_event_list)
            {
                var user = User.GetById((ulong)Convert.ToInt64(channel_event.when));
                var e = Event.GetById(channel_event.Event);

                if (e.what == "say")
                {
                    if (e.how == "remind")
                    {
                        if (user.karma < Convert.ToInt32(Global.GetByName("karma_threshold_to_remind").value))
                        {
                            var message = Event.GetRandomByWhat("reminder");
                            var channel = Program.Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                            var reminder = await channel.SendMessageAsync(embed: Embed.New(Program.Client.GetUser(user.id), Field.CreateFieldBuilder("warning", $"Your karma is {user.karma}.\n{message.how}"), Colors.warning, "friendly reminder"));
                            Message.Add(new Message(user.id, reminder.Id, channel.Id, 'k'));
                        }
                    }
                }
            }

            //renew leaderboard event
            /*
            channel_event_list = Channel_Event.GetAllByType('r');

            foreach (var channel_event in channel_event_list)
            {
                var channel = Program.Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                var e = Event.GetById(channel_event.Event);

                if (e.what == "renew")
                {
                    if (e.how == "leaderboard")
                    {
                        var message = await channel.GetMessageAsync((ulong)Convert.ToInt64(channel_event.when)) as IUserMessage;
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();

                        var user_list = User.GetTop5Karma();
                        int i = 1;

                        foreach (var user in user_list)
                        {
                            string title = "";

                            if (i == 1)
                                title += "🥇" + " - ";
                            else if (i == 2)
                                title += "🥈" + " - ";
                            else if (i == 3)
                                title += "🥉" + " - ";

                            title += user.name;

                            fields.Add(Field.CreateFieldBuilder(title, $"👍 {user.upvotes}\n👎 {user.downvotes}\n🗒️ {user.posts}\n📊 {user.karma}"));
                            i++;
                        }

                        await message.ModifyAsync(x => { x.Embed = Embed.New(x.Embed.Value.Author.Value, fields, Colors.information, "top 5 memers", "ordered by karma and upvotes"); });
                    }
                }
            }
            */

            SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
        }

        //daily_timer event
        private static void Daily_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay + "daily_timer event");

            var user_list = User.GetAllWithKarma();

            foreach (var user in user_list)
            {
                user.karma -= Convert.ToInt32(Global.GetByName("karma_daily_loss").value);
                int karma_minimum = Convert.ToInt32(Global.GetByName("karma_minimum").value);

                if (user.karma < karma_minimum)
                    user.karma = karma_minimum;

                User.Edit(user);
            }

            SetUpDailyTimer(new TimeSpan(Convert.ToInt32(Global.GetByName("daily_timer_hour").value), 0, 0));
        }
    }
}
