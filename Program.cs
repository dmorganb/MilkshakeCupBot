namespace MilkshakeCup
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class Program
    {
        private static ITelegramBotClient botClient;
        
        private static List<Row> GroupA { get; set; }

        private static List<Row> GroupB { get; set; }

        private static GSheetsService SheetsService;

        private class Row
        {
            public string Player { get; set; }
            
            public string Team { get; set; }
            
            public int Points => (Won * 3) + Draw;

            public int TotalGames => Won + Lost + Draw;

            public int Won { get; set; }

            public int Draw { get; set; }

            public int Lost { get; set; }            
            
            public int GoalsInFavor { get; set; }

            public int GoalsAgainst { get; set; }

            public int GoalDifference => GoalsInFavor - GoalsAgainst;

            public Row(
                string player, 
                string team, 
                int won, 
                int draw, 
                int lost, 
                int goalsInFavor, 
                int goalsAgainst)
            {
                Player = player;
                Team = team;
                Won = won;
                Draw = draw;
                Lost = lost;
                GoalsInFavor = goalsInFavor;
                GoalsAgainst = goalsAgainst;
            }
        }

        public static async Task Main(string[] args)
        {
            // app settings
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            var token = configuration["token"];
            var spreadsheetId = configuration["spreadsheetId"];

            // bot client
            botClient = new TelegramBotClient(token);
            botClient.OnMessage += OnMessage;
            botClient.StartReceiving();

            //Google Sheets Service
            SheetsService = new GSheetsService(spreadsheetId);
            await SheetsService.CreateService();

            //Read Group A from Drive And load it to memory
            var groupAFromDrive = await SheetsService.GetSheetAsync("Grupo A - Tabla", "A10", "J");
            GroupA = ReadGroupFromDrive(groupAFromDrive);

            //Read Group B from Drive And load it to memory
            var groupBFromDrive = await SheetsService.GetSheetAsync("Grupo B - Tabla", "A10", "J");
            GroupB = ReadGroupFromDrive(groupBFromDrive);

            // intro
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
            Console.WriteLine("Press esc to exit");

            ConsoleKeyInfo input;
            
            do
            {
                input = Console.ReadKey();
            }
            while (input.Key != ConsoleKey.Escape);
        }

        private static List<Row> ReadGroupFromDrive(List<List<string>> groupFromDrive) 
        {
            var group = new List<Row>();
            foreach (var row in groupFromDrive) 
            {
                group.Add(new Row(
                    row[0].ToLower(),
                    row[1].ToLower(),
                    int.Parse(row[3]),
                    int.Parse(row[4]),
                    int.Parse(row[5]),
                    int.Parse(row[6]),
                    int.Parse(row[7])
                ));
            }

            return group;
        }

        private static async void OnMessage(object sender, MessageEventArgs e) 
        {
            try
            {
                Console.WriteLine(e.Message.Text);
                
                if (e.Message.Text == null)
                {
                    return;
                }

                if (e.Message.Text.StartsWith("/tabla"))
                {
                    await TablasCommand(e);
                    return;
                }
                if (e.Message.Text.StartsWith("/marcador"))
                {
                    await MarcadorCommand(e);
                    return;
                }
            }
            catch(Exception ex)
            {
                // fail silently 
                Console.WriteLine($"There was an exception with message e: {ex.Message}");
            }
        }

        private static async Task TablasCommand(MessageEventArgs e)
        {
            var parameters = e.Message.Text.Split(' ');
            var selectedGroups = "ab";

            if (parameters.Length > 1)
            {
                selectedGroups = parameters[1]?.ToLower();
            }

            selectedGroups = (selectedGroups == "a" || selectedGroups == "b") ? selectedGroups : "ab";

            if (selectedGroups.Contains("a"))
            {
                var message1 = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: GroupTableMessage("Grupo A", GroupA),
                    parseMode: ParseMode.Markdown,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }

            if (selectedGroups.Contains("b"))
            {
                var message2 = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: GroupTableMessage("Grupo B", GroupB),
                    parseMode: ParseMode.Markdown,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }
        }

        private static async Task MarcadorCommand(MessageEventArgs e)
        {
            var parameters = e.Message.Text.Split(' ');

            if (parameters.Length != 5)
            {
                var message = await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Marcador debe llevar: equipo goles equipo goles\nPor ejemplo: Atletico 1 Eibar 0",
                    parseMode: ParseMode.Default,
                    disableNotification: true,
                    replyToMessageId: e.Message.MessageId);
            }
            else
            {
                var team1 = parameters[1]?.ToLower();
                var goals1 = -1;
                var team2 = parameters[3]?.ToLower();
                var goals2 = -1;

                if (!int.TryParse(parameters[2], out goals1))
                {
                    goals1 = -1;
                }
                
                if(!int.TryParse(parameters[4], out goals2))
                {
                    goals2 = -1;
                }

                var row1 = GroupA.FirstOrDefault(x => x.Team.StartsWith(team1) || x.Player.Contains(team1)) ??
                        GroupB.FirstOrDefault(x => x.Team.StartsWith(team1) || x.Player.Contains(team1));
                var row2 = GroupA.FirstOrDefault(x => x.Team.StartsWith(team2) || x.Player.Contains(team2)) ??
                        GroupB.FirstOrDefault(x => x.Team.StartsWith(team2) || x.Player.Contains(team2));

                // row validations:

                // 1) teams or players exist in a group.
                if (row1 == null || row2 == null)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"no encontré alguno de estos equipos: {team1}, {team2}",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // 2) teams or players are different.
                if (row1 == row2)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Diay, jugó solo?",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // 3) Teams belong to the same group.
                if ((GroupA.Contains(row1) && !GroupA.Contains(row2)) ||
                    (GroupB.Contains(row1) && !GroupB.Contains(row2)))
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Buen intento, pero solo acepto partidos de equipos del mismo grupo :P",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // score validations

                // 1) goals reported are valid scores
                if (goals1 < 0 || goals2 < 0)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Alguno de estos marcadores esta mamando: {parameters[2]}, {parameters[4]}",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                const int goalsThreshold = 10; // if someone scored more than 10 in the same game, that's suspicious.
                
                // 2) no one scored more than 10 goals (possible but not probable)
                if (goals1 > goalsThreshold || goals2 > goalsThreshold)
                {
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Mejor vuelvan a jugarlo, pero esta vez con todos los controles encendidos",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                    return;
                }

                // all parameters have valid values
                if (row1 != null && row2 != null && goals1 >= 0 && goals2 >= 0)
                {
                    if (goals1 > goals2) // team 1 won, team 2 lost
                    {
                        row1.Won++;
                        row2.Lost++;
                    }
                    else if (goals1 < goals2) // team 1 lost, team 2 won
                    {
                        row1.Lost++;
                        row2.Won++;
                    }
                    else // team 1 & team 2 draw
                    {
                        row1.Draw++;
                        row2.Draw++;
                    }

                    // goals bookkeeping
                    row1.GoalsInFavor += goals1;
                    row1.GoalsAgainst += goals2;
                    row2.GoalsInFavor += goals2;
                    row2.GoalsAgainst += goals1;

                    // save current table to Drive
                    await WriteGroupIntoDrive(GroupA, "Grupo A - Tabla", "A2", "J7");
                    await WriteGroupIntoDrive(GroupB, "Grupo B - Tabla", "A2", "J7");

                    // confirmation message
                    var message = await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text: $"Anotado!",
                        parseMode: ParseMode.Default,
                        disableNotification: true,
                        replyToMessageId: e.Message.MessageId);
                }

            }
        }

        // TODO: This method can become one-parameter long if we create a Group class with a Location property thet would contain the sheetname and range.
        private async static Task<bool> WriteGroupIntoDrive(List<Row> group, string sheetName, string startRange, string finishRange) 
        {
            var updatingValues = new List<List<string>>();

            foreach (var row in group) 
            {
                var updatedRow = new List<string> {row.Player, row.Team, row.TotalGames.ToString(), row.Won.ToString(), row.Draw.ToString(), 
                                                   row.Lost.ToString(), row.GoalsInFavor.ToString(), row.GoalsAgainst.ToString(), 
                                                   row.GoalDifference.ToString(), row.Points.ToString()};
                updatingValues.Add(updatedRow);
            }

            if (updatingValues.Count > 0)
            {
                var numberOfCellsChanged = await SheetsService.UpdateSheetAsync(sheetName, startRange, finishRange, updatingValues);
                Console.WriteLine(@"{0} cell(s) written", numberOfCellsChanged);
                return true;
            }

            return false;
        }

        private static string GroupTableMessage(string title, List<Row> group) 
        {
            var text = title + "\n";
            text += "```\n";
            text += "Equ|Pt|PJ| G| E| P|GF|Dif\n";
            text += "=========================\n";

            // This is important
            var orderedGroup = group
                .OrderByDescending(x => x.Points)
                .ThenByDescending(x => x.GoalDifference)
                .ThenByDescending(x => x.GoalsInFavor);

            foreach (var row in orderedGroup)
            {
                text += (row.Player.Length > 3? row.Player.Substring(0, 3) : row.Player.PadRight(3)) + "|";
                text += $"{row.Points}".PadLeft(2) + "|";
                text += $"{row.TotalGames}".PadLeft(2) + "|";
                text += $"{row.Won}".PadLeft(2) + "|";
                text += $"{row.Draw}".PadLeft(2) + "|";
                text += $"{row.Lost}".PadLeft(2) + "|";
                text += $"{row.GoalsInFavor}".PadLeft(2) + "|";
                text += $"{row.GoalDifference}".PadLeft(3) + "\n";
            }

            text += "```";

            return text;
        }
    }
}
