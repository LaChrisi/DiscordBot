using System;
using System.Collections.Generic;
using System.Timers;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Core.Classes
{
    class Repetitive_Timer
    {
        public static Timer daily_timer;
        public static Timer hourly_timer;

        public static void SetUpDailyTimer(TimeSpan alertTime)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SetUpHourlyTimer(TimeSpan alertTime)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //hourly_timer event
        private static async void Hourly_timer_Elapsed(object sender, ElapsedEventArgs eArgs)
        {
            try
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "hourly_timer event");
                Log.Information("system - hourly_timer event - start");

                //karma reminder event
                var channel_event_list = Channel_Event.GetAllByType('k');

                foreach (var channel_event in channel_event_list)
                {
                    if (channel_event.aktiv == 1)
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
                }

                //renew event
                channel_event_list = Channel_Event.GetAllByType('r');

                foreach (var channel_event in channel_event_list)
                {
                    if (channel_event.aktiv == 1)
                    {
                        var channel = Program.Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                        var e = Event.GetById(channel_event.Event);

                        if (e.what == "renew")
                        {
                            if (e.how == "leaderboard")
                            {
                                var message = await channel.GetMessageAsync((ulong)Convert.ToInt64(channel_event.when)) as IUserMessage;

                                try
                                {
                                    await message.ModifyAsync(x => { x.Embed = Embed.GetLeaderboard(); });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else if (e.how == "nextBirthday")
                            {
                                var message = await channel.GetMessageAsync((ulong)Convert.ToInt64(channel_event.when)) as IUserMessage;

                                try
                                {
                                    await message.ModifyAsync(x => { x.Embed = Embed.GetLeaderboard(); });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }

                SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error($"system - hourly_timer event - error:{ex.Message}");
            }
        }

        //daily_timer event
        private static void Daily_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "daily_timer event");
                Log.Information("system - daily_timer event - start");

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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error($"system - daily_timer event - error:{ex.Message}");
            }
        }
    }
}
