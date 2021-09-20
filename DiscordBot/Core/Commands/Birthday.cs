using Discord.Commands;
using DiscordBot.Core.Classes;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Core.Commands
{
    public class Birthday : ModuleBase<SocketCommandContext>
    {
        [Command("nextBirthday"), Alias("nb", "nextbirthday"), SummaryAttribute("prints next Birthday")]
        public async Task NextBirthdayModule()
        {
            try
            {
                Log.Information($"command - nextBirthday - start user:{Context.User.Id} channel:{Context.Channel.Id} command:{Context.Message.Content}");
                
                var items = GetNextBirthday();

                string content = "";

                foreach (var item in items)
                {
                    if (items[0].Start.Date == item.Start.Date)
                    {
                        var user = Program.Client.GetUser((ulong)Convert.ToInt64(item.Description));

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

                await Context.Channel.SendMessageAsync(embed: Embed.New(Context.Client.CurrentUser, Field.CreateFieldBuilder(DateToSting(items[0].Start.Date), content), Colors.information, "next birthday"));
            }
            catch (Exception ex)
            {
                Log.Error($"command - nextBirthday - user:{Context.User.Id} channel:{Context.Channel.Id} error:{ex.Message}");
            }
        }

        public static string DateToSting(string input)
        {
            var split = input.Split('-');

            return $"{split[2]}.{split[1]}.{split[0]}";
        }

        public static System.Collections.Generic.IList<Google.Apis.Calendar.v3.Data.Event> GetNextBirthday()
        {
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                ApiKey = Token.apikey,
                ApplicationName = "DiscordBot"
            });

            var request = service.Events.List(Token.calendarID);
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            request.MaxResults = Convert.ToInt32(Global.GetByName("max_birthdays_a_day").value);
            request.TimeMin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            request.Fields = "items(summary,description,start)";

            var response = request.Execute();

            return response.Items;
        }
    }
}