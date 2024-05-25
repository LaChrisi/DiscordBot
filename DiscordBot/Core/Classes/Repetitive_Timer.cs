using System;
using System.Collections.Generic;
using System.Timers;
using Discord;
using Discord.WebSocket;
using DiscordBot.Core.Commands;
using Google.Apis.Auth.OAuth2;
using DiscordBot.Core.Classes;
using Microsoft.VisualBasic;
using MySqlX.XDevAPI;

namespace DiscordBot.Core.Classes
{
    class Repetitive_Timer
    {
        public static Timer daily_timer;
        public static Timer hourly_timer;
        public static Timer minutes_5_timer;

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
                Log.Error(ex.Message);
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
                Log.Error(ex.Message);
            }
        }

        public static void SetUp5MinutesTimer(TimeSpan alertTime)
        {
            try
            {
                DateTime current = DateTime.Now;
                TimeSpan timeToGo = alertTime - current.TimeOfDay;

                minutes_5_timer = new Timer
                {
                    AutoReset = false,
                    Interval = timeToGo.TotalMilliseconds
                };
                minutes_5_timer.Elapsed += Minutes_5_timer_Elapsed;

                minutes_5_timer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error(ex.Message);
            }
        }

        public static async void Minutes_5_timer_Elapsed(object sender, ElapsedEventArgs eArgs)
        {
            try
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "minutes_5_timer event");
                Log.Information("system - minutes_5_timer event - start");

                var global = Global.GetByName("sex_id");
                int i = Convert.ToInt32(global.value);

                if (i > 1)
                {
                    var channel = Program.Client.GetChannel(1242565685146816584) as ISocketMessageChannel;

                    SpreadSheetConnector google = new SpreadSheetConnector();
                    google.ConnectToGoogle();

                    while (true)
                    {
                        var item = google.GetRow(i);
                        Discord.Embed embed = null;

                        if (item == null)
                            break;

                        string notes = item.notes;

                        var split = notes.Split("!alt - ");

                        if (split.Length > 1)
                            notes = $"{split[0]}\n*{split[1]}*";
                        else
                            notes = $"{split[0]}";

                        Program.SexSendStatsDiscord(new Item(item.who, item.type, notes, item.when));

                        Program.AddToSexStats(item, +1);

                        i++;

                        global.value = Convert.ToString(i);
                        Global.Edit(global);
                    }

                    Program.UpdateSexStats();
                }

                try
                {
                    SetUp5MinutesTimer(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + 5, 0));
                }
                catch (Exception ex)
                {
                    SetUp5MinutesTimer(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute + 5, 0));
                    Log.Error($"system - minutes_5_timer event - error:{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error($"system - minutes_5_timer event - error:{ex.Message}");
            }
        }

            //hourly_timer event
            public static async void Hourly_timer_Elapsed(object sender, ElapsedEventArgs eArgs)
        {
            try
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "hourly_timer event");
                Log.Information("system - hourly_timer event - start");

                var channel_event_list = Channel_Event.GetAllByType('k');

                /*
                //karma reminder event

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
                */

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
                                    var items = Birthday.GetNextBirthday();

                                    string content = "";

                                    foreach (var item in items)
                                    {
                                        if (items[0].Start.Date == item.Start.Date)
                                        {
                                            var user = Program.Client.GetUser((ulong) Convert.ToInt64(item.Description));

                                            if (user != null)
                                            {
                                                content = content + user.Username + "\n";
                                            }
                                            else
                                            {
                                                content = content + item.Summary + "\n";
                                            }
                                        }
                                    }

                                    await message.ModifyAsync(x => { x.Embed = Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder(Birthday.DateToSting(items[0].Start.Date), content), Colors.information, "next birthday"); });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                    }
                }
                try
                {
                    SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
                }
                catch (Exception ex)
                {
                    SetUpHourlyTimer(new TimeSpan(DateTime.Now.Hour + 1, 0, 0));
                    Log.Error($"system - hourly_timer event - error:{ex.Message}");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error($"system - hourly_timer event - error:{ex.Message}");
            }
        }

        //daily_timer event
        public static async void Daily_timer_Elapsed(object sender, ElapsedEventArgs eArgs)
        {
            try
            {
                Console.WriteLine(DateTime.Now.TimeOfDay + "daily_timer event");
                Log.Information("system - daily_timer event - start");

                //Karma event

                /*
                var user_list = User.GetAllWithKarma();

                if (user_list != null)
                {
                    foreach (var user in user_list)
                    {
                        user.karma -= Convert.ToInt32(Global.GetByName("karma_daily_loss").value);
                        int karma_minimum = Convert.ToInt32(Global.GetByName("karma_minimum").value);

                        if (user.karma < karma_minimum)
                            user.karma = karma_minimum;

                        User.Edit(user);
                    }
                }

                */
                //todays birthday event

                var channel_event_list = Channel_Event.GetAllByType('s');

                foreach (var channel_event in channel_event_list)
                {
                    if (channel_event.aktiv == 1)
                    {
                        var channel = Program.Client.GetChannel(channel_event.channel) as ISocketMessageChannel;
                        var e = Event.GetById(channel_event.Event);

                        if (e.what == "say")
                        {
                            if (e.how == "todays_birthday")
                            {
                                var message_list = Message.GetAllByChannelAndType(channel.Id, 't');

                                if (message_list != null)
                                {
                                    foreach (var message in message_list)
                                    {
                                        await channel.DeleteMessageAsync(message.message);
                                        Message.DeleteById(message.id);
                                    }
                                }

                                var items = Birthday.GetNextBirthday();

                                if (items != null)
                                {
                                    string content = "";

                                    foreach (var item in items)
                                    {
                                        if (item.Start.Date == DateTime.Now.Date.ToString("yyyy-MM-dd"))
                                        {
                                            var user = Program.Client.GetUser((ulong)Convert.ToInt64(item.Description));

                                            if (user != null)
                                            {
                                                content = content + user.Mention + "\n";
                                            }
                                            else
                                            {
                                                content = content + item.Summary + "\n";
                                            }
                                        }
                                    }

                                    if (content != "")
                                    {
                                        content = content + "\nHappy Birthday!";

                                        var message = await channel.SendMessageAsync(embed: Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder(Birthday.DateToSting(items[0].Start.Date), content), Colors.information, "todays birthday"));

                                        Message.Add(new Message(Program.Client.CurrentUser.Id, message.Id, channel.Id, 't'));
                                    }
                                }
                            }
                        }
                    }
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
