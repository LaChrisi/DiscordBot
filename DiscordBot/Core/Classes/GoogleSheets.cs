using Discord;
using Google.Apis.Requests;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;


namespace DiscordBot.Core.Classes
{
    public class SpreadSheetConnector
    {
        private string applicationName = "DiscordBot";
        private string spreadsheetId = "1ng-C8r1lKptMlkJDrRswRzTswzP-q6HPfXRrFgaeq38";
        private SheetsService sheetsService;

        public void ConnectToGoogle()
        {
            sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = Token.googleCredential,
                ApplicationName = applicationName
            });
        }

        public Item GetRow(int rowID)
        {
            var range = $"Formularantworten!A{rowID}:D{rowID}";
            SpreadsheetsResource.ValuesResource.GetRequest getRequest = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
            
            var getResponse = getRequest.Execute();
            IList<IList<Object>> values = getResponse.Values;
            Item item = Item.New(null, null, null, new DateTime()); ;

            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    var notes = "Ich liebe dich Nadine!\n*Christoph*";

                    try
                    {
                        notes = Convert.ToString(row[3]);
                    }
                    catch (Exception)
                    {

                    }

                    item = new Item(Convert.ToString(row[1]), Convert.ToString(row[2]), notes, Convert.ToDateTime(row[0]));
                }
            }
            else
            {
                return null;
            }

            return item;
        }

        public void AddRow(Item item)
        {
            if (item.who == "Christoph und Nadine")
            {
                item.who = "Christoph, Nadine";
            }
            

            var row = Global.GetByName("sex_id");

            List<object> ranges = new List<object>();

            var range = $"Formularantworten!A{row.value}";

            ValueRange valueRange = new ValueRange();
            var oblist = new List<object>();
            valueRange.MajorDimension = "ROWS";

            oblist.Add(item.when.ToString("dd.MM.yyyy HH:mm:ss"));
            oblist.Add(item.who);
            oblist.Add(item.type);
            oblist.Add(item.notes);

            valueRange.Values = new List<IList<object>> { oblist };

            SpreadsheetsResource.ValuesResource.UpdateRequest update = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = update.Execute();

            return;
        }
        public void DeleteRow(int rowID)
        {
            var range = $"Formularantworten!A{rowID}";

            ValueRange valueRange = new ValueRange();
            var oblist = new List<object>();
            valueRange.MajorDimension = "ROWS";

            oblist.Add("");
            oblist.Add("");
            oblist.Add("");
            oblist.Add("");

            valueRange.Values = new List<IList<object>> { oblist };

            SpreadsheetsResource.ValuesResource.UpdateRequest update = sheetsService.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = update.Execute();

            return;
        }
    }
    
    public class Item
    {
        public  string who { get; set; }
        public  string type { get; set; }
        public  string notes { get; set; }
        public  DateTime when { get; set; }

        public Item(string who, string type, string notes, DateTime when)
        {
            this.who = who;
            this.type = type;
            this.notes = notes;
            this.when = when;
        }

        public override string ToString()
        {
            return this.who + " | " + this.type + " | " + this.notes + " | " + this.when;
        }

        public static Item New(string who, string type, string notes, DateTime when)
        {
            return new Item(who, type, notes, when);
        }

        public Discord.Embed toEmbed()
        {
            var test = Embed.New(Program.Client.CurrentUser, Field.CreateFieldBuilder($"{who} - {type}", $"{notes}"), Colors.information, when);

            return test;
        }
    }
}
