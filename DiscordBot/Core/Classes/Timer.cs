using System;
using System.Timers;

using Discord.WebSocket;

namespace DiscordBot.Core.Classes
{
    class Timer
    {
        public static System.Timers.Timer daily_timer;
        public static System.Timers.Timer hourly_timer;

        public static void SetUpDailyTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;

            if (timeToGo < TimeSpan.Zero)
            {
                timeToGo = new TimeSpan(24, 0, 0) + timeToGo;
            }

            daily_timer = new System.Timers.Timer
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

            hourly_timer = new System.Timers.Timer
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

            var channel_event_list = Channel_Event.GetAllByType('k');

            foreach (var channel_event in channel_event_list)
            {
                var user = User.GetById((ulong)Convert.ToInt64(channel_event.when));
                var e = Event.GetById(channel_event.Event);

                if (e.what == "say")
                {
                    if (e.how == "remind")
                    {
                        if (user.karma < 100)
                        {
                            var channel = Program.Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                            var reminder = await channel.SendMessageAsync(embed: Core.Classes.Embed.New(Program.Client.GetUser(user.id), Field.CreateFieldBuilder("warning", $"Your karma is {user.karma}.\nYou should post some new memes!"), Colors.warning, "friendly reminder"));
                            Message.Add(new Message(user.id, reminder.Id, channel.Id, 'k'));
                        }
                    }
                }
            }

            SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
        }

        //daily_timer event
        private static void Daily_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay + "daily_timer event");

            var user_list = User.GetAllWithKarma();

            foreach (var user in user_list)
            {
                user.karma -= 100;

                if (user.karma < -100)
                    user.karma = -100;

                User.Edit(user);
            }

            SetUpDailyTimer(new TimeSpan(5, 0, 0));
        }
    }
}
